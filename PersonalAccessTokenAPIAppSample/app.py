import uuid
import requests
from flask import Flask, render_template, session, request, redirect, url_for
from flask_session import Session  # https://pythonhosted.org/Flask-Session
import msal
import app_config
import os
import json


app = Flask(__name__)
app.config.from_object(app_config)
Session(app)

# This section is needed for url_for("foo", _external=True) to automatically
# generate http scheme when this sample is running on localhost,
# and to generate https scheme when it is deployed behind reversed proxy.
# See also https://flask.palletsprojects.com/en/1.0.x/deploying/wsgi-standalone/#proxy-setups
from werkzeug.middleware.proxy_fix import ProxyFix
app.wsgi_app = ProxyFix(app.wsgi_app, x_proto=1, x_host=1)

@app.route("/")
def index():
    if not session.get("user"):
        return redirect(url_for("login"))
    return render_template('index.html', user=session["user"], version=msal.__version__)

@app.route("/login")
def login():
    session["state"] = str(uuid.uuid4())
    # Technically we could use empty list [] as scopes to do just sign in,
    # here we choose to also collect end user consent upfront
    auth_url = _build_auth_url(scopes=app_config.SCOPE, state=session["state"])
    return render_template("login.html", auth_url=auth_url, version=msal.__version__)

@app.route(app_config.REDIRECT_PATH)  # Its absolute URL must match your app's redirect_uri set in AAD
def authorized():
    if request.args.get('state') != session.get("state"):
        return redirect(url_for("index"))  # No-OP. Goes back to Index page
    if "error" in request.args:  # Authentication/Authorization failure
        return render_template("auth_error.html", result=request.args)
    if request.args.get('code'):
        cache = _load_cache()
        result = _build_msal_app(cache=cache).acquire_token_by_authorization_code(
            request.args['code'],
            scopes=app_config.SCOPE,  # Misspelled scope would cause an HTTP 400 error here
            redirect_uri=url_for("authorized", _external=True))
        if "error" in result:
            return render_template("auth_error.html", result=result)
        session["user"] = result.get("id_token_claims")
        _save_cache(cache)
    return redirect(url_for("index"))

@app.route("/logout")
def logout():
    session.clear()  # Wipe out user and its token cache from session
    return redirect(  # Also logout from your tenant's web session
        app_config.AUTHORITY + "/oauth2/v2.0/logout" +
        "?post_logout_redirect_uri=" + url_for("index", _external=True))

@app.route("/handle_input", methods=["POST"])
def handle_input():
    route = "/"
    for key, value in request.form.items():
        if key != "route":
            session[key] = value
        else:
            route = value
    return redirect(url_for(route))

@app.route("/pat_get_all", methods=["GET"])
def pat_get_all():
    token = _get_token_from_cache(app_config.SCOPE)
    if not token:
        return redirect(url_for("login"))
    pat_data = requests.get(  # Use token to call downstream service
        app_config.ENDPOINT,
        headers={'Authorization': 'Bearer ' + token['access_token']},
        ).json()
    return render_template('display.html', result=pat_data)

@app.route("/pat_get", methods=["GET"])
def pat_get():
    token = _get_token_from_cache(app_config.SCOPE)
    if not token:
        return redirect(url_for("login"))
    authId = session["authid"]
    pat_data = requests.get(  # Use token to call downstream service
        app_config.ENDPOINT,
        headers={'Authorization': 'Bearer ' + token['access_token']},
        params={'authorizationId':authId}
        ).json()
    return render_template('display.html', result=pat_data)

@app.route("/pat_put", methods=["GET"])
def pat_put():
    token = _get_token_from_cache(app_config.SCOPE)
    if not token:
        return redirect(url_for("login"))
    requestBody = {}
    requestBody["authorizationId"] = session["authid"] # required for every request
    if session["displayname"] != "":
        requestBody["displayName"] = session["displayname"]
    if session["scope"] != "":
        requestBody["scope"] = session["scope"]
    if session["validto"] != "":
        requestBody["validTo"] = session["validto"]
    if session["allorgs"] != "":
        requestBody["allOrgs"] = session["allorgs"]
    pat_data = requests.put(  # Use token to call downstream service
        app_config.ENDPOINT,
        headers={'Authorization': 'Bearer ' + token['access_token'], 'Content-type': 'application/json'},
        data=json.dumps(requestBody)
        ).json()
    return render_template('display.html', result=pat_data)

@app.route("/pat_post", methods=["GET"])
def pat_post():
    token = _get_token_from_cache(app_config.SCOPE)
    if not token:
        return redirect(url_for("login"))
    requestBody = {}
    if session["displayname"] != "":
        requestBody["displayName"] = session["displayname"]
    if session["scope"] != "":
        requestBody["scope"] = session["scope"]
    if session["validto"] != "":
        requestBody["validTo"] = session["validto"]
    if session["allorgs"] != "":
        requestBody["allOrgs"] = session["allorgs"]
    pat_data = requests.post(  # Use token to call downstream service
        app_config.ENDPOINT,
        headers={'Authorization': 'Bearer ' + token['access_token'], 'Content-type': 'application/json'},
        data=json.dumps(requestBody)
        ).json()
    return render_template('display.html', result=pat_data)

@app.route("/pat_delete", methods=["GET"])
def pat_delete():
    token = _get_token_from_cache(app_config.SCOPE)
    if not token:
        return redirect(url_for("login"))
    authId = session["authid"]
    pat_data = requests.delete(  # Use token to call downstream service
        app_config.ENDPOINT,
        headers={'Authorization': 'Bearer ' + token['access_token']},
        params={'authorizationId':authId}
        )
    if pat_data.raw.data == b'':
        pat_data = {'Result':"Successfully revoked token!"}
    else:
        pata_data = pat_data.json()
    return render_template('display.html', result=pat_data)

def _load_cache():
    cache = msal.SerializableTokenCache()
    if session.get("token_cache"):
        cache.deserialize(session["token_cache"])
    return cache

def _save_cache(cache):
    if cache.has_state_changed:
        session["token_cache"] = cache.serialize()

def _build_msal_app(cache=None, authority=None):
    CLIENT_SECRET = app_config.CLIENT_SECRET
    # To switch to using an environment variable to store the client secret, 
    # comment out the line above and replace with the following code block:

    # CLIENT_SECRET = os.getenv("CLIENT_SECRET")
    # if not CLIENT_SECRET:
    #     raise ValueError("Need to define CLIENT_SECRET environment variable")

    return msal.ConfidentialClientApplication(
        app_config.CLIENT_ID, authority=authority or app_config.AUTHORITY,
        client_credential=CLIENT_SECRET, token_cache=cache)

def _build_auth_url(authority=None, scopes=None, state=None):
    return _build_msal_app(authority=authority).get_authorization_request_url(
        scopes or [],
        state=state or str(uuid.uuid4()),
        redirect_uri=url_for("authorized", _external=True))

def _get_token_from_cache(scope=None):
    cache = _load_cache()  # This web app maintains one cache per session
    cca = _build_msal_app(cache=cache)
    accounts = cca.get_accounts()
    if accounts:  # So all account(s) belong to the current signed-in user
        result = cca.acquire_token_silent(scope, account=accounts[0])
        _save_cache(cache)
        return result

app.jinja_env.globals.update(_build_auth_url=_build_auth_url)  # Used in template

if __name__ == "__main__":
    app.run()


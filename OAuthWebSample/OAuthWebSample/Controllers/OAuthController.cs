using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OAuthSample.Models;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace OAuthSample.Controllers
{
    public class OAuthController : Controller
    {
        private static readonly HttpClient s_httpClient = new HttpClient();
        private static readonly Dictionary<Guid, TokenModel> s_authorizationRequests = new Dictionary<Guid, TokenModel>();

        /// <summary>
        /// Start a new authorization request. 
        /// 
        /// This creates a random state value that is used to correlate/validate the request in the callback later.
        /// </summary>
        /// <returns></returns>
        public ActionResult Authorize()
        {
            Guid state = Guid.NewGuid();

            s_authorizationRequests[state] = new TokenModel() { IsPending = true };
            
            return new RedirectResult(GetAuthorizationUrl(state.ToString()));
        }

        /// <summary>
        /// Constructs an authorization URL with the specified state value.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private static String GetAuthorizationUrl(String state)
        {
            UriBuilder uriBuilder = new UriBuilder(ConfigurationManager.AppSettings["AuthUrl"]);
            var queryParams = HttpUtility.ParseQueryString(uriBuilder.Query ?? String.Empty);

            queryParams["client_id"] = ConfigurationManager.AppSettings["ClientAppId"];
            queryParams["response_type"] = "Assertion";
            queryParams["state"] = state;
            queryParams["scope"] = ConfigurationManager.AppSettings["Scope"];
            queryParams["redirect_uri"] = ConfigurationManager.AppSettings["CallbackUrl"];

            uriBuilder.Query = queryParams.ToString();

            return uriBuilder.ToString();
        }

        /// <summary>
        /// Callback action. Invoked after the user has authorized the app.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task<ActionResult> Callback(String code, Guid state)
        {
            String error;
            if (ValidateCallbackValues(code, state.ToString(), out error))
            {
                // Exchange the auth code for an access token and refresh token
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, ConfigurationManager.AppSettings["TokenUrl"]);
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Dictionary<String, String> form = new Dictionary<String, String>()
                {
                    { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
                    { "client_assertion", ConfigurationManager.AppSettings["ClientAppSecret"] },
                    { "grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer" },
                    { "assertion", code },
                    { "redirect_uri", ConfigurationManager.AppSettings["CallbackUrl"] }
                };
                requestMessage.Content = new FormUrlEncodedContent(form);

                HttpResponseMessage responseMessage = await s_httpClient.SendAsync(requestMessage);

                if (responseMessage.IsSuccessStatusCode)
                {
                    String body = await responseMessage.Content.ReadAsStringAsync();

                    TokenModel tokenModel = s_authorizationRequests[state];
                    JsonConvert.PopulateObject(body, tokenModel);
                    
                    ViewBag.Token = tokenModel;
                }
                else
                {
                    error = responseMessage.ReasonPhrase;
                }
            }

            if (!String.IsNullOrEmpty(error))
            {
                ViewBag.Error = error;
            }

            ViewBag.ProfileUrl = ConfigurationManager.AppSettings["ProfileUrl"];

            return View("TokenView");
        }

        /// <summary>
        /// Ensures the specified auth code and state value are valid. If both are valid, the state value is marked so it can't be used again.        
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private static bool ValidateCallbackValues(String code, String state, out String error)
        {
            error = null;

            if (String.IsNullOrEmpty(code))
            {
                error = "Invalid auth code";
            }
            else
            {
                Guid authorizationRequestKey;
                if (!Guid.TryParse(state, out authorizationRequestKey))
                {
                    error = "Invalid authorization request key";
                }
                else
                {
                    TokenModel tokenModel;
                    if (!s_authorizationRequests.TryGetValue(authorizationRequestKey, out tokenModel))
                    {
                        error = "Unknown authorization request key";
                    }
                    else if (!tokenModel.IsPending)
                    {
                        error = "Authorization request key already used";
                    }
                    else
                    {
                        s_authorizationRequests[authorizationRequestKey].IsPending = false; // mark the state value as used so it can't be reused
                    }
                }
            }

            return error == null;
        }
    
        /// <summary>
        /// Gets a new access
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<ActionResult> RefreshToken(string refreshToken)
        {
            String error = null;
            if (!String.IsNullOrEmpty(refreshToken))
            {
                // Form the request to exchange an auth code for an access token and refresh token
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, ConfigurationManager.AppSettings["TokenUrl"]);
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Dictionary<String, String> form = new Dictionary<String, String>()
                {
                    { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
                    { "client_assertion", ConfigurationManager.AppSettings["ClientAppSecret"] },
                    { "grant_type", "refresh_token" },
                    { "assertion", refreshToken },
                    { "redirect_uri", ConfigurationManager.AppSettings["CallbackUrl"] }
                };
                requestMessage.Content = new FormUrlEncodedContent(form);

                // Make the request to exchange the auth code for an access token (and refresh token)
                HttpResponseMessage responseMessage = await s_httpClient.SendAsync(requestMessage);

                if (responseMessage.IsSuccessStatusCode)
                {
                    // Handle successful request
                    String body = await responseMessage.Content.ReadAsStringAsync();
                    ViewBag.Token = JObject.Parse(body).ToObject<TokenModel>();
                }
                else
                {
                    error = responseMessage.ReasonPhrase;
                }
            }
            else
            {
                error = "Invalid refresh token";
            }

            if (!String.IsNullOrEmpty(error))
            {
                ViewBag.Error = error;
            }

            return View("TokenView");
        }       
    }
}

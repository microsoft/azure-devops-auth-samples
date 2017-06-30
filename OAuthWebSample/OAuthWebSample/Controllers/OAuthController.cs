using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OAuthSample.Models;

namespace OAuthSample.Controllers
{
    public class OAuthController : Controller
    {
        //
        // GET: /OAuth/
        public ActionResult Index()
        {

            return View();

        }

        public ActionResult RequestToken(string code, string status)
        {
            return new RedirectResult(GenerateAuthorizeUrl());
        }

        public ActionResult RefreshToken(string refreshToken)
        {
            TokenModel token = new TokenModel();
            String error = null;

            if (!String.IsNullOrEmpty(refreshToken))
            {
                error = PerformTokenRequest(GenerateRefreshPostData(refreshToken), out token);
                if (String.IsNullOrEmpty(error))
                {
                    ViewBag.Token = token;
                }
            }

            ViewBag.Error = error;

            return View("TokenView");
        }

        public ActionResult Callback(string code, string state)
        {
            TokenModel token = new TokenModel();
            String error = null;

            if (!String.IsNullOrEmpty(code))
            {
                error = PerformTokenRequest(GenerateRequestPostData(code), out token);
                if (String.IsNullOrEmpty(error))
                {
                    ViewBag.Token = token;
                }
            }

            ViewBag.Error = error;

            return View("TokenView");
        }

        private String PerformTokenRequest(String postData, out TokenModel token)
        {
            var error = String.Empty;
            var strResponseData = String.Empty;

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(
                ConfigurationManager.AppSettings["TokenUrl"]
                );

            webRequest.Method = "POST";
            webRequest.ContentLength = postData.Length;
            webRequest.ContentType = "application/x-www-form-urlencoded";

            using (StreamWriter swRequestWriter = new StreamWriter(webRequest.GetRequestStream()))
            {
                swRequestWriter.Write(postData);
            }

            try
            {
                HttpWebResponse hwrWebResponse = (HttpWebResponse)webRequest.GetResponse();

                if (hwrWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader srResponseReader = new StreamReader(hwrWebResponse.GetResponseStream()))
                    {
                        strResponseData = srResponseReader.ReadToEnd();
                    }

                    token = JsonConvert.DeserializeObject<TokenModel>(strResponseData);
                    return null;
                }
            }
            catch (WebException wex)
            {
                error = "Request Issue: " + wex.Message;
            }
            catch (Exception ex)
            {
                error = "Issue: " + ex.Message;
            }

            token = new TokenModel();
            return error;
        }

        public String GenerateAuthorizeUrl()
        {
            UriBuilder uriBuilder = new UriBuilder(ConfigurationManager.AppSettings["AuthUrl"]);
            var queryParams = HttpUtility.ParseQueryString(uriBuilder.Query ?? String.Empty);

            queryParams["client_id"] = ConfigurationManager.AppSettings["AppId"];
            queryParams["response_type"] = "Assertion";
            queryParams["state"] = "state";
            queryParams["scope"] = ConfigurationManager.AppSettings["Scope"];
            queryParams["redirect_uri"] = ConfigurationManager.AppSettings["CallbackUrl"];

            uriBuilder.Query = queryParams.ToString();

            return uriBuilder.ToString();
        }

        public string GenerateRequestPostData(string code)
        {
            return string.Format("client_assertion_type=urn:ietf:params:oauth:client-assertion-type:jwt-bearer&client_assertion={0}&grant_type=urn:ietf:params:oauth:grant-type:jwt-bearer&assertion={1}&redirect_uri={2}",
                HttpUtility.UrlEncode(ConfigurationManager.AppSettings["AppSecret"]),
                HttpUtility.UrlEncode(code),
                ConfigurationManager.AppSettings["CallbackUrl"]
                );
        }

        public string GenerateRefreshPostData(string refreshToken)
        {
            return string.Format("client_assertion_type=urn:ietf:params:oauth:client-assertion-type:jwt-bearer&client_assertion={0}&grant_type=refresh_token&assertion={1}&redirect_uri={2}",
                HttpUtility.UrlEncode(ConfigurationManager.AppSettings["AppSecret"]),
                HttpUtility.UrlEncode(refreshToken),
                ConfigurationManager.AppSettings["CallbackUrl"]
                );

        }
    }
}

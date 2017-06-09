using Newtonsoft.Json;
using OAuthSample.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

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

        public ActionResult Authorize(string code, string state)
        {
            var error = String.Empty;
            var strResponseData = String.Empty;
            var strPostData = String.Empty;

            if(!String.IsNullOrEmpty(code))
            {
                strPostData = GeneratePostData(code);

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(
                    ConfigurationManager.AppSettings["TokenUrl"]
                    );

                webRequest.Method = "POST";
                webRequest.ContentLength = strPostData.Length;
                webRequest.ContentType = "application/x-www-form-urlencoded";

                using (StreamWriter swRequestWriter = new StreamWriter(webRequest.GetRequestStream()))
                {
                    swRequestWriter.Write(strPostData);
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

                        TokenModel token = JsonConvert.DeserializeObject<TokenModel>(strResponseData);

                        ViewBag.Token = token;

                        return View("TokenView");
                    }
                }
                catch (WebException wex)
                {
                    error = "<strong>Request Issue:</strong> " + wex.Message.ToString();
                }
                catch (Exception ex)
                {
                    error = "<strong>Issue:</strong> " + ex.Message.ToString();
                }
            }
            else
            {
                error = "<strong>Issue:</strong> Empty authorization code";
            }

            TokenModel emptyToken = new TokenModel();
            emptyToken.Error = error;

            ViewBag.Token = emptyToken;

            return View("TokenView");
        }

        public String GenerateAuthorizeUrl()
        {
            //TODO: Add some form of state manager
            return String.Format("{0}?client_id={1}&response_type=Assertion&state={2}&scope=preview_api_all%20preview_msdn_licensing&redirect_uri={3}",
                ConfigurationManager.AppSettings["AuthUrl"],
                ConfigurationManager.AppSettings["AppId"],
                "state",
                ConfigurationManager.AppSettings["RedirectUrl"]
                );
        }

        public string GeneratePostData(string code)
        {
            return string.Format("client_assertion_type=urn:ietf:params:oauth:client-assertion-type:jwt-bearer&client_assertion={0}&grant_type=urn:ietf:params:oauth:grant-type:jwt-bearer&assertion={1}&redirect_uri={2}",
                HttpUtility.UrlEncode(ConfigurationManager.AppSettings["AppSecret"]),
                HttpUtility.UrlEncode(code),
                ConfigurationManager.AppSettings["RedirectUrl"]
                );

        }

	}
}
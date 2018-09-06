using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using OAuthWebSampleAspNetCore.Models;

namespace OAuthWebSampleAspNetCore.Controllers
{
    public class OAuthController : Controller
    {
        private static readonly HttpClient s_httpClient = new HttpClient();
        private static readonly Dictionary<Guid, Token> s_authorizationRequests = new Dictionary<Guid, Token>();

        private IConfiguration Configuration;
        private OAuthSettings Settings;
 
        public OAuthController(IConfiguration configuration)
        {
            this.Configuration = configuration;
            this.Settings = new OAuthSettings();
            Configuration.GetSection("oauth").Bind(this.Settings);
        }

        /// <summary>
        /// Start a new authorization request. 
        /// 
        /// This creates a random state value that is used to correlate/validate the request in the callback later.
        /// </summary>
        /// <returns></returns>
        public ActionResult Authorize()
        {
            Guid state = Guid.NewGuid();

            s_authorizationRequests[state] = new Token() { IsPending = true };
            
            return new RedirectResult(BuildAuthorizationUrl(state.ToString()));
        }

        /// <summary>
        /// Constructs an authorization URL with the specified state value.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private String BuildAuthorizationUrl(String state)
        {
            UriBuilder uriBuilder = new UriBuilder(this.Settings.AuthorizationUrl);
            var queryParams = HttpUtility.ParseQueryString(uriBuilder.Query ?? String.Empty);

            queryParams["client_id"] = this.Settings.ClientApp.Id.ToString();
            queryParams["response_type"] = "Assertion";
            queryParams["state"] = state;
            queryParams["scope"] = this.Settings.ClientApp.Scope;
            queryParams["redirect_uri"] = this.Settings.ClientApp.CallbackUrl;

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
            TokenViewModel tokenViewModel = new TokenViewModel() { OAuthSettings = this.Settings };
            
            string error;
            if (ValidateCallbackValues(code, state.ToString(), out error))
            {
                // Exchange the auth code for an access token and refresh token
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, this.Settings.TokenUrl);
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Dictionary<String, String> form = new Dictionary<String, String>()
                {
                    { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
                    { "client_assertion", GetClientAppSecret() },
                    { "grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer" },
                    { "assertion", code },
                    { "redirect_uri", this.Settings.ClientApp.CallbackUrl }
                };
                requestMessage.Content = new FormUrlEncodedContent(form);

                HttpResponseMessage responseMessage = await s_httpClient.SendAsync(requestMessage);

                if (responseMessage.IsSuccessStatusCode)
                {
                    String body = await responseMessage.Content.ReadAsStringAsync();

                    Token token = s_authorizationRequests[state];
                    JsonConvert.PopulateObject(body, token);
                    
                    tokenViewModel.Token = token;
                }
                else
                {
                    error = responseMessage.ReasonPhrase;
                }
            }
            else
            {
                tokenViewModel.Error = error;
            }

            return View("TokenView", tokenViewModel);
        }
    
        /// <summary>
        /// Gets a new access token and refresh token using the provided refresh token.
        /// Note: the client should never have access to the refresh token (it should only ever live on the server). This method is only for demonstration purposes.
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<ActionResult> RefreshToken(string refreshToken)
        {
            TokenViewModel tokenViewModel = new TokenViewModel() { OAuthSettings = this.Settings };
            
            if (!String.IsNullOrEmpty(refreshToken))
            {
                // Form the request to exchange an auth code for an access token and refresh token
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, this.Settings.TokenUrl);
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Dictionary<String, String> form = new Dictionary<String, String>()
                {
                    { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
                    { "client_assertion", GetClientAppSecret() },
                    { "grant_type", "refresh_token" },
                    { "assertion", refreshToken },
                    { "redirect_uri", this.Settings.ClientApp.CallbackUrl }
                };
                requestMessage.Content = new FormUrlEncodedContent(form);

                // Make the request to exchange the auth code for an access token (and refresh token)
                HttpResponseMessage responseMessage = await s_httpClient.SendAsync(requestMessage);

                if (responseMessage.IsSuccessStatusCode)
                {
                    // Handle successful request
                    String body = await responseMessage.Content.ReadAsStringAsync();
                    tokenViewModel.Token = JObject.Parse(body).ToObject<Token>();
                }
                else
                {
                    tokenViewModel.Error = responseMessage.ReasonPhrase;
                }
            }
            else
            {
                tokenViewModel.Error = "Invalid refresh token";
            }
            
            return View("TokenView", tokenViewModel);
        }

        /// <summary>
        /// Ensures the specified auth code and state value are valid. 
        /// If both are valid, the state is flagged so it can't be used again (isPending=false)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private bool ValidateCallbackValues(String code, String state, out String error)
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
                    Token tokenModel;
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

        private string GetClientAppSecret()
        {
            return this.Configuration.GetValue<string>("oauth:clientApp:secret");
        }
    }    
}

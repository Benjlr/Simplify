using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using TestApp.Utils;

namespace TestApp.Controllers
{
    [Authorize]
    public class NumbersController : Controller
    {
        private String apiEndpoint = Globals.NumberWordService + @"/api/numbers/";

        //implementation based on https://github.com/Azure-Samples/active-directory-b2c-dotnet-webapp-and-webapi

        public async Task<ActionResult> Index()
        {
            try
            {
                // Retrieve the token with the specified scopes
                var scope = new string[] { Globals.ReadTasksScope };

                IConfidentialClientApplication cca = MsalAppBuilder.BuildConfidentialClientApplication();
                var accounts = await cca.GetAccountsAsync();
                AuthenticationResult result = await cca.AcquireTokenSilent(scope, accounts.FirstOrDefault()).ExecuteAsync();

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiEndpoint);

                // Add token to the Authorization header and make the request
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                HttpResponseMessage response = await client.SendAsync(request);

                // Handle the response
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        String responseString = await response.Content.ReadAsStringAsync();
                        JArray Nws = JArray.Parse(responseString);
                        ViewBag.Numbers = Nws;
                        return View();
                    case HttpStatusCode.Unauthorized:
                        return ErrorAction("Please sign in again. " + response.ReasonPhrase);
                    default:
                        return ErrorAction("Error. Status code = " + response.StatusCode + ": " + response.ReasonPhrase);
                }
            }
            catch (MsalUiRequiredException ex)
            {
                /*
                    If the tokens have expired or become invalid for any reason, ask the user to sign in again.
                    Another cause of this exception is when you restart the app using InMemory cache.
                    It will get wiped out while the user will be authenticated still because of their cookies, requiring the TokenCache to be initialized again
                    through the sign in flow.
                */
                return new RedirectResult("/Account/SignUpSignIn?redirectUrl=/Numbers");
            }
            catch (Exception ex)
            {
                return ErrorAction("Error reading to do list: " + ex.Message);
            }
        }


        [HttpPost]
        public async Task<ActionResult> Create(string description)
        {
            try
            {

                ///
                /// Number validation occurs here, should be moved to external class
                /// 
                if (System.Text.RegularExpressions.Regex.IsMatch(description, @"^[a-zA-Z]+$")) throw new WebException("Please provide a number");
                if (String.IsNullOrEmpty(description)) throw new WebException("Please provide a number");
                if (description.Contains('.') && description.Split('.')[1].Length > 4) throw new WebException("The number can only have a precision of up to 4 decimal places");
                if (description.Length >= 11) throw new WebException("number contains too many digits");


                string accessToken = null;
                var scope = new string[] { Globals.WriteTasksScope };

                IConfidentialClientApplication cca = MsalAppBuilder.BuildConfidentialClientApplication();
                var accounts = await cca.GetAccountsAsync();
                AuthenticationResult result = await cca.AcquireTokenSilent(scope, accounts.FirstOrDefault()).ExecuteAsync();
                accessToken = result.AccessToken;

                // Set the content
                var httpContent = new[] { new KeyValuePair<string, string>("InputNumber", description) };
                
                // Create the request
                HttpClient client = new HttpClient();
                HttpContent content = new FormUrlEncodedContent(httpContent);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiEndpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                request.Content = content;
                HttpResponseMessage response = await client.SendAsync(request);

                // Handle the response
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.NoContent:
                    case HttpStatusCode.Created:
                        return new RedirectResult("/Numbers");
                    case HttpStatusCode.Unauthorized:
                        return ErrorAction("Please sign in again. " + response.ReasonPhrase);
                    default:
                        return ErrorAction("Error. Status code = " + response.StatusCode);
                }
            }
            catch (MsalUiRequiredException ex)
            {
                /*
                    If the tokens have expired or become invalid for any reason, ask the user to sign in again.
                    Another cause of this exception is when you restart the app using InMemory cache.
                    It will get wiped out while the user will be authenticated still because of their cookies, requiring the TokenCache to be initialized again
                    through the sign in flow.
                */
                return new RedirectResult("/Account/SignUpSignIn?redirectUrl=/Numbers");
            }
            catch (Exception ex)
            {
                return ErrorAction("Error writing to list: " + ex.Message);
            }
        }

        /// <summary>
        /// Generates redirect to error html with a error message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private ActionResult ErrorAction(String message)
        {
            return new RedirectResult("/Error?message=" + message);
        }

    }
}

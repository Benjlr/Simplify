using SimpFinanceService.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace SimpFinanceService.Controllers
{
    /// <summary>
    /// The server side controller for the api, implemented from https://github.com/Azure-Samples/active-directory-b2c-dotnet-webapp-and-webapi
    /// </summary>



    public class NumbersController : ApiController
    {
        private readonly Logic _logic;

        public NumbersController()
        {
            _logic = new Logic();
        }

        

        // gets the scope authorisations of the logged in user
        public const string scopeElement = "http://schemas.microsoft.com/identity/claims/scope";
        public const string objectIdElement = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        // API Scopes
        public static string ReadPermission = ConfigurationManager.AppSettings["api:ReadScope"];
        public static string WritePermission = ConfigurationManager.AppSettings["api:WriteScope"];

        /// <summary>
        /// Returns a list of recorded NumberToWord objects from the database
        /// </summary>
        /// <returns></returns>
        public IEnumerable<NumberToWord> Get()
        {
            HasRequiredScopes(ReadPermission);
            var owner = CheckClaimMatch(ClaimTypes.NameIdentifier);
            IEnumerable<NumberToWord> userTasks = _logic.GetAll();
            return userTasks;
        }

        /// <summary>
        /// Adds a new numbertoWord object to the database
        /// </summary>
        /// <param name="nw">This value is taken from the Encoded content of the url</param>
        /// <returns></returns>
        public  IHttpActionResult Post(NumberToWord nw)
        {
            HasRequiredScopes(WritePermission);
            var owner = CheckClaimMatch(ClaimTypes.NameIdentifier);


            _logic.GenerateWord(nw.InputNumber);
            return Ok();
        }


        /// <summary>
        /// Validates the current user claims
        /// </summary>
        /// <param name="claim"></param>
        /// <returns></returns>
        private string CheckClaimMatch(string claim)
        {
            try
            {
                return ClaimsPrincipal.Current.FindFirst(claim).Value;
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = $"Unable to match claim '{claim}' against user claims; click the 'claims' tab to double-check. {e.Message}"
                });
            }
        }

        /// <summary>
        /// Validates the user has relevant permission
        /// </summary>
        /// <param name="permission">The permission</param>
        private void HasRequiredScopes(String permission)
        {
            if (!ClaimsPrincipal.Current.FindFirst(scopeElement).Value.Contains(permission))
            {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    ReasonPhrase = $"The Scope claim does not contain the {permission} permission."
                });
            }
        }

    }
}

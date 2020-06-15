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
    //[Authorize]
    public class NumbersController : ApiController
    {
        private readonly Logic _logic;

        public NumbersController()
        {
            _logic = new Logic();
        }


        // OWIN auth middleware constants -> These claims must match what's in your JWT, like for like. Click the 'claims' tab to check.
        public const string scopeElement = "http://schemas.microsoft.com/identity/claims/scope";
        public const string objectIdElement = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        // API Scopes
        public static string ReadPermission = ConfigurationManager.AppSettings["api:ReadScope"];
        public static string WritePermission = ConfigurationManager.AppSettings["api:WriteScope"];

        /*
         * GET all tasks for user
         */
        public IEnumerable<NumberToWord> Get()
        {
            HasRequiredScopes(ReadPermission);

            var owner = CheckClaimMatch(ClaimTypes.NameIdentifier);

            IEnumerable<NumberToWord> userTasks = _logic.GetAll();
            return userTasks;
        }

        /*
        * POST a new numberword for user
        */
        public  IHttpActionResult Post(NumberToWord nw)
        {
            HasRequiredScopes(WritePermission);
            var owner = CheckClaimMatch(ClaimTypes.NameIdentifier);


            _logic.GenerateWord(nw.InputNumber);
            return Ok();
        }


        /*
         * Check user claims match task details
         */
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

        // Validate to ensure the necessary scopes are present.
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

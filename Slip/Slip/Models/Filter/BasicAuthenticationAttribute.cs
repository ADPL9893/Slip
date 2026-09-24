using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Slip.Models.Filter
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            // Check if Authorization header exists
            if (actionContext.Request.Headers.Authorization != null)
            {
                string authToken = actionContext.Request.Headers.Authorization.Parameter;

                // Decode "username:password"
                string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(authToken));
                string[] parts = decoded.Split(':');

                if (parts.Length != 2)
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    actionContext.Response.Headers.Add("WWW-Authenticate", "Basic");
                    return;
                }

                string username = parts[0];
                string password = parts[1];

                // Validate
                if (IsAuthorizedUser(username, password))
                {
                    // Set principal (API)
                    var identity = new GenericIdentity(username);
                    var principal = new GenericPrincipal(identity, null);

                    Thread.CurrentPrincipal = principal;

                    // Set for MVC context
                    if (HttpContext.Current != null)
                    {
                        HttpContext.Current.User = principal;
                    }

                    return; // IMPORTANT
                }
                else
                {
                    actionContext.Response = new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.Forbidden,
                        Content = new StringContent("Your UserName Or Password is incorrect.")
                    };
                    return;
                }
            }
            else
            {
                // No Authorization header
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                actionContext.Response.Headers.Add("WWW-Authenticate", "Basic");
            }
        }

        private bool IsAuthorizedUser(string Username, string Password)
        {
            // Replace with DB check if needed
            return Username == "AnjaliTech" && Password == "Anj@9893";
        }
    }
}
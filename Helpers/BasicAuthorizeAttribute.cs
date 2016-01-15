using Microsoft.AspNet.Mvc.Filters;
using Microsoft.AspNet.Server.Kestrel.Http;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNet.Mvc;
using AuthorizationContext = Microsoft.AspNet.Mvc.Filters.AuthorizationContext;

namespace ClientDirectory.Helpers
{
    public class BasicAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private IConfigurationRoot _configuration { get; set; }
        private Enums.ClientRole _currentRole { get; set; }
        private string _managerToken { get; set; }
        private string _regularClientToken { get; set; }

        public BasicAuthorizeAttribute(Enums.ClientRole role)
        {
            // TODO: This should be handled by the IdentityServer
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            _configuration = builder.Build();

            _currentRole = role;
            _managerToken = _configuration.GetSection("Tokens:Manager").Value;
            _regularClientToken = _configuration.GetSection("Tokens:RegularClient").Value;
        }
        
        public void OnAuthorization(AuthorizationContext context)
        {
            if (context.HttpContext.Request.Method == "OPTIONS")
            {
                return;
            }

            var authToken = ((FrameRequestHeaders)context.HttpContext.Request.Headers).HeaderAuthorization;

            if (Authorize(authToken) == false)
            {
                context.Result = new HttpStatusCodeResult(403);
                return;
            }

            if (AllowAction(authToken) == false)
            {
                context.Result = new HttpStatusCodeResult(403);
                return;
            }
        }

        private bool AllowAction(string authToken)
        {
            // Here we should take the AuthToken and compare it with the database (IdentityServer)
            // and see if that user has this permission. For the purpose of this demo, we are going
            // to compare it with the local token, so this is insecure.

            if (authToken == _managerToken && _currentRole == Enums.ClientRole.Manager)
            {
                return true;
            }

            if (authToken == _regularClientToken && _currentRole == Enums.ClientRole.RegularClient)
            {
                return true;
            }

            if ((authToken == _regularClientToken || authToken == _managerToken) && _currentRole == Enums.ClientRole.Any)
            {
                return true;
            }

            return false;
        }

        private bool Authorize(string authToken)
        {
            if (authToken != _regularClientToken && authToken != _managerToken)
            {
                return false;
            }

            return true;
        }
    }
}
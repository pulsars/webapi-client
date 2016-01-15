using ClientDirectory.Interfaces;
using ClientDirectory.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Cors;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace ClientDirectory.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly ClientDirectoryContext _context;
        private readonly ILoginHelper _loginHelper;

        public LoginController(ClientDirectoryContext context, ILoginHelper loginHelper)
        {
            _context = context;
            _loginHelper = loginHelper;
        }

        public IConfigurationRoot Configuration { get; set; }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Login login)
        {
            // Try to find the login information.
            var loginData =
                await
                    _context.Login.FirstOrDefaultAsync(
                        a => String.Equals(a.Username, login.Username, StringComparison.CurrentCultureIgnoreCase));

            // User may not exist.
            if (loginData == null)
            {
                return HttpBadRequest("Incorrect Username/Password");
            }

            // Get the client information.
            var client = await _context.Client.FirstOrDefaultAsync(e => e.Id == loginData.IdClient);

            // Client doesn't exist or it is 'Deleted'.
            if (client == null || client.Active == false)
            {
                return HttpBadRequest("Incorrect Username/Password");
            }

            // Verify credentials.
            var isValidLogin = _loginHelper.IsValidLogin(loginData.Password, loginData.Salt, login.Password);

            if (isValidLogin == false)
            {
                return HttpBadRequest("Incorrect Username/Password");
            }

            var clientRole = (Enums.ClientRole)client.IdClientRole;
            var response = new AuthResponse
            {
                Id = client.Id,
                Name = client.Name,
                AuthToken = GetClientToken(clientRole),
                CanManageClients = GetClientPermissions(clientRole)
            };

            return Ok(response);
        }

        /// <summary>
        // Set the Permissions for the current role.
        // TODO: This should be handled by the IdentityServer
        /// </summary>
        private bool GetClientPermissions(Enums.ClientRole clientRole)
        {
            if (clientRole == Enums.ClientRole.Manager)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        // Set the Token based on the current user role.
        // TODO: This should be handled by the IdentityServer, but for
        // purposes of this assesment will be handled like this.
        /// </summary>
        private string GetClientToken(Enums.ClientRole clientRole)
        {
            // Set the Token based on the current user role.
            // TODO: This should be handled by the IdentityServer, but for
            // purposes of this assesment will be handled like this.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            if (clientRole == Enums.ClientRole.Manager)
            {
                return Configuration.GetSection("Tokens:Manager").Value;
            }

            return Configuration.GetSection("Tokens:RegularClient").Value;
        }
    }
}
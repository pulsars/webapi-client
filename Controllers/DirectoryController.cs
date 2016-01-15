using ClientDirectory.Interfaces;
using ClientDirectory.Models;
using Microsoft.AspNet.Cors;
using Microsoft.AspNet.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using ClientDirectory.Helpers;
using Microsoft.AspNet.Http;
using Microsoft.Data.Entity;

namespace ClientDirectory.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    public class DirectoryController : Controller
    {
        private readonly ClientDirectoryContext _context;
        private readonly ILoginHelper _loginHelper;
        private readonly IHttpContextAccessor _contextAccessor;

        public DirectoryController(ClientDirectoryContext context, ILoginHelper loginHelper,
            IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _loginHelper = loginHelper;
        }

        [BasicAuthorize(Enums.ClientRole.Manager)]
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddClient([FromBody] Client client)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    client.Id = Guid.NewGuid();

                    // At this point only regular clients can be created.
                    client.IdClientRole = 1;

                    var clientLogin = _loginHelper.CreateNewLogin(client.Id, client.Name);

                    _context.Client.Add(client);
                    _context.Login.Add(clientLogin);

                    await _context.SaveChangesAsync();

                    transaction.Commit();
                    return Ok("Client succesfully added");
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return HttpBadRequest("Could not create the client");
                }
            }
        }

        [BasicAuthorize(Enums.ClientRole.Manager)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpBadRequest("The id of the client is required");
            }

            // Validates if the Guid is valid.
            Guid clientId;

            if (Guid.TryParse(id, out clientId) == false)
            {
                return HttpBadRequest("Invalid client id");
            }

            // Find the client with the provided id.
            var client = await _context.Client.FirstOrDefaultAsync(e => e.Id == clientId);

            if (client == null)
            {
                return HttpNotFound("Client does not exist");
            }

            client.Active = false;

            _context.Entry(client).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [BasicAuthorize(Enums.ClientRole.Any)]
        [HttpPost]
        [Route("getClients")]
        public async Task<IActionResult> Get()
        {
            var request = _contextAccessor.HttpContext.Request.Form;
            var response = new DataTablesResponse();

            var draw = int.Parse(request["draw"]);
            var start = int.Parse(request["start"]);
            var length = int.Parse(request["length"]);
            var searchTerm = request["search[value]"][0];

            // Require at least 3 characters in order to do a search.
            if (searchTerm.Length > 1 && searchTerm.Length < 3)
            {
                return Ok(response);
            }

            // Search without term.
            if (string.IsNullOrEmpty(searchTerm))
            {
                response.Data = await (from a in _context.Client
                                       where a.Active orderby a.Name ascending
                                       select a).Skip(start).Take(length).ToListAsync();

                response.RecordsFiltered = await (from a in _context.Client
                                                  where a.Active
                                                  select a.Id).CountAsync();
            }
            else
            {
                // Search using the provided term.
                response.Data = await (from a in _context.Client
                    where a.Active &&
                          ((a.Name.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) != -1) ||
                          (a.JobTitle.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) != -1) || 
                          (a.Location.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) != -1))
                          orderby a.Name ascending
                    select a).Skip(start).Take(length).ToListAsync();

                response.RecordsFiltered = await (from a in _context.Client
                                                  where a.Active &&
                                                        ((a.Name.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) != -1) ||
                                                        (a.JobTitle.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) != -1) ||
                                                        (a.Location.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) != -1))
                                                  select a.Id).CountAsync();
            }

            response.Draw = (draw + 1);
            response.RecordsTotal = length;

            return Ok(response);
        }

        [BasicAuthorize(Enums.ClientRole.Any)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpBadRequest("The id of the client is required");
            }

            // Validates if the Guid is valid.
            Guid clientId;

            if (Guid.TryParse(id, out clientId) == false)
            {
                return HttpBadRequest("Invalid client id");
            }

            // Find the client with the provided id.
            var client = await _context.Client.FirstOrDefaultAsync(e => e.Id == clientId);

            if (client == null)
            {
                return Ok("Client not found");
            }

            return Ok(client);
        }

        [BasicAuthorize(Enums.ClientRole.Manager)]
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> UpdateClient([FromBody] Client client)
        {
            _context.Entry(client).State = EntityState.Modified;
            _context.Entry(client).Property(e => e.IdClientRole).IsModified = false;

            await _context.SaveChangesAsync();

            return Ok("Client succesfully updated");
        }
    }
}
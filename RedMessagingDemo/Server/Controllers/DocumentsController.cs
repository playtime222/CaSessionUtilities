using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedMessagingDemo.Server.Data;
using RedMessagingDemo.Shared;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RedMessagingDemo.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {

        // GET: api/<DocumentsController>
        [HttpGet]
        public async Task<DocumentList> GetAsync([FromServices]ApplicationDbContext db)
        {
            var docs = db.Documents
                .Where(x => x.Owner.Id == this.GetUserId())
                .Select(x => new DocumentListItem { Id = x.Id, DisplayName = x.DisplayName })
                .ToArray();

            return new DocumentList()
            {
                Items = docs
            };
        }

        // GET api/<DocumentsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        //Document enrolment result
        // POST api/<DocumentsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // DELETE api/<DocumentsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities.Encoders;
using RedMessagingDemo.Server.Data;
using RedMessagingDemo.Shared;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RedMessagingDemo.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {

        // GET: api/<DocumentsController>
        [HttpGet]
        public async Task<ReceivedMessageList> GetAsync([FromServices]ApplicationDbContext db)
        {
            var items = db.Messages
                .Where(x => x.Document.Owner.Id == this.GetUserId())
                .Select(x => new ReceivedMessageListItem { Id = x.Id, Note= x.Note, SenderEmail = x.FromUser.Email, WhenSent = x.WhenSent})
                .ToArray();

            return new ReceivedMessageList()
            {
                Items = items
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
        [HttpPost("send")]
        public async Task Post([FromServices] ApplicationDbContext db, [FromBody] MessageSendRequestArgs messageRequestArgs)
        {

            var sender = await db.ApplicationUsers.FindAsync(this.GetUserId());
            var receiver = await db.Documents.FindAsync(messageRequestArgs.Receiver);

            var msg = new Models.Message()
            {
                Document = receiver,
                Content = Base64.Decode(messageRequestArgs.MessageBase64),
                FromUser = sender,
                WhenSent = DateTime.Now,
                Note = messageRequestArgs.Note
            };

            await db.Messages.AddAsync(msg);
            await db.SaveChangesAsync();
        }

        // DELETE api/<DocumentsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

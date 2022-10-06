using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities.Encoders;
using RedMessagingDemo.Server.Commands;
using RedMessagingDemo.Server.Data;
using RedMessagingDemo.Server.Models;
using RedMessagingDemo.Shared;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RedMessagingDemo.Server.Controllers.MobileDevices
{
    [Route("api/mobiledevices/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        // GET api/receivers/{id}
        [HttpGet("receivers/{id}")]
        public async Task<ActionResult<ReceiverDocument>> GetReceiverAsync([FromRoute] long id, [FromServices] FindSingleReceiverDocumentCommand findDoc, [FromServices] FindUserFromBearerTokenCommand findUser, [FromHeader] string authorize)
        {
            if (!findUser.TryGet(authorize, out _))
                Unauthorized();

            var doc = await findDoc.ExecuteAsync(id);

            if (doc == null)
                return NotFound();

            return doc;
        }

        // GET api/documents/receivers
        [HttpGet("receivers")]
        public async Task<ReceiverDocumentList> GetReceiversAsync([FromServices] FindReceiverDocumentsCommand findReceivers, [FromServices] FindUserFromBearerTokenCommand findUser, [FromHeader] string authorize)
        {
            if (!findUser.TryGet(authorize, out _))
                Unauthorized();

            return await findReceivers.ExecuteAsync();
        }


        // GET: api/<DocumentsController>
        [HttpGet]
        public async Task<DocumentList> GetAsync([FromServices] ListDocumentsByUserCommand listDocuments, [FromServices] FindUserFromBearerTokenCommand findUser, [FromHeader] string authorize)
        {
            if (!findUser.TryGet(authorize, out var user))
                Unauthorized();

            return await listDocuments.ExecuteAsync(user!.Id);
        }

        //Document args result
        // POST api/<DocumentsController>
        [HttpPost]
        public async Task<DocumentEnrolmentResponse> EnrolAsync([FromBody] DocumentEnrolmentRequestArgs args, [FromServices] EnrolDocumentCommand cmd, [FromServices] FindUserFromBearerTokenCommand findUser, [FromHeader] string authorize)
        {
            if (!findUser.TryGet(authorize, out var user))
                Unauthorized();

            return await cmd.Enrol(args, user!.Id);
        }
    }
}

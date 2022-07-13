using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities.Encoders;
using RedMessagingDemo.Server.Commands;
using RedMessagingDemo.Server.Data;
using RedMessagingDemo.Shared;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860


namespace RedMessagingDemo.Server.Controllers.Web;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class DocumentsController : ControllerBase
{
    // GET api/receivers/{id}
    [HttpGet("receivers/{id}")]
    public async Task<ActionResult<ReceiverDocument>> GetReceiverAsync([FromRoute] long id, [FromServices] FindSingleReceiverDocumentCommand findDoc)
    {
        var doc = await findDoc.ExecuteAsync(id);

        if (doc == null)
            return NotFound();

        return doc;
    }

    // GET api/documents/receivers
    [HttpGet("receivers")]
    public async Task<ReceiverDocumentList> GetReceiversAsync([FromServices] FindReceiverDocumentsCommand findReceivers)
        => await findReceivers.ExecuteAsync();

    // GET: api/<DocumentsController>
    [HttpGet]
    public async Task<DocumentList> GetAsync([FromServices] ListDocumentsByUserCommand listDocuments)
    {
        return await listDocuments.ExecuteAsync(this.GetUserId());
    }

    //Document args result
    // POST api/<DocumentsController>
    [HttpPost]
    public async Task EnrolAsync([FromBody] DocumentEnrolmentRequestArgs args, [FromServices] EnrolDocumentCommand cmd)
    {
        await cmd.Enrol(args, this.GetUserId());
    }

}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedMessagingDemo.Server.Commands;
using RedMessagingDemo.Shared;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RedMessagingDemo.Server.Controllers.MobileDevices;

[Route("api/mobiledevices/[controller]")]
[ApiController]
public class MessagesController : ControllerBase
{

    // GET: api/<DocumentsController>
    [HttpGet]
    public async Task<ActionResult<DocumentList>> GetAsync([FromServices] ListDocumentsByUserCommand listDocuments, [FromServices] FindUserFromBearerTokenCommand findUser, [FromHeader] string authorize)
    {
        if (!findUser.TryGet(authorize, out var user))
            return Unauthorized();

        return await listDocuments.ExecuteAsync(user.Id);
    }

    [HttpGet("received")]
    public async Task<ActionResult<ReceivedMessageList>> GetListAsync([FromServices] ListMessagesByUserCommand listMessages, [FromServices] FindUserFromBearerTokenCommand findUser, [FromHeader] string authorize)
    {
        if (!findUser.TryGet(authorize, out var user))
            return Unauthorized();

        return await listMessages.Execute(user.Id);
    }

    [HttpGet("received/{id}")]
    public async Task<ActionResult<ReceivedMessage>> GetAsync(
        [FromServices] FindSingleMessageCommand findMessage,
        [FromRoute] long id, [FromServices] FindUserFromBearerTokenCommand findUser, [FromHeader] string authorize)
    {
        if (!findUser.TryGet(authorize, out var user))
            return Unauthorized();

        var item = await findMessage.ExecuteAsync(id, user.Id);

        //Simply return 404 as client is already authorised. Its just not their message.
        if (item == null)
            return NotFound();

        return item;
    }

    //Document enrolment result
    // POST api/<DocumentsController>
    [HttpPost]
    public async Task<ActionResult> Post([FromServices] SendMessageCommand sendMessage, [FromBody] MessageSendRequestArgs messageRequestArgs, [FromServices] FindUserFromBearerTokenCommand findUser, [FromHeader] string authorize)
    {
        if (!findUser.TryGet(authorize, out var user))
            return Unauthorized();

        return await sendMessage.ExecuteAsync(messageRequestArgs, user.Id);
    }
}
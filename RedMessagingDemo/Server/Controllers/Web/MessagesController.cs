using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedMessagingDemo.Server.Commands;
using RedMessagingDemo.Shared;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RedMessagingDemo.Server.Controllers.Web;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class MessagesController : ControllerBase
{
    [HttpGet("received")]
    public async Task<ActionResult<ReceivedMessageList>> GetListAsync([FromServices] ListMessagesByUserCommand listMessages)
        => await listMessages.Execute(this.GetUserId());

    [HttpGet("received/{id}")]
    public async Task<ActionResult<ReceivedMessage>> GetAsync(
        [FromServices] FindSingleMessageCommand findMessage,
        [FromRoute] long id)
    {
        var item = await findMessage.ExecuteAsync(id, this.GetUserId());

        //Simply return 404 as client is already authorised. Its just not their message.
        if (item == null)
            return NotFound();

        return item;
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromServices] SendMessageCommand sendMessage, [FromBody] MessageSendRequestArgs messageRequestArgs)
        => await sendMessage.ExecuteAsync(messageRequestArgs, this.GetUserId());
}

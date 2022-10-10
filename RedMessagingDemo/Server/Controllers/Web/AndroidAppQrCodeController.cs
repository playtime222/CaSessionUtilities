using System.Security.Claims;
using System.Text;
using Duende.IdentityServer;
using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RedMessagingDemo.Server.Commands;
using RedMessagingDemo.Shared;
using DownloadUriQrCodeResponse = RedMessagingDemo.Server.Commands.DownloadUriQrCodeResponse;

namespace RedMessagingDemo.Server.Controllers.Web;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class DownloadsController : ControllerBase
{
    /// <summary>
    /// To display as QR code in the client app.
    /// </summary>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<DownloadUriQrCodeResponse> GetToken([FromServices] CreateDownloadAsSvgCommand cmd, [FromRoute] string id)
        => await cmd.ExecuteAsync(id);
}
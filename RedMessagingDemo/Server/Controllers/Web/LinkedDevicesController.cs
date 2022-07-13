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

namespace RedMessagingDemo.Server.Controllers.Web;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class LinkedDevicesController : ControllerBase
{
    /// <summary>
    /// To display as QR code in the client app.
    /// </summary>
    /// <returns></returns>
    [HttpGet("token")]
    [Authorize]
    public async Task<MobileDeviceLinkResponse> GetToken([FromServices] CreateTokenAsSvgCommand cmd)
        => await cmd.ExecuteAsync(this.GetUserId());
}
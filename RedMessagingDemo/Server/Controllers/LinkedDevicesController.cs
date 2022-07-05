using System.Net;
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
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using RedMessagingDemo.Server.Data;
using RedMessagingDemo.Server.Models;
using RedMessagingDemo.Shared;
using ZXing;
using ZXing.QrCode;

namespace RedMessagingDemo.Server.Controllers;


[Route("api/[controller]")]
[ApiController]
public class LinkedDevicesController : ControllerBase
{
    [HttpGet("token")]
    [Authorize]
    /// <summary>
    /// To display as QR code in the client app.
    /// </summary>
    /// <returns></returns>
    public async Task<MobileDeviceLinkResponse> GetToken([FromServices] ApplicationDbContext db)
    {
        var randomBuffer = new byte[32];
        new SecureRandom().NextBytes(randomBuffer);
        var tokenValue = Hex.ToHexString(randomBuffer);

        var userId = this.GetUserId();
        var user = db.Users.Find(userId);
        db.FakeApiTokens.Add(new() { ApplicationUser = user, Token = tokenValue });
        await db.SaveChangesAsync();

        var writer = new BarcodeWriterSvg();
        writer.Format = BarcodeFormat.QR_CODE;
        writer.Options = new QrCodeEncodingOptions
        {
            DisableECI = true,
            CharacterSet = "UTF-8",
            Width = 150,
            Height = 150,
        };
        var image = writer.Write(tokenValue);
        var result = new MobileDeviceLinkResponse { Svg = WebUtility.HtmlEncode(image.Content) };
        return result;
    }

    //private async Task<string> GetCurrentUserToken(ITokenService tokenService, IUserClaimsPrincipalFactory<ApplicationUser> principalFactory,
    //    IdentityServerOptions options, ApplicationDbContext db)
    //{
    //    try
    //    {
    //        //ApplicationUserManager
    //        var request = new TokenCreationRequest();
    //        var userId = this.GetUserId();
    //        var user = db.Users.Find(userId);
    //        var identityPrincipal = await principalFactory.CreateAsync(user);
    //        var identityUser = new IdentityServerUser(user.Id.ToString());
    //        identityUser.AdditionalClaims = identityPrincipal.Claims.ToArray();
    //        identityUser.DisplayName = User.Claims.First(x => x.Type == ClaimTypes.Email).Value;
    //        identityUser.AuthenticationTime = System.DateTime.UtcNow;
    //        identityUser.IdentityProvider = IdentityServerConstants.LocalIdentityProvider;
    //        request.Subject = identityUser.CreatePrincipal();
    //        request.IncludeAllIdentityClaims = true;
    //        request.ValidatedRequest = new ValidatedRequest();
    //        request.ValidatedRequest.Subject = request.Subject;
    //        //request.ValidatedRequest.SetClient(Config.GetClients().First());
    //        //request.ValidatedResources= new Resources(Config.GetIdentityResources(), Config.GetApiResources());
    //        request.ValidatedResources = new();
    //        request.ValidatedRequest.Options = options;
    //        request.ValidatedRequest.ClientClaims = identityUser.AdditionalClaims;
    //        var token = await tokenService.CreateAccessTokenAsync(request);
    //        token.Issuer = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value;
    //        return await tokenService.CreateSecurityTokenAsync(token);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw;
    //    }
    //}

    bool TryGet(ApplicationDbContext db, string headerValue, out ApplicationUser? user)
    {
        if (!headerValue.StartsWith("bearer", StringComparison.InvariantCultureIgnoreCase))
        {
            user = null;
            return false;
        }

        var token = headerValue.Replace("bearer", "", StringComparison.InvariantCultureIgnoreCase).Trim();

        user = db.FakeApiTokens.Where(x => x.Token == token)
            .Select(x => x.ApplicationUser).FirstOrDefault();

        return user != null;
    }

    ////[Authorize(IdentityServerConstants.LocalApi.AuthenticationScheme)]
    //[HttpPost("{deviceId}")]
    ////For demo scenario, probably dont need this.
    //public ActionResult LinkDevice([FromRoute]string deviceId, [FromHeader]string token, [FromHeader(Name = "authorization")] string authHeader, [FromServices] ApplicationDbContext db)
    //{
    //    if (!TryGet(db, authHeader, out var user))
    //        return new ForbidResult();

    //    //Mobile device generates an id?
    //}

    //bearer 2d3c491515386a6a7ddaeac69f77b171ff916ef863bf4eb30b1fae01b91963d0

    //[Authorize(IdentityServerConstants.LocalApi.AuthenticationScheme)]
    [HttpGet("received")]
    public async Task<ActionResult<ReceivedMessageList>> GetListAsync([FromHeader] string authorize, [FromServices] ApplicationDbContext db)
    {
        if (!TryGet(db, authorize, out var user))
            return new ForbidResult();

        var items = await db.Messages
            .Where(x => x.Document.Owner.Id == user.Id)
            .Select(x => new ReceivedMessageListItem { Id = x.Id, Note = x.Note, SenderEmail = x.FromUser.Email, WhenSent = x.WhenSent })
            .ToArrayAsync();

        return new ReceivedMessageList()
        {
            Items = items
        };
    }

    //[Authorize(IdentityServerConstants.LocalApi.AuthenticationScheme)]
    [HttpGet("received/{id}")]
    public async Task<ActionResult<ReceivedMessage>> GetAsync([FromHeader] string authorize, [FromServices] ApplicationDbContext db, [FromRoute] long id)
    {
        if (!TryGet(db, authorize, out var user))
            return new ForbidResult();

        var item = await db.Messages
            .Where(x => x.Document.Owner.Id == user.Id && x.Id == id)
            .Select(x => new ReceivedMessage { Id = x.Id, Note = x.Note, SenderEmail = x.FromUser.Email, WhenSent = x.WhenSent, ContentBase64 = Base64.ToBase64String(x.Content) })
            .SingleOrDefaultAsync();

        //Simply return 404 as client is already authorised. Its just not their message.
        if (item == null)
            return NotFound();

        return item;
    }
}


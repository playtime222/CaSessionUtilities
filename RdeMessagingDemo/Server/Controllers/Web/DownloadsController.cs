using System.IO;
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
using Microsoft.AspNetCore.Mvc.Filters;
using RdeMessagingDemo.Server.Commands;
using RdeMessagingDemo.Shared;
using DownloadUriQrCodeResponse = RdeMessagingDemo.Server.Commands.DownloadUriQrCodeResponse;

namespace RdeMessagingDemo.Server.Controllers.Web;

[Route("api/[controller]")]
[ApiController]
public class DownloadsController : ControllerBase
{
    [HttpGet("{fileName}")]
    public IActionResult Download([FromRoute] string fileName)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "downloads", fileName);

        try
        {
            var memoryStream = new MemoryStream();

            using (var stream = new FileStream(filePath, FileMode.Open))
                stream.CopyTo(memoryStream);
            
            memoryStream.Position = 0;


            //// Get the MIMEType for the File
            //var mimeType = (string file) =>
            //{
            //    var mimeTypes = MimeTypes.GetMimeTypes();
            //    var extension = Path.GetExtension(file).ToLowerInvariant();
            //    return mimeTypes[extension];
            //};

            return File(memoryStream, "application/vnd.android.package-archive", Path.GetFileName(filePath));
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// To display as QR code in the client app.
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpGet("qr/{id}")]
    public async Task<DownloadUriQrCodeResponse> GetToken([FromServices] CreateDownloadAsSvgCommand cmd, [FromRoute] string id)
        => await cmd.ExecuteAsync(id);
}
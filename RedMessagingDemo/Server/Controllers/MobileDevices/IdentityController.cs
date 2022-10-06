using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RedMessagingDemo.Shared;

namespace RedMessagingDemo.Server.Controllers.MobileDevices;

[Route("api/[controller]")]
[ApiController]
public class IdentityController: ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IdentityDocument>> GetIdentityAsync([FromServices] IConfiguration configuration)
    {
        var baseUrl = configuration.GetValue(typeof(string), "BaseApiUri") as string ?? throw new InvalidOperationException("BaseApiUri setting not found.");

        return new IdentityDocument()
        {
            Services = new[]
            {
                new IdentityDocumentService
                {
                    Id="documents.add",
                    Url = $"{baseUrl}/documents",
                },
                new IdentityDocumentService
                {
                    Id="messages.list",
                    Url = $"{baseUrl}/messages/received",
                },
                new IdentityDocumentService
                {
                    Id="messages.getById",
                    Url = $"{baseUrl}/messages/received", //+/id
                },
            }
        };
    }
}
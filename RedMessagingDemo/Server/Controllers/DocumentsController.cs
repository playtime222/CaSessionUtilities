using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities.Encoders;
using RedMessagingDemo.Server.Data;
using RedMessagingDemo.Server.Models;
using RedMessagingDemo.Shared;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RedMessagingDemo.Server.Controllers
{

    public static class MappingEx
    {
        public static RedMessagingDemo.Shared.CaSessionArgs ToCaSessionArgs(this CaSessionUtilities.CaSessionArgs thiz)
                => new()
                {
                    ProtocolOid = thiz.ProtocolOid,
                    PublicKeyInfo = thiz.PublicKeyInfo.ToPublicKeyInfo()
                };

        public static RedMessagingDemo.Shared.ChipAuthenticationPublicKeyInfo ToPublicKeyInfo(this CaSessionUtilities.ChipAuthenticationPublicKeyInfo thiz)
                => new()
                {
                    PublicKeyBase64 = Base64.ToBase64String(thiz.PublicKey)
                }
        ;
    }

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        // GET: api/<DocumentsController>
        [HttpGet]
        public async Task<DocumentList> GetAsync([FromServices]ApplicationDbContext db)
        {
            var docs = db.Documents
                .Where(x => x.Owner.Id == this.GetUserId())
                .Select(x => new DocumentListItem { Id = x.Id, DisplayName = x.DisplayName })
                .ToArray();

            return new DocumentList()
            {
                Items = docs
            };
        }

        // GET api/receivers/{id}
        [HttpGet("receivers/{id}")]
        public async Task<ReceiverDocument> GetReceiversAsync([FromServices] ApplicationDbContext db, [FromRoute] long id)
        {
            Document? doc;
            try
            {
                doc = await db.Documents.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw;
            }


            var result = new ReceiverDocument()
            {
                CaSessionArgs = new() { ProtocolOid = doc.CaProtocolOid, PublicKeyInfo = new ChipAuthenticationPublicKeyInfo { PublicKeyBase64 = Base64.ToBase64String(Hex.Decode(doc.CaProtocolPublicKey)) } },
                FileContentBase64 = Base64.ToBase64String(doc.FileContent),
                FileShortId = doc.FileId,
                ReadLength = doc.FileReadLength
            };

            return result;
        }

        // GET api/documents/receivers
        [HttpGet("receivers")]
        public async Task<ReceiverDocumentList> GetReceiversAsync([FromServices] ApplicationDbContext db)
        {
            var docs = db.Documents
                .Where(x => x.Owner.Id == this.GetUserId())
                .Select(x => new ReceiverDocumentListItem { Id = x.Id, EmailAddress = x.Owner.Email, DisplayName = x.DisplayName })
                .ToArray();

            return new ReceiverDocumentList { Items = docs };
        }

        //Document enrolment result
        // POST api/<DocumentsController>
        [HttpPost]
        public void Post([FromBody] Enrolment enrolment, [FromServices] ApplicationDbContext db)
        {
            var user = db.Users.Single(x => x.Id == this.GetUserId());
            var doc = new Document
            {
                CaProtocolPublicKey = enrolment.ChipAuthenticationProtocolInfo.PublicKeyInfo.PublicKeyBase64,
                CaProtocolOid = enrolment.ChipAuthenticationProtocolInfo.ProtocolOid,
                DataGroup14 = Base64.Decode(enrolment.DataGroup14Base64),
                DisplayName = enrolment.DisplayName,
                FileContent= Base64.Decode(enrolment.FileContentsBase64),
                FileId = enrolment.FileId,
                FileReadLength = enrolment.FileReadLength,
                Owner = user,
            };
            db.Documents.Add(doc);
            db.SaveChanges();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Utilities.Encoders;
using RedMessagingDemo.Server.Data;
using RedMessagingDemo.Server.Models;
using RedMessagingDemo.Shared;

namespace RedMessagingDemo.Server.Commands;

public class EnrolDocumentCommand
{
    private readonly ApplicationDbContext _Db;

    public EnrolDocumentCommand(ApplicationDbContext db)
    {
        _Db = db;
    }

    public async Task<DocumentEnrolmentResponse> Enrol([FromBody] DocumentEnrolmentRequestArgs args, string userId)
    {
        try
        {
            var user = _Db.Users.Single(x => x.Id == userId);
            var doc = new Document
            {
                CaProtocolPublicKey = Hex.ToHexString(Base64UrlEncoder.DecodeBytes(args.ChipAuthenticationProtocolInfo.PublicKeyInfo.PublicKeyBase64)),
                CaProtocolOid = args.ChipAuthenticationProtocolInfo.ProtocolOid,
                DataGroup14 = Base64UrlEncoder.DecodeBytes(args.DataGroup14Base64),
                DisplayName = args.DisplayName,
                FileContent = Base64UrlEncoder.DecodeBytes(args.FileContentsBase64),
                FileId = args.FileId,
                FileReadLength = args.FileReadLength,
                Owner = user,
            };
            await _Db.Documents.AddAsync(doc);
            await _Db.SaveChangesAsync();
            return new() { Id = doc.Id };
        }
        catch (FormatException ex)
        {
            return new() { Message = "Failed" };
        }
        catch (DbUpdateException ex)
        {
            return new() { Message = "Failed" };
        }
    }

}
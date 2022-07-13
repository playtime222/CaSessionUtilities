using System.Net;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using RedMessagingDemo.Server.Data;
using RedMessagingDemo.Shared;
using ZXing;
using ZXing.QrCode;

namespace RedMessagingDemo.Server.Commands;

public class CreateTokenAsSvgCommand
{
    private readonly ApplicationDbContext _Db;

    public CreateTokenAsSvgCommand(ApplicationDbContext db)
    {
        _Db = db;
    }

    public async Task<MobileDeviceLinkResponse> ExecuteAsync(string userId)
    {
        var user = await _Db.Users.FindAsync(userId) ?? throw new InvalidOperationException("User not found.");

        var randomBuffer = new byte[32];
        new SecureRandom().NextBytes(randomBuffer);
        var tokenValue = Hex.ToHexString(randomBuffer);

        _Db.FakeApiTokens.Add(new() { ApplicationUser = user, Token = tokenValue });
        await _Db.SaveChangesAsync();

        var writer = new BarcodeWriterSvg
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                DisableECI = true,
                CharacterSet = "UTF-8",
                Width = 150,
                Height = 150,
            }
        };
        var image = writer.Write(tokenValue);
        var result = new MobileDeviceLinkResponse
        { TokenQrCodeSvg = WebUtility.HtmlEncode(image.Content), Token = tokenValue };

        return result;
    }
}
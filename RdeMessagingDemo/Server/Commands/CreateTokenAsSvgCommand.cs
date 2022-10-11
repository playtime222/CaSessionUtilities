using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using RdeMessagingDemo.Server.Data;
using RdeMessagingDemo.Shared;
using ZXing;
using ZXing.QrCode;

namespace RdeMessagingDemo.Server.Commands;

public class CreateTokenAsSvgCommand
{
    private readonly ApplicationDbContext _Db;
    private readonly IConfiguration _Configuration;

    public CreateTokenAsSvgCommand(ApplicationDbContext db, IConfiguration configuration)
    {
        _Db = db;
        _Configuration = configuration;
    }

    public async Task<MobileDeviceLinkResponse> ExecuteAsync(string userId)
    {
        var user = await _Db.Users.FindAsync(userId) ?? throw new InvalidOperationException("User not found.");

        var randomBuffer = new byte[32];
        new SecureRandom().NextBytes(randomBuffer);

        var baseUrl = _Configuration.GetValue(typeof(string), "BaseApiUri") as string ?? throw new InvalidOperationException("BaseApiUri setting not found.");

        var identityUrl = $"{baseUrl}/identity";

        var token = new ServicesToken()
        {
            AuthToken = Hex.ToHexString(randomBuffer),
            IdentityUrl = identityUrl
        };

        var tokenValue = JsonConvert.SerializeObject(token, new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        });

        _Db.FakeApiTokens.Add(new() { ApplicationUser = user, Token = token.AuthToken });
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
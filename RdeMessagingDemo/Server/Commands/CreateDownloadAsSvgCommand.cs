using System.Net;
using ZXing;
using ZXing.QrCode;

namespace RdeMessagingDemo.Server.Commands;

public class CreateDownloadAsSvgCommand
{
    private readonly IConfiguration _Configuration;

    public CreateDownloadAsSvgCommand(IConfiguration configuration)
    {
        _Configuration = configuration;
    }

    public async Task<DownloadUriQrCodeResponse> ExecuteAsync(string fileName)
    {
        //var contentUrl = _Configuration.GetValue(typeof(string), "AndroidApkUri") as string ?? throw new InvalidOperationException("AndroidApkUri setting not found.");

        var contentUrl = _Configuration.GetValue(typeof(string), "BaseApiUri") as string ?? throw new InvalidOperationException("Base API Uri setting not found.");

        contentUrl = $"{contentUrl}/downloads/{fileName}";

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
        var image = writer.Write(contentUrl);
        var result = new DownloadUriQrCodeResponse
        {
            ContentUrl = contentUrl,
            QrCodeSvg = WebUtility.HtmlEncode(image.Content)
        };

        return result;
    }
}
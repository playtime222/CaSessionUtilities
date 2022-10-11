using System;
using System.Linq;

namespace RdeMessagingDemo.Shared;

public class DownloadUriQrCodeResponse
{
    public string ContentUrl { get; set; }
    public string QrCodeSvg { get; set; }
}
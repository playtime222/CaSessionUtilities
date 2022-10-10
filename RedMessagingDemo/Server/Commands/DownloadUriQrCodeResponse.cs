using System;
using System.Linq;

namespace RedMessagingDemo.Server.Commands;

public class DownloadUriQrCodeResponse
{
    public string ContentUrl { get; set; }
    public string QrCodeSvg { get; set; }
}
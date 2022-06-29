using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilities.Messaging.zipV2;

public sealed class ZipMessageEncoder : IMessageEncoder
{
    public static string Version = "1.0.0";
    public static string VersionEntryName = "R_1_1";
    public static string VersionGmacEntryName = "AT_1";

    //JSON Not encrypted
    public static string RdeSessionArgsEntryName = "R_2_1";
    public static string RdeSessionArgsGmacEntryName = "AT_2";

    public static string NoteEntryName = "R_3_1";
    public static string NoteGmacEntryName = "AT_3";

    public static string MetadataEntryName = "R_4_1";
    public static string MetadataGmacEntryName = "AT_4";

    //private Gson gson = new Gson();
    private byte[] secretKey;
    private byte[] iv;

    private MemoryStream result;
    private ZipArchive zipStream;

    public ZipMessageEncoder()
    {
    }

    private byte[] generateIv() {
        var result = new byte[16];
        new SecureRandom().NextBytes(result);
        this.iv = result;
        return result;
    }

    private int fileCounter = 4; //-> First one is R_5_1
    //TODO private int filePartCounter...
    private string nextEntryName() {fileCounter++; return string.Format("R_{0}_1", fileCounter);}
    private string gmacEntryName() {return string.Format("A_{0}", fileCounter);}

    //TODO version marker entry
    public byte[] encode(MessageContentArgs messageArgs, RdeSessionArgs rdeSessionArgs, byte[] secretKey)
    {
        this.secretKey = secretKey;
        rdeSessionArgs.iv = Hex.ToHexString(generateIv());
        //messageCipher = CipherUtilities.GetCipher("AES/CBC/PKCS5Padding");
        //messageCipher.Init(true, secretKey, new IvParameterSpec(rdeSessionArgs.getIv()));

        using (result = new MemoryStream())
        using (zipStream = new ZipArchive(result, ZipArchiveMode.Create, true, Encoding.UTF8))
        {

            writePlain(VersionEntryName, VersionGmacEntryName, Encoding.UTF8.GetBytes(Version));
            writePlain(NoteEntryName, NoteGmacEntryName, Encoding.UTF8.GetBytes(messageArgs.getUnencryptedNote()));
            var json = JsonConvert.SerializeObject(rdeSessionArgs);
            writePlain(RdeSessionArgsEntryName, RdeSessionArgsGmacEntryName, Encoding.UTF8.GetBytes(json));

            var filenames = new List<string>();
            for (var i = 0; i < messageArgs.getFileArgs().Length; i++)
            {
                var item = messageArgs.getFileArgs()[i];
                filenames.Add(item.getName());
                writeEncrypted(item.getContent());
            }

            var metadata = new Metadata();
            var v = filenames.ToArray();
            metadata.Filenames = v;
            writeEncrypted(MetadataEntryName, MetadataGmacEntryName, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(metadata)));
        }
        result.Flush();
        return result.ToArray();
    }

    //public void Dispose() 
    //{
    //    zipStream?.Dispose();
    //    result?.Dispose();
    //}

    private void writePlain(string entryName, string gmacEntryName, byte[] content)
    {
        write(entryName, content);
        writeGmac(gmacEntryName, content);
    }

    private void writeEncrypted(byte[] content) 
    {
        var e = nextEntryName();
        writeEncrypted(e, gmacEntryName(), content);
    }

    private void writeEncrypted(string entryName, string gmacEntryName, byte[] content) 
    {
        write(entryName, Crypto.getAESCBCPKCS5PaddingCipherText(this.secretKey, this.iv, content));
        writeGmac(gmacEntryName, content);
    }

    private void writeGmac(string entryName, byte[] content)
    {
        write(entryName, Crypto.getAesGMac(secretKey,iv,content));
    }

    private void write(string entryName, byte[] content) 
    {
        var entry = zipStream.CreateEntry(entryName);
        //zipStream.putNextEntry(entry);
        using var stream = entry.Open();
        stream.Write(content);
        stream.Flush();
    }
}
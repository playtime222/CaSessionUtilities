using System.Globalization;
using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilities.Messaging.zipV2;

public class ZipMessageDecoder
{
    private string version;
    private RdeSessionArgs rdeSessionArgs;
    private byte[] message;
    private string rdeSessionArgsJson;

    private List<MessageFile> files = new();
    private byte[] secretKey;
    private byte[] iv;

    public RdeSessionArgs decodeRdeSessionArgs(byte[] message)
    {
        if (this.message != null) throw new ArgumentException();
        this.message = message;

        version = readPlainTextString(ZipMessageEncoder.VersionEntryName);
        if (!version.equals(ZipMessageEncoder.Version))
            throw new ArgumentException("Version not supported.");

        rdeSessionArgsJson = readPlainTextString(ZipMessageEncoder.RdeSessionArgsEntryName);
        rdeSessionArgs = JsonConvert.DeserializeObject<RdeSessionArgs>(rdeSessionArgsJson);
        iv = Hex.Decode(rdeSessionArgs.iv);
        return rdeSessionArgs;
    }

    public MessageV2 decode(byte[] secretKey)
    {
        if (this.message == null) throw new InvalidOperationException();
        //if (this.messageCipher != null) throw new InvalidOperationException();

        this.secretKey = secretKey;

        //messageCipher = Cipher.getInstance("AES/CBC/PKCS5Padding");
        //messageCipher.init(Cipher.DECRYPT_MODE, secretKey, new IvParameterSpec(rdeSessionArgs.getIv()));

        verifyPlainTextString(this.version, ZipMessageEncoder.VersionGmacEntryName);
        verifyPlainTextString(this.rdeSessionArgsJson, ZipMessageEncoder.RdeSessionArgsGmacEntryName);
        var note = readPlainTextString(ZipMessageEncoder.NoteEntryName);
        verifyPlainTextString(note, ZipMessageEncoder.NoteGmacEntryName);

        var metadataJsonBytes = readAndVerify(ZipMessageEncoder.MetadataEntryName, ZipMessageEncoder.MetadataGmacEntryName);
        var json = Encoding.UTF8.GetString(metadataJsonBytes);
        var metadata = JsonConvert.DeserializeObject<Metadata>(json);

        for (var i = 0; i < metadata.Filenames.Length; i++)
        {
            var entryName = nextEntryName();
            files.Add(new MessageFile(metadata.Filenames[i], readAndVerify(entryName, gmacEntryName())));
        }

        return new MessageV2(note, rdeSessionArgs, files.ToArray());
    }

    private byte[] readAndVerify(string entryName, string gmacEntryName)
    {
        var cipherText = readEntry(entryName);
        var result = Crypto.getAESCBCPKCS5PaddingPlainText(secretKey, iv, cipherText);
        verify(result, gmacEntryName);
        return result;
    }

    private int fileCounter = 4; //-> First one is R_5_1
    private string nextEntryName() { fileCounter++; return string.Format(CultureInfo.InvariantCulture, "R_{0}_1", fileCounter); }
    private string gmacEntryName() { return string.Format(CultureInfo.InvariantCulture, "A_{0}", fileCounter); }

    private string readPlainTextString(string entryName)
    {
        return Encoding.UTF8.GetString(readEntry(entryName));
    }

    private void verifyPlainTextString(string value, string entryName)
    {
        verify(Encoding.UTF8.GetBytes(value), entryName);
    }

    private void verify(byte[] value, string gmacEntryName)
    {
        var gmac = readEntry(gmacEntryName);
        Crypto.verifyAesGMac(secretKey, iv, value, gmac);
    }

    private byte[] readEntry(string name)
    {
        using (var input = new MemoryStream(this.message))
        {
            using (var zipStream = new ZipArchive(input, ZipArchiveMode.Read, true, Encoding.UTF8))
            {
                var zipEntry = zipStream.Entries.Single(x => x.Name == name);

                using var stream = zipEntry.Open();
                var buffer = new byte[2048];
                var result = new MemoryStream();
                int len = stream.Read(buffer);
                while (len > 0)
                {
                    result.write(buffer, 0, len);
                    len = stream.Read(buffer);
                }
                return result.ToArray();
            }
        }
    }
}

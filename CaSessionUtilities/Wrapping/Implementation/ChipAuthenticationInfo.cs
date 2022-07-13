namespace CaSessionUtilities.Wrapping.Implementation;

public static class ChipAuthenticationInfo
{

    public static ChipAuthenticationCipherInfo Find(string oid)
        => _CipherInfo.TryGetValue(oid, out var result) ? result : throw new InvalidOperationException("Unsupported OID.");

    private readonly static Dictionary<string, ChipAuthenticationCipherInfo> _CipherInfo = new()
    {
        {ID_CA_DH_3DES_CBC_CBC, new("DESede",128) },
        {ID_CA_ECDH_3DES_CBC_CBC , new("DESede",128) },
        {ID_CA_DH_AES_CBC_CMAC_128 , new("AES",128) },
        {ID_CA_DH_AES_CBC_CMAC_192 , new("AES",192) },
        {ID_CA_DH_AES_CBC_CMAC_256 , new("AES",256) },
        {ID_CA_ECDH_AES_CBC_CMAC_128, new("AES",128) },
        {ID_CA_ECDH_AES_CBC_CMAC_192, new("AES",192) },
        {ID_CA_ECDH_AES_CBC_CMAC_256, new("AES",256) },
    };

    private const string ID_CA_DH_3DES_CBC_CBC =       "0.4.0.127.0.7.2.2.2";
    private const string ID_CA_ECDH_3DES_CBC_CBC =     "0.4.0.127.0.7.2.2.3.2.1 ";
    private const string ID_CA_DH_AES_CBC_CMAC_128 =   "0.4.0.127.0.7.2.2.3.1.2";
    private const string ID_CA_DH_AES_CBC_CMAC_192 =   "0.4.0.127.0.7.2.2.3.1.3";
    private const string ID_CA_DH_AES_CBC_CMAC_256 =   "0.4.0.127.0.7.2.2.3.1.4";
    private const string ID_CA_ECDH_AES_CBC_CMAC_128 = "0.4.0.127.0.7.2.2.3.2.2";
    private const string ID_CA_ECDH_AES_CBC_CMAC_192 = "0.4.0.127.0.7.2.2.3.2.3";
    private const string ID_CA_ECDH_AES_CBC_CMAC_256 = "0.4.0.127.0.7.2.2.3.2.4";

    public static string toCipherAlgorithm(string oid)
    {
        if (ID_CA_DH_3DES_CBC_CBC.Equals(oid, StringComparison.InvariantCultureIgnoreCase)
            || ID_CA_ECDH_3DES_CBC_CBC.Equals(oid, StringComparison.InvariantCultureIgnoreCase))
        {
            return "DESede";
        }
        else if (ID_CA_DH_AES_CBC_CMAC_128.Equals(oid, StringComparison.InvariantCultureIgnoreCase)
                 || ID_CA_DH_AES_CBC_CMAC_192.Equals(oid, StringComparison.InvariantCultureIgnoreCase)
                 || ID_CA_DH_AES_CBC_CMAC_256.Equals(oid, StringComparison.InvariantCultureIgnoreCase)
                 || ID_CA_ECDH_AES_CBC_CMAC_128.Equals(oid, StringComparison.InvariantCultureIgnoreCase)
                 || ID_CA_ECDH_AES_CBC_CMAC_192.Equals(oid, StringComparison.InvariantCultureIgnoreCase)
                 || ID_CA_ECDH_AES_CBC_CMAC_256.Equals(oid, StringComparison.InvariantCultureIgnoreCase))
        {
            return "AES";
        }

        throw new InvalidOperationException("Unsupported OID.");
    }

    public static int toKeyLength(string oid)
    {
        if (ID_CA_DH_3DES_CBC_CBC.Equals(oid, StringComparison.InvariantCultureIgnoreCase)
            || ID_CA_ECDH_3DES_CBC_CBC.Equals(oid, StringComparison.InvariantCultureIgnoreCase)
            || ID_CA_DH_AES_CBC_CMAC_128.Equals(oid, StringComparison.InvariantCultureIgnoreCase)
            || ID_CA_ECDH_AES_CBC_CMAC_128.Equals(oid, StringComparison.InvariantCultureIgnoreCase))
        {
            return 128;
        }
        else if (ID_CA_DH_AES_CBC_CMAC_192.Equals(oid, StringComparison.InvariantCultureIgnoreCase)
                 || ID_CA_ECDH_AES_CBC_CMAC_192.Equals(oid, StringComparison.InvariantCultureIgnoreCase))
        {
            return 192;
        }
        else if (ID_CA_DH_AES_CBC_CMAC_256.Equals(oid, StringComparison.InvariantCultureIgnoreCase)
                 || ID_CA_ECDH_AES_CBC_CMAC_256.Equals(oid, StringComparison.InvariantCultureIgnoreCase))
        {
            return 256;
        }

        throw new InvalidOperationException("Unsupported OID.");
    }

    /// <summary>
    /// OID of DG14 CA Session Public Key
    /// </summary>
    /// <param name="oid"></param>
    /// <returns></returns>
    /// <exception cref="NumberFormatException"></exception>
    public static string toKeyAgreementAlgorithm(string oid)
    {
        if (oid == null)
            throw new ArgumentException("Unknown OID: null");


        if (ID_CA_DH_3DES_CBC_CBC.Equals(oid, StringComparison.InvariantCultureIgnoreCase)
            || ID_CA_DH_AES_CBC_CMAC_128.Equals(oid, StringComparison.InvariantCultureIgnoreCase)
            || ID_CA_DH_AES_CBC_CMAC_192.Equals(oid, StringComparison.InvariantCultureIgnoreCase)
            || ID_CA_DH_AES_CBC_CMAC_256.Equals(oid, StringComparison.InvariantCultureIgnoreCase))
        {
            return "DH";
        }
        else if (ID_CA_ECDH_3DES_CBC_CBC.Equals(oid, StringComparison.InvariantCultureIgnoreCase)
                 || ID_CA_ECDH_AES_CBC_CMAC_128.Equals(oid, StringComparison.InvariantCultureIgnoreCase)
                 || ID_CA_ECDH_AES_CBC_CMAC_192.Equals(oid, StringComparison.InvariantCultureIgnoreCase)
                 || ID_CA_ECDH_AES_CBC_CMAC_256.Equals(oid, StringComparison.InvariantCultureIgnoreCase))
        {
            return "ECDH";
        }

        throw new InvalidOperationException("Unsupported OID.");
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilities
{
    public static class Crypto
    {
        public static byte[] getCipherText(string alg, byte[] key, byte[] iv, byte[] data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alg">Better be "AES/ECB/NoPadding"</param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static byte[] getCipherText(string alg, byte[] key, byte[] data)
        {
            throw new NotImplementedException();
        }

        public static byte[] getMac(string alg, byte[] key, byte[] data)
        {
            if (!alg.Equals("AESCMAC", StringComparison.InvariantCultureIgnoreCase))
                throw new ArgumentException();

            var mac = new CMac(new AesEngine(), 128);
            var macKey = new KeyParameter(key);

            mac.Init(macKey);
            mac.BlockUpdate(data, 0, data.Length);
            
            var result = new byte[mac.GetMacSize()];
            mac.DoFinal(result, 0);
            return result;
        }
    }
}

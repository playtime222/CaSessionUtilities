using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilities
{
    public static class Crypto
    {
        public static byte[] getAESCBCPKCS5PaddingCipherText(byte[] key, byte[] iv, byte[] plainText)
        {
            var messageCipher = CipherUtilities.GetCipher("AES/CBC/PKCS5Padding");
            messageCipher.Init(true, new ParametersWithIV(new KeyParameter(key), iv));
            return messageCipher.DoFinal(plainText);
        }

        public static byte[] getAESCBCNoPaddingCipherText(byte[] key, byte[] iv, byte[] plainText)
        {
            var messageCipher = CipherUtilities.GetCipher("AES/CBC/NoPadding");
            messageCipher.Init(true, new ParametersWithIV(new KeyParameter(key), iv));
            return messageCipher.DoFinal(plainText);
        }

        public static byte[] getDESedeCBCNoPaddingCipherText(byte[] key, byte[] iv, byte[] plainText)
        {
            var messageCipher = CipherUtilities.GetCipher("DESede/CBC/NoPadding");
            messageCipher.Init(true, new ParametersWithIV(new KeyParameter(key), iv));
            return messageCipher.DoFinal(plainText);
        }

        public static byte[] getAESCBCPKCS5PaddingPlainText(byte[] key, byte[] iv, byte[] cipherText)
        {
            var messageCipher = CipherUtilities.GetCipher("AES/CBC/PKCS5Padding");
            messageCipher.Init(false, new ParametersWithIV(new KeyParameter(key), iv));
            return messageCipher.DoFinal(cipherText);
        }

        //Also "AES/ECB/NoPadding"
        public static byte[] getSscCipherText(string algName, byte[] key, byte[] data)
        {
            var messageCipher = CipherUtilities.GetCipher(algName);
            messageCipher.Init(true, new KeyParameter(key));
            return messageCipher.DoFinal(data);
        }




        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="alg">Better be "AES/ECB/NoPadding"</param>
        ///// <param name="key"></param>
        ///// <param name="plainText"></param>
        ///// <returns></returns>
        ///// <exception cref="NotImplementedException"></exception>
        //public static byte[] getCipherText(string alg, byte[] key, byte[] plainText)
        //{
        //    throw new NotImplementedException();
        //}

        public static byte[] getAesCMac(/*string alg,*/ byte[] key, byte[] data)
        {
            //if (!alg.Equals("AESCMAC", StringComparison.InvariantCultureIgnoreCase))
            //    throw new ArgumentException();

            var mac = new CMac(new AesEngine(), 128);
            var macKey = new KeyParameter(key);

            mac.Init(macKey);
            mac.BlockUpdate(data, 0, data.Length);

            var result = new byte[mac.GetMacSize()];
            mac.DoFinal(result, 0);
            return result;
        }

        public static byte[] getAesGMac(/*string alg,*/ byte[] key, byte[] iv, byte[] data)
        {
            //if (!alg.Equals("AESGMAC", StringComparison.InvariantCultureIgnoreCase))
            //    throw new ArgumentException();

            IMac mac = new GMac(new GcmBlockCipher(new AesEngine())); //??? correct size?
            ICipherParameters mackey = new KeyParameter(key);
            mac.Init(new ParametersWithIV(mackey, iv));
            mac.BlockUpdate(data, 0, data.Length);

            var result = new byte[mac.GetMacSize()];
            mac.DoFinal(result, 0);
            return result;
        }

        public static void verifyAesGMac(/*string alg,*/ byte[] key, byte[] iv, byte[] data, byte[] value)
        {
            var decryptCipher = new GcmBlockCipher(new AesEngine());
            ICipherParameters mackey = new KeyParameter(key);
            decryptCipher.Init(false, new ParametersWithIV(mackey, iv));
            var something = new byte[2048];
            decryptCipher.ProcessAadBytes(data, 0, data.Length);
            decryptCipher.ProcessBytes(value, 0, value.Length, something, 0);
            decryptCipher.DoFinal(something, 0); //Throws if bad gmac
        }

        public static byte[] getISO9797Alg3Mac(byte[] ksMac, byte[] joined)
        {
            throw new NotImplementedException();
        }
    }
}

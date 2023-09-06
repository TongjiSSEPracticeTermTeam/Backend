using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Text;

namespace Cinema.Services
{
    /// <summary>
    /// AES加密工具类
    /// </summary>
    public class AesEncryptionervice
    {
        private static KeyParameter keyParam = null!;
        private static byte[] iv = null!;

        /// <summary>
        /// 初始化并随机生成密钥和IV
        /// </summary>
        public AesEncryptionervice()
        {
            if (keyParam == null)
            {
                var random = new SecureRandom();
                keyParam = GenerateRandomKey(random, 256);  // 256-bit key
                iv = GenerateRandomBytes(random, 128);      // 128-bit IV
            }
        }

        private static KeyParameter GenerateRandomKey(SecureRandom random, int bitLength)
        {
            var bytes = GenerateRandomBytes(random, bitLength);
            return new KeyParameter(bytes);
        }

        private static byte[] GenerateRandomBytes(SecureRandom random, int bitLength)
        {
            var bytes = new byte[bitLength / 8];
            random.NextBytes(bytes);
            return bytes;
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public string Encrypt(string plainText)
        {
            var blockCipher = CreateBlockCipher(true);
            var inputBytes = Encoding.UTF8.GetBytes(plainText);
            var outputBytes = new byte[blockCipher.GetOutputSize(inputBytes.Length)];

            var length = blockCipher.ProcessBytes(inputBytes, outputBytes, 0);
            blockCipher.DoFinal(outputBytes, length);

            return Convert.ToBase64String(outputBytes);
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public string Decrypt(string cipherText)
        {
            var blockCipher = CreateBlockCipher(false);
            var inputBytes = Convert.FromBase64String(cipherText);
            var outputBytes = new byte[blockCipher.GetOutputSize(inputBytes.Length)];

            var length = blockCipher.ProcessBytes(inputBytes, outputBytes, 0);
            blockCipher.DoFinal(outputBytes, length);

            return Encoding.UTF8.GetString(outputBytes).TrimEnd('\0');
        }

        private static PaddedBufferedBlockCipher CreateBlockCipher(bool forEncryption)
        {
            var engine = new AesEngine();
            var blockCipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(engine));
            var keyParamWithIV = new ParametersWithIV(keyParam, iv, 0, iv.Length);
            blockCipher.Init(forEncryption, keyParamWithIV);
            return blockCipher;
        }
    }
}

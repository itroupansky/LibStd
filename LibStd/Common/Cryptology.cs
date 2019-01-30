using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

//using CryptoLightLib;

/// <summary>
/// Any changes made here should be copied over to sqlserverhelper projects cryptology class
/// </summary>
namespace LibStd.Common
{
    public interface ICryptology
    {
        string Decrypt(string Data, string IV);
        EncryptData Encrypt(string Data, string IVSALT);
    }

    public class EncryptData
    {
        public string EncryptedString = "";
        public string EncryptedIV = "";
    }
  
    public class CryptologyWeb :ICryptology
    {
        private readonly string _seed;
        private readonly string _seed2;
        private const string _seed1="1234";
        /// <summary>
        /// constructor with 2 different seeds 
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="seed2"></param>
        public CryptologyWeb(string seed, string seed2)
        {
            _seed = seed;
            _seed2 = seed2;
        }
        public string Decrypt(string Data, string IV)
        {
            return DecryptRijndael(Data, IV);
        }

        public EncryptData Encrypt(string Data, string IVSALT)
        {
            if(string.IsNullOrEmpty(IVSALT))
            {
                IVSALT = System.Guid.NewGuid().ToString();
            }

            EncryptData toRet = new EncryptData();
            toRet.EncryptedIV = IVSALT;
            toRet.EncryptedString = EncryptRijndael(Data, toRet.EncryptedIV);
            return toRet;
        }

   

        #region Consts
        /// <summary>
        /// Change the Inputkey GUID when you use this code in your own program.
        /// Keep this inputkey very safe and prevent someone from decoding it some way!!
        /// </summary>
        /// 
        public string InputKey()
        {
           
            string retStr = $"{_seed}{_seed1}{_seed2}";
            return retStr;
        }
        //internal const string Inputkey = "8cbeba04-edba-4647-b28e-a2a9dd96f677";
        #endregion

        #region Rijndael Encryption

        /// <summary>
        /// Encrypt the given text and give the byte array back as a BASE64 string
        /// </summary>
        /// <param name="text" />The text to encrypt
        /// <param name="salt" />The pasword salt
        /// <returns>The encrypted text</returns>
        private string EncryptRijndael(string text, string salt)
        {
            //if (string.IsNullOrEmpty(text))
            if (text == null)
                throw new ArgumentNullException("No string to encrypt.");

            var aesAlg = NewRijndaelManaged(salt);

            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(text);
            }

            return Convert.ToBase64String(msEncrypt.ToArray());
        }
        #endregion

        #region Rijndael Dycryption
        /// <summary>
        /// Checks if a string is base64 encoded
        /// </summary>
        /// <param name="base64String" />The base64 encoded string
        /// <returns>
        private static bool IsBase64String(string base64String)
        {
            base64String = base64String.Trim();
            return (base64String.Length % 4 == 0) &&
                   Regex.IsMatch(base64String, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

        /// <summary>
        /// Decrypts the given text
        /// </summary>
        /// <param name="cipherText" />The encrypted BASE64 text
        /// <param name="salt" />The pasword salt
        /// <returns>De gedecrypte text</returns>
        private string DecryptRijndael(string cipherText, string salt)
        {
            if (string.IsNullOrEmpty(cipherText))
                return null; //throw new ArgumentNullException("cipherText");

            if (!IsBase64String(cipherText))
                throw new Exception("The cipherText input parameter is not base64 encoded");

            string text;

            var aesAlg = NewRijndaelManaged(salt);
            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            var cipher = Convert.FromBase64String(cipherText);
            using (var msDecrypt = new MemoryStream(cipher))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        try
                        {
                            text = srDecrypt.ReadToEnd();
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
            }
            return text;
        }
        #endregion

        #region NewRijndaelManaged
        /// <summary>
        /// Create a new RijndaelManaged class and initialize it
        /// </summary>
        /// <param name="salt" />The pasword salt
        /// <returns>
        private RijndaelManaged NewRijndaelManaged(string salt)
        {
            if (salt == null) throw new ArgumentNullException("No salt found.");
            var saltBytes = Encoding.ASCII.GetBytes(salt);
            var key = new Rfc2898DeriveBytes(InputKey(), saltBytes);

            var aesAlg = new RijndaelManaged();
            aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
            aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

            return aesAlg;
        }
        #endregion

    }

}
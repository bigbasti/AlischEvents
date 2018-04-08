using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;


namespace AlischEvents.Web.Security {
    public class Hashing {

        /// <summary>
        /// Generiert einen MD5-Hash zu einem angegebenen String
        /// </summary>
        /// <param name="wert">String für den der MD5-Hash errechnet werden soll</param>
        /// <returns>String mit dem MD5-Hash</returns>
        public static String GenerateMD5(String wert) {
            byte[] bWert = Encoding.UTF8.GetBytes(wert);
            MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(bWert);
            string md5Wert = BitConverter.ToString(hash).Replace("-", "").ToLower();

            return md5Wert;
        }
    }
}
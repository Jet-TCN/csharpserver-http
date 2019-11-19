using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json;
namespace CSServer
{
    class Password
    {
        public static string EncodeMD5(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

	
        private static Dictionary<string, string> data;
        static Password()
        {
            string path = File.ReadAllText(@"C:\Users\Jet\source\repos\CSServer\CSServer\passwords.json");
            var res = JsonConvert.DeserializeObject<dynamic>(path);
            data = res.ToObject<Dictionary<string,string>>();
        }

        public static string getData(string key)
        {
            if (data.ContainsKey(key)) return (data[key]);
            return "";
        }
    }
}

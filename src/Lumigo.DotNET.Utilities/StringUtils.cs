using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using Amazon.DynamoDBv2.Model;

namespace Lumigo.DotNET.Utilities
{
    public static class StringUtils
    {
        private static string CandidateChars = "abcdefghijklmnopqrstuvwxyz1234567890";

        public static string GetMaxSizeString(string input, int maxStringSize)
        {
            if (string.IsNullOrEmpty(input) || maxStringSize == 0)
                return null;
            if (input != null && input.Length > maxStringSize)
            {
                return input.Substring(0, maxStringSize);
            }
            return input;
        }

        public static string RandomStringAndNumbers(int size)
        {
            StringBuilder sb = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < size; i++)
            {
                sb.Append(CandidateChars[random.Next(CandidateChars.Length)]);
            }

            return sb.ToString();
        }

        public static string BuildMd5Hash(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static string DynamoDBItemToHash(Dictionary<string, AttributeValue> item)
        {
            return BuildMd5Hash(JsonConvert.SerializeObject(item));
        }

        public static int GetBase64Size(string value)
        {
            return (int)Math.Round(Math.Floor((double)(Encoding.UTF8.GetBytes(value).Length / 3) + 1) * 4);
        }

        public static string StreamToString(Stream stream)
        {
            var pos = stream.Position;
            StreamReader reader = new StreamReader(stream);
            string text = reader.ReadToEnd();
            stream.Position = pos;
            return text;
        }
    }
}

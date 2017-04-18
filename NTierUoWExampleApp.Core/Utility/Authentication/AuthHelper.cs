using NTierUoWExampleApp.Common.Enum.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.Core.Utility.Authentication
{
    public class AuthHelper
    {
        public static string GetHash(string input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }

        public static string ConvertWebApiApplicationTypesToString(int value)
        {
            switch (value)
            {
                case (int)WebApiApplicationTypes.Global:
                    {
                        return WebApiApplicationTypes.Global.ToString();
                    }
                case (int)WebApiApplicationTypes.Internal:
                    {
                        return WebApiApplicationTypes.Internal.ToString();
                    }
            }
            return "Unknown";
        }

        public string ConvertWebApiApplicationDataAccessTypesToString(int value)
        {
            switch (value)
            {
                case (int)WebApiApplicationDataAccessTypes.Confidential:
                    {
                        return WebApiApplicationDataAccessTypes.Confidential.ToString();
                    }
                case (int)WebApiApplicationDataAccessTypes.NonConfidential:
                    {
                        return WebApiApplicationDataAccessTypes.NonConfidential.ToString();
                    }
            }
            return "Unknown";
        }
    }
}

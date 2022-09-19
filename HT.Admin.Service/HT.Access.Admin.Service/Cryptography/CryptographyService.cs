using System;
using System.Security.Cryptography;
using System.Text;
using HT.Access.Admin.Service.Cryptography.Interfaces;

namespace HT.Access.Admin.Service.Cryptography
{
    public class CryptographyService : ICryptographyService
    {
        private static readonly object __hashLock = new();

        public byte[] Hash(string valueToHash)
        {
            if (valueToHash == null) return null;
            if (string.IsNullOrEmpty(valueToHash)) return Array.Empty<byte>();

            var bytes = Encoding.UTF8.GetBytes(valueToHash);
            lock (__hashLock) // this works around thread locking issues in Net implementation in some high volume scenarios
            {
                var shaM = SHA512.Create();
                return shaM.ComputeHash(bytes);
            }
        }
    }
}

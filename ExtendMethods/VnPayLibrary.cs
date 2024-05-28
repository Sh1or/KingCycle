using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XEDAPVIP.ExtendMethods
{
    public class VnPayLibrary
    {
        private SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
        private SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _responseData.Add(key, value);
            }
        }

        public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
        {
            var data = new StringBuilder();
            foreach (var kv in _requestData)
            {
                if (data.Length > 0)
                {
                    data.Append("&");
                }
                data.Append($"{kv.Key}={WebUtility.UrlEncode(kv.Value)}");
            }

            var rawData = string.Join("&", _requestData.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            var vnpSecureHash = HmacSHA512(vnp_HashSecret, rawData);
            data.Append($"&vnp_SecureHash={vnpSecureHash}");

            return $"{baseUrl}?{data}";
        }

        public bool ValidateSignature(string inputHash, string secretKey)
        {
            var responseRawData = string.Join("&", _responseData.Where(kvp => kvp.Key != "vnp_SecureHash").Select(kvp => $"{kvp.Key}={kvp.Value}"));
            var myChecksum = HmacSHA512(secretKey, responseRawData);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            var keyByte = Encoding.UTF8.GetBytes(key);
            using (var hmacsha512 = new HMACSHA512(keyByte))
            {
                var hashValue = hmacsha512.ComputeHash(Encoding.UTF8.GetBytes(inputData));
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }
            return hash.ToString();
        }
    }

    public class VnPayCompare : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return x.CompareTo(y);
        }
    }
}
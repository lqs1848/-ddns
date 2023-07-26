using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ddns
{
    public static class Aliyun
    {
        private static readonly string Separator = "&";
        public static string Url = "http://alidns.aliyuncs.com/?"; //接口地址

        public static string RecordList(string accessKeyId,string accessKeySecret,string domain)
        {
            var dic = new Dictionary<string, string>();

            //公共请求参数
            dic.Add("Action", "DescribeSubDomainRecords");
            dic.Add("Version", "2015-01-09");
            dic.Add("Format", "JSON");
            dic.Add("AccessKeyId", accessKeyId);
            dic.Add("SignatureNonce", Guid.NewGuid().ToString());
            dic.Add("Timestamp", DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"));
            dic.Add("SignatureMethod", "HMAC-SHA1");
            dic.Add("SignatureVersion", "1.0");

            //请求参数
            dic.Add("SubDomain", domain);

            //接口签名
            var signature = GetSignature(dic, "GET", accessKeySecret);
            dic.Add("Signature", signature);

            var parms = "";
            foreach (var pair in dic) parms += pair.Key + "=" + PercentEncode(pair.Value) + "&"; //参数值需用阿里云约定编码方式进行编码
            parms = parms.TrimEnd('&');

            return HttpGet(Url + parms);
        }

        public static string UpdateRecord(string accessKeyId, string accessKeySecret,string recordId,string ip,string domain)
        {
            var dic = new Dictionary<string, string>();

            //公共请求参数
            dic.Add("Action", "UpdateDomainRecord");
            dic.Add("Version", "2015-01-09");
            dic.Add("Format", "JSON");
            dic.Add("AccessKeyId", accessKeyId);
            dic.Add("SignatureNonce", Guid.NewGuid().ToString());
            dic.Add("Timestamp", DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"));
            dic.Add("SignatureMethod", "HMAC-SHA1");
            dic.Add("SignatureVersion", "1.0");

            //请求参数
            //dic.Add("TTL", "600");
            dic.Add("Type", "A");
            dic.Add("Value", ip);
            dic.Add("RR", domain.Split('.')[0]);
            dic.Add("RecordId", recordId);

            //接口签名
            var signature = GetSignature(dic, "GET", accessKeySecret);
            dic.Add("Signature", signature);

            var parms = "";
            foreach (var pair in dic) parms += pair.Key + "=" + PercentEncode(pair.Value) + "&"; //参数值需用阿里云约定编码方式进行编码
            parms = parms.TrimEnd('&');

            return HttpGet(Url + parms);
        }

        public static string AddRecord(string accessKeyId, string accessKeySecret, string ip, string domain)
        {
            var dic = new Dictionary<string, string>();

            string rr = domain.Split('.')[0];

            //公共请求参数
            dic.Add("Action", "AddDomainRecord");
            dic.Add("Version", "2015-01-09");
            dic.Add("Format", "JSON");
            dic.Add("AccessKeyId", accessKeyId);
            dic.Add("SignatureNonce", Guid.NewGuid().ToString());
            dic.Add("Timestamp", DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"));
            dic.Add("SignatureMethod", "HMAC-SHA1");
            dic.Add("SignatureVersion", "1.0");

            //请求参数
            //dic.Add("TTL", "600");
            dic.Add("Type", "A");
            dic.Add("Value", ip);
            dic.Add("RR", rr);
            dic.Add("DomainName", domain.Substring(rr.Length + 1));

            //接口签名
            var signature = GetSignature(dic, "GET", accessKeySecret);
            dic.Add("Signature", signature);

            var parms = "";
            foreach (var pair in dic) parms += pair.Key + "=" + PercentEncode(pair.Value) + "&"; //参数值需用阿里云约定编码方式进行编码
            parms = parms.TrimEnd('&');

            return HttpGet(Url + parms);
        }

        /// <summary>
        ///     发起网络请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string HttpGet(string url)
        {
            var encode = Encoding.UTF8;
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html, application/xhtml+xml, */*";

            var response = (HttpWebResponse)request.GetResponse();
            var rs = response.GetResponseStream();
            var sr = new StreamReader(rs, encode);
            var result = sr.ReadToEnd();
            sr.Close();
            rs.Close();
            return result;
        }

        /// <summary>
        ///     签名算法
        /// </summary>
        /// <param name="signedParams"></param>
        /// <param name="method"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static string GetSignature(
            Dictionary<string, string> signedParams,
            string method,
            string secret)
        {
            var list = signedParams.Keys.ToList();

            list.Sort(StringComparer.Ordinal); //排序

            var builder = new StringBuilder();
            foreach (var key in list)
                if (signedParams[key] != null)
                    builder.Append("&").Append(PercentEncode(key)).Append("=").Append(PercentEncode(signedParams[key]));

            var builder2 = new StringBuilder();
            builder2.Append(method);
            builder2.Append(Separator);
            builder2.Append(PercentEncode("/"));
            builder2.Append(Separator);
            builder2.Append(PercentEncode(builder.ToString().Substring(1)));

            byte[] hash = { };
            using (var fromName = CryptoConfig.CreateFromName("HMACSHA1") as KeyedHashAlgorithm)
            {
                if (fromName != null)
                {
                    fromName.Key = Encoding.UTF8.GetBytes(secret + Separator);
                    hash = fromName.ComputeHash(Encoding.UTF8.GetBytes(builder2.ToString().ToCharArray()));
                }
            }

            return Convert.ToBase64String(hash);
        }

        /// <summary>
        ///     编码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string PercentEncode(string value)
        {
            if (value == null)
                return null;
            var stringBuilder = new StringBuilder();
            var str = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            foreach (char ch in Encoding.UTF8.GetBytes(value))
                if (str.IndexOf(ch) >= 0)
                    stringBuilder.Append(ch);
                else
                    stringBuilder.Append("%").Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)ch));
            return stringBuilder.ToString().Replace("+", "%20").Replace("*", "%2A").Replace("%7E", "~");
        }

        public static string ToForm(IDictionary filter)
        {
            if (filter == null)
                return string.Empty;
            var dictionary = filter.Keys.Cast<string>().ToDictionary(key => key, key => filter[key]);
            var dicOut = new Dictionary<string, string>();
            TileDict(dicOut, dictionary);
            var values = new List<string>();
            foreach (var keyValuePair in dicOut)
                if (!string.IsNullOrEmpty(keyValuePair.Value))
                    values.Add(PercentEncode(keyValuePair.Key) + "=" + PercentEncode(keyValuePair.Value));
            return string.Join("&", values.ToArray());
        }

        internal static void TileDict(Dictionary<string, string> dicOut, object obj, string parentKey = "")
        {
            if (obj == null)
                return;
            if (typeof(IDictionary).IsAssignableFrom(obj.GetType()))
            {
                foreach (var keyValuePair in ((IDictionary)obj).Keys.Cast<string>()
                         .ToDictionary(key => key, key => ((IDictionary)obj)[key]))
                {
                    var parentKey1 = parentKey + "." + keyValuePair.Key;
                    if (keyValuePair.Value != null)
                        TileDict(dicOut, keyValuePair.Value, parentKey1);
                }
            }
            else if (typeof(IList).IsAssignableFrom(obj.GetType()))
            {
                var num = 1;
                foreach (var obj1 in (IEnumerable)obj)
                {
                    TileDict(dicOut, obj1, parentKey + "." + num.ToSafeString());
                    ++num;
                }
            }
            else if (obj.GetType() == typeof(byte[]))
            {
                dicOut.Add(parentKey.TrimStart('.'), Encoding.UTF8.GetString((byte[])obj));
            }
            else
            {
                dicOut.Add(parentKey.TrimStart('.'), obj.ToSafeString(""));
            }
        }

        public static string ToSafeString(this object obj, string defaultStr = null)
        {
            if (obj == null)
                return defaultStr;
            try
            {
                return obj.ToString();
            }
            catch
            {
                return defaultStr;
            }
        }
    }
}

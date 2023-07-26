using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ddns
{
    public class DdnsTask
    {
        private StringBuilder txtLog = new StringBuilder();
        public StringBuilder TxtLog { get { return txtLog; } }

        private string txtIp = "";
        public string TxtIp { get { return txtIp; } }
        private string txtAddress;
        public string TxtAddress { get { return txtAddress; } }
        private string txtDate;
        public string TxtDate { get { return txtDate; } }

        public void DdnsStart()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(DdnsTaskFun), null);
        }//method

        private void OutLog(string str)
        {
            txtLog.Append(DateTime.Now.ToString("yy/MM/dd HH:mm:ss"));
            txtLog.Append(" ");
            txtLog.Append(str);
            txtLog.Append("\r\n");
        }

        private void DdnsTaskFun(object obj)
        {
            //OutLog("开始更新DDNS");
            
            txtDate = DateTime.Now.ToString("yy/MM/dd HH:mm");
            string ipStr = "";
            string ip6Str = "";// GetIpv6();
            string beforeIp = txtIp;

            ipStr = GetIp0();
            if(ipStr == "")
            {
                ipStr = GetIp1();
                if (ipStr == "")
                {
                    ipStr = GetIp2();
                }
            }

            if (ipStr == "")
            {
                OutLog("查询外网ip失败");
                return;
                //OutLog("将直接访问DDNS 让动态域名服务商自动识别ip");
            }

            if (!txtIp.Equals(beforeIp))
            {
                OutLog(beforeIp + "->" + txtIp);
            }
            else 
            {
                //ip无变化不更新
                return;
            }

            string urls = ConfigAppSettings.GetValue("ddns");
            if (!string.IsNullOrEmpty(urls))
            {
                List<string> urlArr = urls.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
                foreach (string pathStr in urlArr)
                {
                    if (!string.IsNullOrEmpty(pathStr))
                    {
                        ChangeIp(pathStr, ipStr, ip6Str);
                    }
                }
            }

            //更新阿里云
            string aliyunDomain = ConfigAppSettings.GetValue("aliyunDomain");
            if (!string.IsNullOrEmpty(aliyunDomain))
            {
                string aliyunAppId = ConfigAppSettings.GetValue("aliyunAppId");
                string aliyunKeySecret = ConfigAppSettings.GetValue("aliyunKeySecret");

                List<string> domains = aliyunDomain.Split(new string[] { ",", "，"}, StringSplitOptions.None).ToList();
                foreach(string domain in domains)
                {
                    if (!string.IsNullOrEmpty(domain))
                    {
                        string recordId = "";
                        try
                        {
                            string jsonResult = Aliyun.RecordList(aliyunAppId, aliyunKeySecret, domain);
                            Match match = new Regex(@"""RecordId"":""(?<recordId>.*?)""").Match(jsonResult);
                            if (match.Success)
                            {
                                recordId = match.Groups["recordId"].Value;
                            }
                        }
                        catch (Exception ex)
                        {
                            OutLog("阿里云获取解析列表失败");
                            Console.Write(ex.StackTrace);

                        }

                        try
                        {
                            if (!string.IsNullOrEmpty(recordId))
                            {
                                //更新解析
                                string result = Aliyun.UpdateRecord(aliyunAppId, aliyunKeySecret, recordId, ipStr, domain);
                                OutLog(domain + "->" + ipStr);
                            }
                            else
                            {
                                //新增解析
                                string result = Aliyun.AddRecord(aliyunAppId, aliyunKeySecret, ipStr, domain);
                                OutLog(domain + "->" + ipStr);
                            }
                        }
                        catch (Exception ex)
                        {
                            OutLog(domain + "-> Error");
                            Console.Write(ex.StackTrace);
                        }
                        
                    }//if
                }//for
            }//if 更新阿里云
        }//method

        private void ChangeIp(string pathStr, string ipStr,string ip6Str)
        {
            string ddnsKey = "no init host";
            try
            {
                ddnsKey = new Uri(pathStr).Host;
                Match match = new Regex(@"\.(?<key>.*?)\.").Match(ddnsKey);
                if (match.Success) ddnsKey = match.Groups["key"].Value;
            }
            catch (Exception ex)
            {
                OutLog("该地址无法被解析["+pathStr+"]");
                Console.Write(ex.StackTrace);
            }
            
            try
            {
                pathStr = pathStr.Replace("{ip4}", ipStr);
                pathStr = pathStr.Replace("{ip6}", ip6Str);
                string updateRes = "no init update result";
                if (pathStr.IndexOf("@") != -1)
                {
                    string auth = new Regex("//(?<auth>.*?)@").Match(pathStr).Groups["auth"].Value;
                    string url = pathStr.Replace(auth+"@", "");
                    HttpWebRequest mRequest = (HttpWebRequest)WebRequest.Create(url);
                    mRequest.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(auth)));
                    HttpWebResponse wresp = (HttpWebResponse)mRequest.GetResponse();
                    StreamReader sr = new StreamReader(wresp.GetResponseStream(), System.Text.Encoding.UTF8);
                    updateRes = sr.ReadToEnd();
                    sr.Close();
                    wresp.Close();
                }
                else
                { 
                    updateRes = new HttpClient().GetStringAsync(pathStr).Result;
                }
                OutLog(ddnsKey + "->" + updateRes.Trim());
            }
            catch (Exception ex)
            {
                OutLog("更新[" + ddnsKey + "]异常:" + ex.Message);
                Console.Write(ex.StackTrace);
            }
        }//method

        public string GetIp0()
        {
            string ipStr = "";
            string html = "";
            try
            {
                html = GetHtml("https://www.ipip.net");
            }
            catch (Exception e)
            {
                Console.Error.Write(e);
            }
            Match match = new Regex(@"<span>IP地址</span>\s?<a href="".*?"">(?<ip>.*?)</a>[\s\S]*<span>位置信息</span>(?<address>.*?)</li>").Match(html);
            if (!match.Success)
            {
                return ipStr;
            }
            else
            {
                ipStr = match.Groups["ip"].Value;
                txtIp = ipStr;
                txtAddress = match.Groups["address"].Value;
            }
            return ipStr;
        }

        public string GetIp1()
        {
            string ipStr = "";
            string html = "";
            try
            {
                html = GetHtml("http://members.3322.org/dyndns/getip", Encoding.GetEncoding("GBK"));
                ipStr = html;
            }
            catch (Exception e)
            {
                Console.Error.Write(e);
            }
            Match match = new Regex(@"([0-9]+\.){3}[0-9]+").Match(html);
            if (!match.Success)
            {
                return ipStr;
            }
            else
            {
                ipStr = match.Groups[0].Value;
                txtIp = ipStr;
            }
            return ipStr;
        }

        public string GetIp2()
        {
            string ipStr = "";
            string html = "";
            try
            {
                html = GetHtml("http://ip.3322.net", Encoding.GetEncoding("GBK"));
                ipStr = html;
            }
            catch (Exception e)
            {
                Console.Error.Write(e);
            }
            Match match = new Regex(@"([0-9]+\.){3}[0-9]+").Match(html);
            if (!match.Success)
            {
                return ipStr;
            }
            else
            {
                ipStr = match.Groups[0].Value;
                txtIp = ipStr;
            }
            return ipStr;
        }


        public string GetIpv6()
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名  
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址  
                    //AddressFamily.InterNetwork表示此IP为IPv4,  
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型  
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                //txt_log.AppendText(DateTime.Now.ToString("HH:mm:ss") + "读取系统路由表异常：" + ex.Message);
            }
            return "";
        }//method





        public string GetHtml(string url, Encoding ed)
        {
            string Html = string.Empty;//初始化新的webRequst
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(url);

            Request.KeepAlive = true;
            Request.ProtocolVersion = HttpVersion.Version11;
            Request.Method = "GET";
            Request.Accept = "*/* ";
            Request.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/536.5 (KHTML, like Gecko) Chrome/19.0.1084.56 Safari/536.5";
            Request.Referer = url;

            HttpWebResponse htmlResponse = (HttpWebResponse)Request.GetResponse();
            //从Internet资源返回数据流
            Stream htmlStream = htmlResponse.GetResponseStream();
            //读取数据流
            StreamReader weatherStreamReader = new StreamReader(htmlStream, ed);
            //读取数据

            Html = weatherStreamReader.ReadToEnd();
            weatherStreamReader.Close();
            htmlStream.Close();
            htmlResponse.Close();
            //针对不同的网站查看html源文件
            return Html;
        }

        public string GetHtml(string url)
        {
            return GetHtml(url, Encoding.UTF8);
        }
    }
}

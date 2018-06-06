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

        private int count = 0;

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
            string ip6Str = GetIpv6();
            string addressStr = "";
            string beforeIp = txtIp;
            string ipJson = "";
            try
            {
                ipJson = new HttpClient().GetStringAsync("http://ip.chinaz.com/getip.aspx").Result;
            }
            catch (Exception ex)
            {

            }
            Match match = new Regex("ip:'(?<ip>.*?)',address:'(?<address>.*?)'").Match(ipJson);
            if (!match.Success)
            {
                OutLog("查询外网ip失败:"+ipJson);
                OutLog("将直接访问DDNS 让动态域名服务商自动识别ip");

                txtIp = "auto";
                txtAddress = ipJson;
            }
            else
            {
                ipStr = match.Groups["ip"].Value;
                addressStr = match.Groups["address"].Value;

                if (!string.IsNullOrEmpty(ipStr) && TxtIp.Equals(ipStr) && count < 10)
                {
                    count++;
                    return;
                }

                txtIp = ipStr;
                txtAddress = addressStr;
            }
            
            if (!txtIp.Equals(beforeIp))
            {
                OutLog(beforeIp + "->" + txtIp);
            }

            string urls = ConfigAppSettings.GetValue("ddns");
            if (string.IsNullOrEmpty(urls))
            {
                OutLog("ddns路径未配置!");
                return;
            }

            List<string> urlArr = urls.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
            foreach(string pathStr in urlArr)
            {
                if (!string.IsNullOrEmpty(pathStr))
                {
                    ChangeIp(pathStr, ipStr, ip6Str);
                }
            }
            count = 0;
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
    }
}

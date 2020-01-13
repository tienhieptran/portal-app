using System;
using System.Configuration;
//using System.DirectoryServices.Protocols;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Text;

namespace BI_Project.Helpers
{
    public class TableauHelper
    {
        private static string _tableauUrl = "";
        private static string _tableauUser = "";
        private static string _clientIp = "";
        static public string GetTicket(string site)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; }; // control validate brower
            
            string str;

            try
            {
                if (_tableauUrl == "")
                {
                    _tableauUrl = ConfigurationManager.AppSettings["TABLEAU_SERVER"];
                    if (_tableauUrl[_tableauUrl.Length - 1] != '/') _tableauUrl += "/";
                }
                if (_tableauUser == "") _tableauUser = ConfigurationManager.AppSettings["TABLEAU_USER"];
                if (_clientIp == "") _clientIp = ConfigurationManager.AppSettings["CLIENT_IP"];


                var request = (HttpWebRequest)WebRequest.Create(_tableauUrl + "trusted");
                var encoding = new UTF8Encoding();
                var postData = "username=" + _tableauUser;
                postData += "&client_ip=" + _clientIp;
                postData += "&target_site=" + site;
                byte[] data = encoding.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                str = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {
                str = ex.Message;
            }
            return str;
        }

    }
}
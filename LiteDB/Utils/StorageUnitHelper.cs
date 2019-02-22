﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LiteDB
{
    /// <summary>
    /// Parse/Format storage unit format (kb/mb/gb)
    /// </summary>
    internal class StorageUnitHelper
    {


        /// <summary>
        /// Format a long file length to pretty file unit
        /// </summary>
        public static string FormatFileSize(long byteCount)
        {
            var suf = new string[] { "B", "KB", "MB", "GB", "TB" }; //Longs run out around EB
            if (byteCount == 0) return "0" + suf[0];
            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }
    }
}
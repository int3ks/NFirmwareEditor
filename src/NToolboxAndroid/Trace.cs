﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

namespace NCore
{
    class Trace
    {
        internal static void Warn(Exception ex, string v, string key)
        {
            Log.WriteLine(LogPriority.Warn,"NToolbox" , $"{v}\n{key}\n{ex}");
        }

        internal static void Warn(string v)
        {
            Log.WriteLine(LogPriority.Warn, "NToolbox", v);
        }
    }
}
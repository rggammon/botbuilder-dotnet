using System;
using System.Collections.Generic;
using System.Text;

namespace CustomMiddleware
{
    public static class Utilities
    {
        public static string Timestamp
        {
            get
            {
                var now = DateTimeOffset.Now;
                double value = now.Hour;
                value *= 60.0;
                value += now.Minute;
                value *= 60.0;
                value += now.Second;
                value += (double)now.Millisecond / 1000.0;
                return value.ToString("00,000.000");
            }
        }
    }
}

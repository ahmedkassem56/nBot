using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace nBot
{
    public class Utility
    {
        public static string HexDump(byte[] buffer)
        {
            return HexDump(buffer, 0, buffer.Length);
        }

        public static string HexDump(byte[] buffer, int offset, int count)
        {
            const int bytesPerLine = 16;
            StringBuilder output = new StringBuilder();
            int length = count;
            if (length % bytesPerLine != 0)
            {
                length += bytesPerLine - length % bytesPerLine;
            }
            for (int x = 0; x <= length; ++x)
            {
                if (x < count)
                {
                    output.AppendFormat("{0:X2}", buffer[offset + x]);
                }
            }
            return output.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;

namespace nBot
{
    class Captcha
    {
        [DllImport("ZlibDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Decompress(byte[] compressed_buffer, int compressed_size, byte[] decompressed_buffer, ref int decompressed_size);

        [DllImport("ZlibDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Compress(byte[] decompressed_buffer, int decompressed_size, byte[] compressed_buffer, ref int compressed_size);
        public static string lastImageName;
        public static UInt32[] GeneratePacketCaptcha(Packet packet)
        {
            byte flag = packet.ReadBYTE();
            UInt16 remain = packet.ReadWORD();
            UInt16 compressed = packet.ReadWORD();
            UInt16 uncompressed = packet.ReadWORD();
            UInt16 width = packet.ReadWORD();
            UInt16 height = packet.ReadWORD();

            if (width != 200 || height != 64)
            {
                throw new NotImplementedException("The captcha is expected to be 200 x 64 pixels.");
            }

            byte[] compressed_buffer = packet.ReadBYTEArray(compressed);

            if (packet.RemainingRead() != 0)
            {
                throw new Exception("Unknown captcha packet.");
            }

            Int32 uncompressed_size = uncompressed;

            byte[] uncompressed_buffer = new byte[uncompressed];
            int result = Decompress(compressed_buffer, compressed, uncompressed_buffer, ref uncompressed_size);
            if (result != 0)
            {
                throw new Exception("Decompress returned error code " + result);
            }

            byte[] uncompressed_bytes = new byte[uncompressed_size];
            Buffer.BlockCopy(uncompressed_buffer, 0, uncompressed_bytes, 0, uncompressed_size);
            uncompressed_buffer = null;

            UInt32[] pixels = new UInt32[width * height];

            int ind_ = 0;
            for (int row_ = 0; row_ < height; ++row_)
            {
                for (int col_ = 0; col_ < width; ++col_)
                {
                    UInt32 write_index = (UInt32)(row_ * width + col_);
                    pixels[write_index] = (UInt32)((byte)(Math.Pow(2.0f, ind_++ % 8)) & uncompressed_bytes[write_index / 8]);
                    if (pixels[write_index] == 0)
                    {
                        pixels[write_index] = 0xFF000000;
                    }
                    else
                    {
                        pixels[write_index] = 0xFFFFFFFF;
                    }
                }
            }

            return pixels;
        }

        public static void SaveCaptchaToBMP(UInt32[] pixels, String filename)
        {
            const Int32 width = 200;
            const Int32 height = 64;
            lastImageName = filename;
            // Hard coded image header for the type the captcha uses
            byte[] header = new byte[]
	        {
		        0x42, 0x4D, 0x7A, 0xC8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7A, 0x00, 0x00, 0x00, 0x6C, 0x00, 
		        0x00, 0x00, 0xC8, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x01, 0x00, 0x20, 0x00, 0x03, 0x00, 
		        0x00, 0x00, 0x00, 0xC8, 0x00, 0x00, 0x12, 0x0B, 0x00, 0x00, 0x12, 0x0B, 0x00, 0x00, 0x00, 0x00, 
		        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0xFF, 0x00, 0x00, 0xFF, 0x00, 
		        0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
		        0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 
		        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 
		        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
	        };

            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(header);
                    for (int c = height - 1; c >= 0; --c)
                    {
                        for (int r = 0; r < width; ++r)
                        {
                            bw.Write((UInt32)pixels[c * width + r]);
                        }
                    }
                    bw.Flush();
                }
            }
            Globals.Main.pic.Image = new Bitmap(filename);
        }
        public static void SendCaptcha(string msg)
        {
            Packet p = new Packet(0x6323);
            p.AddSTRING(msg, "ascii");
            Gateway.SendToServer(p);
        }

    }
}

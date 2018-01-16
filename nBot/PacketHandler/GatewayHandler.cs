using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
namespace nBot
{
    class GatewayHandler
    {
        public static void RemotePacketHandler(Packet packet) // Gateway S -> C packet handler
        {
            Analyzer.ServerAnalyze(packet);
            switch (packet.Opcode)
            {
                case 0x5000:
                case 0x9000:
                    break;
                case 0xA101:
                   Login.ParseServerStats(packet);
                    Gateway.SendToClient(packet);
                    break;
                case 0xA102:
                    if (Globals.LoginType == Globals.enumLoginType.Client)
                        Login.LoginReply(packet);
                    break;
                case 0x2322:
                    Gateway.SendToClient(packet);
                    if (Globals.LoginType == Globals.enumLoginType.Clientless)
                    {
                        UInt32[] pixels = Captcha.GeneratePacketCaptcha(packet);
                        Captcha.SaveCaptchaToBMP(pixels, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + new Random().Next(00000, 99999) + ".bmp");
                        Globals.Main.captcha_send.Enabled = true;
                        Globals.Main.captcha_text.Enabled = true;
                        Globals.Main.btnLogin.Enabled = false;
                        //Captcha.SendCaptcha("1");
                    }

                    break;
                default:
                    Gateway.SendToClient(packet);
                    break;
            }
        }
        public static void LocalPacketHandler(Packet packet) // Gateway C -> S packet handler
        {
            Analyzer.ClientAnalyze(packet);
            switch (packet.Opcode)
            {
                case 0x5000:
                case 0x9000:
                case 0x2001:
                    break;
                default:
                    Gateway.SendToServer(packet);
                    break;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace nBot
{
    class Loader
    {
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, Int32 bInheritHandle, Int32 dwProcessId);
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        public static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        public static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        private static extern IntPtr CreateMutex(IntPtr lpMutexAttributes, bool bInitialOwner, string lpName);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);
        [DllImport("kernel32.dll")]
        static extern IntPtr WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, string lpBuffer, UIntPtr nSize, out IntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern UIntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32")]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, UIntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out IntPtr lpThreadId);
        [DllImport("kernel32", SetLastError = true, ExactSpelling = true)]
        internal static extern IntPtr WaitForSingleObject(IntPtr handle, Int32 milliseconds);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint dwFreeType);
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);
        public static Process sro_process;
        public void StartClient()
        {
            string new_text = "[loader]" + Environment.NewLine + "ip=127.0.0.1" + Environment.NewLine + "port=" + Globals.ListenPort;
            System.IO.File.WriteAllText(Environment.CurrentDirectory + @"\loader.ini", new_text);
            string path = @"D:\Online Games\Myth Client v4000\MythClient.exe";
            CreateMutex(IntPtr.Zero, false, "Myth Online Launcher    ");
            CreateMutex(IntPtr.Zero, false, "Ready");
            sro_process = Process.Start(path, "0 /22 0 0");
            SuspendProcess(sro_process.Id);
            InjectDLL(sro_process, Environment.CurrentDirectory + @"\loader.dll");
            ResumeProcess(sro_process.Id);
            Globals.SROClient = sro_process;
        }

        public static void InjectDLL(Process process, string DLLPath)
        {
            IntPtr bytesout;
            Int32 bufferSize = DLLPath.Length + 1;
            IntPtr hProcess = OpenProcess(0x1F0FFF, 1, process.Id);
            IntPtr AllocMem = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)bufferSize, 4096, 4);
            WriteProcessMemory(hProcess, AllocMem, DLLPath, (UIntPtr)bufferSize, out bytesout);
            UIntPtr Injector = (UIntPtr)GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            IntPtr hThread = (IntPtr)CreateRemoteThread(hProcess, (IntPtr)null, 0, Injector, AllocMem, 0, out bytesout);
            CloseHandle(hThread);
        }

        public void SuspendProcess(int PID)
        {
            Process proc = Process.GetProcessById(PID);

            if (proc.ProcessName == string.Empty)
                return;

            foreach (ProcessThread pT in proc.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    break;
                }

                SuspendThread(pOpenThread);
            }
        }

        public void ResumeProcess(int PID)
        {
            Process proc = Process.GetProcessById(PID);

            if (proc.ProcessName == string.Empty)
                return;

            foreach (ProcessThread pT in proc.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    break;
                }

                ResumeThread(pOpenThread);
            }
        }
    }
}

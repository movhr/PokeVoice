using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Speech_Recognition_test
{
    public static class KeySender
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr hWnd);


        const int KEY_DOWN_EVENT = 0x0001; //Key down flag
        const int KEY_UP_EVENT = 0x0002; //Key up flag

        static void EmuKeyPress(Keys key, int holdTime = 50, int pauseAfter = 100)
        {
            foreach (var proc in Form1.VBA)
            {
                SetForegroundWindow(proc.MainWindowHandle);
                keybd_event((byte)key, 0, KEY_DOWN_EVENT, 0);
                Thread.Sleep(holdTime);
                keybd_event((byte)key, 0, KEY_UP_EVENT, 0);
                Thread.Sleep(pauseAfter);
            }
        }

        public static void Up() => EmuKeyPress(Keys.W);
        public static void Down() => EmuKeyPress(Keys.S);
        public static void Left() => EmuKeyPress(Keys.A);
        public static void Right() => EmuKeyPress(Keys.D);
        public static void Confirm() => EmuKeyPress(Keys.V);
        public static void Back() => EmuKeyPress(Keys.C);
    }
}
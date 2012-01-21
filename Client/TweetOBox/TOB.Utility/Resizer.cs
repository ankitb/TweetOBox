using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TOB.Utility
{
    public class Resizer
    {
        #region sizing event handlers

        const int WM_SYSCOMMAND = 0x112;
        const int SC_SIZE = 0xF000;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public void DragSize(IntPtr handle, SizingAction sizingAction)
        {
            if (System.Windows.Input.Mouse.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                SendMessage(handle, WM_SYSCOMMAND, (IntPtr)(SC_SIZE + sizingAction), IntPtr.Zero);
                SendMessage(handle, 514, IntPtr.Zero, IntPtr.Zero);
            }
        }

        #endregion
    }

    public enum SizingAction
    {
        North = 3,
        South = 6
    }
}

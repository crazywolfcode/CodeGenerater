using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerater
{
   public class Win32Helper
    {
       [DllImport("user32.dll",ExactSpelling = true,CharSet = CharSet.Auto)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}

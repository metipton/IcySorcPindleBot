
namespace IcySorcPindleBot.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class ClientHelper
    {
        static IntPtr? d2handle = null;

        public IntPtr GetD2WinHandle()
        {
            if (d2handle.HasValue)
            {
                return d2handle.Value;
            }

            var d2process = Process.GetProcesses().Where(process => process.MainWindowTitle.Equals("Diablo II: Resurrected")).ToList();

            if (d2process.Count() > 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: More than 1 process detected.");
                Console.ForegroundColor = ConsoleColor.White;
                return IntPtr.Zero;
            }

            if (d2process.Count() == 0)
            {
                Thread.Sleep(2000);
                return IntPtr.Zero;
            }

            if (d2process.Count() == 1)
            {
                d2handle = d2process[0].MainWindowHandle;
                return d2handle.Value;
            }

            return IntPtr.Zero;
        }
    }
}

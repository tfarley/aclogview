using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace aclogview {
    using Heading;   

    public class Globals
    {
        private static bool _useHex;       

        public static bool UseHex
        {
            get
            {
                // Reads are usually simple
                return _useHex;
            }
            set
            {
                // You can add logic here for race conditions,
                // or other measurements
                _useHex = value;
            }
        }

        public string FormatGuid(uint theValue)
        {
            if (Globals.UseHex)
            {
                return "0x" + theValue.ToString("X");
            }

            return theValue.ToString();
        }
    }

  
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(args));
        }
    }
}

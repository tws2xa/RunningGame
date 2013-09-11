using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RunningGame
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// This just kind of appeared and I don't know what it is...
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormSpring());
        }
    }
}

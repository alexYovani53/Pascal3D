using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pascal3D
{
    static class Program
    {

        static Pascal interfaz = null;

        public static Pascal getIntefaz()
        {
            return interfaz;
        }

        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            interfaz = new Pascal();
            Application.Run(interfaz);
        }
    }
}

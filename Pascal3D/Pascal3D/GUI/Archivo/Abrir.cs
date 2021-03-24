using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CompiPascal.GUI.Archivo
{
    public static class Abrir
    {


        public static string cargarArchivo()
        {

            String cadena = "";

            OpenFileDialog solicitud = new OpenFileDialog();

            solicitud.Filter = " Pascal File (*.pas)|*.pas";
            solicitud.InitialDirectory = @"C:\";
            solicitud.Title = "Seleccionar entrada archivo pascal";
            solicitud.CheckFileExists = true;
            solicitud.CheckPathExists = true;
            solicitud.DefaultExt = "txt";
            solicitud.FilterIndex = 1;
            solicitud.RestoreDirectory = true;
            solicitud.ReadOnlyChecked = true;
            solicitud.ShowReadOnly = true;

            if(solicitud.ShowDialog() == DialogResult.OK)
            {
                cadena = File.ReadAllText(solicitud.FileName);
            }


            return cadena;

        }

    }
}

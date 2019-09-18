using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace op_finanzas_api.Helpers
{
    /// <summary>
    /// Arreglar Nombres para URL
    /// </summary>
    public class FixText
    {
        public static string Fix(string r)
        {
            r = r.Replace("á", "a");
            r = r.Replace("é", "e");
            r = r.Replace("í", "i");
            r = r.Replace("ó", "o");
            r = r.Replace("ú", "u");
            r = r.Replace("Á", "A");
            r = r.Replace("É", "E");
            r = r.Replace("Í", "I");
            r = r.Replace("Ó", "O");
            r = r.Replace("Ú", "U");
            return r;
        }
    }
}

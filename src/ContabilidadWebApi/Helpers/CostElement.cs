using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContabilidadWebApi.Helper
{
    public class CostElement
    {
        Dictionary<string, Dictionary<string,string>> elements;


        public CostElement()
        {
            elements = new Dictionary<string, Dictionary<string,string>>();
        }

        public bool AddElement(string item)
        {
            if (elements.ContainsKey(item)) return false;

            Dictionary<string, string> current = new Dictionary<string, string>();
            current.Add("Elemento","");
            current.Add("PlanMes","");
            current.Add("RealMes","");
            current.Add("%Ejecuc","");
            current.Add("PlanAcumulado","");
            current.Add("RealAcumulado","");
            current.Add("%EjecucAcumulado","");
            elements.Add(item, current);

            return true;
        }

        public bool removeElement(string item)
        {
            if (!elements.ContainsKey(item))
            {

            }

            return false;

        }
    }
}

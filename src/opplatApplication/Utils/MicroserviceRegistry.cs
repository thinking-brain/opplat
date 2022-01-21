using System.Collections.Generic;
using System.Linq;

namespace opplatApplication.Utils
{
    public class MicroserviceRegistry
    {
        public static ICollection<Microservice> Services { get; set; }
        // public static ICollection<VisualMicroservice> VisualServices
        // {
        //     get
        //     {
        //         return Services.Where(s => s is VisualMicroservice).Select(s => s as VisualMicroservice).ToList();
        //     }
        // }
    }
}
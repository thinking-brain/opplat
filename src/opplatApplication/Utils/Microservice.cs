using System;
using System.Collections.Generic;

namespace opplatApplication.Utils
{
    public class Microservice
    {
        public string Name { get; set; }
        public string RootUrl { get; set; }
        public List<string> Endpoints { get; set; }
    }
}

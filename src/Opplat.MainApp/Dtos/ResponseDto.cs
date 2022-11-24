using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opplat.MainApp.Dtos;

    public class ResponseDto
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
    }

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace op_costos_api.Models
{
    [Table("Costos_ExportExcel")]
    public class ExportExcel
    {
        public int Id { get; set; }
        public string Objeto { get; set; }
    }
}

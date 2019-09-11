using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace op_costos_api.ViewModels
{
    public class SubMayorVM
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Ano { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Cta { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(20)]
        public string SubCta { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(20)]
        public string Analisis { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(20)]
        public string SubAnalisis { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(20)]
        public string Epigrafe { get; set; }

        [Key]
        [Column(Order = 6, TypeName = "smalldatetime")]
        public DateTime Fecha { get; set; }

        [Key]
        [Column(Order = 7, TypeName = "money")]
        public decimal Debe { get; set; }

        [Key]
        [Column(Order = 8, TypeName = "money")]
        public decimal Haber { get; set; }


        [Key]
        [Column(Order = 9)]
        public byte Mes { get; set; }
    }
}

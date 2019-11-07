using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ContabilidadWebApi.Models
{
    public class SubMayor
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Ano { get; set; }

        [Key]
        [Column(Order = 1)]
        public byte Mes { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(20)]
        public string Cta { get; set; }


        [Key]
        [Column(Order = 3)]
        [StringLength(20)]
        public string Epigrafe { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(20)]
        public string Analisis { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(20)]
        public string SubAnalisis { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(20)]
        public string SubCta { get; set; }

        [Key]
        [Column(Order = 7)]
        public DateTime Fecha { get; set; }

        [Key]
        [Column(Order = 8, TypeName = "money")]
        public decimal Debe { get; set; }

        [Key]
        [Column(Order = 9, TypeName = "money")]
        public decimal Haber { get; set; }

    }
}

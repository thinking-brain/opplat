using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opplat.MainApp.Dtos
{
    public class InventoryDto
    {
        public Guid ProductId { get; set; }

        public string Product { get; set; }

        public decimal Quantity { get; set; }

        public string Unit { get; set; }
    }
}
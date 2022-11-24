using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Domain.Inventory.Entities;

namespace Opplat.MainApp.Dtos
{
    public class ProductMovementDto
    {
        public DateTime? Date { get; set; }
        public Guid ProductId { get; set; }
        public Guid StorageId { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public MovementType Type { get; set; }
        public string Observations { get; set; }
    }
}
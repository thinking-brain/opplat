using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Domain.Inventory.Dtos;
using Opplat.Domain.Inventory.Entities;
using Opplat.Domain.Inventory.Repositories;
using Opplat.Shared.Services;

namespace Opplat.Domain.Inventory.Services;

public interface IMovementTypeService
{
    IEnumerable<MovementTypeDto> List();
    int GetFactor(MovementType type);
}

public class MovementTypeService : IMovementTypeService
{
    public int GetFactor(MovementType type)
    {
        var result = type switch
        {
            MovementType.Input 
                or MovementType.InputInternalTranfer 
                or MovementType.InputForErrorInOutput 
                or MovementType.InputByAdjustment
                or MovementType.InputProductConvertion => 1,
            MovementType.ToProduction 
                or MovementType.IndependentSale 
                or MovementType.Waste 
                or MovementType.OutpuByAdjustment
                or MovementType.OutputForErrorInInput
                or MovementType.OutputInternalTranfer
                or MovementType.OutputProductConversion => -1,
            _ => throw new ArgumentOutOfRangeException(nameof(type), $"Not expected type value: {type}"),
        };        
        return result;
    }

    public IEnumerable<MovementTypeDto> List()
    {
        var list = new List<MovementTypeDto>{
            new MovementTypeDto{ Id = 1, Name = "Entrada"},
            new MovementTypeDto{ Id = 2, Name = "Salida a producción"},
            new MovementTypeDto{ Id = 3, Name = "Venta independiente"},
            new MovementTypeDto{ Id = 4, Name = "Merma"},
            new MovementTypeDto{ Id = 5, Name = "Salida traslado interno"},
            new MovementTypeDto{ Id = 6, Name = "Entrada traslado interno"},
            new MovementTypeDto{ Id = 7, Name = "Entrada por error en salida"},
            new MovementTypeDto{ Id = 8, Name = "Salida por error en entrada"},
            new MovementTypeDto{ Id = 9, Name = "Entrada por ajuste"},
            new MovementTypeDto{ Id = 10, Name = "Salida por ajuste"},
            new MovementTypeDto{ Id = 11, Name = "Salida por conversion de producto"},
            new MovementTypeDto{ Id = 12, Name = "Entrada por conversion de producto"}
        };
        return list;
    }
}

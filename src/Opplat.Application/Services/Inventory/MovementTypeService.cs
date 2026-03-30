using Opplat.Domain.Dtos.Inventory;
using Opplat.Domain.Entities.Inventory;

namespace Opplat.Application.Services.Inventory;

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
        return [
            new(1, "Entrada"),
            new(2, "Salida a producción"),
            new(3, "Venta independiente"),
            new(4, "Merma"),
            new(5, "Salida traslado interno"),
            new(6, "Entrada traslado interno"),
            new(7, "Entrada por error en salida"),
            new(8, "Salida por error en entrada"),
            new(9, "Entrada por ajuste"),
            new(10, "Salida por ajuste"),
            new(11, "Salida por conversion de producto"),
            new(12, "Entrada por conversion de producto")
        ];
    }
}

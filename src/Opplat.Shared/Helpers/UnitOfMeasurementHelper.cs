using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Shared.Entities;

namespace Opplat.Shared.Helpers;

public class UnitOfMeasurementHelper
{
    private static readonly List<UnitOfMeasurement> _units = new(){
                new UnitOfMeasurement{ Abbreviation = "ml", Name = "Mililitros", CovertionFactor = 1,UnitType = UnitType.Capacity  },
                new UnitOfMeasurement{ Abbreviation = "lt", Name = "Litros", CovertionFactor = 0.001m,UnitType = UnitType.Capacity  },
                new UnitOfMeasurement{ Abbreviation = "g", Name = "Gramos", CovertionFactor = 1,UnitType = UnitType.Weight  },
                new UnitOfMeasurement{ Abbreviation = "Kg", Name = "Kilogramos", CovertionFactor = 0.001m,UnitType = UnitType.Weight  },
                new UnitOfMeasurement{ Abbreviation = "oz", Name = "Onza", CovertionFactor = 0.0353m,UnitType = UnitType.Capacity  },
                new UnitOfMeasurement{ Abbreviation = "lb", Name = "Libra", CovertionFactor = 0.0022m,UnitType = UnitType.Weight  },
                new UnitOfMeasurement{ Abbreviation = "u", Name = "Unidad", CovertionFactor = 1,UnitType = UnitType.Quantity  },
            };
    public static IEnumerable<UnitOfMeasurement> GetUnits()
    {
        return _units;
    }

    public static UnitOfMeasurement GetByAbbreviation(string abbreviation)
    {
        var result = _units.FirstOrDefault(u => u.Abbreviation.ToLower() == abbreviation.ToLower());
        return result;
    }

    public static UnitOfMeasurement GetByName(string name)
    {
        var result = _units.FirstOrDefault(u => u.Name.ToLower() == name.ToLower());
        return result;
    }
}

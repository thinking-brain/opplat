using Opplat.Application.Abstractions.Messaging;
using Opplat.Shared.Entities;
using Opplat.Shared.Helpers;

namespace Opplat.Application.Inventory.UnitsOfMeasurement;

public sealed record ListUnitsOfMeasurementQuery() : IQuery<IReadOnlyList<UnitOfMeasurement>>;

public sealed class ListUnitsOfMeasurementQueryHandler
    : IQueryHandler<ListUnitsOfMeasurementQuery, IReadOnlyList<UnitOfMeasurement>>
{
    public Task<IReadOnlyList<UnitOfMeasurement>> Handle(ListUnitsOfMeasurementQuery request, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<UnitOfMeasurement>>(UnitOfMeasurementHelper.GetUnits().ToList());
}


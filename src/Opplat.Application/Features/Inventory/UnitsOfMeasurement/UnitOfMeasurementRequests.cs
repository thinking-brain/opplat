using Opplat.Application.Abstractions.Messaging;
using Opplat.Application.Helpers;
using Opplat.Domain.Entities;

namespace Opplat.Application.Features.Inventory.UnitsOfMeasurement;

public sealed record ListUnitsOfMeasurementQuery() : IQuery<IReadOnlyList<UnitOfMeasurement>>;

public sealed class ListUnitsOfMeasurementQueryHandler
    : IQueryHandler<ListUnitsOfMeasurementQuery, IReadOnlyList<UnitOfMeasurement>>
{
    public Task<IReadOnlyList<UnitOfMeasurement>> Handle(ListUnitsOfMeasurementQuery request, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<UnitOfMeasurement>>([.. UnitOfMeasurementHelper.GetUnits()]);
}


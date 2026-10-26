using Opplat.Application.Abstractions.Pos;

namespace Opplat.Infrastructure.Services.Pos;

public sealed class PaymentTerminalProviderRegistry(IEnumerable<IPaymentTerminalProvider> providers)
    : IPaymentTerminalProviderRegistry
{
    private readonly Dictionary<string, IPaymentTerminalProvider> _providers =
        providers.ToDictionary(provider => provider.Key, StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<string> Keys => _providers.Keys;

    public IPaymentTerminalProvider? Find(string key)
        => _providers.TryGetValue(key, out var provider) ? provider : null;
}

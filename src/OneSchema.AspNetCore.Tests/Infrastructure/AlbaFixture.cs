using Alba;
using TUnit.Core.Interfaces;

namespace OneSchema.AspNetCore.Tests.Infrastructure;

public sealed class AlbaFixture : IAsyncInitializer, IAsyncDisposable
{
    public IAlbaHost Host { get; private set; } = null!;

    public async Task InitializeAsync() 
    {
        Host = await AlbaHost.For<Program>();
    }

    public async ValueTask DisposeAsync()
    {
        await Host.DisposeAsync();
    }
}
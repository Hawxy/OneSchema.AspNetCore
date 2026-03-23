using Alba;

namespace OneSchema.AspNetCore.Tests.Infrastructure;

public abstract class AlbaTestBase(AlbaFixture albaBootstrap)
{
    protected IAlbaHost Host => albaBootstrap.Host;
}
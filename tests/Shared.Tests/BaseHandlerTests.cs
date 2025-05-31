using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Shared.Tests;

public abstract class BaseHandlerTests : IClassFixture<TestWebFactory>, IAsyncLifetime
{
    protected readonly TestWebFactory _webFactory;
    protected readonly IServiceScope _scope;

    public BaseHandlerTests(TestWebFactory webFactory)
    {
        _webFactory = webFactory;
        _scope = _webFactory.Services.CreateScope();
    }

    public virtual async Task DisposeAsync()
    {
        await _webFactory.ResetDatabaseAsync();
        _scope.Dispose();
    }

    public virtual async Task InitializeAsync() { }
}

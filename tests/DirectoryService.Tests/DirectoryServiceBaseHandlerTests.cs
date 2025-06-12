using DirectoryProject.DirectoryService.Infrastructure.DatabaseWrite;
using Microsoft.Extensions.DependencyInjection;
using Shared.Tests;

namespace DirectoryService.Tests;

public class DirectoryServiceBaseHandlerTests : BaseHandlerTests
{
    public DirectoryServiceBaseHandlerTests(TestWebFactory webFactory) : base(webFactory)
    {
        _context = _scope.ServiceProvider.GetRequiredService<ApplicationWriteDBContext>();
    }

    protected readonly ApplicationWriteDBContext _context;

    public override async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await base.DisposeAsync();
    }
}

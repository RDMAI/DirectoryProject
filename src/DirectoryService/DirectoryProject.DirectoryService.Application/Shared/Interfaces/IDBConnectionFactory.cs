using System.Data;

namespace DirectoryProject.DirectoryService.Application.Shared.Interfaces;

public interface IDBConnectionFactory
{
    public IDbConnection Create();
}

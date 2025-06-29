using System.Data;

namespace DirectoryProject.SharedService.Core.Database;

public interface IDBConnectionFactory
{
    public IDbConnection Create();
}

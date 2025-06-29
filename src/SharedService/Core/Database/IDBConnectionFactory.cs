using System.Data;

namespace Core.Database;

public interface IDBConnectionFactory
{
    public IDbConnection Create();
}

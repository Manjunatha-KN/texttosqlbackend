using System.Data;

namespace InnovationInc.TextToSql.WebApi.Interfaces
{
    public interface IDbService
    {
        Task<DataTable> ExecuteSqlQueryAsync(string sqlQuery, CancellationToken cancellationToken);
    }
}

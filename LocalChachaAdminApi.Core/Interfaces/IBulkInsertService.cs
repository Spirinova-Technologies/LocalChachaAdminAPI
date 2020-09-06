using System.Threading.Tasks;

namespace LocalChachaAdminApi.Core.Interfaces
{
    public interface IBulkInsertService
    {
        Task InsertBulkData();
    }
}
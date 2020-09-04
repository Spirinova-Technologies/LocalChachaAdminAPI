using System.Threading.Tasks;

namespace LocalChachaAdminApi.Interfaces
{
    public interface IBulkInsertService
    {
        public Task InsertBulkData();
    }
}
using System.Threading.Tasks;

namespace Glendale.Design.Data
{
    public interface IDesignDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}

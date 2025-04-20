namespace DataMigrationUtilit.Application.Contracts
{
    public interface IImportDataService
    {
        Task Import(string fileName, string type);
    }
}

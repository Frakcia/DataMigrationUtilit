namespace DataMigrationUtilit.Application.Contracts
{
    public interface IDataImport
    {
        Task Import(IEnumerable<string> lines, int minRowCount);
    }
}

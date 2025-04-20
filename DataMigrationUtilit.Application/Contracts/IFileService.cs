namespace DataMigrationUtilit.Application.Contracts
{
    public interface IFileService
    {
        IAsyncEnumerable<List<string>> ReadLinesAsync(string path, int batchCount);
        bool IsFileExist(string path);
    }
}

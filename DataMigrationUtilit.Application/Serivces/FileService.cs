using DataMigrationUtilit.Application.Contracts;

namespace DataMigrationUtilit.Application.Serivces
{
    public class FileService : IFileService
    {
        public static string BaseDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
        readonly string baseDirPath = BaseDir + "/Files";
        private const string Format = ".tsv";

        public bool IsFileExist(string fileName)
        {
            var filePath = Path.Combine(baseDirPath, fileName + Format);
            return File.Exists(filePath);
        }

        public async IAsyncEnumerable<List<string>> ReadLinesAsync(string fileName, int batchCount)
        {
            string line;
            var filePath = Path.Combine(baseDirPath, fileName + Format);
            List<string> importLines = new List<string>();

            using (StreamReader sr = new StreamReader(filePath))
            {
                //Пропускаем первую строку с названиями колонок
                _ = sr.ReadLine();

                while ((line = await sr.ReadLineAsync()) != null)
                {
                    importLines.Add(line);

                    if (importLines.Count == batchCount)
                    {
                        yield return importLines;
                        importLines.Clear();
                    }
                }

                if (importLines.Count > 0)
                {
                    yield return importLines;
                }
            }
        }
    }
}

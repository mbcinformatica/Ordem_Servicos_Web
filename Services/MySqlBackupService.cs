using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Text;

namespace Ordem_Servicos_Web.Services
{
    public class MySqlBackupService
    {
        private readonly string _connectionString;

        public MySqlBackupService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<string>> BackupTablesAsync(List<string> tableNames)
        {
            var builder = new MySqlConnectionStringBuilder(_connectionString);

            string backupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backup");
            if (!Directory.Exists(backupDir))
                Directory.CreateDirectory(backupDir);

            var arquivosGerados = new List<string>();

            string mysqldumpPath = @"C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqldump.exe";

            foreach (var tableName in tableNames)
            {
                string backupFilePath = Path.Combine(
                    backupDir,
                    $"Backup_{tableName}_{DateTime.Now:yyyyMMddHHmmss}.sql"
                );

                // Gerar apenas os dados, um INSERT por linha
                string arguments =
                    $"--no-create-info --skip-extended-insert --lock-tables " +
                    $"--default-character-set=utf8mb4 " + // 🔹 força UTF-8
                    $"-h {builder.Server} -P {builder.Port} -u {builder.UserID} -p{builder.Password} " +
                    $"{builder.Database} {tableName}";

                var psi = new ProcessStartInfo
                {
                    FileName = mysqldumpPath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8 // 🔹 garante que o dump seja lido em UTF-8
                };

                using var process = Process.Start(psi);
                if (process == null)
                    throw new InvalidOperationException($"Não foi possível iniciar o mysqldump para a tabela {tableName}.");

                string dump = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                    throw new Exception($"Erro ao executar mysqldump para {tableName}: {error}");

                await File.WriteAllTextAsync(backupFilePath, dump, Encoding.UTF8);
                arquivosGerados.Add(backupFilePath);
            }

            return arquivosGerados;
        }
    }
}
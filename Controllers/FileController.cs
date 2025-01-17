using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DavidPaarup.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        [HttpPost]
        public async Task SaveAsync([FromBody] SavePayload payload)
        {
            var connectionString = "Server=(LocalDb)\\MSSQLLocalDB;database=davidpaarup";
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var getFileSql = "SELECT name from file_content where name = @name";
            var getFileCommand = new SqlCommand(getFileSql, connection);
            getFileCommand.Parameters.AddWithValue("@name", payload.File);
            var reader = await getFileCommand.ExecuteReaderAsync();

            string? name = null;

            var hasValue = await reader.ReadAsync();

            if (hasValue)
            {
                name = reader.GetString(0);
            }

            await reader.CloseAsync();

            if (name != null)
            {
                var sql = "UPDATE file_content set content = @content where name = @name";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@name", payload.File);
                command.Parameters.AddWithValue("@content", payload.Content);
                await command.ExecuteNonQueryAsync();
            }
            else
            {
                var sql = "INSERT INTO file_content (name, content) VALUES (@name, @content)";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@name", payload.File);
                command.Parameters.AddWithValue("@content", payload.Content);
                await command.ExecuteNonQueryAsync();
            }

            await connection.CloseAsync();
        }
    }

    public class SavePayload(string file, string content)
    {
        public string File { get; } = file;
        public string Content { get; } = content;
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Data.Odbc;

namespace OracleOdbcApi.Controllers
{
  [ApiController]
    [Route("[controller]")]
    public class QueryController : ControllerBase
    {
        private readonly IConfiguration _config;

        public QueryController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] string sql)
        {
            // SQLクエリが指定されていない場合
            if (string.IsNullOrWhiteSpace(sql))
            {
                return BadRequest(new { error = "SQLクエリが指定されていません。" });
            }

            // 接続文字列の取得とnullチェック
            string? connStr = _config.GetConnectionString("OracleOdbc");
            if (string.IsNullOrWhiteSpace(connStr))
            {
                return StatusCode(500, new { error = "接続文字列が設定されていません。" });
            }

            try
            {
                using var conn = new OdbcConnection(connStr);
                using var cmd = new OdbcCommand(sql, conn);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                var results = new List<Dictionary<string, object>>();

                while (reader.Read())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader.IsDBNull(i) ? null! : reader.GetValue(i);
                    }
                    results.Add(row);
                }

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}


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
            string connStr = _config.GetConnectionString("OracleOdbc");
            using var conn = new OdbcConnection(connStr);
            using var cmd = new OdbcCommand(sql, conn);

            try
            {
                conn.Open();
                using var reader = cmd.ExecuteReader();
                var results = new List<Dictionary<string, object>>();

                while (reader.Read())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                        row[reader.GetName(i)] = reader.GetValue(i);
                    results.Add(row);
                }

                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}

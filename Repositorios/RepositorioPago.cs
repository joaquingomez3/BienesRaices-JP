using bienesraices.Models;
using bienesraices.Repositorios;
using MySql.Data.MySqlClient;

public class RepositorioPago : RepositorioBase
{
    public RepositorioPago(IConfiguration configuration) : base(configuration)
    {

    }

    public async Task<int> ContarPagos()
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();

            var query = "SELECT COUNT(*) FROM pago";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }

    }

    public async Task<List<Pago>> PagosPaginados(int idContrato, int page, int pageSize)
    {
        var lista = new List<Pago>();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();

            var query = @"
        SELECT Id, Id_contrato, Numero_pago, Fecha_pago, Detalle, Importe, Estado, 
               Id_usuario_creador, Id_usuario_anulador
        FROM pago
        WHERE Id_contrato = @IdContrato
        ORDER BY Id
        LIMIT @PageSize OFFSET @Offset";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@IdContrato", idContrato);
                command.Parameters.AddWithValue("@PageSize", pageSize);
                command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new Pago
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Id_contrato = reader.GetInt32(reader.GetOrdinal("Id_contrato")),
                            Numero_pago = reader.GetInt32(reader.GetOrdinal("Numero_pago")),
                            Fecha_pago = reader.GetDateTime(reader.GetOrdinal("Fecha_pago")),
                            Detalle = reader.GetString(reader.GetOrdinal("Detalle")),
                            Importe = reader.GetDecimal(reader.GetOrdinal("Importe")),
                            Estado = reader.GetString(reader.GetOrdinal("Estado")),
                            Id_usuario_creador = reader.GetInt32(reader.GetOrdinal("Id_usuario_creador")),
                            Id_usuario_anulador = reader.IsDBNull(reader.GetOrdinal("Id_usuario_anulador"))
                                                  ? null
                                                  : reader.GetInt32(reader.GetOrdinal("Id_usuario_anulador"))
                        });
                    }
                }
            }
        }

        return lista;
    }

    public async Task<int> ContarPagosPorActivos(int idContrato)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {

            await connection.OpenAsync();

            var query = "SELECT COUNT(*) FROM pago WHERE Id_contrato = @idContrato AND Estado = 'Activo'";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@idContrato", idContrato);
                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }



        }
    }

}
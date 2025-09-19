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

    public int Crear(Pago pago)
    {
        int id = 0; // Variable para almacenar el ID del pago recién insertado
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            // 1. Obtener el próximo número de pago para el contrato específico
            var sqlObtenerProximoNumero = "SELECT IFNULL(MAX(Numero_pago), 0) + 1 FROM Pago WHERE Id_contrato = @Id_contrato;";
            int proximoNumeroPago;
            using (var commandGetNext = new MySqlCommand(sqlObtenerProximoNumero, connection))
            {
                commandGetNext.Parameters.AddWithValue("@Id_contrato", pago.Id_contrato);
                proximoNumeroPago = Convert.ToInt32(commandGetNext.ExecuteScalar());
            }

            // 2. Insertar el nuevo pago en la base de datos
            var sqlInsert = @"
                INSERT INTO Pago (Id_contrato, Numero_pago, Fecha_pago, Detalle, Importe, Estado, Id_usuario_creador)
                VALUES (@Id_contrato, @Numero_pago, @Fecha_pago, @Detalle, @Importe, 'PAGADO', @Id_usuario_creador);
                
                SELECT LAST_INSERT_ID();"; // Obtiene el ID del último registro insertado

            using (var commandInsert = new MySqlCommand(sqlInsert, connection))
            {
                // Asignar los valores a los parámetros de la consulta
                commandInsert.Parameters.AddWithValue("@Id_contrato", pago.Id_contrato);
                commandInsert.Parameters.AddWithValue("@Numero_pago", proximoNumeroPago);
                commandInsert.Parameters.AddWithValue("@Fecha_pago", pago.Fecha_pago);
                commandInsert.Parameters.AddWithValue("@Detalle", pago.Detalle ?? "");
                commandInsert.Parameters.AddWithValue("@Importe", pago.Importe);
                commandInsert.Parameters.AddWithValue("@Id_usuario_creador", pago.Id_usuario_creador);

                // Ejecutar la consulta y obtener el ID del pago creado
                id = Convert.ToInt32(commandInsert.ExecuteScalar());
            }
        }
        return id;
    }


    public int Eliminar(int id)
    {
        int filasAfectadas = 0;
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            var sqlDelete = "DELETE FROM Pago WHERE Id = @Id";

            using (var commandDelete = new MySqlCommand(sqlDelete, connection))
            {
                commandDelete.Parameters.AddWithValue("@Id", id);

                filasAfectadas = commandDelete.ExecuteNonQuery();
            }
        }
        return filasAfectadas;
    }
}
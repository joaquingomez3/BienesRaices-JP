using MySql.Data.MySqlClient;
using bienesraices.Models;


namespace bienesraices.Repositorios;

public class RepositorioContrato : RepositorioBase
{

    public RepositorioContrato(IConfiguration configuration) : base(configuration)
    {


    }

    public async Task<int> ContarContratos(string? filtroDni)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();

            var query = @"
            SELECT COUNT(*) 
            FROM contrato c
            JOIN inquilino i ON i.id = c.id_inquilino
            WHERE (@filtro IS NULL OR i.dni LIKE @filtro);";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                string? filtroNormalizado = string.IsNullOrWhiteSpace(filtroDni) ? null : $"%{filtroDni}%";
                command.Parameters.AddWithValue("@filtro", (object?)filtroNormalizado ?? DBNull.Value);

                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }
    }


    public async Task<List<Contrato>> ContratosPaginados(string? filtroDni, int page, int pageSize)
    {
        var lista = new List<Contrato>();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();

            var query = @"
            SELECT c.id, c.id_inquilino, c.id_inmueble, c.fecha_inicio, c.fecha_fin, c.estado, c.fecha_terminacion, 
                   c.monto_mensual, c.id_usuario_creador, c.id_usuario_finalizador, c.multa,
                   i.nombre_completo AS inquilino_nombre, i.dni AS inquilino_dni,
                   CONCAT('<strong>',ti.nombre,'</strong>',' ', '/',' ', inm.direccion) AS inmueble_nombre,
                   inm.uso,
                   CONCAT(uc.apellido_usuario, ' ', uc.nombre_usuario) AS creador,
                   CONCAT(uf.apellido_usuario, ' ', uf.nombre_usuario) AS finalizador
            FROM contrato c
            JOIN inquilino i ON i.id = c.id_inquilino
            JOIN inmueble inm ON inm.id = c.id_inmueble
            JOIN tipo_inmueble ti ON ti.id = inm.id_tipo
            JOIN usuario uc ON uc.id = c.id_usuario_creador
            LEFT JOIN usuario uf ON uf.id = c.id_usuario_finalizador
            WHERE (@filtro IS NULL OR i.dni LIKE @filtro)
            ORDER BY c.id
            LIMIT @PageSize OFFSET @Offset;";

            string? filtroNormalizado = string.IsNullOrWhiteSpace(filtroDni) ? null : $"%{filtroDni}%";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@filtro", (object?)filtroNormalizado ?? DBNull.Value);
                command.Parameters.AddWithValue("@PageSize", pageSize);
                command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new Contrato
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Id_inquilino = reader.GetInt32(reader.GetOrdinal("id_inquilino")),
                            Id_inmueble = reader.GetInt32(reader.GetOrdinal("id_inmueble")),
                            Fecha_inicio = reader.GetDateTime(reader.GetOrdinal("fecha_inicio")),
                            Fecha_fin = reader.GetDateTime(reader.GetOrdinal("fecha_fin")),
                            Estado = reader.IsDBNull(reader.GetOrdinal("estado")) ? null : reader.GetString(reader.GetOrdinal("estado")),
                            Fecha_terminacion = reader.IsDBNull(reader.GetOrdinal("fecha_terminacion"))
                                                ? (DateTime?)null
                                                : reader.GetDateTime(reader.GetOrdinal("fecha_terminacion")),
                            Monto_mensual = reader.GetDecimal(reader.GetOrdinal("monto_mensual")),
                            Id_usuario_creador = reader.GetInt32(reader.GetOrdinal("id_usuario_creador")),
                            Id_usuario_finalizador = reader.IsDBNull(reader.GetOrdinal("id_usuario_finalizador"))
                                                ? (int?)null
                                                : reader.GetInt32(reader.GetOrdinal("id_usuario_finalizador")),
                            Multa = reader.IsDBNull(reader.GetOrdinal("multa")) ? null : reader.GetDecimal(reader.GetOrdinal("multa")),

                            // extras para mostrar
                            InquilinoContrato = reader.GetString(reader.GetOrdinal("inquilino_nombre")),
                            InmuebleContrato = reader.GetString(reader.GetOrdinal("inmueble_nombre")),
                            InmuebleUso = reader.GetString(reader.GetOrdinal("uso")),
                            Creador = reader.GetString(reader.GetOrdinal("creador")),
                            Finalizador = reader.IsDBNull(reader.GetOrdinal("finalizador"))
                                                ? null
                                                : reader.GetString(reader.GetOrdinal("finalizador"))
                        });
                    }
                }
            }
        }

        return lista;
    }


    public Contrato? ObtenerContratoPorId(int id)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = @"
            SELECT c.id, c.id_inquilino, c.id_inmueble, c.fecha_inicio, c.fecha_fin,c.estado, c.fecha_terminacion, 
                   c.monto_mensual, c.id_usuario_creador, c.id_usuario_finalizador, c.multa,
                   i.nombre_completo AS inquilino_nombre,
                   CONCAT('<strong>',ti.nombre,'</strong>',' ', '/',' ', inm.direccion) AS inmueble_nombre,
                   inm.uso,
                   CONCAT(uc.apellido_usuario, ' ', uc.nombre_usuario) AS creador,
                   CONCAT(uf.apellido_usuario, ' ', uf.nombre_usuario) AS finalizador
            FROM contrato c
            JOIN inquilino i ON i.id = c.id_inquilino
            JOIN inmueble inm ON inm.id = c.id_inmueble
            JOIN tipo_inmueble ti ON ti.id = inm.id_tipo   
            JOIN usuario uc ON uc.id = c.id_usuario_creador
            LEFT JOIN usuario uf ON uf.id = c.id_usuario_finalizador
            Where c.id = @id";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();

                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new Contrato
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Id_inquilino = reader.GetInt32(reader.GetOrdinal("id_inquilino")),
                        Id_inmueble = reader.GetInt32(reader.GetOrdinal("id_inmueble")),
                        Fecha_inicio = reader.GetDateTime(reader.GetOrdinal("fecha_inicio")),
                        Fecha_fin = reader.GetDateTime(reader.GetOrdinal("fecha_fin")),
                        Estado = reader.IsDBNull(reader.GetOrdinal("estado")) ? null : reader.GetString(reader.GetOrdinal("estado")),
                        Fecha_terminacion = reader.IsDBNull(reader.GetOrdinal("fecha_terminacion"))
                                                ? (DateTime?)null
                                                : reader.GetDateTime(reader.GetOrdinal("fecha_terminacion")),
                        Monto_mensual = reader.GetDecimal(reader.GetOrdinal("monto_mensual")),
                        Id_usuario_creador = reader.GetInt32(reader.GetOrdinal("id_usuario_creador")),
                        Id_usuario_finalizador = reader.IsDBNull(reader.GetOrdinal("id_usuario_finalizador"))
                                                ? (int?)null
                                                : reader.GetInt32(reader.GetOrdinal("id_usuario_finalizador")),
                        Multa = reader.IsDBNull(reader.GetOrdinal("multa")) ? null : reader.GetDecimal(reader.GetOrdinal("multa")),
                        InquilinoContrato = reader.GetString(reader.GetOrdinal("inquilino_nombre")),
                        InmuebleContrato = reader.GetString(reader.GetOrdinal("inmueble_nombre")),
                        InmuebleUso = reader.GetString(reader.GetOrdinal("uso")),
                        Creador = reader.GetString(reader.GetOrdinal("creador")),
                        Finalizador = reader.IsDBNull(reader.GetOrdinal("finalizador"))
                                                ? null
                                                : reader.GetString(reader.GetOrdinal("finalizador"))
                    };
                }

            }
        }
        return null;

    }

    public decimal CalcularMulta(int id_contrato)
    {
        decimal multa = 0;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = @"
                    SELECT c.id, c.fechaInicio, c.fechaFin, c.Monto_mensual
                    FROM Contrato c
                    WHERE c.Id = @idContrato;
                ";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id_contrato", id_contrato);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        DateTime fechaInicio = reader.GetDateTime("FechaInicio");
                        DateTime fechaFin = reader.GetDateTime("FechaFin");
                        decimal montoMensual = reader.GetDecimal("MontoAlquilerMensual");

                        DateTime fechaRescicion = DateTime.Now;
                        int totalMeses = (fechaFin.Year - fechaInicio.Year) * 12 + (fechaFin.Month - fechaInicio.Month);
                        int mesesTranscurridos = ((fechaRescicion.Year - fechaInicio.Year) * 12) + fechaRescicion.Month - fechaInicio.Month;

                        if (mesesTranscurridos > totalMeses / 2)
                        {
                            multa = montoMensual * 2;
                        }
                        else
                        {
                            multa = montoMensual * 1;

                        }

                    }
                }
            }
        }
        return multa;
    }

    public void CrearContrato(Contrato contrato)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = @"INSERT INTO contrato 
            (id_inquilino,id_inmueble,fecha_inicio,fecha_fin,estado,fecha_terminacion,monto_mensual,id_usuario_creador,id_usuario_finalizador,multa) 
            VALUES 
            (@id_inquilino,@id_inmueble,@fecha_inicio,@fecha_fin,@estado,@fecha_terminacion,@monto_mensual,@id_usuario_creador,@id_usuario_finalizador,@multa)";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id_inquilino", contrato.Id_inquilino);
                command.Parameters.AddWithValue("@id_inmueble", contrato.Id_inmueble);
                command.Parameters.AddWithValue("@fecha_inicio", contrato.Fecha_inicio);
                command.Parameters.AddWithValue("@fecha_fin", contrato.Fecha_fin);
                command.Parameters.AddWithValue("@estado", string.IsNullOrEmpty(contrato.Estado) ? "Vigente" : contrato.Estado);

                command.Parameters.AddWithValue("@fecha_terminacion", contrato.Fecha_terminacion ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@monto_mensual", contrato.Monto_mensual);
                command.Parameters.AddWithValue("@id_usuario_creador", contrato.Id_usuario_creador);
                command.Parameters.AddWithValue("@id_usuario_finalizador", contrato.Id_usuario_finalizador ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@multa", contrato.Multa ?? (object)DBNull.Value);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al crear contrato: " + ex.Message, ex);
                }
                finally
                {
                    connection.Close();
                }

            }
        }
    }


}



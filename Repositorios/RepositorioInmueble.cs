using MySql.Data.MySqlClient;
using bienesraices.Models;

namespace bienesraices.Repositorios;

using System.Collections.Generic;
using System.Data;

public class RepositorioInmueble : RepositorioBase
{
    public RepositorioInmueble(IConfiguration configuration) : base(configuration)
    {

    }

    public void CrearInmueble(Inmueble inmueble)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "INSERT INTO inmueble (direccion, uso, ambientes, coordenadas, precio, estado, id_propietario, id_tipo) VALUES (@direccion, @uso, @ambientes, @coordenadas, @precio, @estado, @id_propietario, @id_tipo)";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                command.Parameters.AddWithValue("@uso", inmueble.Uso);
                command.Parameters.AddWithValue("@ambientes", inmueble.Ambientes);
                command.Parameters.AddWithValue("@coordenadas", inmueble.Coordenadas);
                command.Parameters.AddWithValue("@precio", inmueble.Precio);
                command.Parameters.AddWithValue("@estado", inmueble.Estado);
                command.Parameters.AddWithValue("@id_propietario", inmueble.Id_Propietario);
                command.Parameters.AddWithValue("@id_tipo", inmueble.Id_Tipo);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public Inmueble? ObtenerPorId(int id)
    {
        Inmueble? inmueble = null;
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = @"SELECT i.id, i.direccion, i.uso, i.ambientes, i.coordenadas, 
                           i.precio, i.estado, i.id_propietario, i.id_tipo, i.descripcion,
                           p.dni, p.apellido, p.nombre,
                           t.nombre AS tipo_inmueble
                    FROM inmueble i
                    INNER JOIN propietario p ON p.id = i.id_propietario
                    INNER JOIN tipo_inmueble t ON t.id = i.id_tipo
                    WHERE i.id = @id";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    inmueble = new Inmueble
                    {
                        Id = reader.GetInt32("id"),
                        Direccion = reader.GetString("direccion"),
                        Uso = reader.GetString("uso"),
                        Ambientes = reader.GetInt32("ambientes"),
                        Coordenadas = reader.GetString("coordenadas"),
                        Precio = reader.GetDecimal("precio"),
                        Estado = reader.GetString("estado"),
                        Id_Propietario = reader.GetInt32("id_propietario"),
                        Id_Tipo = reader.GetInt32("id_tipo"),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : reader.GetString("descripcion"),
                        PropietarioNombre = reader.GetString("dni") + " - " + reader.GetString("apellido") + " " + reader.GetString("nombre"),
                        TipoInmuebleNombre = reader.GetString("tipo_inmueble")
                    };
                }
                connection.Close();
            }
        }
        return inmueble;
    }


    public int Editar(Inmueble inmueble)
    {
        int res = -1;
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = @"UPDATE inmueble 
                            SET direccion=@direccion, uso=@uso, ambientes=@ambientes,
                                coordenadas=@coordenadas, precio=@precio, estado=@estado,
                                id_propietario=@id_propietario, id_tipo=@id_tipo, descripcion=@descripcion
                            WHERE id=@id";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                command.Parameters.AddWithValue("@uso", inmueble.Uso);
                command.Parameters.AddWithValue("@ambientes", inmueble.Ambientes);
                command.Parameters.AddWithValue("@coordenadas", inmueble.Coordenadas);
                command.Parameters.AddWithValue("@precio", inmueble.Precio);
                command.Parameters.AddWithValue("@estado", inmueble.Estado);
                command.Parameters.AddWithValue("@id_propietario", inmueble.Id_Propietario);
                command.Parameters.AddWithValue("@id_tipo", inmueble.Id_Tipo);
                command.Parameters.AddWithValue("@id", inmueble.Id);
                command.Parameters.AddWithValue("@descripcion", inmueble.Descripcion);

                connection.Open();
                res = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return res;
    }



    public void EliminarInmueble(Inmueble inmueble)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "UPDATE inmueble SET estado = 0 WHERE id = @id";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", inmueble.Id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

            }
        }
    }

    // Método que devuelve la cantidad total de inmuebles aplicando filtros dinámicos
    public async Task<int> ContarInmuebles(
    string? propietario,
    string? estado,
    string? uso,
    string? tipo,
    string? ambientes,
    decimal? precioMin,
    decimal? precioMax,
    DateTime? fechaDesde,
    DateTime? fechaHasta)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();

            var query = @"
            SELECT COUNT(*)
            FROM inmueble i
            INNER JOIN propietario p ON i.id_propietario = p.id
            INNER JOIN tipo_inmueble t ON i.id_tipo = t.id
            WHERE i.estado <> 0
        ";

            // 🔹 filtros dinámicos
            if (!string.IsNullOrEmpty(propietario))
                query += " AND (p.apellido LIKE @prop OR p.nombre LIKE @prop)";

            if (!string.IsNullOrEmpty(estado))
                query += " AND i.estado = @estado";

            if (!string.IsNullOrEmpty(uso))
                query += " AND i.uso = @uso";

            if (!string.IsNullOrEmpty(tipo))
                query += " AND t.nombre = @tipo";

            if (!string.IsNullOrEmpty(ambientes))
                query += " AND i.ambientes = @ambientes";

            if (precioMin.HasValue)
                query += " AND i.precio >= @precioMin";

            if (precioMax.HasValue)
                query += " AND i.precio <= @precioMax";

            // Filtro de fechas: excluir inmuebles que tengan contratos solapados
            if (fechaDesde.HasValue && fechaHasta.HasValue)
            {
                query += @"
                    AND i.Id NOT IN (
                        SELECT c.id_inmueble
                        FROM Contrato c
                        WHERE (c.fecha_inicio <= @fechaHasta) AND (c.fecha_fin >= @fechaDesde)
                    )";
            }

            using (var command = new MySqlCommand(query, connection))
            // 🔹 asignación de parámetros (previene SQL Injection)
            {
                if (!string.IsNullOrEmpty(propietario))
                    command.Parameters.AddWithValue("@prop", "%" + propietario + "%");

                if (!string.IsNullOrEmpty(estado))
                    command.Parameters.AddWithValue("@estado", estado);

                if (!string.IsNullOrEmpty(uso))
                    command.Parameters.AddWithValue("@uso", uso);

                if (!string.IsNullOrEmpty(tipo))
                    command.Parameters.AddWithValue("@tipo", tipo);

                if (!string.IsNullOrEmpty(ambientes))
                    command.Parameters.AddWithValue("@ambientes", ambientes);

                if (precioMin.HasValue)
                    command.Parameters.AddWithValue("@precioMin", precioMin);

                if (precioMax.HasValue)
                    command.Parameters.AddWithValue("@precioMax", precioMax);

                if (fechaDesde.HasValue)
                {
                    command.Parameters.AddWithValue("@fechaDesde", fechaDesde.Value.ToString("yyyy-MM-dd"));

                }
                if (fechaHasta.HasValue)
                {
                    command.Parameters.AddWithValue("@fechaHasta", fechaHasta.Value.ToString("yyyy-MM-dd"));
                }

                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }
    }

    // Método que trae la lista de inmuebles filtrados y paginados
    public async Task<List<Inmueble>> InmueblesFiltrados(
        int page,
        int pageSize,
        string? propietario,
        string? estado,
        string? uso,
        string? tipo,
        string? ambientes,
        decimal? precioMin,
        decimal? precioMax,
        DateTime? fechaDesde,
        DateTime? fechaHasta)
    {
        var lista = new List<Inmueble>();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();

            var query = @"
            SELECT 
                i.id,
                i.direccion,
                i.uso,
                i.ambientes,
                i.coordenadas,
                i.precio,
                i.estado,
                i.descripcion,
                CONCAT(p.apellido, ' ', p.nombre) AS propietario,
                t.nombre AS tipo_inmueble
            FROM inmueble i
            INNER JOIN propietario p ON i.id_propietario = p.id
            INNER JOIN tipo_inmueble t ON i.id_tipo = t.id
            WHERE i.estado <> 0
        ";

            // filtros dinámicos
            if (!string.IsNullOrEmpty(propietario))
                query += " AND (p.apellido LIKE @prop OR p.nombre LIKE @prop)";

            if (!string.IsNullOrEmpty(estado))
                query += " AND i.estado = @estado";

            if (!string.IsNullOrEmpty(uso))
                query += " AND i.uso = @uso";

            if (!string.IsNullOrEmpty(tipo))
                query += " AND t.nombre = @tipo";

            if (!string.IsNullOrEmpty(ambientes))
                query += " AND i.ambientes = @ambientes";

            if (precioMin.HasValue)
                query += " AND i.precio >= @precioMin";

            if (precioMax.HasValue)
                query += " AND i.precio <= @precioMax";

            // Filtro de fechas: excluir inmuebles que tengan contratos solapados
            if (fechaDesde.HasValue && fechaHasta.HasValue)
            {
                query += @"
                    AND i.Id NOT IN (
                        SELECT c.id_inmueble
                        FROM Contrato c
                        WHERE (c.fecha_inicio <= @fechaHasta) AND (c.fecha_fin >= @fechaDesde)
                    )";
            }


            query += @" ORDER BY i.id
                    LIMIT @PageSize OFFSET @Offset";

            using (var command = new MySqlCommand(query, connection))
            {
                if (!string.IsNullOrEmpty(propietario))
                    command.Parameters.AddWithValue("@prop", "%" + propietario + "%");

                if (!string.IsNullOrEmpty(estado))
                    command.Parameters.AddWithValue("@estado", estado);

                if (!string.IsNullOrEmpty(uso))
                    command.Parameters.AddWithValue("@uso", uso);

                if (!string.IsNullOrEmpty(tipo))
                    command.Parameters.AddWithValue("@tipo", tipo);

                if (!string.IsNullOrEmpty(ambientes))
                    command.Parameters.AddWithValue("@ambientes", ambientes);

                if (precioMin.HasValue)
                    command.Parameters.AddWithValue("@precioMin", precioMin);

                if (precioMax.HasValue)
                    command.Parameters.AddWithValue("@precioMax", precioMax);

                if (fechaDesde.HasValue)
                {
                    command.Parameters.AddWithValue("@fechaDesde", fechaDesde.Value.ToString("yyyy-MM-dd"));

                }
                if (fechaHasta.HasValue)
                {
                    command.Parameters.AddWithValue("@fechaHasta", fechaHasta.Value.ToString("yyyy-MM-dd"));
                }

                command.Parameters.AddWithValue("@PageSize", pageSize);
                command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new Inmueble
                        {
                            Id = reader.GetInt32("id"),
                            Direccion = reader.GetString("direccion"),
                            Uso = reader.GetString("uso"),
                            Ambientes = reader.GetInt32("ambientes"),
                            Coordenadas = reader.GetString("coordenadas"),
                            Precio = reader.GetDecimal("precio"),
                            Estado = reader.GetString("estado"),
                            PropietarioNombre = reader.GetString("propietario"),
                            TipoInmuebleNombre = reader.GetString("tipo_inmueble")
                        });
                    }
                }
            }
        }
        return lista;
    }



    public List<Inmueble> ObtenerInmueblesDisponibles()
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "SELECT * FROM Inmueble WHERE estado = 'DISPONIBLE'";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    var inmuebles = new List<Inmueble>();
                    while (reader.Read())
                    {
                        inmuebles.Add(new Inmueble
                        {
                            Id = reader.GetInt32("id"),
                            Direccion = reader.GetString("direccion"),
                            Uso = reader.GetString("uso"),
                            Ambientes = reader.GetInt32("ambientes"),
                            Coordenadas = reader.GetString("coordenadas"),
                            Precio = reader.GetDecimal("precio"),
                            Estado = reader.GetString("estado"),
                            Id_Propietario = reader.GetInt32("id_propietario"),
                            Id_Tipo = reader.GetInt32("id_tipo")
                        });
                    }

                    return inmuebles;
                }


            }
        }
    }
}

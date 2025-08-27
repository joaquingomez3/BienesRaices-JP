using MySql.Data.MySqlClient;
using bienesraices.Models;

namespace bienesraices.Repositorios;

using System.Collections.Generic;
public class RepositorioInmueble : RepositorioBase
{
    public RepositorioInmueble(IConfiguration configuration) : base(configuration)
    {

    }

    // public List<Inmueble> ObtenerInmuebles()
    // {
    //     List<Inmueble> inmuebles = new List<Inmueble>();

    //     using (MySqlConnection connection = new MySqlConnection(connectionString))
    //     {
    //         var query = @"SELECT i.id, i.direccion, i.uso, i.ambientes, i.coordenadas, i.precio, i.estado, 
    //                     p.nombre AS propietario, t.nombre AS tipo_inmueble 
    //                     FROM inmueble i INNER JOIN propietario p ON i.id_propietario = p.id INNER JOIN tipo_inmueble t ON i.id_tipo = t.id; ";

    //         using (MySqlCommand command = new MySqlCommand(query, connection))
    //         {
    //             connection.Open();
    //             var reader = command.ExecuteReader();

    //             while (reader.Read())
    //             {
    //                 inmuebles.Add(new Inmueble
    //                 {
    //                     Id = reader.GetInt32("id"),
    //                     Direccion = reader.GetString("direccion"),
    //                     Uso = reader.GetString("uso"),
    //                     Ambientes = reader.GetInt32("ambientes"),
    //                     Coordenadas = reader.GetString("coordenadas"),
    //                     Precio = reader.GetDecimal("precio"),
    //                     Estado = reader.GetString("estado"),
    //                     PropietarioNombre = reader.GetString("propietario"),
    //                     TipoInmuebleNombre = reader.GetString("tipo_inmueble")
    //                 });
    //             }
    //             connection.Close();
    //         }
    //         return inmuebles;
    //     }

    // }
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

    public async Task<int> ContarInmuebles()
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();

            var query = "SELECT COUNT(*) FROM inmueble WHERE estado = 1";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }

    }

    public async Task<List<Inmueble>> InmueblesPaginados(int page, int pageSize)
    {
        var lista = new List<Inmueble>();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();

            var query = @"
                SELECT Id, Direccion, Uso, Ambientes, Coordenadas, Precio, Estado, Id_Propietario, Id_Tipo
                FROM inmueble
                WHERE estado = 1
                ORDER BY Id
                LIMIT @PageSize OFFSET @Offset";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@PageSize", pageSize);
                command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new Inmueble
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Direccion = reader.GetString(reader.GetOrdinal("Direccion")),
                            Uso = reader.GetString(reader.GetOrdinal("Uso")),
                            Ambientes = reader.GetInt32(reader.GetOrdinal("Ambientes")),
                            Coordenadas = reader.GetString(reader.GetOrdinal("Coordenadas")),
                            Precio = reader.GetDecimal(reader.GetOrdinal("Precio")),
                            Estado = reader.GetString(reader.GetOrdinal("Estado")),
                            Id_Propietario = reader.GetInt32(reader.GetOrdinal("Id_Propietario")),
                            Id_Tipo = reader.GetInt32(reader.GetOrdinal("Id_Tipo"))
                        });
                    }
                }
            }
            await connection.CloseAsync();
        }
        return lista;
    }
}
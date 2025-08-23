using MySql.Data.MySqlClient;
using bienesraices.Models;

namespace bienesraices.Repositorios;

using System.Collections.Generic;
public class RepositorioInmueble : RepositorioBase
{
    public RepositorioInmueble(IConfiguration configuration) : base(configuration)
    {

    }

    public List<Inmueble> ObtenerInmuebles()
    {
        List<Inmueble> inmuebles = new List<Inmueble>();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = @"SELECT i.id, i.direccion, i.uso, i.ambientes, i.coordenadas, i.precio, i.estado, 
                        p.nombre AS propietario, t.nombre AS tipo_inmueble 
                        FROM inmueble i INNER JOIN propietario p ON i.id_propietario = p.id INNER JOIN tipo_inmueble t ON i.id_tipo = t.id; ";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();

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
                        PropietarioNombre = reader.GetString("propietario"),
                        TipoInmuebleNombre = reader.GetString("tipo_inmueble")
                    });
                }
                connection.Close();
            }
            return inmuebles;
        }

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

}
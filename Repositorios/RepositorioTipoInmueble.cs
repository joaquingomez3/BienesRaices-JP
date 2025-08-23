using MySql.Data.MySqlClient;
using bienesraices.Models;

namespace bienesraices.Repositorios;

using System.Collections.Generic;

public class RepositorioTipoInmueble : RepositorioBase
{
    public RepositorioTipoInmueble(IConfiguration configuration) : base(configuration)
    {

    }

    public List<Tipo_inmueble> ObtenerTiposInmueble()
    {
        List<Tipo_inmueble> tipos = new List<Tipo_inmueble>();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "SELECT * FROM tipo_inmueble";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    tipos.Add(new Tipo_inmueble
                    {
                        Id = reader.GetInt32("id"),
                        Nombre = reader.GetString("nombre")
                    });
                }
                connection.Close();
            }
            return tipos;

        }
    }


}
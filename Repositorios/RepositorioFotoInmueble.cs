using bienesraices.Models;
using MySql.Data.MySqlClient;

namespace bienesraices.Repositorios;

public class RepositorioFotoInmueble : RepositorioBase
{
    public RepositorioFotoInmueble(IConfiguration configuration) : base(configuration)
    {

    }

    public FotoInmueble ObtenerFotoPorId(int id)
    {
        FotoInmueble? foto = null;

        using var connection = new MySqlConnection(connectionString);
        var query = "SELECT id_foto, id_inmueble, url, archivo FROM foto_inmueble WHERE id_foto = @id";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        connection.Open();

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            foto = new FotoInmueble
            {
                Id_foto = reader.GetInt32("id_foto"),
                Id_inmueble = reader.GetInt32("id_inmueble"),
                Url = reader.IsDBNull(reader.GetOrdinal("url")) ? null : reader.GetString("url"),
                Archivo = reader.IsDBNull(reader.GetOrdinal("archivo")) ? null : (byte[])reader["archivo"]
            };
        }

        return foto!;
    }
    public List<FotoInmueble> ObtenerFotosPorInmuebleId(int id)
    {
        var fotos = new List<FotoInmueble>();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "SELECT id_foto, id_inmueble, url, archivo FROM foto_inmueble WHERE id_inmueble = @id";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var foto = new FotoInmueble
                        {
                            Id_foto = reader.GetInt32("id_foto"),
                            Id_inmueble = reader.GetInt32("id_inmueble"),
                            Url = reader.IsDBNull(reader.GetOrdinal("url")) ? null : reader.GetString("url"),
                            Archivo = reader.IsDBNull(reader.GetOrdinal("archivo")) ? null : (byte[])reader["archivo"]
                        };
                        fotos.Add(foto);
                    }
                }
            }
        }
        return fotos;
    }

    public int AgregarFoto(FotoInmueble foto)
    {
        int res = -1;
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = @"INSERT INTO foto_inmueble (id_inmueble, url, archivo)
                    VALUES (@id_inmueble, @url, @archivo)";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.Add("@id_inmueble", MySqlDbType.Int32).Value = foto.Id_inmueble;
                var parametroUrl = command.Parameters.Add("@url", MySqlDbType.VarChar, 255);
                if (!string.IsNullOrEmpty(foto.Url))
                {
                    parametroUrl.Value = foto.Url;
                }
                else
                {
                    parametroUrl.Value = DBNull.Value;
                }
                var parametroArchivo = command.Parameters.Add("@archivo", MySqlDbType.LongBlob);
                if (foto.Archivo != null)
                {
                    parametroArchivo.Value = foto.Archivo;
                }
                else
                {
                    parametroArchivo.Value = DBNull.Value;
                }

                connection.Open();
                res = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return res;
    }

    public void EliminarFoto(int idFoto)
    {
        using var connection = new MySqlConnection(connectionString);
        var query = "DELETE FROM foto_inmueble WHERE id_foto = @idFoto";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@idFoto", idFoto);
        connection.Open();
        command.ExecuteNonQuery();
    }
}

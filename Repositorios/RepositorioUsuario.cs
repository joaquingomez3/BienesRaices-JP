using MySql.Data.MySqlClient;
using bienesraices.Models;
namespace bienesraices.Repositorios;

public class RepositorioUsuario
{
    string ConnectionString = "Server=localhost;User=root;Password=;Database=Inmobiliaria;SslMode=none";

    public Usuario? ObtenerUsuario(string email, string password)
    {
        Usuario? usuario = null;

        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            string query = @"SELECT id, nombre_usuario, apellido_usuario, email, password, id_tipo_usuario, activo, foto 
                             FROM usuario 
                             WHERE email = @Email AND password = @Password AND activo = 1";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        usuario = new Usuario
                        {
                            Id = reader.GetInt32("id"),
                            Nombre_usuario = reader.GetString("nombre_usuario"),
                            Apellido_usuario = reader.GetString("apellido_usuario"),
                            Email = reader.GetString("email"),
                            Password = reader.GetString("password"),
                            Id_tipo_usuario = reader.GetInt32("id_tipo_usuario"),
                            Activo = reader.GetBoolean("activo"),
                            Foto = reader["foto"].ToString()
                        };
                    }
                }
            }
        }

        return usuario;
    }


}

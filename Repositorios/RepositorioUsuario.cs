using MySql.Data.MySqlClient;
using bienesraices.Models;
namespace bienesraices.Repositorios;

public class RepositorioUsuario : RepositorioBase
{
    public RepositorioUsuario(IConfiguration configuration) : base(configuration)
    {

    }

    public List<Usuario> ObtenerUsuarios()
    {
        List<Usuario> usuarios = new List<Usuario>();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "SELECT * FROM usuario";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    usuarios.Add(new Usuario
                    {
                        Id = reader.GetInt32("id"),
                        Nombre_usuario = reader.GetString("nombre_usuario"),
                        Apellido_usuario = reader.GetString("apellido_usuario"),
                        Email = reader.GetString("email"),
                        Password = reader.GetString("password"),
                        Id_tipo_usuario = reader.GetInt32("id_tipo_usuario"),
                        Activo = reader.GetBoolean("activo"),
                        

                    });
                }
                connection.Close();
            }
            return usuarios;

        }
    }

    public void CrearUsuario(Usuario usuario)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "INSERT INTO usuario (nombre_usuario, apellido_usuario, email, password, id_tipo_usuario, activo) VALUES (@nombre_usuario, @apellido_usuario, @email, @password, @id_tipo_usuario, @activo)";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@nombre_usuario", usuario.Nombre_usuario);
                command.Parameters.AddWithValue("@apellido_usuario", usuario.Apellido_usuario);
                command.Parameters.AddWithValue("@email", usuario.Email);
                command.Parameters.AddWithValue("@password", usuario.Password);
                command.Parameters.AddWithValue("@id_tipo_usuario", usuario.Id_tipo_usuario);
                command.Parameters.AddWithValue("@activo", usuario.Activo);
                

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public void ActualizarUsuario(Usuario usuario)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "UPDATE usuario SET nombre_usuario=@nombre_usuario, apellido_usuario=@apellido_usuario, email=@email, password=@password, id_tipo_usuario=@id_tipo_usuario, activo=@activo WHERE id=@id";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@nombre_usuario", usuario.Nombre_usuario);
                command.Parameters.AddWithValue("@apellido_usuario", usuario.Apellido_usuario);
                command.Parameters.AddWithValue("@email", usuario.Email);
                command.Parameters.AddWithValue("@password", usuario.Password);
                command.Parameters.AddWithValue("@id_tipo_usuario", usuario.Id_tipo_usuario);
                command.Parameters.AddWithValue("@activo", usuario.Activo);
                command.Parameters.AddWithValue("@id", usuario.Id);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public void EliminarUsuario(Usuario usuario)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "DELETE FROM usuario WHERE id=@id";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", usuario.Id);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public Usuario? ObtenerUsuarioPorId(int id)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "SELECT * FROM usuario WHERE id = @id";

            using(MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();

                var reader = command.ExecuteReader();
                    if (reader.Read()) 
                    {
                        return new Usuario
                        {
                            Id = reader.GetInt32("id"),
                            Nombre_usuario = reader.GetString("nombre_usuario"),
                            Apellido_usuario = reader.GetString("apellido_usuario"),
                            Email = reader.GetString("email"),
                            Password = reader.GetString("password"),
                            Id_tipo_usuario = reader.GetInt32("id_tipo_usuario"),
                            Activo = reader.GetBoolean("activo"),
                            

                        };
                    }
            }
        }
        return null;
    }
    public Usuario? ObtenerUsuarioPorEmailClave(string email, string password)
    {
        Usuario? usuario = null;
    
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "SELECT * FROM usuario WHERE email = @email AND password = @password";

            using(MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@email", email );
                command.Parameters.AddWithValue("@password", password );
                connection.Open();

                var reader = command.ExecuteReader();
                    if (reader.Read()) 
                    {
                        return new Usuario
                        {
                            Id = reader.GetInt32("id"),
                            Nombre_usuario = reader.GetString("nombre_usuario"),
                            Apellido_usuario = reader.GetString("apellido_usuario"),
                            Email = reader.GetString("email"),
                            Password = reader.GetString("password"),
                            Id_tipo_usuario = reader.GetInt32("id_tipo_usuario"),
                            Activo = reader.GetBoolean("activo"),
                            

                        };
                    }
            }
        }
        return usuario;
    }

}

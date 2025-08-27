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
            var query = @"SELECT u.id, u.nombre_usuario, u.apellido_usuario, u.email, u.password, 
                             u.id_tipo_usuario, u.activo, u.foto,
                             t.rol_usuario
                      FROM usuario u
                      INNER JOIN tipo_usuario t ON u.id_tipo_usuario = t.id_tipo_usuario";

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
                        RolUsuario = reader.GetString("rol_usuario"),
                        Activo = reader.GetBoolean("activo"),
                        Foto = reader.IsDBNull(reader.GetOrdinal("foto")) ? null : reader["foto"].ToString(),
                       // ← nuevo campo


                    });
                }
                connection.Close();
            }
            return usuarios;

        }
    }

    public int Alta(Usuario usuario)
{
    int res = -1;
    using (MySqlConnection connection = new MySqlConnection(connectionString))
    {
            string sql = @"INSERT INTO Usuario 
                       (Nombre_usuario, Apellido_usuario, Email, Password, Id_tipo_usuario, Activo, Foto)
                       VALUES 
                       (@nombre, @apellido, @email, @password, @idTipo, @activo, @foto);
                       SELECT LAST_INSERT_ID()";

        using (MySqlCommand cmd = new MySqlCommand(sql, connection))
        {
            cmd.Parameters.AddWithValue("@nombre", usuario.Nombre_usuario);
            cmd.Parameters.AddWithValue("@apellido", usuario.Apellido_usuario);
            cmd.Parameters.AddWithValue("@email", usuario.Email);
            cmd.Parameters.AddWithValue("@password", usuario.Password);
            cmd.Parameters.AddWithValue("@idTipo", usuario.Id_tipo_usuario);
            cmd.Parameters.AddWithValue("@activo", usuario.Activo);
            cmd.Parameters.AddWithValue("@foto", (object?)usuario.Foto ?? DBNull.Value);

            connection.Open();
            res = Convert.ToInt32(cmd.ExecuteScalar());
            usuario.Id = res;
        }
    }
    return res;
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

            using (MySqlCommand command = new MySqlCommand(query, connection))
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

    public Usuario? ObtenerUsuarioPorEmail(string email)
{
    Usuario? usuario = null;

    using (MySqlConnection connection = new MySqlConnection(connectionString))
    {
        string sql = @"SELECT 
                   u.Id, 
                   u.Nombre_usuario, 
                   u.Apellido_usuario, 
                   u.Email, 
                   u.Password, 
                   u.Id_tipo_usuario, 
                   u.Activo, 
                   u.Foto, 
                   t.rol_usuario
               FROM Usuario u
               JOIN tipo_usuario t ON t.id_tipo_usuario = u.Id_tipo_usuario
               WHERE u.Email = @email";

        using (MySqlCommand cmd = new MySqlCommand(sql, connection))
        {
            cmd.Parameters.AddWithValue("@email", email);
            connection.Open();

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    usuario = new Usuario
                    {
                        Id = reader.GetInt32("Id"),
                        Nombre_usuario = reader.GetString("Nombre_usuario"),
                        Apellido_usuario = reader.GetString("Apellido_usuario"),
                        Email = reader.GetString("Email"),
                        Password = reader.GetString("Password"),
                        Id_tipo_usuario = reader.GetInt32("Id_tipo_usuario"),
                        Activo = reader.GetBoolean("Activo"),
                        Foto = reader.IsDBNull(reader.GetOrdinal("Foto")) ? null : reader.GetString("Foto"),
                        RolUsuario = reader.GetString("rol_usuario") 
                    };
                }
            }
        }
    }

    return usuario;
}
   

}

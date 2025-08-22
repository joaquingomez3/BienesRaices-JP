using MySql.Data.MySqlClient;
using bienesraices.Models;

namespace bienesraices.Repositorios;

using System.Collections.Generic;
// Repositorio para manejar las operaciones relacionadas con los propietarios
public class RepositorioPropietario:RepositorioBase
{
    public RepositorioPropietario(IConfiguration configuration):base(configuration)
    {
    
   }

    public List<Propietario> ObtenerPropietarios()
    {
        List<Propietario> propietarios = new List<Propietario>();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "SELECT * FROM  propietario WHERE estado = 1";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    propietarios.Add(new Propietario
                    {
                        Id = reader.GetInt32("id"),
                        Dni = reader.GetString("dni"),
                        Apellido = reader.GetString("apellido"),
                        Nombre = reader.GetString("nombre"),
                        Telefono = reader.GetString("telefono"),
                        Email = reader.GetString("email"),
                        Direccion = reader.GetString("direccion")

                    });
                }
                connection.Close();
            }
            return propietarios;

        }
    }
    public void CrearPropietario(Propietario propietario)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "INSERT INTO propietario (dni, apellido, nombre, telefono, email, direccion, estado) VALUES (@dni, @apellido, @nombre, @telefono, @email, @direccion, @estado)";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@dni", propietario.Dni);
                command.Parameters.AddWithValue("@apellido", propietario.Apellido);
                command.Parameters.AddWithValue("@nombre", propietario.Nombre);
                command.Parameters.AddWithValue("@telefono", propietario.Telefono);
                command.Parameters.AddWithValue("@email", propietario.Email);
                command.Parameters.AddWithValue("@direccion", propietario.Direccion);
                command.Parameters.AddWithValue("@estado", propietario.Estado);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public Propietario? ObtenerPropietarioPorId(int id)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "SELECT * FROM propietario WHERE id = @id";

            using(MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();

                var reader = command.ExecuteReader();


                    if (reader.Read()) 
                    {
                        return new Propietario
                        {
                            Id = reader.GetInt32("id"),
                            Dni = reader.GetString("dni"),
                            Apellido = reader.GetString("apellido"),
                            Nombre = reader.GetString("nombre"),
                            Telefono = reader.GetString("telefono"),
                            Email = reader.GetString("email"),
                            Direccion = reader.GetString("direccion"),
                            Estado = reader.GetInt32("estado")

                        };
                    }
            }
        }
        return null;
    }
    public void ActualizarPropietario(Propietario propietario)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "UPDATE propietario SET dni = @dni, apellido = @apellido, nombre = @nombre, telefono = @telefono, email = @email, direccion = @direccion, estado = @estado WHERE id = @id";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", propietario.Id);
                command.Parameters.AddWithValue("@dni", propietario.Dni);
                command.Parameters.AddWithValue("@apellido", propietario.Apellido);
                command.Parameters.AddWithValue("@nombre", propietario.Nombre);
                command.Parameters.AddWithValue("@telefono", propietario.Telefono);
                command.Parameters.AddWithValue("@email", propietario.Email);
                command.Parameters.AddWithValue("@direccion", propietario.Direccion);
                command.Parameters.AddWithValue("@estado", propietario.Estado);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
        public void EliminarPropietario(Propietario propietario)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "UPDATE propietario SET estado = 0 WHERE id = @id";

            using (MySqlCommand command = new MySqlCommand(query, connection))
           {
                command.Parameters.AddWithValue("@id", propietario.Id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

           }
        }
    }

}
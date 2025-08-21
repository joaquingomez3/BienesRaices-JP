using MySql.Data.MySqlClient;
using bienesraices.Models;

namespace bienesraices.Repositorios;

using System.Collections.Generic;
// Repositorio para manejar las operaciones relacionadas con los propietarios
public class RepositorioPropietario
{
    string ConnectionString = "Server=localhost;User=root;Password=;Database=Inmobiliaria;SslMode=none";

    public List<Propietario> ObtenerPropietarios()
    {
        List<Propietario> propietarios = new List<Propietario>();

        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = "SELECT * FROM  propietario";

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
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = "INSERT INTO propietario (dni, apellido, nombre, telefono, email, direccion) VALUES (@dni, @apellido, @nombre, @telefono, @email, @direccion)";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@dni", propietario.Dni);
                command.Parameters.AddWithValue("@apellido", propietario.Apellido);
                command.Parameters.AddWithValue("@nombre", propietario.Nombre);
                command.Parameters.AddWithValue("@telefono", propietario.Telefono);
                command.Parameters.AddWithValue("@email", propietario.Email);
                command.Parameters.AddWithValue("@direccion", propietario.Direccion);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public void ActualizarPropietario(Propietario propietario)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = "UPDATE propietario SET dni = @dni, apellido = @apellido, nombre = @nombre, telefono = @telefono, email = @email, direccion = @direccion WHERE id = @id";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", propietario.Id);
                command.Parameters.AddWithValue("@dni", propietario.Dni);
                command.Parameters.AddWithValue("@apellido", propietario.Apellido);
                command.Parameters.AddWithValue("@nombre", propietario.Nombre);
                command.Parameters.AddWithValue("@telefono", propietario.Telefono);
                command.Parameters.AddWithValue("@email", propietario.Email);
                command.Parameters.AddWithValue("@direccion", propietario.Direccion);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }



}
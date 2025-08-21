using MySql.Data.MySqlClient;
using bienesraices.Models;

namespace bienesraices.Repositorios;

using System.Collections.Generic;
public class RepositorioInquilino
{
    string ConnectionString = "Server=localhost;User=root;Password=;Database=Inmobiliaria;SslMode=none";

    public List<Inquilino> ObtenerInquilinos()
    {
        List<Inquilino> inquilinos = new List<Inquilino>();

        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = "SELECT * FROM  inquilino";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    inquilinos.Add(new Inquilino
                    {
                        Id = reader.GetInt32("id"),
                        Dni = reader.GetString("dni"),
                        Nombre_completo = reader.GetString("nombre_completo"),
                        Telefono = reader.GetString("telefono"),
                        Email = reader.GetString("email"),
                        Direccion = reader.GetString("direccion")
                    });
                }
                connection.Close();
            }
            return inquilinos;

        }
    }
    public void CrearInquilino(Inquilino inquilino)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = "INSERT INTO inquilino (dni, nombre_completo, telefono, email, direccion) VALUES (@dni, @nombre_completo, @telefono, @email, @direccion)";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@dni", inquilino.Dni);
                command.Parameters.AddWithValue("@nombre_completo", inquilino.Nombre_completo);
                command.Parameters.AddWithValue("@telefono", inquilino.Telefono);
                command.Parameters.AddWithValue("@email", inquilino.Email);
                command.Parameters.AddWithValue("@direccion", inquilino.Direccion);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public void ActualizarInquilino(Inquilino inquilino)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = "UPDATE inquilino SET dni = @dni, nombre_completo = @nombre_completo, telefono = @telefono, email = @email, direccion = @direccion WHERE id = @id";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", inquilino.Id);
                command.Parameters.AddWithValue("@dni", inquilino.Dni);
                command.Parameters.AddWithValue("@nombre_completo", inquilino.Nombre_completo);
                command.Parameters.AddWithValue("@telefono", inquilino.Telefono);
                command.Parameters.AddWithValue("@email", inquilino.Email);
                command.Parameters.AddWithValue("@direccion", inquilino.Direccion);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}
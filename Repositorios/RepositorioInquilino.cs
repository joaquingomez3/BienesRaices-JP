using MySql.Data.MySqlClient;
using bienesraices.Models;
namespace bienesraices.Repositorios;

using System.Collections.Generic;
using Mysqlx.Crud;
using MySqlX.XDevAPI.Common;

public class RepositorioInquilino : RepositorioBase
{

    public RepositorioInquilino(IConfiguration configuration) : base(configuration)
    {

    }
    /*public List<Inquilino> ObtenerInquilinos()
    {
        List<Inquilino> inquilinos = new List<Inquilino>();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "SELECT * FROM  inquilino WHERE estado = 1";

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
    }*/
    public void CrearInquilino(Inquilino inquilino)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "INSERT INTO inquilino (dni, nombre_completo, telefono, email, direccion, estado) VALUES (@dni, @nombre_completo, @telefono, @email, @direccion, @estado)";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@dni", inquilino.Dni);
                command.Parameters.AddWithValue("@nombre_completo", inquilino.Nombre_completo);
                command.Parameters.AddWithValue("@telefono", inquilino.Telefono);
                command.Parameters.AddWithValue("@email", inquilino.Email);
                command.Parameters.AddWithValue("@direccion", inquilino.Direccion);
                command.Parameters.AddWithValue("@estado", inquilino.Estado);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public void ActualizarInquilino(Inquilino inquilino)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "UPDATE inquilino SET dni = @dni, nombre_completo = @nombre_completo, telefono = @telefono, email = @email, direccion = @direccion, estado = @estado WHERE id = @id";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", inquilino.Id);
                command.Parameters.AddWithValue("@dni", inquilino.Dni);
                command.Parameters.AddWithValue("@nombre_completo", inquilino.Nombre_completo);
                command.Parameters.AddWithValue("@telefono", inquilino.Telefono);
                command.Parameters.AddWithValue("@email", inquilino.Email);
                command.Parameters.AddWithValue("@direccion", inquilino.Direccion);
                command.Parameters.AddWithValue("@estado", inquilino.Estado);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public Inquilino? ObtenerPropietarioPorId(int id)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "SELECT * FROM inquilino WHERE id = @id";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();

                var reader = command.ExecuteReader();


                if (reader.Read())
                {
                    return new Inquilino
                    {
                        Id = reader.GetInt32("id"),
                        Dni = reader.GetString("dni"),
                        Nombre_completo = reader.GetString("nombre_completo"),
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

    public void EliminarInquilino(Inquilino inquilino)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "UPDATE inquilino SET estado = 0 WHERE id = @id";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", inquilino.Id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

            }
        }
    }

    public async Task<int> ContarInquilinos()
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();

            var query = "SELECT COUNT(*) FROM inquilino";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }

    }
    public async Task<List<Inquilino>> InquilinosPaginados(int page, int pageSize)
    {
        var lista = new List<Inquilino>();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();

            var query = @"
            SELECT Id, Dni, Nombre_completo, Telefono, Email, Direccion,Estado
            FROM inquilino
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
                        lista.Add(new Inquilino
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Dni = reader.GetString(reader.GetOrdinal("Dni")),
                            Nombre_completo = reader.GetString(reader.GetOrdinal("Nombre_completo")),
                            Telefono = reader.GetString(reader.GetOrdinal("Telefono")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Direccion = reader.GetString(reader.GetOrdinal("Direccion")),
                            Estado = reader.GetInt32(reader.GetOrdinal("Estado"))
                        });
                    }
                }
            }
        }

        return lista;
    }

}
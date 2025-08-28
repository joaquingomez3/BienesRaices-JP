using bienesraices.Models;

public class FotoInmueble
{
    public int Id_foto { get; set; }

    public int Id_inmueble { get; set; }

    public string? Url { get; set; } 

    public byte[]? Archivo { get; set; } 

    public Inmueble? Inmueble { get; set; }

}
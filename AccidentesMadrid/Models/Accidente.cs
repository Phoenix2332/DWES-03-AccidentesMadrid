using AccidentesMadrid.Enums;

namespace AccidentesMadrid.Models;

public class Accidente {
    public string NumExpediente { get; init; } = string.Empty;
    public DateOnly Fecha { get; init; }
    public TimeOnly Hora { get; init; }
    public string Localizacion { get; init; } = string.Empty;
    public int? Numero { get; init; }
    public int CodDistrito { get; init; }
    public string Distrito { get; init; } = string.Empty;
    public TipoAccidente TipoAccidente { get; init; }
    public string EstadoMeteorologico { get; init; } = string.Empty;
    public string TipoVehiculo { get; init; } = string.Empty;
    public TipoPersona TipoPersona { get; init; }
    public string RangoEdad { get; init; } = string.Empty;
    public Sexo Sexo { get; init; }
    public CodigoAccidente CodLesividad { get; init; }
    public Gravedad Gravedad { get; init; }
    public string? CoordenadaXUtm { get; init; }
    public string? CoordenadaYUtm { get; init; }
    public bool PositivoAlcohol { get; init; }
    public bool PositivoDroga { get; init; }
}
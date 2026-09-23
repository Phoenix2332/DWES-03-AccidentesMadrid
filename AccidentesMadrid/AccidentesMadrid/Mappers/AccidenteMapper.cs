using System.Globalization;
using AccidentesMadrid.Enums;
using AccidentesMadrid.Models;
using CsvHelper.Configuration;
using Serilog;

namespace AccidentesMadrid.Mappers;

public class AccidenteMapper : ClassMap<Accidente> {
    private static readonly CultureInfo _es = CultureInfo.GetCultureInfo("es-Es");
    private readonly ILogger _logger = Log.ForContext<AccidenteMapper>();

    public AccidenteMapper() {
        _logger.Debug("[MAP-MAP] Mapeando accidentes");
        Map(m => m.NumExpediente).Name("num_expediente");
        Map(m => m.Fecha).Name("fecha").Convert(args =>
            DateOnly.TryParseExact(args.Row.GetField("fecha"), "dd/MM/yyyy",
                _es, DateTimeStyles.None, out var fecha)
                ? fecha
                : DateOnly.MinValue
        );
        Map(m => m.Hora).Name("hora").Convert(args =>
            TimeOnly.TryParseExact(args.Row.GetField("hora"), ["H:mm:ss", "HH:mm:ss"],
                _es, DateTimeStyles.None, out var hora)
                ? hora
                : TimeOnly.MinValue
        );
        Map(m => m.Localizacion).Name("localizacion");
        Map(m => m.Numero).Name("numero").Convert(args =>
            int.TryParse(args.Row.GetField("numero"), NumberStyles.Integer, _es, out var numero)
                ? numero
                : null
        );
        Map(m => m.CodDistrito).Name("cod_distrito").Convert(args =>
            int.TryParse(args.Row.GetField("cod_distrito"), NumberStyles.Integer, _es, out var cod)
                ? cod
                : 0
        );
        Map(m => m.Distrito).Name("distrito");
        Map(m => m.TipoAccidente).Name("tipo_accidente").Convert(args => ObtenerTipoAccidente(
            args.Row.GetField("tipo_accidente"))
        );
        Map(m => m.EstadoMeteorologico).Name("estado_meteorológico").Convert(args =>
            string.IsNullOrWhiteSpace(args.Row.GetField("estado_meteorológico"))
                ? "Se desconoce"
                : args.Row.GetField("estado_meteorológico")
        );
        Map(m => m.TipoVehiculo).Name("tipo_vehiculo");
        Map(m => m.TipoPersona).Name("tipo_persona").Convert(args => ObtenerTipoPersona(
            args.Row.GetField("tipo_persona"))
        );
        Map(m => m.RangoEdad).Name("rango_edad");
        Map(m => m.Sexo).Name("sexo").Convert(args => ObtenerSexo(
            args.Row.GetField("sexo"))
        );
        Map(m => m.CodLesividad).Name("cod_lesividad").Convert(args => ObtenerCodigoLesividad(
            args.Row.GetField("cod_lesividad"))
        );
        Map(m => m.Gravedad).Name("cod_lesividad").Convert(args => ObtenerGravedad(
            args.Row.GetField("cod_lesividad"))
        );
        Map(m => m.CoordenadaXUtm).Name("coordenada_x_utm").Convert(args =>
            string.IsNullOrWhiteSpace(args.Row.GetField("coordenada_x_utm"))
                ? null
                : args.Row.GetField("coordenada_x_utm")
        );
        Map(m => m.CoordenadaYUtm).Name("coordenada_y_utm").Convert(args =>
            string.IsNullOrWhiteSpace(args.Row.GetField("coordenada_y_utm"))
                ? null
                : args.Row.GetField("coordenada_y_utm")
        );
        Map(m => m.PositivoAlcohol).Name("positiva_alcohol").Convert(args =>
            string.Equals(args.Row.GetField("positiva_alcohol")?.Trim(), "S", StringComparison.OrdinalIgnoreCase));
        Map(m => m.PositivoDroga).Name("positiva_droga").Convert(args =>
            string.Equals(args.Row.GetField("positiva_droga")?.Trim(), "1", StringComparison.OrdinalIgnoreCase));

        _logger.Debug("[MAP-MAP] Accidentes mapeados");
    }

    private static TipoAccidente ObtenerTipoAccidente(string? valor) {
        if (string.IsNullOrWhiteSpace(valor))
            return TipoAccidente.Desconocido;

        var texto = valor.Trim().ToLowerInvariant();

        if (texto.Contains("múltiple"))
            return TipoAccidente.ColisionMultiple;

        if (texto.Contains("colisión"))
            return TipoAccidente.ColisionDoble;

        if (texto.Contains("alcance"))
            return TipoAccidente.Alcance;

        if (texto.Contains("obstáculo"))
            return TipoAccidente.ChoqueObstaculo;

        if (texto.Contains("atropello"))
            return TipoAccidente.AtropelloPersona;

        if (texto.Contains("vuelco"))
            return TipoAccidente.Vuelco;

        if (texto.Contains("caída"))
            return TipoAccidente.Caida;

        return texto.Contains("otras") ? TipoAccidente.OtrasCausas : TipoAccidente.Desconocido;
    }

    private static TipoPersona ObtenerTipoPersona(string? valor) {
        return valor?.Trim().ToLowerInvariant() switch {
            "conductor" => TipoPersona.Conductor,
            "pasajero" => TipoPersona.Pasajero,
            "peatón" => TipoPersona.Peatón,
            _ => TipoPersona.Conductor
        };
    }

    private static Sexo ObtenerSexo(string? valor) {
        return valor?.Trim().ToLowerInvariant() switch {
            "hombre" => Sexo.Hombre,
            "mujer" => Sexo.Mujer,
            _ => Sexo.NoAsignado
        };
    }

    private static CodigoAccidente ObtenerCodigoLesividad(string? valor) {
        if (int.TryParse(valor, NumberStyles.Integer, _es, out var codigo)
            && Enum.IsDefined(typeof(CodigoAccidente), codigo))
            return (CodigoAccidente)codigo;

        return CodigoAccidente.Desconocida;
    }

    private static Gravedad ObtenerGravedad(string? valor) {
        if (!int.TryParse(valor, NumberStyles.Integer, _es, out var codigo))
            return Gravedad.SinAsistencia;

        return codigo switch {
            14 => Gravedad.SinAsistencia,
            04 => Gravedad.Fallecido,
            02 or 03 => Gravedad.Grave,
            01 or 05 or 06 or 07 => Gravedad.Leve,
            _ => Gravedad.Desconocido
        };
    }
}
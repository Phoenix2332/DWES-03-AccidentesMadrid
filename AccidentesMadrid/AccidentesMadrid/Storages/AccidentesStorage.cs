using System.Globalization;
using System.Text;
using AccidentesMadrid.Mappers;
using AccidentesMadrid.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Serilog;

namespace AccidentesMadrid.Storages;

public class AccidentesStorage : IAccidentesStorage {
    private readonly CsvConfiguration _csvConfiguration = new(CultureInfo.InvariantCulture) {
        Delimiter = ";",
        HasHeaderRecord = true,
        MissingFieldFound = null,
        HeaderValidated = null
    };

    private readonly ILogger _logger = Log.ForContext<AccidentesStorage>();

    public async Task<IEnumerable<Accidente>> Cargar() {
        _logger.Debug("[STO-POST] Obteniendo accidentes");
        _logger.Debug("[STO-POST] Buscando directorio");
        var directorioData = ObtenerDirectorioData();

        _logger.Debug("[STO-POST] Buscando archivos CSV");
        var archivos = Directory.GetFiles(directorioData, "*.csv").OrderBy(x => x).ToList();

        if (archivos.Count == 0)
            throw new FileNotFoundException($"No se encontraron archivos CSV en {directorioData}");

        _logger.Debug("[STO-POST] Leyendo CSVs");
        var tareas = archivos.Select(archivo => Task.Run(() => LeerArchivo(archivo))).ToArray();

        var resultados = await Task.WhenAll(tareas);

        return resultados.SelectMany(x => x).ToList();
    }

    private List<Accidente> LeerArchivo(string archivo) {
        using var reader = new StreamReader(archivo, Encoding.UTF8);
        using var csv = new CsvReader(reader, _csvConfiguration);

        csv.Context.RegisterClassMap<AccidenteMapper>();

        var accidentes = csv.GetRecords<Accidente>().ToList();

        _logger.Information("Leídos {Total} accidentes de {Archivo}", accidentes.Count, Path.GetFileName(archivo));

        return accidentes;
    }

    private static string ObtenerDirectorioData() {
        var directorio = AppContext.BaseDirectory;

        while (directorio != null) {
            var data = Path.Combine(directorio, "data");

            if (Directory.Exists(data))
                return data;

            directorio = Directory.GetParent(directorio)?.FullName;
        }

        throw new DirectoryNotFoundException("No se encontró el directorio data.");
    }
}
using System.Diagnostics;
using System.Text;
using AccidentesMadrid.Analyzers;
using AccidentesMadrid.Repositories;
using AccidentesMadrid.Services;
using AccidentesMadrid.Storages;
using Serilog;
using static System.Console;

var loggerConfiguration = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(
        outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
    .Enrich.WithProperty("Application", "Agenda")
    .CreateLogger();
Log.Logger = loggerConfiguration;

OutputEncoding = Encoding.UTF8;

IAccidentesStorage storage = new AccidentesStorage();
IAccidentesRepository repository = new AccidentesRepository();
var service = new AccidentesService(storage, repository);
var cronometro = Stopwatch.StartNew();
await service.CargarAccidentes();
cronometro.Stop();
var accidentes = await service.GetAll();
var linqAnalyzer = new AccidentesLinqAnalyzer();
var dataFrameAnalyzer = new AccidentesDataFrameAnalyzer();

WriteLine("========== LINQ / PLINQ ==========");
var linq = linqAnalyzer.Analizar(accidentes);
WriteLine();
WriteLine("========== DATAFRAME ==========");
var dataFrame = dataFrameAnalyzer.Analizar(accidentes);

WriteLine();
WriteLine("========== COMPARATIVA DE TIEMPOS ==========");
WriteLine($"Tiempo Total Lectura CSV (130.864 Accidentes): {cronometro.Elapsed.TotalMilliseconds:F3}ms");
WriteLine(
    $"Tiempo Aprox. Lectura CSV - 2024 (49.340 Accidentes): {cronometro.Elapsed.TotalMilliseconds / 0.3770:F3}ms");
WriteLine(
    $"Tiempo Aprox. Lectura CSV - 2025 (51.067 Accidentes): {cronometro.Elapsed.TotalMilliseconds / 0.3902:F3}ms");
WriteLine(
    $"Tiempo Aprox. Lectura CSV - 2026 (30.457 Accidentes): {cronometro.Elapsed.TotalMilliseconds / 0.2327:F3}ms");
WriteLine();
WriteLine($"Tiempo Total LINQ/PLINQ: {linq.Elapsed.TotalMilliseconds:F3}ms");
WriteLine($"Tiempo medio por consulta LINQ/PLINQ: {linq.Elapsed.TotalMilliseconds / 30:F3}ms");
WriteLine();
WriteLine($"Tiempo Total DataFrame: {dataFrame.Elapsed.TotalMilliseconds:F3}ms");
WriteLine($"Tiempo medio por consulta DataFrame: {dataFrame.Elapsed.TotalMilliseconds / 30:F3}ms");
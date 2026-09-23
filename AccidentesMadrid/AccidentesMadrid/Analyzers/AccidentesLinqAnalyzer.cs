using System.Diagnostics;
using AccidentesMadrid.Enums;
using AccidentesMadrid.Models;
using Serilog;
using static System.Console;

namespace AccidentesMadrid.Analyzers;

public class AccidentesLinqAnalyzer : IAccidentesAnalyzer {
    private readonly ILogger _logger = Log.ForContext<AccidentesDataFrameAnalyzer>();

    public Stopwatch Analizar(IEnumerable<Accidente> accidentes) {
        _logger.Debug("[P/LINQ-GET] Realizando consultas");
        var datos = accidentes.ToList();

        WriteLine("========================================");
        WriteLine("       ANÁLISIS LINQ / PLINQ");
        WriteLine("========================================");
        WriteLine();

        var cronometroGlobal = Stopwatch.StartNew();

        EjecutarConsulta(1, "Total de accidentes",
            () => Consulta01_TotalAccidentes(datos));

        EjecutarConsulta(2, "Accidentes por distrito (Top 5)",
            () => Consulta02_AccidentesPorDistrito(datos));

        EjecutarConsulta(3, "Accidentes por tipo",
            () => Consulta03_AccidentesPorTipo(datos));

        EjecutarConsulta(4, "Accidentes por estado meteorológico",
            () => Consulta04_AccidentesPorEstadoMeteorologico(datos));

        EjecutarConsulta(5, "Accidentes por sexo",
            () => Consulta05_AccidentesPorSexo(datos));

        EjecutarConsulta(6, "Accidentes por rango de edad",
            () => Consulta06_AccidentesPorRangoEdad(datos));

        EjecutarConsulta(7, "Positivos en alcohol",
            () => Consulta07_PositivosAlcohol(datos));

        EjecutarConsulta(8, "Positivos en drogas",
            () => Consulta08_PositivosDrogas(datos));

        EjecutarConsulta(9, "Accidentes por día de la semana",
            () => Consulta09_AccidentesPorDiaSemana(datos));

        EjecutarConsulta(10, "Accidentes por mes",
            () => Consulta10_AccidentesPorMes(datos));

        EjecutarConsulta(11, "Hora con más accidentes",
            () => Consulta11_HoraConMasAccidentes(datos));

        EjecutarConsulta(12, "Lesividades más frecuentes",
            () => Consulta12_LesividadesMasFrecuentes(datos));

        EjecutarConsulta(13, "Tipo de vehículo más implicado",
            () => Consulta13_TipoVehiculoMasImplicado(datos));

        EjecutarConsulta(14, "Accidentes con peatones",
            () => Consulta14_AccidentesConPeatones(datos));

        EjecutarConsulta(15, "Proporción hombre/mujer",
            () => Consulta15_ProporcionHombreMujer(datos));

        EjecutarConsulta(16, "Distritos con más peatones",
            () => Consulta16_DistritosConMasPeatones(datos));

        EjecutarConsulta(17, "Fin de semana vs entre semana",
            () => Consulta17_FinDeSemanaVsEntreSemana(datos));

        EjecutarConsulta(18, "Media de accidentes por día",
            () => Consulta18_MediaAccidentesPorDia(datos));

        EjecutarConsulta(19, "Accidentes con alcohol y droga",
            () => Consulta19_AlcoholYDroga(datos));

        EjecutarConsulta(20, "Rangos de edad más vulnerables entre peatones",
            () => Consulta20_RangosEdadPeatones(datos));

        EjecutarConsulta(21, "Distritos con más positivos en alcohol",
            () => Consulta21_DistritosMasAlcohol(datos));

        EjecutarConsulta(22, "Accidentes por código de distrito",
            () => Consulta22_AccidentesPorCodigoDistrito(datos));

        EjecutarConsulta(23, "Accidentes por año",
            () => Consulta23_AccidentesPorAno(datos));

        EjecutarConsulta(24, "Evolución mensual por año",
            () => Consulta24_EvolucionMensualPorAno(datos));

        EjecutarConsulta(25, "Distrito con más accidentes por año",
            () => Consulta25_DistritoMasAccidentesPorAno(datos));

        EjecutarConsulta(26, "Tendencia de alcohol por año",
            () => Consulta26_TendenciaAlcoholPorAno(datos));

        EjecutarConsulta(27, "Fin de semana vs entre semana por año",
            () => Consulta27_FinSemanaPorAno(datos));

        EjecutarConsulta(28, "Hora pico por año",
            () => Consulta28_HoraPicoPorAno(datos));

        EjecutarConsulta(29, "Lesividad más frecuente por año",
            () => Consulta29_LesividadPorAno(datos));

        EjecutarConsulta(30, "Evolución de peatones por año",
            () => Consulta30_PeatonesPorAno(datos));

        cronometroGlobal.Stop();

        return cronometroGlobal;
    }

    private static void EjecutarConsulta(int numero, string descripcion, Action consulta) {
        WriteLine($"Consulta {numero:00} - {descripcion}");
        var cronometro = Stopwatch.StartNew();
        consulta();
        cronometro.Stop();
        WriteLine($"Tiempo empleado: {cronometro.Elapsed.TotalMilliseconds:F3} ms");
        WriteLine();
    }


    // ============================================================
    // 01. TOTAL DE ACCIDENTES
    // ============================================================

    private static void Consulta01_TotalAccidentes(IEnumerable<Accidente> accidentes) {
        var total = accidentes.Count();
        WriteLine($"   Total: {total:N0}");
    }


    // ============================================================
    // 02. ACCIDENTES POR DISTRITO - TOP 5
    // ============================================================

    private static void Consulta02_AccidentesPorDistrito(IEnumerable<Accidente> accidentes) {
        var resultado = accidentes
            .AsParallel()
            .GroupBy(a => a.Distrito)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => new {
                Distrito = g.Key,
                Total = g.Count()
            })
            .ToList();

        foreach (var item in resultado)
            WriteLine($"   {item.Distrito}: {item.Total:N0}");
    }


    // ============================================================
    // 03. ACCIDENTES POR TIPO
    // ============================================================

    private static void Consulta03_AccidentesPorTipo(IEnumerable<Accidente> accidentes) {
        var resultado = accidentes
            .AsParallel()
            .GroupBy(a => a.TipoAccidente)
            .OrderByDescending(g => g.Count())
            .Select(g => new {
                Tipo = g.Key,
                Total = g.Count()
            })
            .ToList();

        foreach (var item in resultado)
            WriteLine($"   {item.Tipo}: {item.Total:N0}");
    }


    // ============================================================
    // 04. ACCIDENTES POR ESTADO METEOROLÓGICO
    // ============================================================

    private static void Consulta04_AccidentesPorEstadoMeteorologico(IEnumerable<Accidente> accidentes) {
        var resultado = accidentes
            .AsParallel()
            .GroupBy(a => a.EstadoMeteorologico)
            .OrderByDescending(g => g.Count())
            .Select(g => new {
                Estado = g.Key,
                Total = g.Count()
            })
            .ToList();

        foreach (var item in resultado)
            WriteLine($"   {item.Estado}: {item.Total:N0}");
    }


    // ============================================================
    // 05. ACCIDENTES POR SEXO
    // ============================================================

    private static void Consulta05_AccidentesPorSexo(IEnumerable<Accidente> accidentes) {
        var resultado = accidentes
            .AsParallel()
            .GroupBy(a => a.Sexo)
            .OrderByDescending(g => g.Count())
            .Select(g => new {
                Sexo = g.Key,
                Total = g.Count()
            })
            .ToList();

        foreach (var item in resultado)
            WriteLine($"   {item.Sexo}: {item.Total:N0}");
    }


    // ============================================================
    // 06. ACCIDENTES POR RANGO DE EDAD
    // ============================================================

    private static void Consulta06_AccidentesPorRangoEdad(IEnumerable<Accidente> accidentes) {
        var resultado = accidentes
            .AsParallel()
            .GroupBy(a => a.RangoEdad)
            .OrderByDescending(g => g.Count())
            .Select(g => new {
                RangoEdad = g.Key,
                Total = g.Count()
            })
            .ToList();

        foreach (var item in resultado)
            WriteLine($"   {item.RangoEdad}: {item.Total:N0}");
    }


    // ============================================================
    // 07. POSITIVOS EN ALCOHOL
    // ============================================================

    private static void Consulta07_PositivosAlcohol(IEnumerable<Accidente> accidentes) {
        var total = accidentes
            .AsParallel()
            .Count(a => a.PositivoAlcohol);

        WriteLine($"   Positivos: {total:N0}");
    }


    // ============================================================
    // 08. POSITIVOS EN DROGAS
    // ============================================================

    private static void Consulta08_PositivosDrogas(IEnumerable<Accidente> accidentes) {
        var total = accidentes
            .AsParallel()
            .Count(a => a.PositivoDroga);

        WriteLine($"   Positivos: {total:N0}");
    }


    // ============================================================
    // 09. ACCIDENTES POR DÍA DE LA SEMANA
    // ============================================================

    private static void Consulta09_AccidentesPorDiaSemana(IEnumerable<Accidente> accidentes) {
        var resultado = accidentes
            .AsParallel()
            .GroupBy(a => a.Fecha.DayOfWeek)
            .OrderByDescending(g => g.Count())
            .Select(g => new {
                Dia = g.Key,
                Total = g.Count()
            })
            .ToList();

        foreach (var item in resultado)
            WriteLine($"   {item.Dia}: {item.Total:N0}");
    }


    // ============================================================
    // 10. ACCIDENTES POR MES
    // ============================================================

    private static void Consulta10_AccidentesPorMes(IEnumerable<Accidente> accidentes) {
        var resultado = accidentes
            .AsParallel()
            .GroupBy(a => a.Fecha.Month)
            .OrderBy(g => g.Key)
            .Select(g => new {
                Mes = g.Key,
                Total = g.Count()
            })
            .ToList();

        foreach (var item in resultado)
            WriteLine($"   Mes {item.Mes}: {item.Total:N0}");
    }


    // ============================================================
    // 11. HORA CON MÁS ACCIDENTES
    // ============================================================

    private static void Consulta11_HoraConMasAccidentes(IEnumerable<Accidente> accidentes) {
        var resultado = accidentes
            .AsParallel()
            .GroupBy(a => a.Hora.Hour)
            .OrderByDescending(g => g.Count())
            .Select(g => new {
                Hora = g.Key,
                Total = g.Count()
            })
            .FirstOrDefault();

        if (resultado != null)
            WriteLine($"   Hora: {resultado.Hora:00}:00 - {resultado.Total:N0} accidentes");
    }


    // ============================================================
    // 12. LESIVIDADES MÁS FRECUENTES
    // ============================================================

    private static void Consulta12_LesividadesMasFrecuentes(IEnumerable<Accidente> accidentes) {
        var resultado = accidentes
            .AsParallel()
            .GroupBy(a => a.CodLesividad)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => new {
                Lesividad = g.Key,
                Total = g.Count()
            })
            .ToList();

        foreach (var item in resultado)
            WriteLine($"   {item.Lesividad}: {item.Total:N0}");
    }


    // ============================================================
    // 13. TIPO DE VEHÍCULO MÁS IMPLICADO
    // ============================================================

    private static void Consulta13_TipoVehiculoMasImplicado(IEnumerable<Accidente> accidentes) {
        var resultado = accidentes
            .AsParallel()
            .GroupBy(a => a.TipoVehiculo)
            .OrderByDescending(g => g.Count())
            .Select(g => new {
                Vehiculo = g.Key,
                Total = g.Count()
            })
            .FirstOrDefault();

        if (resultado != null)
            WriteLine($"   {resultado.Vehiculo}: {resultado.Total:N0}");
    }


    // ============================================================
    // 14. ACCIDENTES CON PEATONES
    // ============================================================

    private static void Consulta14_AccidentesConPeatones(IEnumerable<Accidente> accidentes) {
        var total = accidentes
            .AsParallel()
            .Count(a => a.TipoPersona == TipoPersona.Peatón);

        WriteLine($"   Peatones: {total:N0}");
    }


    // ============================================================
    // 15. PROPORCIÓN HOMBRE / MUJER
    // ============================================================

    private static void Consulta15_ProporcionHombreMujer(IEnumerable<Accidente> accidentes) {
        var resultado = accidentes
            .AsParallel()
            .Where(a => a.Sexo is Sexo.Hombre or Sexo.Mujer)
            .GroupBy(a => a.Sexo)
            .Select(g => new {
                Sexo = g.Key,
                Total = g.Count()
            })
            .ToList();

        var total = resultado.Sum(x => x.Total);

        foreach (var item in resultado) {
            var porcentaje = total == 0 ? 0 : item.Total * 100.0 / total;

            WriteLine($"   {item.Sexo}: {item.Total:N0} ({porcentaje:F2} %)");
        }
    }


    // ============================================================
    // 16. DISTRITOS CON MÁS PEATONES
    // ============================================================

    private static void Consulta16_DistritosConMasPeatones(IEnumerable<Accidente> accidentes) {
        var resultado = accidentes
            .AsParallel()
            .Where(a => a.TipoPersona == TipoPersona.Peatón)
            .GroupBy(a => a.Distrito)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => new {
                Distrito = g.Key,
                Total = g.Count()
            })
            .ToList();

        foreach (var item in resultado)
            WriteLine($"   {item.Distrito}: {item.Total:N0}");
    }


    // ============================================================
    // 17. FIN DE SEMANA VS ENTRE SEMANA
    // ============================================================

    private static void Consulta17_FinDeSemanaVsEntreSemana(IEnumerable<Accidente> accidentes) {
        var resultado = accidentes
            .AsParallel()
            .GroupBy(a =>
                a.Fecha.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday ? "Fin de semana" : "Entre semana")
            .Select(g => new {
                Periodo = g.Key,
                Total = g.Count()
            })
            .ToList();

        foreach (var item in resultado)
            WriteLine($"   {item.Periodo}: {item.Total:N0}");
    }


    // ============================================================
    // 18. MEDIA DE ACCIDENTES POR DÍA
    // ============================================================

    private static void Consulta18_MediaAccidentesPorDia(IEnumerable<Accidente> accidentes) {
        var grupos = accidentes
            .AsParallel()
            .GroupBy(a => a.Fecha)
            .ToList();

        var media = grupos.Count == 0 ? 0 : grupos.Average(g => g.Count());

        WriteLine($"   Media: {media} accidentes/día");
    }


    // ============================================================
    // 19. ACCIDENTES CON ALCOHOL + DROGA
    // ============================================================

    private static void Consulta19_AlcoholYDroga(IEnumerable<Accidente> accidentes) {
        var total = accidentes
            .AsParallel()
            .Count(a => a.PositivoAlcohol && a.PositivoDroga);

        WriteLine($"   Alcohol + droga: {total:N0}");
    }


    // ============================================================
    // 20. RANGOS DE EDAD MÁS VULNERABLES - PEATONES
    // ============================================================

    private static void Consulta20_RangosEdadPeatones(IEnumerable<Accidente> accidentes) {
        var resultado = accidentes
            .AsParallel()
            .Where(a => a.TipoPersona == TipoPersona.Peatón)
            .GroupBy(a => a.RangoEdad)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => new {
                RangoEdad = g.Key,
                Total = g.Count()
            })
            .ToList();

        foreach (var item in resultado)
            WriteLine($"   {item.RangoEdad}: {item.Total:N0}");
    }


    // ============================================================
    // 21. DISTRITOS CON MÁS POSITIVOS EN ALCOHOL
    // ============================================================

    private static void Consulta21_DistritosMasAlcohol(IEnumerable<Accidente> accidentes) {
        var resultado = accidentes
            .AsParallel()
            .Where(a => a.PositivoAlcohol)
            .GroupBy(a => a.Distrito)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => new {
                Distrito = g.Key,
                Total = g.Count()
            })
            .ToList();

        foreach (var item in resultado)
            WriteLine($"   {item.Distrito}: {item.Total:N0}");
    }


    // ============================================================
    // 22. ACCIDENTES POR CÓDIGO DE DISTRITO
    // ============================================================

    private static void Consulta22_AccidentesPorCodigoDistrito(IEnumerable<Accidente> accidentes) {
        var resultado = accidentes
            .AsParallel()
            .GroupBy(a => a.CodDistrito)
            .OrderByDescending(g => g.Count())
            .Select(g => new {
                Codigo = g.Key,
                Total = g.Count()
            })
            .ToList();

        foreach (var item in resultado)
            WriteLine($"   Distrito {item.Codigo}: {item.Total:N0}");
    }


    // ============================================================
    // 23. ACCIDENTES POR AÑO
    // ============================================================

    private static void Consulta23_AccidentesPorAno(IEnumerable<Accidente> accidentes) {
        var resultado = accidentes
            .AsParallel()
            .GroupBy(a => a.Fecha.Year)
            .OrderBy(g => g.Key)
            .Select(g => new {
                Ano = g.Key,
                Total = g.Count()
            })
            .ToList();

        foreach (var item in resultado)
            WriteLine($"   {item.Ano}: {item.Total:N0}");
    }


    // ============================================================
    // 24. EVOLUCIÓN MENSUAL POR AÑO
    // ============================================================

    private static void Consulta24_EvolucionMensualPorAno(IEnumerable<Accidente> accidentes) {
        var resultado = accidentes
            .AsParallel()
            .GroupBy(a => new {
                Ano = a.Fecha.Year,
                Mes = a.Fecha.Month
            })
            .OrderBy(g => g.Key.Ano)
            .ThenBy(g => g.Key.Mes)
            .Select(g => new {
                g.Key.Ano,
                g.Key.Mes,
                Total = g.Count()
            })
            .ToList();

        foreach (var item in resultado) {
            WriteLine($"   {item.Ano} - Mes {item.Mes}: {item.Total:N0}");
            if (item.Mes == 12)
                WriteLine();
        }
    }


    // ============================================================
    // 25. DISTRITO CON MÁS ACCIDENTES POR AÑO
    // ============================================================

    private static void Consulta25_DistritoMasAccidentesPorAno(IEnumerable<Accidente> accidentes) {
        var resultado = accidentes
            .AsParallel()
            .GroupBy(a => a.Fecha.Year)
            .Select(g => new {
                Ano = g.Key,
                Distrito = g
                    .GroupBy(a => a.Distrito)
                    .OrderByDescending(d => d.Count())
                    .Select(d => d.Key)
                    .FirstOrDefault(),
                Total = g
                    .GroupBy(a => a.Distrito)
                    .Max(d => d.Count())
            })
            .OrderBy(x => x.Ano)
            .ToList();

        foreach (var item in resultado)
            WriteLine($"   {item.Ano}: {item.Distrito} ({item.Total:N0})");
    }


    // ============================================================
    // 26. TENDENCIA DE ALCOHOL POR AÑO
    // ============================================================

    private static void Consulta26_TendenciaAlcoholPorAno(IEnumerable<Accidente> accidentes) {
        var resultado = accidentes
            .AsParallel()
            .GroupBy(a => a.Fecha.Year)
            .OrderBy(g => g.Key)
            .Select(g => new {
                Ano = g.Key,
                Total = g.Count(),
                Alcohol = g.Count(a => a.PositivoAlcohol),
                Porcentaje = !g.Any() ? 0 : g.Count(a => a.PositivoAlcohol) * 100.0 / g.Count()
            })
            .ToList();

        foreach (var item in resultado)
            WriteLine($"   {item.Ano}: {item.Alcohol:N0} de {item.Total:N0} ({item.Porcentaje:F2} %)");
    }


    // ============================================================
    // 27. FIN DE SEMANA VS ENTRE SEMANA POR AÑO
    // ============================================================

    private static void Consulta27_FinSemanaPorAno(IEnumerable<Accidente> accidentes) {
        var resultado = accidentes
            .AsParallel()
            .GroupBy(a => new {
                Ano = a.Fecha.Year,
                FinDeSemana = a.Fecha.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday
            })
            .OrderBy(g => g.Key.Ano)
            .ThenBy(g => g.Key.FinDeSemana)
            .Select(g => new {
                g.Key.Ano,
                Periodo = g.Key.FinDeSemana ? "Fin de semana" : "Entre semana",
                Total = g.Count()
            })
            .ToList();

        foreach (var item in resultado)
            WriteLine($"   {item.Ano} - {item.Periodo}: {item.Total:N0}");
    }


    // ============================================================
    // 28. HORA PICO POR AÑO
    // ============================================================

    private static void Consulta28_HoraPicoPorAno(IEnumerable<Accidente> accidentes) {
        var resultado = accidentes
            .AsParallel()
            .GroupBy(a => a.Fecha.Year)
            .Select(g => new {
                Ano = g.Key,
                Hora = g
                    .GroupBy(a => a.Hora.Hour)
                    .OrderByDescending(h => h.Count())
                    .Select(h => h.Key)
                    .FirstOrDefault(),
                Total = g
                    .GroupBy(a => a.Hora.Hour)
                    .Max(h => h.Count())
            })
            .OrderBy(x => x.Ano)
            .ToList();

        foreach (var item in resultado)
            WriteLine($"   {item.Ano}: {item.Hora:00}:00 ({item.Total:N0})");
    }


    // ============================================================
    // 29. LESIVIDAD MÁS FRECUENTE POR AÑO
    // ============================================================

    private static void Consulta29_LesividadPorAno(IEnumerable<Accidente> accidentes) {
        var resultado = accidentes
            .AsParallel()
            .GroupBy(a => a.Fecha.Year)
            .Select(g => new {
                Ano = g.Key,
                Lesividad = g
                    .GroupBy(a => a.CodLesividad)
                    .OrderByDescending(l => l.Count())
                    .Select(l => l.Key)
                    .FirstOrDefault(),
                Total = g
                    .GroupBy(a => a.CodLesividad)
                    .Max(l => l.Count())
            })
            .OrderBy(x => x.Ano)
            .ToList();

        foreach (var item in resultado)
            WriteLine($"   {item.Ano}: {item.Lesividad} ({item.Total:N0})");
    }


    // ============================================================
    // 30. EVOLUCIÓN DE PEATONES POR AÑO
    // ============================================================

    private static void Consulta30_PeatonesPorAno(IEnumerable<Accidente> accidentes) {
        var resultado = accidentes
            .AsParallel()
            .GroupBy(a => a.Fecha.Year)
            .OrderBy(g => g.Key)
            .Select(g => new {
                Ano = g.Key,
                Peatones = g.Count(a => a.TipoPersona == TipoPersona.Peatón)
            })
            .ToList();

        foreach (var item in resultado)
            WriteLine($"   {item.Ano}: {item.Peatones:N0}");
    }
}
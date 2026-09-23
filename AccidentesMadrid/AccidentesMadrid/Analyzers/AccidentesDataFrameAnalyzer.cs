using System.Diagnostics;
using AccidentesMadrid.Enums;
using AccidentesMadrid.Models;
using Microsoft.Data.Analysis;
using Serilog;
using static System.Console;

namespace AccidentesMadrid.Analyzers;

public class AccidentesDataFrameAnalyzer : IAccidentesAnalyzer {
    private readonly ILogger _logger = Log.ForContext<AccidentesDataFrameAnalyzer>();

    public Stopwatch Analizar(IEnumerable<Accidente> accidentes) {
        _logger.Debug("[DTF-GET] Realizando consultas");
        var datos = accidentes.ToList();

        var dataframe = CrearDataFrame(datos);

        WriteLine("========================================");
        WriteLine("          ANÁLISIS DATAFRAME");
        WriteLine("========================================");
        WriteLine();

        var cronometroGlobal = Stopwatch.StartNew();

        EjecutarConsulta(1, "Total de accidentes",
            () => Consulta01_TotalAccidentes(dataframe));

        EjecutarConsulta(2, "Accidentes por distrito (Top 5)",
            () => Consulta02_AccidentesPorDistrito(dataframe));

        EjecutarConsulta(3, "Accidentes por tipo",
            () => Consulta03_AccidentesPorTipo(dataframe));

        EjecutarConsulta(4, "Accidentes por estado meteorológico",
            () => Consulta04_AccidentesPorEstadoMeteorologico(dataframe));

        EjecutarConsulta(5, "Accidentes por sexo",
            () => Consulta05_AccidentesPorSexo(dataframe));

        EjecutarConsulta(6, "Accidentes por rango de edad",
            () => Consulta06_AccidentesPorRangoEdad(dataframe));

        EjecutarConsulta(7, "Positivos en alcohol",
            () => Consulta07_PositivosAlcohol(dataframe));

        EjecutarConsulta(8, "Positivos en drogas",
            () => Consulta08_PositivosDrogas(dataframe));

        EjecutarConsulta(9, "Accidentes por día de la semana",
            () => Consulta09_AccidentesPorDiaSemana(dataframe));

        EjecutarConsulta(10, "Accidentes por mes",
            () => Consulta10_AccidentesPorMes(dataframe));

        EjecutarConsulta(11, "Hora con más accidentes",
            () => Consulta11_HoraConMasAccidentes(dataframe));

        EjecutarConsulta(12, "Lesividades más frecuentes",
            () => Consulta12_LesividadesMasFrecuentes(dataframe));

        EjecutarConsulta(13, "Tipo de vehículo más implicado",
            () => Consulta13_TipoVehiculoMasImplicado(dataframe));

        EjecutarConsulta(14, "Accidentes con peatones",
            () => Consulta14_AccidentesConPeatones(dataframe));

        EjecutarConsulta(15, "Proporción hombre/mujer",
            () => Consulta15_ProporcionHombreMujer(dataframe));

        EjecutarConsulta(16, "Distritos con más peatones",
            () => Consulta16_DistritosConMasPeatones(dataframe));

        EjecutarConsulta(17, "Fin de semana vs entre semana",
            () => Consulta17_FinDeSemanaVsEntreSemana(dataframe));

        EjecutarConsulta(18, "Media de accidentes por día",
            () => Consulta18_MediaAccidentesPorDia(dataframe));

        EjecutarConsulta(19, "Accidentes con alcohol y droga",
            () => Consulta19_AlcoholYDroga(dataframe));

        EjecutarConsulta(20, "Rangos de edad más vulnerables entre peatones",
            () => Consulta20_RangosEdadPeatones(dataframe));

        EjecutarConsulta(21, "Distritos con más positivos en alcohol",
            () => Consulta21_DistritosMasAlcohol(dataframe));

        EjecutarConsulta(22, "Accidentes por código de distrito",
            () => Consulta22_AccidentesPorCodigoDistrito(dataframe));

        EjecutarConsulta(23, "Accidentes por año",
            () => Consulta23_AccidentesPorAno(dataframe));

        EjecutarConsulta(24, "Evolución mensual por año",
            () => Consulta24_EvolucionMensualPorAno(dataframe));

        EjecutarConsulta(25, "Distrito con más accidentes por año",
            () => Consulta25_DistritoMasAccidentesPorAno(dataframe));

        EjecutarConsulta(26, "Tendencia de alcohol por año",
            () => Consulta26_TendenciaAlcoholPorAno(dataframe));

        EjecutarConsulta(27, "Fin de semana vs entre semana por año",
            () => Consulta27_FinSemanaPorAno(dataframe));

        EjecutarConsulta(28, "Hora pico por año",
            () => Consulta28_HoraPicoPorAno(dataframe));

        EjecutarConsulta(29, "Lesividad más frecuente por año",
            () => Consulta29_LesividadPorAno(dataframe));

        EjecutarConsulta(30, "Evolución de peatones por año",
            () => Consulta30_PeatonesPorAno(dataframe));

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
    // CREACIÓN DEL DATAFRAME
    // ============================================================

    private static DataFrame CrearDataFrame(IEnumerable<Accidente> accidentes) {
        var datos = accidentes.ToList();

        return new DataFrame(
            new StringDataFrameColumn("NumExpediente", datos.Select(a => a.NumExpediente)),
            new PrimitiveDataFrameColumn<DateTime>("Fecha", datos.Select(a => a.Fecha.ToDateTime(TimeOnly.MinValue))),
            new PrimitiveDataFrameColumn<TimeSpan>("Hora", datos.Select(a => a.Hora.ToTimeSpan())),
            new StringDataFrameColumn("Localizacion", datos.Select(a => a.Localizacion)),
            new PrimitiveDataFrameColumn<int>("Numero", datos.Select(a => a.Numero)),
            new PrimitiveDataFrameColumn<int>("CodDistrito", datos.Select(a => a.CodDistrito)),
            new StringDataFrameColumn("Distrito", datos.Select(a => a.Distrito)),
            new StringDataFrameColumn("TipoAccidente", datos.Select(a => a.TipoAccidente.ToString())),
            new StringDataFrameColumn("EstadoMeteorologico", datos.Select(a => a.EstadoMeteorologico)),
            new StringDataFrameColumn("TipoVehiculo", datos.Select(a => a.TipoVehiculo)),
            new StringDataFrameColumn("TipoPersona", datos.Select(a => a.TipoPersona.ToString())),
            new StringDataFrameColumn("RangoEdad", datos.Select(a => a.RangoEdad)),
            new StringDataFrameColumn("Sexo", datos.Select(a => a.Sexo.ToString())),
            new PrimitiveDataFrameColumn<int>("CodLesividad", datos.Select(a => (int)a.CodLesividad)),
            new StringDataFrameColumn("Gravedad", datos.Select(a => a.Gravedad.ToString())),
            new StringDataFrameColumn("CoordenadaXUtm", datos.Select(a => a.CoordenadaXUtm)),
            new StringDataFrameColumn("CoordenadaYUtm", datos.Select(a => a.CoordenadaYUtm)),
            new BooleanDataFrameColumn("PositivoAlcohol", datos.Select(a => a.PositivoAlcohol)),
            new BooleanDataFrameColumn("PositivoDroga", datos.Select(a => a.PositivoDroga))
        );
    }


    // ============================================================
    // 01. TOTAL DE ACCIDENTES
    // ============================================================

    private static void Consulta01_TotalAccidentes(DataFrame df) {
        var total = df.Rows.Count;
        WriteLine($"   Total: {total:N0}");
    }


    // ============================================================
    // 02. ACCIDENTES POR DISTRITO - TOP 5
    // ============================================================

    private static void Consulta02_AccidentesPorDistrito(DataFrame df) {
        foreach (var item in AgruparPorColumna(df, "Distrito")
                     .OrderByDescending(x => x.Value).Take(5))
            WriteLine($"   {item.Key}: {item.Value:N0}");
    }


    // ============================================================
    // 03. ACCIDENTES POR TIPO
    // ============================================================

    private static void Consulta03_AccidentesPorTipo(DataFrame df) {
        foreach (var item in AgruparPorColumna(df, "TipoAccidente")
                     .OrderByDescending(x => x.Value).Take(5))
            WriteLine($"   {item.Key}: {item.Value:N0}");
    }


    // ============================================================
    // 04. ACCIDENTES POR ESTADO METEOROLÓGICO
    // ============================================================

    private static void Consulta04_AccidentesPorEstadoMeteorologico(DataFrame df) {
        foreach (var item in AgruparPorColumna(df, "EstadoMeteorologico")
                     .OrderByDescending(x => x.Value).Take(5))
            WriteLine($"   {item.Key}: {item.Value:N0}");
    }


    // ============================================================
    // 05. ACCIDENTES POR SEXO
    // ============================================================

    private static void Consulta05_AccidentesPorSexo(DataFrame df) {
        foreach (var item in AgruparPorColumna(df, "Sexo")
                     .OrderByDescending(x => x.Value).Take(5))
            WriteLine($"   {item.Key}: {item.Value:N0}");
    }


    // ============================================================
    // 06. ACCIDENTES POR RANGO DE EDAD
    // ============================================================

    private static void Consulta06_AccidentesPorRangoEdad(DataFrame df) {
        foreach (var item in AgruparPorColumna(df, "RangoEdad")
                     .OrderByDescending(x => x.Value).Take(5))
            WriteLine($"   {item.Key}: {item.Value:N0}");
    }


    // ============================================================
    // 07. POSITIVOS EN ALCOHOL
    // ============================================================

    private static void Consulta07_PositivosAlcohol(DataFrame df) {
        var alcohol = (BooleanDataFrameColumn)df["PositivoAlcohol"];

        var total = alcohol.Count(t => t == true);

        WriteLine($"   Positivos: {total:N0}");
    }


    // ============================================================
    // 08. POSITIVOS EN DROGAS
    // ============================================================

    private static void Consulta08_PositivosDrogas(DataFrame df) {
        var drogas = (BooleanDataFrameColumn)df["PositivoDroga"];

        var total = drogas.Count(t => t == true);

        WriteLine($"   Positivos: {total:N0}");
    }


    // ============================================================
    // 09. ACCIDENTES POR DÍA DE LA SEMANA
    // ============================================================

    private static void Consulta09_AccidentesPorDiaSemana(DataFrame df) {
        var fechas = (PrimitiveDataFrameColumn<DateTime>)df["Fecha"];
        var contador = new int[7];

        foreach (var t in fechas)
            contador[(int)t!.Value.DayOfWeek]++;

        foreach (var item in Enumerable.Range(0, 7).Select(i => new { Dia = (DayOfWeek)i, Total = contador[i] })
                     .OrderByDescending(x => x.Total))
            WriteLine($"   {item.Dia}: {item.Total:N0}");
    }


    // ============================================================
    // 10. ACCIDENTES POR MES
    // ============================================================

    private static void Consulta10_AccidentesPorMes(DataFrame df) {
        var fechas = (PrimitiveDataFrameColumn<DateTime>)df["Fecha"];
        var meses = new int[13];

        foreach (var t in fechas)
            meses[t!.Value.Month]++;

        for (var mes = 1; mes <= 12; mes++)
            WriteLine($"   Mes {mes}: {meses[mes]:N0}");
    }


    // ============================================================
    // 11. HORA CON MÁS ACCIDENTES
    // ============================================================

    private static void Consulta11_HoraConMasAccidentes(DataFrame df) {
        var horas = (PrimitiveDataFrameColumn<TimeSpan>)df["Hora"];
        var contador = new int[24];

        foreach (var t in horas)
            contador[t!.Value.Hours]++;

        var mejorHora = 0;

        for (var h = 1; h < 24; h++)
            if (contador[h] > contador[mejorHora])
                mejorHora = h;

        WriteLine($"   Hora: {mejorHora:00}:00 - {contador[mejorHora]:N0} accidentes");
    }


    // ============================================================
    // 12. LESIVIDADES MÁS FRECUENTES
    // ============================================================

    private static void Consulta12_LesividadesMasFrecuentes(DataFrame df) {
        foreach (var item in AgruparPorColumna(df, "CodLesividad")
                     .OrderByDescending(x => x.Value).Take(5))
            WriteLine($"   {item.Key}: {item.Value:N0}");
    }


    // ============================================================
    // 13. TIPO DE VEHÍCULO MÁS IMPLICADO
    // ============================================================

    private static void Consulta13_TipoVehiculoMasImplicado(DataFrame df) {
        var vehiculos = (StringDataFrameColumn)df["TipoVehiculo"];
        var contador = new Dictionary<string, int>();

        foreach (var t in vehiculos) {
            var vehiculo = t ?? "";

            if (!contador.TryAdd(vehiculo, 1))
                contador[vehiculo]++;
        }

        var mejor = contador.MaxBy(x => x.Value);

        WriteLine($"   {mejor.Key}: {mejor.Value:N0}");
    }


    // ============================================================
    // 14. ACCIDENTES CON PEATONES
    // ============================================================

    private static void Consulta14_AccidentesConPeatones(DataFrame df) {
        var columna = (StringDataFrameColumn)df["TipoPersona"];
        var total = columna.Count(x => x == nameof(TipoPersona.Peatón));
        WriteLine($"   Peatones: {total:N0}");
    }


    // ============================================================
    // 15. PROPORCIÓN HOMBRE / MUJER
    // ============================================================

    private static void Consulta15_ProporcionHombreMujer(DataFrame df) {
        var sexos = (StringDataFrameColumn)df["Sexo"];

        var hombres = 0;
        var mujeres = 0;

        foreach (var t in sexos)
            switch (t) {
                case nameof(Sexo.Hombre):
                    hombres++;
                    break;

                case nameof(Sexo.Mujer):
                    mujeres++;
                    break;
            }

        var total = hombres + mujeres;

        WriteLine($"   Hombre: {hombres:N0} ({hombres * 100.0 / total:F2} %)");

        WriteLine($"   Mujer: {mujeres:N0} ({mujeres * 100.0 / total:F2} %)");
    }


    // ============================================================
    // 16. DISTRITOS CON MÁS PEATONES
    // ============================================================

    private static void Consulta16_DistritosConMasPeatones(DataFrame df) {
        var personas = (StringDataFrameColumn)df["TipoPersona"];
        var distritos = (StringDataFrameColumn)df["Distrito"];

        var contador = new Dictionary<string, int>();

        for (var i = 0; i < df.Rows.Count; i++) {
            if (personas[i] != nameof(TipoPersona.Peatón))
                continue;

            var distrito = distritos[i]!;

            if (!contador.TryAdd(distrito, 1))
                contador[distrito]++;
        }

        foreach (var item in contador.OrderByDescending(x => x.Value).Take(5))
            WriteLine($"   {item.Key}: {item.Value:N0}");
    }


    // ============================================================
    // 17. FIN DE SEMANA VS ENTRE SEMANA
    // ============================================================

    private static void Consulta17_FinDeSemanaVsEntreSemana(DataFrame df) {
        var fechas = (PrimitiveDataFrameColumn<DateTime>)df["Fecha"];

        var fin = 0;
        var entre = 0;

        foreach (var t in fechas) {
            var dia = t!.Value.DayOfWeek;

            if (dia is DayOfWeek.Saturday or DayOfWeek.Sunday)
                fin++;
            else
                entre++;
        }

        WriteLine($"   Fin de semana: {fin:N0}");
        WriteLine($"   Entre semana: {entre:N0}");
    }


    // ============================================================
    // 18. MEDIA DE ACCIDENTES POR DÍA
    // ============================================================

    private static void Consulta18_MediaAccidentesPorDia(DataFrame df) {
        var fechas = (PrimitiveDataFrameColumn<DateTime>)df["Fecha"];
        var dias = new Dictionary<DateTime, int>();

        foreach (var t in fechas) {
            var fecha = t!.Value.Date;

            if (!dias.TryAdd(fecha, 1))
                dias[fecha]++;
        }

        var media = dias.Values.Average();

        WriteLine($"   Media: {media:F2} accidentes/día");
    }


    // ============================================================
    // 19. ACCIDENTES CON ALCOHOL + DROGA
    // ============================================================

    private static void Consulta19_AlcoholYDroga(DataFrame df) {
        var alcohol = (BooleanDataFrameColumn)df["PositivoAlcohol"];
        var droga = (BooleanDataFrameColumn)df["PositivoDroga"];

        var total = alcohol.Where((t, i) => t == true && droga[i] == true).Count();

        WriteLine($"   Alcohol + droga: {total:N0}");
    }


    // ============================================================
    // 20. RANGOS DE EDAD MÁS VULNERABLES - PEATONES
    // ============================================================

    private static void Consulta20_RangosEdadPeatones(DataFrame df) {
        var personas = (StringDataFrameColumn)df["TipoPersona"];
        var distritos = (StringDataFrameColumn)df["RangoEdad"];

        var contador = new Dictionary<string, int>();

        for (var i = 0; i < df.Rows.Count; i++) {
            if (personas[i] != nameof(TipoPersona.Peatón))
                continue;

            var distrito = distritos[i]!;

            if (!contador.TryAdd(distrito, 1))
                contador[distrito]++;
        }

        foreach (var item in contador.OrderByDescending(x => x.Value).Take(5))
            WriteLine($"   {item.Key}: {item.Value:N0}");
    }


    // ============================================================
    // 21. DISTRITOS CON MÁS POSITIVOS EN ALCOHOL
    // ============================================================

    private static void Consulta21_DistritosMasAlcohol(DataFrame df) {
        var alcohol = (BooleanDataFrameColumn)df["PositivoAlcohol"];
        var distritos = (StringDataFrameColumn)df["Distrito"];

        var contador = new Dictionary<string, int>();

        for (var i = 0; i < alcohol.Length; i++) {
            if (alcohol[i] != true)
                continue;

            var distrito = distritos[i] ?? "";

            if (!contador.TryAdd(distrito, 1))
                contador[distrito]++;
        }

        foreach (var item in contador.OrderByDescending(x => x.Value).Take(5))
            WriteLine($"   {item.Key}: {item.Value:N0}");
    }


    // ============================================================
    // 22. ACCIDENTES POR CÓDIGO DE DISTRITO
    // ============================================================

    private static void Consulta22_AccidentesPorCodigoDistrito(DataFrame df) {
        foreach (var item in AgruparPorColumna(df, "CodDistrito")
                     .OrderByDescending(x => x.Value).Take(5))
            WriteLine($"   {item.Key}: {item.Value:N0}");
    }


    // ============================================================
    // 23. ACCIDENTES POR AÑO
    // ============================================================

    private static void Consulta23_AccidentesPorAno(DataFrame df) {
        var fechas = (PrimitiveDataFrameColumn<DateTime>)df["Fecha"];
        var contador = new Dictionary<int, int>();

        foreach (var t in fechas) {
            var ano = t!.Value.Year;

            if (!contador.TryAdd(ano, 1))
                contador[ano]++;
        }

        foreach (var item in contador.OrderBy(x => x.Key))
            WriteLine($"   {item.Key}: {item.Value:N0}");
    }


    // ============================================================
    // 24. EVOLUCIÓN MENSUAL POR AÑO
    // ============================================================

    private static void Consulta24_EvolucionMensualPorAno(DataFrame df) {
        var fechas = (PrimitiveDataFrameColumn<DateTime>)df["Fecha"];

        var contador = new Dictionary<(int Año, int Mes), int>();

        foreach (var t in fechas) {
            var fecha = t!.Value;
            var clave = (fecha.Year, fecha.Month);

            if (!contador.TryAdd(clave, 1))
                contador[clave]++;
        }

        foreach (var item in contador.OrderBy(x => x.Key.Año).ThenBy(x => x.Key.Mes)) {
            WriteLine($"   {item.Key.Año} - Mes {item.Key.Mes}: {item.Value:N0}");

            if (item.Key.Mes == 12)
                WriteLine();
        }
    }


    // ============================================================
    // 25. DISTRITO CON MÁS ACCIDENTES POR AÑO
    // ============================================================

    private static void Consulta25_DistritoMasAccidentesPorAno(DataFrame df) {
        var fechas = (PrimitiveDataFrameColumn<DateTime>)df["Fecha"];
        var distritos = (StringDataFrameColumn)df["Distrito"];

        var datos = new Dictionary<int, Dictionary<string, int>>();

        for (var i = 0; i < fechas.Length; i++) {
            var ano = fechas[i]!.Value.Year;
            var distrito = distritos[i] ?? "";

            if (!datos.TryGetValue(ano, out var mapa)) {
                mapa = [];
                datos[ano] = mapa;
            }

            if (!mapa.TryAdd(distrito, 1))
                mapa[distrito]++;
        }

        foreach (var año in datos.OrderBy(x => x.Key)) {
            var mejor = año.Value.MaxBy(x => x.Value);

            WriteLine($"   {año.Key}: {mejor.Key} ({mejor.Value:N0})");
        }
    }


    // ============================================================
    // 26. TENDENCIA DE ALCOHOL POR AÑO
    // ============================================================

    private static void Consulta26_TendenciaAlcoholPorAno(DataFrame df) {
        var fechas = (PrimitiveDataFrameColumn<DateTime>)df["Fecha"];
        var alcohol = (BooleanDataFrameColumn)df["PositivoAlcohol"];

        var datos = new Dictionary<int, (int Total, int Positivos)>();

        for (var i = 0; i < fechas.Length; i++) {
            var ano = fechas[i]!.Value.Year;

            datos.TryGetValue(ano, out var valor);

            valor.Total++;

            if (alcohol[i] == true)
                valor.Positivos++;

            datos[ano] = valor;
        }

        foreach (var item in datos.OrderBy(x => x.Key)) {
            var porcentaje = item.Value.Positivos * 100.0 / item.Value.Total;

            WriteLine($"   {item.Key}: {item.Value.Positivos:N0} de {item.Value.Total:N0} ({porcentaje:F2} %)");
        }
    }


    // ============================================================
    // 27. FIN DE SEMANA VS ENTRE SEMANA POR AÑO
    // ============================================================

    private static void Consulta27_FinSemanaPorAno(DataFrame df) {
        var fechas = (PrimitiveDataFrameColumn<DateTime>)df["Fecha"];

        var datos = new Dictionary<int, (int Semana, int FinSemana)>();

        foreach (var t in fechas) {
            var fecha = t!.Value;
            var ano = fecha.Year;

            datos.TryGetValue(ano, out var valor);

            if (fecha.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                valor.FinSemana++;
            else
                valor.Semana++;

            datos[ano] = valor;
        }

        foreach (var item in datos.OrderBy(x => x.Key)) {
            WriteLine($"   {item.Key} - Entre semana: {item.Value.Semana:N0}");

            WriteLine($"   {item.Key} - Fin de semana: {item.Value.FinSemana:N0}");
        }
    }


    // ============================================================
    // 28. HORA PICO POR AÑO
    // ============================================================

    private static void Consulta28_HoraPicoPorAno(DataFrame df) {
        var fechas = (PrimitiveDataFrameColumn<DateTime>)df["Fecha"];
        var horas = (PrimitiveDataFrameColumn<TimeSpan>)df["Hora"];

        var datos = new Dictionary<int, int[]>();

        for (var i = 0; i < fechas.Length; i++) {
            var ano = fechas[i]!.Value.Year;
            var hora = horas[i]!.Value.Hours;

            if (!datos.TryGetValue(ano, out var contador)) {
                contador = new int[24];
                datos[ano] = contador;
            }

            contador[hora]++;
        }

        foreach (var item in datos.OrderBy(x => x.Key)) {
            var mejorHora = 0;

            for (var h = 1; h < 24; h++)
                if (item.Value[h] > item.Value[mejorHora])
                    mejorHora = h;

            WriteLine($"   {item.Key}: {mejorHora:00}:00 ({item.Value[mejorHora]:N0})");
        }
    }


    // ============================================================
    // 29. LESIVIDAD MÁS FRECUENTE POR AÑO
    // ============================================================

    private static void Consulta29_LesividadPorAno(DataFrame df) {
        var fechas = (PrimitiveDataFrameColumn<DateTime>)df["Fecha"];
        var lesividades = (PrimitiveDataFrameColumn<int>)df["CodLesividad"];

        var datos = new Dictionary<int, Dictionary<int, int>>();

        for (var i = 0; i < fechas.Length; i++) {
            var ano = fechas[i]!.Value.Year;
            var codigo = lesividades[i]!.Value;

            if (!datos.TryGetValue(ano, out var mapa)) {
                mapa = [];
                datos[ano] = mapa;
            }

            if (!mapa.TryAdd(codigo, 1))
                mapa[codigo]++;
        }

        foreach (var item in datos.OrderBy(x => x.Key)) {
            var mejor = item.Value.MaxBy(x => x.Value);

            WriteLine($"   {item.Key}: {(CodigoAccidente)mejor.Key} ({mejor.Value:N0})");
        }
    }


    // ============================================================
    // 30. EVOLUCIÓN DE PEATONES POR AÑO
    // ============================================================

    private static void Consulta30_PeatonesPorAno(DataFrame df) {
        var fechas = (PrimitiveDataFrameColumn<DateTime>)df["Fecha"];
        var personas = (StringDataFrameColumn)df["TipoPersona"];

        var contador = new Dictionary<int, int>();

        for (var i = 0; i < fechas.Length; i++) {
            if (personas[i] != nameof(TipoPersona.Peatón))
                continue;

            var ano = fechas[i]!.Value.Year;

            if (!contador.TryAdd(ano, 1))
                contador[ano]++;
        }

        foreach (var item in contador.OrderBy(x => x.Key))
            WriteLine($"   {item.Key}: {item.Value:N0}");
    }


    // ============================================================
    // MÉTODOS AUXILIARES
    // ============================================================

    private static Dictionary<string, int> AgruparPorColumna(DataFrame df, string nombreColumna) {
        var columna = df[nombreColumna];
        var contador = new Dictionary<string, int>();

        foreach (var t in columna) {
            var clave = t?.ToString() ?? "";

            if (!contador.TryAdd(clave, 1))
                contador[clave]++;
        }

        return contador;
    }
}
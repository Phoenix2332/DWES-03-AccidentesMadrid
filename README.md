# DWES-03-AccidentesMadrid

Procesamiento de los ficheros de accidentalidad del Ayuntamiento de Madrid (2024, 2025 y 2026) utilizando **LINQ/PLINQ** y **Microsoft.Data.Analysis (DataFrame)** para comparar rendimiento y justificar las decisiones de diseño.

---

# Tecnologías

| Tecnología | Uso |
|------------|-----|
| C# 14 | Lenguaje principal |
| CsvHelper | Lectura de ficheros CSV |
| LINQ | Consultas sobre colecciones |
| PLINQ | Paralelización de consultas |
| Microsoft.Data.Analysis | DataFrames |
| Serilog | Registro de eventos |

---

# Estructura del proyecto

```text
AccidentesMadrid/
├── Program.cs
├── data/
│   ├── 2024-Accidentalidad.csv
│   ├── 2025-Accidentalidad.csv
│   └── 2026-Accidentalidad.csv
├── Models/
│   └── Accidente.cs
├── Enums/
│   ├── CodigoAccidente.cs
│   ├── Gravedad.cs
│   ├── Sexo.cs
│   ├── TipoAccidente.cs
│   └── TipoPersona.cs
├── Mappers/
│   └── AccidenteMapper.cs
├── Storages/
│   ├── IStorage.cs
│   ├── IAccidentesStorage.cs
│   └── AccidentesStorage.cs
├── Repositories/
│   ├── IRepository.cs
│   ├── IAccidentesRepository.cs
│   └── AccidentesRepository.cs
├── Services/
│   ├── IAccidentesService.cs
│   └── AccidentesService.cs
└── Analyzers/
    ├── IAccidentesAnalyzer.cs
    ├── AccidentesLinqAnalyzer.cs
    └── AccidentesDataFrameAnalyzer.cs
```

---

## Ejecución

El programa realiza automáticamente:

- Lectura concurrente de los tres CSV.
- Combinación de todos los registros.
- Ejecución de 30 consultas LINQ / PLINQ.
- Ejecución de las mismas 30 consultas mediante DataFrame.
- Medición del tiempo individual y global de cada técnica.

---

## Justificación del diseño
### Arquitectura por capas
Se ha utilizado una arquitectura separando cada responsabilidad en una capa distinta.

| Capa | Responsabilidad |
|------|-----------------|
| **Storage** | Leer los ficheros CSV |
| **Repository** | Almacenar los accidentes en memoria |
| **Service** | Orquestar la carga de datos |
| **Analyzer** | Ejecutar las consultas LINQ / PLINQ y DataFrame |

### Justificación

Esta separación evita acoplamiento entre la lectura de datos y la lógica de análisis. Si el origen de datos cambiase (base de datos, API, etc.), únicamente sería necesario modificar la capa **Storage**.

---

## Modelo de datos

Se ha utilizado una clase `Accidente` con propiedades `init`.

### Justificación

Inicialmente se utilizó un `record`, pero **CsvHelper** requiere crear las instancias mediante un constructor vacío durante la deserialización. La clase con propiedades `init` mantiene la inmutabilidad práctica una vez construido el objeto y permite la integración con CsvHelper.

Además, los campos categóricos se representan mediante **enum**:

- Sexo
- TipoPersona
- TipoAccidente
- CodigoAccidente
- Gravedad

Esto evita cadenas literales, mejora la legibilidad y reduce errores durante las consultas.

---

## Lectura concurrente de los CSV

Los tres ficheros son independientes, por lo que su lectura se realiza simultáneamente utilizando asincronía y concurrencia.

```csharp
var tareas = archivos
    .Select(archivo => Task.Run(() => LeerArchivo(archivo))).ToArray();

var resultados = await Task.WhenAll(tareas);

return resultados.SelectMany(x => x).ToList();
```

### Justificación

Cada CSV contiene decenas de miles de registros (130.864 entre los 3). Leerlos de forma secuencial desaprovecha los recursos del procesador. `Task.WhenAll()` permite ejecutar las tres lecturas al mismo tiempo y esperar únicamente cuando todas han finalizado.

Después de la lectura, los resultados se combinan en una única colección de accidentes.

---

## Repositorio en memoria

Los accidentes se almacenan en una `List<Accidente>`.

### Justificación

Las 60 consultas (30 LINQ y 30 DataFrame) trabajan sobre los mismos datos. Mantener toda la colección en memoria evita volver a acceder al disco y elimina tiempos de lectura innecesarios.

---

## LINQ y PLINQ

No todas las consultas utilizan paralelización.

| Tipo de consulta | Técnica |
|-----------------|----------|
| Conteos simples | LINQ |
| Agrupaciones grandes | PLINQ |
| Top 5 y estadísticas | PLINQ |

### Justificación

PLINQ introduce un coste asociado a la creación y coordinación de tareas. En consultas muy simples (`Count()`), ese coste puede ser superior al beneficio. Por ello únicamente se emplea `.AsParallel()` en operaciones donde el volumen de datos hace rentable la paralelización.

---

## DataFrame

El DataFrame se construye una única vez al comenzar el análisis.

```csharp
var dataframe = CrearDataFrame(accidentes);

[...]

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
```

### Justificación

Construir un DataFrame implica recorrer todos los registros y crear columnas tipadas. Repetir este proceso en cada consulta multiplicaría el tiempo total de ejecución, por lo que se reutiliza la misma estructura para las 30 consultas.

---

## Medición del rendimiento

Se utilizan dos cronómetros diferentes.

### Tiempo individual

Cada consulta mide exclusivamente su propia ejecución.

```csharp
var sw = Stopwatch.StartNew();

Consulta();

sw.Stop();
```

### Tiempo global

Cada analizador mide también el tiempo total empleado en ejecutar las 30 consultas.

De esta forma pueden compararse tanto consultas concretas como el rendimiento global entre LINQ/PLINQ y DataFrame.

De la misma manera, se utiliza un cronómetro global para el tiempo de lectura de los ficheros, realizando una aproximación de tiempos para cada fichero según el porcentaje aproximado del cómputo de líneas.

---

## Resultados obtenidos

### Lectura de ficheros

| Operación | Tiempo |
|-----------|--------|
| Lectura de los 3 CSV | **~1.264,181 ms** |

Los tiempos de lectura pueden variar según el rendimiento del dispositivo en el momento de la ejecución del programa y al ser de manera asíncrona `Task.WhenAll()`, dependerá del tiempo que tarde en leer el archivo CSV más largo y pesado.

### Tiempo total por técnica

| Técnica | Tiempo | Tiempo/Consulta |
|---------|--------|-----------------|
| LINQ / PLINQ | **~621,010ms** | **~20,700ms** |
| DataFrame | **~608,813ms** | **~20,294ms** |

Los tiempos de lectura pueden variar según el rendimiento del dispositivo en el momento de la ejecución del programa.

### Comparativa de consultas

| Consulta | LINQ / PLINQ | DataFrame |
|----------|--------------:|----------:|
| 01 | 0,469 ms | 0,398 ms |
| 02 | 51,028 ms | 30,591 ms |
| 03 | 33,748 ms | 17,568 ms |
| 04 | 30,278 ms | 21,327 ms |
| 05 | 30,188 ms | 12,798 ms |
| 06 | 25,413 ms | 24,743 ms |
| 07 | 19,630 ms | 13,279 ms |
| 08 | 6,722 ms | 11,237 ms |
| 09 | 20,481 ms | 17,926 ms |
| 10 | 20,841 ms | 13,399 ms |
| 11 | 11,810 ms | 13,135 ms |
| 12 | 28,339 ms | 43,074 ms |
| 13 | 26,200 ms | 25,903 ms |
| 14 | 5,865 ms | 4,022 ms |
| 15 | 19,823 ms | 4,897 ms |
| 16 | 6,410 ms | 5,370 ms |
| 17 | 20,455 ms | 12,703 ms |
| 18 | 25,213 ms | 33,353 ms |
| 19 | 5,038 ms | 15,271 ms |
| 20 | 7,229 ms | 5,326 ms |
| 21 | 6,377 ms | 11,854 ms |
| 22 | 11,973 ms | 38,760 ms |
| 23 | 13,837 ms | 19,378 ms |
| 24 | 21,838 ms | 42,836 ms |
| 25 | 38,127 ms | 45,634 ms |
| 26 | 19,575 ms | 38,057 ms |
| 27 | 25,887 ms | 23,227 ms |
| 28 | 36,767 ms | 22,574 ms |
| 29 | 32,877 ms | 31,594 ms |
| 30 | 14,815 ms | 4,384 ms |

---

## Análisis de resultados

### Rendimiento de LINQ / PLINQ

Las consultas que realizan agrupaciones y ordenaciones sobre muchos registros son las que más tiempo necesitan. Por ejemplo, la consulta por distrito tarda **51,028 ms**, mientras que la de distrito por año tarda **38,127 ms**.

Las consultas más sencillas, como contar peatones o combinar alcohol y drogas, tienen tiempos menores.

El uso de PLINQ permite aprovechar varios núcleos del procesador, aunque en consultas sencillas el coste del paralelismo puede hacer que la mejora sea pequeña.

### Rendimiento de DataFrame

DataFrame obtiene mejores tiempos en varias consultas, especialmente cuando se recorren directamente las columnas y se utilizan estructuras como `Dictionary` o arrays.

Por ejemplo, la consulta de accidentes por distrito baja de **51,028 ms** en LINQ/PLINQ a **30,591 ms** en DataFrame.

Sin embargo, no todas las consultas mejoran. Algunas, como la consulta de evolución mensual por año, tardan más en DataFrame (**42,836 ms**) que en LINQ/PLINQ (**21,838 ms**).

### Comparación general

| Técnica | Ventajas |
|----------|----------|
| LINQ | Código sencillo y expresivo |
| PLINQ | Permite aprovechar varios núcleos |
| DataFrame | Adecuado para trabajar con datos organizados por columnas |

La técnica más eficiente depende del tipo de consulta; no existe un enfoque universalmente superior.

---

## Conclusiones

Esta práctica demuestra que el rendimiento no depende únicamente del lenguaje o de la biblioteca utilizada, sino también del diseño del algoritmo.

Las principales optimizaciones aplicadas han sido:

- Lectura concurrente de los tres ficheros CSV mediante `Task.WhenAll()`.
- Reutilización de una única colección en memoria.
- Uso selectivo de PLINQ únicamente cuando compensa.
- Optimización de las consultas DataFrame mediante recorridos lineales y diccionarios para reducir asignaciones y recorridos innecesarios.

El resultado es una aplicación capaz de procesar más de 100.000 registros y comparar de forma objetiva tres enfoques distintos de análisis de datos en .NET.
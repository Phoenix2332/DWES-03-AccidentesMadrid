using System.Diagnostics;
using AccidentesMadrid.Models;

namespace AccidentesMadrid.Analyzers;

public interface IAccidentesAnalyzer {
    Stopwatch Analizar(IEnumerable<Accidente> accidentes);
}
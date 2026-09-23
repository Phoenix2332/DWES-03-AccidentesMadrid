using AccidentesMadrid.Models;
using Serilog;

namespace AccidentesMadrid.Repositories;

public class AccidentesRepository : IAccidentesRepository {
    private readonly List<Accidente> _accidentes = [];
    private readonly ILogger _logger = Log.ForContext<AccidentesRepository>();

    public Task<IEnumerable<Accidente>> GetAll() {
        _logger.Debug("[REPO-GET] Obteniendo accidentes");
        var resultado = _accidentes
            .OrderBy(a => a.NumExpediente)
            .ToList();

        return Task.FromResult<IEnumerable<Accidente>>(resultado);
    }

    public Task SaveAll(IEnumerable<Accidente> accidentes) {
        _logger.Debug("[REPO-POST] Guardando accidentes en memoria");
        _accidentes.Clear();
        foreach (var accidente in accidentes) _accidentes.Add(accidente);

        _logger.Information("[REPO-POST] Total accidentes almacenados: {Total}", _accidentes.Count);

        return Task.CompletedTask;
    }
}
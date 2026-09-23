using AccidentesMadrid.Models;
using AccidentesMadrid.Repositories;
using AccidentesMadrid.Storages;
using Serilog;

namespace AccidentesMadrid.Services;

public class AccidentesService(
    IAccidentesStorage storage,
    IAccidentesRepository repository
) : IAccidentesService {
    private readonly ILogger _logger = Log.ForContext<AccidentesService>();

    public async Task CargarAccidentes() {
        _logger.Debug("[SERV-POST] Guardando accidentes en memoria");
        var accidentes = await storage.Cargar();
        await repository.SaveAll(accidentes);
    }

    public async Task<IEnumerable<Accidente>> GetAll() {
        _logger.Debug("[REPO-GET] Obteniendo accidentes");
        return await repository.GetAll();
    }
}
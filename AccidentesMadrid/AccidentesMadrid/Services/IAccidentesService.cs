using AccidentesMadrid.Models;

namespace AccidentesMadrid.Services;

public interface IAccidentesService {
    Task CargarAccidentes();
    Task<IEnumerable<Accidente>> GetAll();
}
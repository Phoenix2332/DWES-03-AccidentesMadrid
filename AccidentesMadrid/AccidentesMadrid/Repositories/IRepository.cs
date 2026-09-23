using AccidentesMadrid.Models;

namespace AccidentesMadrid.Repositories;

public interface IRepository<T> where T : class {
    Task<IEnumerable<T>> GetAll();
    Task SaveAll(IEnumerable<Accidente> accidentes);
}
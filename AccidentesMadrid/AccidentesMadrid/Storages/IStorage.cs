namespace AccidentesMadrid.Storages;

public interface IStorage<T> {
    Task<IEnumerable<T>> Cargar();
}
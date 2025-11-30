namespace UHO_API.Interfaces.IRepository;

public interface IUnitOfWorks:IDisposable
{
    IAreaRepository Area { get; }
    Task SaveChangesAsync();
    void SaveChanges();
}
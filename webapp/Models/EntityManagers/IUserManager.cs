namespace webapp.Models.EntityManagers;

public interface IUserManager<T>
{
    public bool DbContains(string id);
    public void Add(T user);
    public T? GetById(string id);
    public T? GetByEmail(string email);
}
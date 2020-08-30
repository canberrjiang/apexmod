using System;
using System.Collections;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;

namespace Infrastructure.Data
{
  public class UnitOfWork : IUnitOfWork
  {
    private readonly StoreContext _context;

    // Used to hold all the respositories
    private Hashtable _repositories;
    public UnitOfWork(StoreContext context)
    {
      _context = context;
    }

    public async Task<int> Complete()
    {
      return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
      _context.Dispose();
    }

    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
    {
      // Create a new Hashtable if there is not one.
      if (_repositories == null) _repositories = new Hashtable();

      // Get incoming entity name e.g. Product, Product Name...
      var type = typeof(TEntity).Name;

      if (!_repositories.ContainsKey(type))
      {
        var respositoryType = typeof(GenericRepository<>);
        var respositoryInstance = Activator.CreateInstance(respositoryType.MakeGenericType(typeof(TEntity)), _context);
        _repositories.Add(type, respositoryInstance);
      }

      return (IGenericRepository<TEntity>)_repositories[type];
    }
  }
}
using Microsoft.EntityFrameworkCore;

namespace Unitable.DataAccess;

public class UnitableDbContext : DbContext
{
    public UnitableDbContext()
    {
         
    }

    public UnitableDbContext(DbContextOptions<UnitableDbContext> options) : base(options)
    {

    }
}
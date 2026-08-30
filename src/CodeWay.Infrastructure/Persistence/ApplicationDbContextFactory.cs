namespace CodeWay.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=CodeWayDB;User ID=sa;Password=StrongPassword123;Encrypt=True;TrustServerCertificate=True;");


        return new ApplicationDbContext(optionsBuilder.Options);
    }
}


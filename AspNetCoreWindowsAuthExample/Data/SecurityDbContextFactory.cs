using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using AspNetCoreWindowsAuthExample.Models;

namespace AspNetCoreWindowsAuthExample.Data
{
    public class SecurityDbContextFactory : IDesignTimeDbContextFactory<SecurityContext>
    {
        public SecurityContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<SecurityContext>();
            builder.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=config;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new SecurityContext(builder.Options);
        }
    }
}
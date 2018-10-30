using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using AspNetCoreWindowsAuthExample.Models;

namespace AspNetCoreWindowsAuthExample.Repositories
{
    public interface IUserRoleRepository : IRepository<UserRole>
    {
        //UserInformation GetUserWithRoles(string UserLanId);
    }

    public class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
    {
        public DbContext context
        {
            get { return db as DbContext; }
        }

        public UserRoleRepository(DbContext _db) : base(_db)
        {
        }
    }
}
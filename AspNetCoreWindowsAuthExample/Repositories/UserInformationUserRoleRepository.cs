using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using AspNetCoreWindowsAuthExample.Models;

namespace AspNetCoreWindowsAuthExample.Repositories
{
    public interface IUserInformationUserRoleRepository : IRepository<UserInformationUserRole>
    {
        //UserInformation GetUserWithRoles(string UserLanId);
    }

    public class UserInformationUserRoleRepository : Repository<UserInformationUserRole>, IUserInformationUserRoleRepository
    {
        public DbContext context
        {
            get { return db as DbContext; }
        }

        public UserInformationUserRoleRepository(DbContext _db) : base(_db)
        {
        }
    }
}
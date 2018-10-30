using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AspNetCoreWindowsAuthExample;
using AspNetCoreWindowsAuthExample.Models;

namespace AspNetCoreWindowsAuthExample.Repositories
{
    public interface IUserInformationRepository : IRepository<UserInformation>
    {
        UserInformation GetUserWithRoles(string UserLanId);

        IEnumerable<UserInformation> GetUserListWithRoles(Expression<Func<UserInformation, bool>> filter, Func<IQueryable<UserInformation>, IOrderedQueryable<UserInformation>> orderBy);

        Task<List<UserInformation>> GetUserListWithRolesAsync(Expression<Func<UserInformation, bool>> filter,
            Func<IQueryable<UserInformation>, IOrderedQueryable<UserInformation>> orderBy);
    }

    public class UserInformationRepository : Repository<UserInformation>, IUserInformationRepository
    {
        public DbContext context
        {
            get
            {
                return db as DbContext;
            }
        }

        public UserInformationRepository(DbContext _db) : base(_db)
        {
        }

        public UserInformation GetUserWithRoles(string UserLanId)
        {
            return db.Set<UserInformation>().AsNoTracking().Where(u => u.LanId == UserLanId)
                .Include(ur => ur.UserInformationUserRoles)
                .ThenInclude(r => r.UserRole).SingleOrDefault();
        }

        public virtual IEnumerable<UserInformation> GetUserListWithRoles(
                   Expression<Func<UserInformation, bool>> filter = null,
                   Func<IQueryable<UserInformation>, IOrderedQueryable<UserInformation>> orderBy = null
                   )
        {
            IQueryable<UserInformation> query = db.Set<UserInformation>()
                .Include(ur => ur.UserInformationUserRoles)
                .ThenInclude(r => r.UserRole);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public Task<List<UserInformation>> GetUserListWithRolesAsync(Expression<Func<UserInformation, bool>> filter,
            Func<IQueryable<UserInformation>, IOrderedQueryable<UserInformation>> orderBy)
        {
            IQueryable<UserInformation> query = db.Set<UserInformation>()
                .Include(ur => ur.UserInformationUserRoles)
                .ThenInclude(r => r.UserRole);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToListAsync();
            }
            else
            {
                return query.ToListAsync();
            }
        }
    }
}
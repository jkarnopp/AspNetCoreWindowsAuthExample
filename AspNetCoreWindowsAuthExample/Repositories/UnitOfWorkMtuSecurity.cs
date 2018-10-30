using Microsoft.EntityFrameworkCore;
using AspNetCoreWindowsAuthExample.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AspNetCoreWindowsAuthExample.Repositories;

namespace AspNetCoreWindowsAuthExample.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserInformationRepository UserInformations { get; }
        IUserRoleRepository UserRoles { get; }
        IUserInformationUserRoleRepository UserInformationUserRole { get; }
        ISysConfigRepository SysConfigs { get; }

        int SaveChanges();

        Task<int> SaveChangesAsync();
    }

    /// <summary>
    /// The Unit of Work pattern allows you to share a single DbContext among several queries allow you to rollback if one fails.
    /// For each repository, create a new entry in the interface and copy one of the get sections below changing the type information.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext db;

        public UnitOfWork(SecurityContext db)
        {
            this.db = db;
        }

        private IUserInformationRepository _userInformations;

        public IUserInformationRepository UserInformations
        {
            get
            {
                if (this._userInformations == null)
                {
                    this._userInformations = new UserInformationRepository(db);
                }
                return this._userInformations;
            }
        }

        private IUserRoleRepository _userRoles;

        public IUserRoleRepository UserRoles
        {
            get
            {
                if (this._userRoles == null)
                {
                    this._userRoles = new UserRoleRepository(db);
                }
                return this._userRoles;
            }
        }

        private IUserInformationUserRoleRepository _userInformationUserRoles;

        public IUserInformationUserRoleRepository UserInformationUserRole
        {
            get
            {
                if (this._userInformationUserRoles == null)
                {
                    this._userInformationUserRoles = new UserInformationUserRoleRepository(db);
                }
                return this._userInformationUserRoles;
            }
        }

        private ISysConfigRepository _sysConfigRepository;

        public ISysConfigRepository SysConfigs
        {
            get
            {
                if (this._sysConfigRepository == null)
                {
                    this._sysConfigRepository = new SysConfigRepository((SecurityContext)db);
                }
                return this._sysConfigRepository;
            }
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return db.SaveChangesAsync();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
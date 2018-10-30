using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AspNetCoreWindowsAuthExample.Data;
using AspNetCoreWindowsAuthExample.Models;

namespace AspNetCoreWindowsAuthExample.Repositories
{
    public interface ISysConfigRepository : IDisposable
    {
        Task<SysConfig> GetSysConfig();

        void UpdateSysConfig(SysConfig sysConfig);

        Task<int> Save();
    }

    public class SysConfigRepository : ISysConfigRepository
    {
        private SecurityContext context;

        public SysConfigRepository(SecurityContext context)
        {
            this.context = context;
        }

        public Task<SysConfig> GetSysConfig()
        {
            return context.SysConfigs.FindAsync(1);
        }

        public void UpdateSysConfig(SysConfig sysConfig)
        {
            context.Entry(sysConfig).State = EntityState.Modified;
        }

        public Task<int> Save()
        {
            return context.SaveChangesAsync();
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
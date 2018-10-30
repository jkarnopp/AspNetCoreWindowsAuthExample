using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreWindowsAuthExample.Dtos
{
    public class AssignedRoleData
    {
        public int UserRoleId { get; set; }
        public string Name { get; set; }
        public bool Assigned { get; set; }
    }
}
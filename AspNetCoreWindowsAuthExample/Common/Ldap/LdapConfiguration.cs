namespace AspNetCoreWindowsAuthExample.Common.Ldap
{
    public interface ILdapConfiguration
    {
        string GlobalCatalog { get; }
        string BasePath { get; }
        string LdapUsername { get; set; }
        string LdapPassword { get; set; }
    }

    public class LdapConfiguration : ILdapConfiguration
    {
        public string GlobalCatalog { get; set; }
        public string BasePath { get; set; }
        public string LdapUsername { get; set; }
        public string LdapPassword { get; set; }
    }
}
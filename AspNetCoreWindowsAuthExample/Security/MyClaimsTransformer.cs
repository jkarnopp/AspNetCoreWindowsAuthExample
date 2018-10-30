using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using AspNetCoreWindowsAuthExample.Data;
using AspNetCoreWindowsAuthExample.Repositories;

namespace AspNetCoreWindowsAuthExample.Security
{
    public class MyClaimsTransformer : IClaimsTransformation
    {
        private readonly IUnitOfWork _unitOfWork;

        public MyClaimsTransformer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Each time HttpContext.AuthenticateAsync() or HttpContext.SignInAsync(...) is called the claims transformer is invoked. So this might be invoked multiple times.
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = principal.Identities.FirstOrDefault(x => x.IsAuthenticated);
            if (identity == null) return principal;

            var user = identity.Name;
            if (user == null) return principal;

            //Get user with roles from repository.
            var dbUser = _unitOfWork.UserInformations.GetUserWithRoles(user);
            if (dbUser == null) return principal;

            var claims = new List<Claim>();

            //The claim identity uses a claim with the claim type below to determine the name property.
            claims.Add(new Claim(@"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", user, "Name"));

            //todo: We should probably create a cache for this
            // Get User Roles from database and add to list of claims.
            foreach (var role in dbUser.UserInformationUserRoles.Select((r => r.UserRole)))
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var newClaimsIdentity = new ClaimsIdentity(claims, "Kerberos", "", "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");

            var newClaimsPrincipal = new ClaimsPrincipal(newClaimsIdentity);

            return new ClaimsPrincipal(newClaimsPrincipal);
        }
    }
}
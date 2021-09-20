using Microsoft.AspNetCore.Authorization;
using WeeControl.Common.UserSecurityLib.EntityGroups;

namespace WeeControl.Common.UserSecurityLib.Helpers
{
    public abstract class PolicyBuilderBase
    {
        protected readonly AuthorizationPolicyBuilder Builder;

        protected PolicyBuilderBase()
        {
            Builder = new AuthorizationPolicyBuilder();
            Builder.AddAuthenticationSchemes("Bearer");
            //Builder.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
            Builder.RequireClaim(ClaimGroup.ClaimType.Session);
        }

        public Microsoft.AspNetCore.Authorization.AuthorizationPolicy GetPolicy()
        {
            return Builder.Build();
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using WeeControl.SharedKernel.Essential;

namespace WeeControl.SharedKernel.Services.PolicyBuild;

public abstract class PolicyBuilderBase
{
    protected readonly AuthorizationPolicyBuilder Builder;

    protected PolicyBuilderBase()
    {
        Builder = new AuthorizationPolicyBuilder();
        Builder.AddAuthenticationSchemes("Bearer");
        Builder.RequireClaim(HumanResourcesData.Claims.Session);
    }

    public Microsoft.AspNetCore.Authorization.AuthorizationPolicy GetPolicy()
    {
        return Builder.Build();
    }
}
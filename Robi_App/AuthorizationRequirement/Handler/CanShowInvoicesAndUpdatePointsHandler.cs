using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Robi_App.Models;

namespace Robi_App.AuthorizationRequirement.Handler
{
    public class CanShowInvoicesAndUpdatePointsHandler : AuthorizationHandler
       <CanShowInvoicesAndUpdatePoints, Invoice>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanShowInvoicesAndUpdatePoints requirement, Invoice resource)
        {
            if (context.User.HasClaim(C => C.Type == SD.Role_Admin))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            var StoreClaim = context.User.FindFirst(c => c.Type == SD.ForStore);
            if (StoreClaim != null && resource.StoreId.ToString() == StoreClaim.Value)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;

        }
    }
}

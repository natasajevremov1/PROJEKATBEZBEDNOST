using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class CustomAuthorizationManager : ServiceAuthorizationManager
    {
        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            CustomPrincipal principal = operationContext.ServiceSecurityContext.
                 AuthorizationContext.Properties["Principal"] as CustomPrincipal;
            return principal.IsInRole("ExchangeSessionKey");
        }
    }
}

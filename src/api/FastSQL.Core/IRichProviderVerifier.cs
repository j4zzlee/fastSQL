using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Core
{
    public interface IRichProviderVerifier
    {
        IRichProvider GetProvider();
        bool IsProvider(string providerId);
        bool IsProvider(IRichProvider provider);
    }
}

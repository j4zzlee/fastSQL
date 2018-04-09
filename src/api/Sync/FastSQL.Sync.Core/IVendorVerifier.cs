using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core
{
    public interface IVendorVerifier
    {
        IVendor GetVendor();
        bool IsVendor(string vendorId);
        bool IsVendor(IVendor vendor);
    }
}

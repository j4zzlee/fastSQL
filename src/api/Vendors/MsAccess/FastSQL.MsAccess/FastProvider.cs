using FastSQL.Core;

namespace FastSQL.MsAccess
{
    public class FastProvider : BaseProvider
    {
        public override string Id => "1064aecb081027138f91e1e7e401a99239f8928d";
        public override string Name => "MsAccess";

        public override string DisplayName => "Microsoft Access";

        public override string Description => "Microsoft Access";
        
        public FastProvider(ProviderOptionManager options) : base(options)
        {
        }
    }
}

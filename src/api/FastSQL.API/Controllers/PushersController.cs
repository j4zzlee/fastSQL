using FastSQL.Sync.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastSQL.API.Controllers
{
    [Route("api/[controller]")]
    public class PushersController
    {
        private readonly IEnumerable<IPusher> _pushers;
        private readonly IEnumerable<IIndexer> _indexers;

        public PushersController(IEnumerable<IPusher> pushers, IEnumerable<IIndexer> indexers)
        {
            _pushers = pushers;
            _indexers = indexers;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastSQL.Sync.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FastSQL.API.Controllers
{
    [Route("api/[controller]")]
    public class PullersController : Controller
    {
        private readonly IEnumerable<IPuller> _pullers;
        private readonly IEnumerable<IIndexer> _indexers;

        public PullersController(IEnumerable<IPuller> pullers, IEnumerable<IIndexer> indexers)
        {
            _pullers = pullers;
            _indexers = indexers;
        }
    }
}
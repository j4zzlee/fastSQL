using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastSQL.Sync.Core.Repositories
{
    public class RepositoryFactory
    {
        private readonly object lockObject = new object();
        private readonly ResolverFactory _resolverFactory;
        private readonly Dictionary<object, List<BaseRepository>> _registeredInstances;

        public RepositoryFactory(ResolverFactory resolverFactory)
        {
            _resolverFactory = resolverFactory;
            _registeredInstances = new Dictionary<object, List<BaseRepository>>();
        }

        public T Create<T>(object owner) where T : BaseRepository
        {
            lock (lockObject)
            {
                T result = null;
                List<BaseRepository> repos = null;
                if (!_registeredInstances.ContainsKey(owner))
                {
                    repos = new List<BaseRepository>();
                    _registeredInstances.Add(owner, repos);
                }
                else
                {
                    repos = _registeredInstances[owner]?.Where(r => r != null && !r.IsDisposed).ToList();
                }

                if (repos == null)
                {
                    repos = new List<BaseRepository>();
                    _registeredInstances[owner] = repos;
                }

                result = repos.Where(r => r is T).Select(r => r as T).FirstOrDefault();
                if (result == null || result.IsDisposed)
                {
                    result = _resolverFactory.Resolve<T>();
                    repos.Add(result);
                }
                // Update the list
                _registeredInstances[owner] = repos;
                return result;
            }
        }

        public void Release(object owner)
        {
            lock (lockObject)
            {
                if (!_registeredInstances.ContainsKey(owner))
                {
                    return;
                }

                var repos = _registeredInstances[owner];
                if (repos != null && repos.Count > 0)
                {
                    foreach (var r in repos)
                    {
                        _resolverFactory.Release(r);
                    }
                }

                _registeredInstances.Remove(owner);
            }
        }
    }
}

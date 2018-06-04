using System.Collections.Generic;
using System.Data.Common;
using FastSQL.Core;
using FastSQL.Sync.Core.ExtensionMethods;
using FastSQL.Sync.Core.Models;

namespace FastSQL.Sync.Core.Repositories
{
    public abstract class BaseGenericRepository<TModel> : BaseRepository
         where TModel : class, new()
    {
        protected BaseGenericRepository(DbConnection connection) : base(connection)
        {
        }

        public virtual TModel GetById(string id)
        {
            return GetById<TModel>(id);
        }

        public virtual IEnumerable<TModel> GetByIds(IEnumerable<string> ids)
        {
            return GetByIds<TModel>(ids);
        }

        public virtual IEnumerable<TModel> GetByIds (params string[] ids)
        {
            return GetByIds<TModel>(ids);
        }

        public virtual IEnumerable<TModel> GetAll(int? limit = null, int? offset = null)
        {
            return GetAll<TModel>(limit, offset);
        }

        public virtual int DeleteById(string id)
        {
            return DeleteById<TModel>(id);
        }

        public virtual string Create(object @params)
        {
            return Create<TModel>(@params);
        }

        public virtual int Update(string id, object @params)
        {
            return Update<TModel>(id, @params);
        }

        public override void LinkOptions(string id, IEnumerable<OptionItem> options)
        {
            LinkOptions(id, typeof(TModel).GetEntityType(), options);
        }

        public override void UnlinkOptions(string id, IEnumerable<string> optionGroups = null)
        {
            UnlinkOptions(id, typeof(TModel).GetEntityType(), optionGroups);
        }
        
        public override IEnumerable<OptionModel> LoadOptions(string entityId, IEnumerable<string> optionGroups = null)
        {
            return LoadOptions(entityId, typeof(TModel).GetEntityType(), optionGroups);
        }

        public override IEnumerable<OptionModel> LoadOptions(IEnumerable<string> entityIds, IEnumerable<string> optionGroups = null)
        {
            return LoadOptions(entityIds, typeof(TModel).GetEntityType(), optionGroups);
        }
    }
}

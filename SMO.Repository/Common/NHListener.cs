using NHibernate.Event;
using NHibernate.Persister.Entity;

using SMO.Core.Entities;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace SMO.Repository.Common
{
    public class NHListener : IPreInsertEventListener, IPreUpdateEventListener
    {
        public bool OnPreInsert(PreInsertEvent @event)
        {
            if (@event.Entity as BaseEntity == null)
                return false;
            var time = DateTime.Now;
            Set(@event.Persister, @event.State, "CREATE_DATE", time);
            (@event.Entity as BaseEntity).CREATE_DATE = time;
            return false;
        }

        public async Task<bool> OnPreInsertAsync(PreInsertEvent @event, CancellationToken cancellationToken)
        {
            return await Task.FromResult(false);
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            if (!(@event.Entity is BaseEntity baseEntity))
                return false;
            var time = DateTime.Now;
            Set(@event.Persister, @event.State, "UPDATE_DATE", time);
            baseEntity.UPDATE_DATE = time;
            return false;
        }

        public async Task<bool> OnPreUpdateAsync(PreUpdateEvent @event, CancellationToken cancellationToken)
        {
            return await Task.FromResult(false);
        }

        private void Set(IEntityPersister persister, object[] state, string propertyName, object value)
        {
            var index = Array.IndexOf(persister.PropertyNames, propertyName);
            if (index == -1)
                return;
            state[index] = value;
        }
    }
}

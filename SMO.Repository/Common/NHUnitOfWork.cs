using NHibernate;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SMO.Repository.Common
{
    public class NHUnitOfWork : IUnitOfWork
    {
        private ISession _session;
        private IStatelessSession _statelessSession;
        private ITransaction _transaction;
        public ISession Session { get { return _session; } }
        public IStatelessSession StatelessSession { get { return _statelessSession; } }
        private readonly Dictionary<Type, dynamic> repositories;

        public NHUnitOfWork()
        {
            repositories = new Dictionary<Type, dynamic>();
            OpenSession();
        }

        public ISession GetSession()
        {
            return Session;
        }

        public IStatelessSession GetStatelessSession()
        {
            return StatelessSession;
        }

        /// <summary>
        /// 
        /// </summary>
        private void OpenSession()
        {
            if (_session == null || !_session.IsConnected)
            {
                if (_session != null)
                    _session.Dispose();

                _session = NHSessionFactorySingleton.SessionFactory.OpenSession();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OpenStatelessSession()
        {
            if (_statelessSession == null || !_statelessSession.IsConnected)
            {
                if (_statelessSession != null)
                    _statelessSession.Dispose();

                _statelessSession = NHSessionFactorySingleton.SessionFactory.OpenStatelessSession();
            }
        }

        public T Repository<T>() where T : class
        {
            if (repositories.Keys.Contains(typeof(T)) == true)
            {
                return repositories[typeof(T)];//as T
            }
            var repo = Activator.CreateInstance(typeof(T), this);
            repositories.Add(typeof(T), repo);
            return repo as T;
        }



        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (_transaction == null || !_transaction.IsActive)
            {
                if (_transaction != null)
                    _transaction.Dispose();

                _transaction = _session.BeginTransaction(isolationLevel);
            }
        }

        public void Commit()
        {
            try
            {
                if (HasOpenTransaction())
                {
                    _transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                //this.Rollback();
                throw ex;
            }
        }

        public void Flush()
        {
            try
            {
                Session.Flush();
            }
            catch (Exception ex)
            {
                //this.Rollback();
                throw ex;
            }
        }

        public void Clear()
        {
            try
            {
                Session.Clear();
            }
            catch (Exception ex)
            {
                //this.Rollback();
                throw ex;
            }
        }

        public void Rollback()
        {
            try
            {
                if (HasOpenTransaction())
                {
                    _transaction.Rollback();
                }
            }
            catch
            {
            }

            //CloseSession();
        }

        public bool HasOpenTransaction()
        {
            return _transaction != null && !_transaction.WasCommitted && !_transaction.WasRolledBack;
        }

        /// <summary>
        /// Flushes anything left in the session and closes the connection.
        /// </summary>
        public void CloseSession()
        {
            if (_session != null && _session.IsOpen)
            {
                //this._session.Flush();
                _session.Close();
            }
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }

            if (_session != null)
            {
                _session.Dispose();
                _session = null;
            }
        }
    }
}

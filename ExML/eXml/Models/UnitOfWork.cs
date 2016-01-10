using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eXml.Abstractions;
using eXml.Entities;


namespace eXml.Models
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private eXmlContext DbContext { get; set; }
        private IRepository<PostedTransaction> _pTrans;
        private IRepository<PurchaseRegister> _purRegister;
        public UnitOfWork()
        {
            CreateDbContext();
        }

        protected void CreateDbContext()
        {
            DbContext = new eXmlContext();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (DbContext != null)
                {
                    DbContext.Dispose();
                }
            }
        }
        public IRepository<PostedTransaction> PostedTransactions
        {
            get
            {
                if (_pTrans == null)
                {
                    _pTrans = new Repository<PostedTransaction>(DbContext);
                }
                return _pTrans;
            }
        }
        public IRepository<PurchaseRegister> PurchaseRegister
        {
            get
            {
                if (_purRegister == null)
                {
                    _purRegister = new Repository<PurchaseRegister>(DbContext);
                }
                return _purRegister;
            }
        }
        public void Commit()
        {
            DbContext.SaveChanges();
        }
    }
}
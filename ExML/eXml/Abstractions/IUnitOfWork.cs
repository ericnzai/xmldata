using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eXml.Entities;

namespace eXml.Abstractions
{
    public interface IUnitOfWork 
    {
        void Commit();
        void Dispose();
        IRepository<PostedTransaction> PostedTransactions { get; }
        IRepository<PurchaseRegister> PurchaseRegister { get; }
    }
}

using Refactored.Persistence.Entities;

namespace Refactored.Persistence.Data.Repository.Interface
{
    public interface IInvoiceRepository : IBaseRepository<Invoice>
    {
        public Invoice GetInvoice(string reference);

        public void SaveInvoice(Invoice invoice);

        public void Add(Invoice invoice);
    }
}


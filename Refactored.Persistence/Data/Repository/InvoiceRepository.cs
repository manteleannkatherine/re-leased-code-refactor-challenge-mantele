using Refactored.Persistence.Data.Repository.Interface;
using Refactored.Persistence.Entities;

namespace Refactored.Persistence.Data.Repository
{
    public class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(AppDbContext context) : base(context)
        {
        }

        public Invoice GetInvoice(string reference)
        {
            var _invoice = this.FindByAndAllIncluding(x => x.Payments.Any(p => p.Reference == reference), x => x.Payments).FirstOrDefault();
            return _invoice;
        }

        public void SaveInvoice(Invoice invoice)
        {
            this.UpdateCommit(invoice);
        }

        public void Add(Invoice invoice)
        {
            this.AddCommit(invoice);
        }
    }
}


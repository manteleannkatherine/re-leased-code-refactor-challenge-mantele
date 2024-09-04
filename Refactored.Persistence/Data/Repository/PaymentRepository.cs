using Refactored.Persistence.Data.Repository.Interface;
using Refactored.Persistence.Entities;

namespace Refactored.Persistence.Data.Repository
{
    public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(AppDbContext context) : base(context)
        {
        }
    }
}


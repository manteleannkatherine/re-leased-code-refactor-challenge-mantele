using Refactored.Persistence.Entities;

namespace Refactored.Domain.Services.Interface
{
	public interface IInvoiceService
	{
		public string ProcessPayment(Payment payment);
	}
}


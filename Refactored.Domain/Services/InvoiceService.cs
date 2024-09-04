using Refactored.Domain.Services.Interface;
using Refactored.Persistence.Data.Repository.Interface;
using Refactored.Persistence.Entities;
using static Refactored.Persistence.Enums.ResponseMessages;
using static Refactored.Persistence.Enums.CommonEnum;

namespace Refactored.Domain.Services
{
    public class InvoiceService : IInvoiceService
    {
        public readonly IInvoiceRepository _invoiceRepository;

        public InvoiceService(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        private string ValidateInvoice(Invoice invoice)
        {
            if (invoice == null)
                throw new InvalidOperationException(Messages["InvoiceNotFound"]);

            if (invoice.Amount == 0)
            {
                if (invoice.Payments == null || !invoice.Payments.Any())
                {
                    return Messages["NoPaymentNeeded"];
                }
                else
                    throw new InvalidOperationException(Messages["InvalidInvoice"]);
            }

            return string.Empty;
        }

        public string ProcessPayment(Payment payment)
        {
            var invoice = _invoiceRepository.GetInvoice(payment.Reference);

            string validationMessage = ValidateInvoice(invoice);

            if (validationMessage != string.Empty)
                return validationMessage;

            string responseMessage = CheckPaymentStatus(payment, invoice);
            ApplyPayment(payment, invoice);

            _invoiceRepository.SaveInvoice(invoice);
            return responseMessage;

        }

        private string CheckPaymentStatus(Payment payment, Invoice invoice)
        {
            decimal totalPayments = invoice.Payments?.Sum(x => x.Amount) ?? 0;
            decimal invoiceBalance = invoice.Amount - invoice.AmountPaid;

            if (invoice.Payments != null && invoice.Payments.Any())
            {
                if (totalPayments != 0 && invoice.Amount == totalPayments)
                    return Messages["InvoiceAlreadyFullyPaid"];
                if (payment.Amount > invoiceBalance)
                    return Messages["PaymentGreaterThanRemaining"];
            }
            else
            {
                if (payment.Amount > invoice.Amount)
                    return Messages["PaymentGreaterThanInvoiceAmount"];
                else
                    return payment.Amount == invoice.Amount ?
                        Messages["InvoiceFullyPaid"] :
                        Messages["InvoicePartiallyPaid"];
            }

            if (payment.Amount == invoiceBalance)
                return Messages["FinalPaymentReceived"];
            else return Messages["PartialPaymentReceived"];

        }

        private void ApplyPayment(Payment payment, Invoice invoice)
        {
            bool isCommercial = invoice.Type == InvoiceType.Commercial;

            invoice.AmountPaid += payment.Amount;

            if (isCommercial)
                invoice.TaxAmount += payment.Amount * 0.14m;

            if (payment.Amount == invoice.Amount)
                invoice.TaxAmount = payment.Amount * 0.14m;

            invoice.Payments.Add(payment);
        }
    }
}
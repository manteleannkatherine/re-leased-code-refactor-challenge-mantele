namespace Refactored.Persistence.Enums
{
    public class ResponseMessages
    {
        public static readonly Dictionary<string, string> Messages = new()
        {
			{ "InvoiceNotFound", "No invoice matching this payment." },

			{ "InvalidInvoice", "Invalid invoice: amount is zero but payments exist." },

			{ "NoPaymentNeeded", "Payment not required." },

            { "InvoiceAlreadyFullyPaid", "Invoice was already fully paid." },

			{ "PaymentGreaterThanRemaining", "Payment exceeds the remaining invoice balance." },

			{ "PaymentGreaterThanInvoiceAmount", "Payment exceeds the invoice amount." },

			{ "FinalPaymentReceived", "Final payment received. Invoice is now fully paid." },

            { "PartialPaymentReceived", "Partial payment received. Invoice remains partially unpaid." },

			{ "InvoiceFullyPaid", "Invoice has been fully paid." },

			{ "InvoicePartiallyPaid", "Invoice has been partially paid." }
        };
    }
}


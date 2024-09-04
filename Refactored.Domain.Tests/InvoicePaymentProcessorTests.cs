using Moq;
using Refactored.Domain.Services.Interface;
using Refactored.Domain.Services;
using Refactored.Persistence.Data.Repository.Interface;
using Refactored.Persistence.Enums;
using Refactored.Persistence.Entities;

namespace Refactored.Domain.Tests;


public class InvoicePaymentProcessorTests
{
    private Mock<IInvoiceRepository> _mockInvoiceRepository;
    private IInvoiceService _invoiceService;

    [SetUp]
    public void Setup()
    {
        _mockInvoiceRepository = new Mock<IInvoiceRepository>();
        _invoiceService = new InvoiceService(_mockInvoiceRepository.Object);
    }

    [Test]
    public void ProcessPayment_Should_ThrowException_When_NoInvoiceFoundForPaymentReference()
    {
        var payment = new Payment { Reference = "INV001", Amount = 9999.99m };

        _mockInvoiceRepository
            .Setup(repo => repo.GetInvoice(It.IsAny<string>()))
            .Returns((Invoice)null);

        var ex = Assert.Throws<InvalidOperationException>(() => _invoiceService.ProcessPayment(payment));
        Assert.That(ex.Message, Is.EqualTo(ResponseMessages.Messages["InvoiceNotFound"]));
    }

    [Test]
    public void ProcessPayment_Should_ThrowException_When_InvoiceAmountIsZeroButPaymentsExist()
    {
        var invoice = new Invoice
        {
            Amount = 0,
            AmountPaid = 0,
            Payments = new List<Payment> { new Payment { Amount = 9999.99m } }
        };

        _mockInvoiceRepository
            .Setup(repo => repo.GetInvoice(It.IsAny<string>()))
            .Returns(invoice);

        var payment = new Payment { Reference = "INV001", Amount = 9999.99m };

        var ex = Assert.Throws<InvalidOperationException>(() => _invoiceService.ProcessPayment(payment));
        Assert.That(ex.Message, Is.EqualTo(ResponseMessages.Messages["InvalidInvoice"]));
    }

    [Test]
    public void ProcessPayment_Should_ReturnNoPaymentNeeded_When_InvoiceAmountIsZeroAndNoPayments()
    {
        var invoice = new Invoice
        {
            Amount = 0,
            AmountPaid = 0,
            Payments = null
        };

        _mockInvoiceRepository
            .Setup(repo => repo.GetInvoice(It.IsAny<string>()))
            .Returns(invoice);

        var payment = new Payment { Reference = "INV001" };

        var result = _invoiceService.ProcessPayment(payment);

        Assert.That(result, Is.EqualTo(ResponseMessages.Messages["NoPaymentNeeded"]));
    }

    [Test]
    public void ProcessPayment_Should_ReturnInvoiceAlreadyFullyPaid_When_InvoiceAmountIsEqualsTotalPayments()
    {
        var invoice = new Invoice
        {
            Amount = 10000.00m,
            AmountPaid = 10000.00m,
            Payments = new List<Payment> {
                { new Payment { Amount = 499.25m } },
                { new Payment { Amount = 9500.75m } }
            }
        };

        _mockInvoiceRepository
            .Setup(repo => repo.GetInvoice(It.IsAny<string>()))
            .Returns(invoice);

        var payment = new Payment { Reference = "INV001" };

        var result = _invoiceService.ProcessPayment(payment);

        Assert.That(result, Is.EqualTo(ResponseMessages.Messages["InvoiceAlreadyFullyPaid"]));
    }

    [Test]
    public void ProcessPayment_Should_ReturnPaymentGreaterThanRemaining_When_PaymentAmountIsGreaterThanInvoiceBalance()
    {
        var invoice = new Invoice
        {
            Amount = 10000.00m,
            AmountPaid = 9999.99m,
            Payments = new List<Payment> { new Payment { Amount = 9999.99m } }
        };

        _mockInvoiceRepository
            .Setup(repo => repo.GetInvoice(It.IsAny<string>()))
            .Returns(invoice);

        var payment = new Payment { Reference = "INV001", Amount = 1000.00m };

        var result = _invoiceService.ProcessPayment(payment);

        Assert.That(result, Is.EqualTo(ResponseMessages.Messages["PaymentGreaterThanRemaining"]));
    }

    [Test]
    public void ProcessPayment_Should_ReturnPaymentGreaterThanInvoiceAmount_When_PaymentAmountIsGreaterThanInvoiceAmount()
    {
        var invoice = new Invoice
        {
            Amount = 10000.00m,
            AmountPaid = 0,
            Payments = new List<Payment> { }
        };

        _mockInvoiceRepository
            .Setup(repo => repo.GetInvoice(It.IsAny<string>()))
            .Returns(invoice);

        var payment = new Payment { Reference = "INV001", Amount = 99999.99m };

        var result = _invoiceService.ProcessPayment(payment);

        Assert.That(result, Is.EqualTo(ResponseMessages.Messages["PaymentGreaterThanInvoiceAmount"]));
    }

    [Test]
    public void ProcessPayment_Should_ReturnInvoiceFullyPaid_When_PaymentIsEqualsInvoiceAmount()
    {
        var invoice = new Invoice
        {
            Amount = 10000.00m,
            AmountPaid = 0,
            Payments = new List<Payment> { }
        };

        _mockInvoiceRepository
            .Setup(repo => repo.GetInvoice(It.IsAny<string>()))
            .Returns(invoice);

        var payment = new Payment { Reference = "INV001", Amount = 10000.00m };

        var result = _invoiceService.ProcessPayment(payment);

        Assert.That(result, Is.EqualTo(ResponseMessages.Messages["InvoiceFullyPaid"]));
    }

    [Test]
    public void ProcessPayment_Should_ReturnInvoicePartiallyPaid_When_NoPartialPaymentsExistAndInvoiceIsStillPartiallyPaid()
    {
        var invoice = new Invoice
        {
            Amount = 10000.00m,
            AmountPaid = 0,
            Payments = new List<Payment> { }
        };

        _mockInvoiceRepository
            .Setup(repo => repo.GetInvoice(It.IsAny<string>()))
            .Returns(invoice);

        var payment = new Payment { Reference = "INV001", Amount = 999.99m };

        var result = _invoiceService.ProcessPayment(payment);

        Assert.That(result, Is.EqualTo(ResponseMessages.Messages["InvoicePartiallyPaid"]));
    }

    [Test]
    public void ProcessPayment_Should_ReturnFinalPaymentReceived_When_PartialPaymentsExistAndInvoiceIsFullyPaid()
    {
        var invoice = new Invoice
        {
            Amount = 10000.00m,
            AmountPaid = 2500.00m,
            Payments = new List<Payment> { new Payment { Amount = 2500.00m } }
        };

        _mockInvoiceRepository
            .Setup(repo => repo.GetInvoice(It.IsAny<string>()))
            .Returns(invoice);

        var payment = new Payment { Reference = "INV001", Amount = 7500.00m };

        var result = _invoiceService.ProcessPayment(payment);

        Assert.That(result, Is.EqualTo(ResponseMessages.Messages["FinalPaymentReceived"]));
    }

    [Test]
    public void ProcessPayment_Should_ReturnPartialPaymentReceived_When_PartialPaymentsExistAndInvoiceIsStillPartiallyPaid()
    {
        var invoice = new Invoice
        {
            Amount = 10000.00m,
            AmountPaid = 2500.00m,
            Payments = new List<Payment> { new Payment { Amount = 2500.00m } }
        };

        _mockInvoiceRepository
            .Setup(repo => repo.GetInvoice(It.IsAny<string>()))
            .Returns(invoice);

        var payment = new Payment { Reference = "INV001", Amount = 499.99m };

        var result = _invoiceService.ProcessPayment(payment);

        Assert.That(result, Is.EqualTo(ResponseMessages.Messages["PartialPaymentReceived"]));
    }
}

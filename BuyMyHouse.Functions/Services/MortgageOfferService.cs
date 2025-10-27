using BuyMyHouse.Core.Models;
using Microsoft.Extensions.Logging;

namespace BuyMyHouse.Functions.Services;

public class MortgageOfferService : IMortgageOfferService
{
    private readonly ILogger<MortgageOfferService> _logger;

    public MortgageOfferService(ILogger<MortgageOfferService> logger)
    {
        _logger = logger;
    }

    public bool IsApproved(MortgageApplication application)
    {
        // Business rule: Maximum loan is 5x annual income
        var maxLoan = application.AnnualIncome * 5;
        
        // Business rule: Minimum income required
        var minIncome = 20000m;

        var isApproved = application.AnnualIncome >= minIncome 
                        && application.RequestedAmount <= maxLoan;

        _logger.LogInformation(
            "Mortgage application {ApplicationId} - Requested: {Requested}, Max Allowed: {MaxLoan}, Approved: {IsApproved}",
            application.Id, application.RequestedAmount, maxLoan, isApproved);

        return isApproved;
    }

    public Task<MortgageOffer> GenerateOfferAsync(MortgageApplication application)
    {
        var maxLoan = application.AnnualIncome * 5;
        var approvedAmount = Math.Min(application.RequestedAmount, maxLoan);

        // Calculate interest rate based on loan-to-income ratio
        var loanToIncomeRatio = approvedAmount / application.AnnualIncome;
        var interestRate = CalculateInterestRate(loanToIncomeRatio);

        // Default term: 30 years
        var termInYears = 30;

        // Calculate monthly payment
        var monthlyPayment = CalculateMonthlyPayment(approvedAmount, interestRate, termInYears);

        var offer = new MortgageOffer
        {
            Id = Guid.NewGuid().ToString(),
            ApplicationId = application.Id,
            ApprovedAmount = approvedAmount,
            InterestRate = interestRate,
            TermInYears = termInYears,
            MonthlyPayment = monthlyPayment,
            OfferDate = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddDays(7), // Offer valid for 7 days
            OfferDocumentUrl = string.Empty // Will be set after document generation
        };

        _logger.LogInformation(
            "Generated mortgage offer {OfferId} for application {ApplicationId} - Amount: {Amount}, Rate: {Rate}%",
            offer.Id, application.Id, offer.ApprovedAmount, offer.InterestRate);

        return Task.FromResult(offer);
    }

    private static decimal CalculateInterestRate(decimal loanToIncomeRatio)
    {
        // Base rate: 3.5%
        var baseRate = 3.5m;

        // Add risk premium based on loan-to-income ratio
        if (loanToIncomeRatio > 4.5m)
            return baseRate + 1.5m; // 5.0%
        else if (loanToIncomeRatio > 4.0m)
            return baseRate + 1.0m; // 4.5%
        else if (loanToIncomeRatio > 3.5m)
            return baseRate + 0.5m; // 4.0%
        else
            return baseRate; // 3.5%
    }

    private static decimal CalculateMonthlyPayment(decimal principal, decimal annualInterestRate, int termInYears)
    {
        var monthlyRate = (double)(annualInterestRate / 100 / 12);
        var numberOfPayments = termInYears * 12;

        if (monthlyRate == 0)
            return principal / numberOfPayments;

        var monthlyPayment = (decimal)(
            (double)principal * 
            (monthlyRate * Math.Pow(1 + monthlyRate, numberOfPayments)) /
            (Math.Pow(1 + monthlyRate, numberOfPayments) - 1)
        );

        return Math.Round(monthlyPayment, 2);
    }
}

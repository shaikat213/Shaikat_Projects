
$(document).ready(function () {
    var AutoLoanViewModel = function () {
        var self = this;

        self.IncomeFromSalary = ko.observable();
        self.IncomeFromBusiness = ko.observable();
        self.OtherAllowance = ko.observable();
        self.RentalIncome = ko.observable();
        self.IncomeFromFDR = ko.observable();
        self.IncomeFromFamilyMember = ko.observable();

        self.TotalMonthlyIncome = ko.pureComputed(function () {
            var incomeFromSalary = parseFloat(self.IncomeFromSalary()) || 0;
            var incomeFromBusiness = parseFloat(self.IncomeFromBusiness()) || 0;
            var otherAllowance = parseFloat(self.OtherAllowance()) || 0;
            var rentalIncome = parseFloat(self.RentalIncome()) || 0;
            var incomeFromFDR = parseFloat(self.IncomeFromFDR()) || 0;
            var incomeFromFamilyMember = parseFloat(self.IncomeFromFamilyMember()) || 0;

            return (incomeFromSalary + otherAllowance + incomeFromBusiness + rentalIncome + incomeFromFDR + incomeFromFamilyMember);
        });

        self.HasOwnHouse = ko.observable(false);

        self.MaxConsiderIncomePercentRounded = ko.pureComputed(function () {
            var totalMonthlyIncome = parseFloat(self.TotalMonthlyIncome()) || 0;

            var maxConPercen;

            $.each(dbrIncomeRange, function (index, value) {
                if (value.LoanType === 2 && totalMonthlyIncome >= value.MinIncome && totalMonthlyIncome <= value.MaxIncome) {
                    if (self.HasOwnHouse()) {
                        maxConPercen = value.DBROwnHouse;
                        return;
                    }
                    maxConPercen = value.DBR;
                    return;
                }
            })
            if (maxConPercen > 0)
                return ("% " + maxConPercen);
            return;
        });


        self.MaxConsiderIncomePercent = ko.pureComputed(function () {
            var totalMonthlyIncome = parseFloat(self.TotalMonthlyIncome()) || 0;

            var maxConPercen;
            $.each(dbrIncomeRange, function (index, value) {
                if (value.LoanType === 2 && totalMonthlyIncome >= value.MinIncome && totalMonthlyIncome <= value.MaxIncome) {
                    if (self.HasOwnHouse()) {
                        maxConPercen = (value.DBROwnHouse / 100);
                        return;
                    }
                    maxConPercen = (value.DBR / 100);
                    return;
                }
            })
            return maxConPercen;
        });

        self.CreditCardLimit = ko.observable();

        self.MaxConsiderIncomeforTotalEmi = ko.pureComputed(function () {
            var totalMonthlyIncome = parseFloat(self.TotalMonthlyIncome()) || 0;
            var maxConsiderIncomePercent = parseFloat(self.MaxConsiderIncomePercent()) || 0;
            return (totalMonthlyIncome * maxConsiderIncomePercent);
        });

        self.ExistingEmi = ko.observable();

        self.MaxconsiderablincomeIPDCloanEMI = ko.pureComputed(function () {
            var maxConsiderIncomeforTotalEmi = parseFloat(self.MaxConsiderIncomeforTotalEmi()) || 0;
            var existingEmi = parseFloat(self.ExistingEmi()) || 0;
            var creditCardLimit = (self.CreditCardLimit() ? (parseFloat(self.CreditCardLimit()) * 0.05) : 0);
            return (maxConsiderIncomeforTotalEmi - existingEmi - creditCardLimit);
        });

        self.MaxLoanamountbasedonIncome = ko.pureComputed(function () {
            var maxconsiderablincomeIPDCloanEMI = parseFloat(self.MaxconsiderablincomeIPDCloanEMI()) || 0;
            var emiPerBdtOnelakPMT = parseFloat(self.EmiPerBdtOnelakPMT()) || 0;
            var result = ((maxconsiderablincomeIPDCloanEMI / emiPerBdtOnelakPMT) * 100000).toFixed(2);
            self.RecomendedLoan(result);
            return result;
        });
        self.RecomendedLoan = ko.observable(1700000);
        //self.MaxLoanamountbasedonIncome.subscribe(function () {
        //    
        //})

        self.Tenor = ko.observable(60);
        self.InterestRate = ko.observable(10.9);
        self.EmiPerBdtOnelakPMT = ko.pureComputed(function () {
            var tenor = self.Tenor();
            var interestRate = self.InterestRate();

            var rate = interestRate / 100 / 12;
            return Math.round(((100000)/((1-(1/(Math.pow((1+rate),tenor))))/(rate))));
        });
        
        self.PMTRecomendedLoan = ko.pureComputed(function () {
            var tenor = self.Tenor();
            var interestRate = self.InterestRate();
            var recommendedLoan = self.RecomendedLoan();
            var rate = interestRate / 100 / 12;
            //var denominator = Math.Pow((1 + rate), 12) - 1;
            //return (rate + (rate / denominator)) * ((-1) * 100000);
            return Math.round(((recommendedLoan) / ((1 - (1 / (Math.pow((1 + rate), tenor)))) / (rate))));
        });
        //self.EmiPerBDT =
        self.QuotationPrice = ko.observable(2500000);
        self.LTVPercentage = ko.observable(85);
        self.MaxLoanamountbasedonLTV = ko.pureComputed(function () {
            var quotationPrice = self.QuotationPrice();
            var maxLoanamountPercent = (self.LTVPercentage() ? (parseFloat(self.LTVPercentage()) / 100) : 0);

            return (quotationPrice * maxLoanamountPercent).toFixed(2);
        })

        self.ExistingEmiBind = ko.pureComputed(function () {
            var existingEmi = parseFloat(self.ExistingEmi()) || 0;
            var creditCard = parseFloat(self.CreditCardLimit()*0.05) || 0
            return existingEmi + creditCard;
        });

        self.MonthlyDbtCommitment = ko.pureComputed(function () {
            var pmtRecomendedLoan = parseFloat(self.PMTRecomendedLoan()) || 0;
            var existingEmi = parseFloat(self.ExistingEmiBind()) || 0;
            return (pmtRecomendedLoan + existingEmi).toFixed(2);
        });
        
        self.PercenOfQuotation = ko.pureComputed(function () {
            var recommendedLoan = self.RecomendedLoan();
            var quotationPrice = self.QuotationPrice();
            return ("% " + ((recommendedLoan / quotationPrice) * 100).toFixed(2));
        });

        self.DBR = ko.pureComputed(function () {
            var monthlyDbtCommitment = parseFloat(self.MonthlyDbtCommitment()) || 0;
            var totalmonthlyIncome = parseFloat(self.TotalMonthlyIncome()) || 0;
            if (monthlyDbtCommitment == 0 && totalmonthlyIncome == 0)
                return "0";
            return ("% " + ((monthlyDbtCommitment / totalmonthlyIncome) * 100).toFixed(2));
        });
    };

    var alVm = new AutoLoanViewModel();
    ko.applyBindings(alVm, document.getElementById("autoLoan")[0]);
})
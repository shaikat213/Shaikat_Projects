$(function () {
    $('#txtIncomeAssessmentDate').datetimepicker({ format: 'DD/MM/YYYY' });
});
function CIF_AdditionalIncomeDeclared(data) {
    var self = this;
    self.Id = ko.observable();
    self.CIF_IncomeStatementId = ko.observable(data ? data.CIF_IncomeStatementId : 0);
    self.SourceOfIncome = ko.observable(data ? data.SourceOfIncome : "");
    self.IncomeAmount = ko.observable(data ? data.IncomeAmount : 0);
    self.LoadData = function (data) {
        self.Id(data ? data.Id : '');
        self.CIF_IncomeStatementId(data ? data.CIF_IncomeStatementId : 0);
        self.SourceOfIncome(data ? data.SourceOfIncome : "");
        self.IncomeAmount(data ? data.IncomeAmount : 0);
    }
}

var CIFIncomeStatementViewModel = function () {
    var self = this;
    self.CIF_PersonalId = ko.observable('');
    self.Id = ko.observable('');
    //self.queryString = function getParameterByName(name) {
    //    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    //    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
    //        results = regex.exec(location.search);
    //    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    //}

    //Income Variables

    self.MonthlySalaryDeclared = ko.observable();
    //self.MonthlySalaryAssessed = ko.observable();
    self.MonthlyInterestIncomeDeclared = ko.observable();
    //self.MonthlyInterestIncomeAssessed = ko.observable();
    self.MonthlyRentalIncomeDeclared = ko.observable();
    //self.MonthlyRentalIncomeAssessed = ko.observable();
    self.MonthlyOtherIncomeDeclared = ko.observable();
    //self.MonthlyOtherIncomeAssessed = ko.observable();
    self.OtherIncomeSource = ko.observable();
    self.MonthlyBusinessIncomeDeclared = ko.observable();
    //self.MonthlyBusinessIncomeAssessed = ko.observable();

    //Expense Variables
    self.MonthlyExpenseFoodAndClothingDeclared = ko.observable();
    //self.MonthlyExpenseFoodAndClothingAssessed = ko.observable();
    self.MonthlyExpenseTransportationDeclared = ko.observable();
    //self.MonthlyExpenseTransportationAssessed = ko.observable();
    self.MonthlyExpenseEducationDeclared = ko.observable();
    //self.MonthlyExpenseEducationAssessed = ko.observable();
    self.MonthlyExpenseRentAndUtilityDeclared = ko.observable();
    //self.MonthlyExpenseRentAndUtilityAssessed = ko.observable();
    self.MonthlyExpenseInstallmentsDeclared = ko.observable();
    //self.MonthlyExpenseInstallmentsAssessed = ko.observable();
    self.MonthlyExpenseOthersDeclared = ko.observable();
    //self.MonthlyExpenseOthersAssessed = ko.observable();

    //Loan Installment Variables

    self.MonthlyPayableLoanInstallmentDeclared = ko.observable();
    //self.MonthlyPayableLoanInstallmentAssessed = ko.observable();

    self.IncomeAssessmentDate = ko.observable('');
    self.IncomeAssessmentDateTxt = ko.observable('');
    self.TotalIncome = ko.observable(false);

    // Total Income Computation
    //self.CIF_PersonalId = ko.computed(function () {
    //    var value = 1;
    //    return value;
    //});


    self.MonthlyIncomeTotalDeclaredComputed = ko.pureComputed(function () {

        self.monthlySalaryDeclared = parseFloat(self.MonthlySalaryDeclared()) || 0;
        self.monthlyInterestIncomeDeclared = parseFloat(self.MonthlyInterestIncomeDeclared()) || 0;
        self.monthlyRentalIncomeDeclared = parseFloat(self.MonthlyRentalIncomeDeclared()) || 0;
        self.monthlyOtherIncomeDeclared = parseFloat(self.MonthlyOtherIncomeDeclared()) || 0;
        self.monthlyBusinessIncomeDeclared = parseFloat(self.MonthlyBusinessIncomeDeclared()) || 0;
        var totalAdditionalIncome = 0.0;
        $.each(self.AdditionalIncomeDeclared(),
         function (index, value) {
             totalAdditionalIncome = totalAdditionalIncome + parseFloat(value.IncomeAmount());
         });
        return (self.monthlySalaryDeclared + self.monthlyInterestIncomeDeclared + self.monthlyRentalIncomeDeclared + self.monthlyOtherIncomeDeclared + self.monthlyBusinessIncomeDeclared + totalAdditionalIncome);
    });

    self.MonthlyIncomeTotalDeclared = ko.observable();

    self.EnableFieldsTotalincome = function () {

        if (self.MonthlyIncomeTotalDeclaredComputed() > 0)
            return false;
        else
            return true;
    }



    //self.MonthlyIncomeTotalAssessed = ko.pureComputed(function () {

    //    self.monthlySalaryAssessed = parseFloat(self.MonthlySalaryAssessed()) || 0;
    //    self.monthlyInterestIncomeAssessed = parseFloat(self.MonthlyInterestIncomeAssessed()) || 0;
    //    self.monthlyRentalIncomeAssessed = parseFloat(self.MonthlyRentalIncomeAssessed()) || 0;
    //    self.monthlyOtherIncomeAssessed = parseFloat(self.MonthlyOtherIncomeAssessed()) || 0;
    //    self.monthlyBusinessIncomeAssessed = parseFloat(self.MonthlyBusinessIncomeAssessed()) || 0;

    //    return (self.monthlySalaryAssessed + self.monthlyInterestIncomeAssessed + self.monthlyRentalIncomeAssessed + self.monthlyOtherIncomeAssessed + self.monthlyBusinessIncomeAssessed);
    //});

    // Total Expense Computation

    self.MonthlyExpenseTotalDeclaredComputer = ko.pureComputed(function () {

        self.monthlyExpenseFoodAndClothingDeclared = parseFloat(self.MonthlyExpenseFoodAndClothingDeclared()) || 0;
        self.monthlyExpenseTransportationDeclared = parseFloat(self.MonthlyExpenseTransportationDeclared()) || 0;
        self.monthlyExpenseEducationDeclared = parseFloat(self.MonthlyExpenseEducationDeclared()) || 0;
        self.monthlyExpenseRentAndUtilityDeclared = parseFloat(self.MonthlyExpenseRentAndUtilityDeclared()) || 0;
        self.monthlyExpenseInstallmentsDeclared = parseFloat(self.MonthlyExpenseInstallmentsDeclared()) || 0;
        self.monthlyExpenseOthersDeclared = parseFloat(self.MonthlyExpenseOthersDeclared()) || 0;

        return (self.monthlyExpenseFoodAndClothingDeclared
            + self.monthlyExpenseTransportationDeclared
            + self.monthlyExpenseEducationDeclared
            + self.monthlyExpenseRentAndUtilityDeclared
            + self.monthlyExpenseInstallmentsDeclared
            + self.monthlyExpenseOthersDeclared);
    });

    self.MonthlyExpenseTotalDeclared = ko.observable();


    self.EnableFieldsTotalexpense = function () {

        if (self.MonthlyExpenseTotalDeclaredComputer() > 0)
            return false;
        else
            return true;
    }
    self.IsCreateNew = ko.observable(false);
    self.IsNew = function() {
        self.IsCreateNew(true);
        self.Submit();
    }
    self.AdditionalIncomeDeclared = ko.observableArray([]);
    self.AddAdditionalIncome = function () {
        var aDetail = new CIF_AdditionalIncomeDeclared();
        self.AdditionalIncomeDeclared.push(aDetail);
    }
    self.RemovedAdditionalIncomeDeclared = ko.observableArray([]);
    self.RemoveAdditionalIncome = function (line) {
        if (line.Id() > 0)
            self.RemovedAdditionalIncomeDeclared.push(line.Id());
        self.AdditionalIncomeDeclared.remove(line);
    };
    //self.MonthlyExpenseTotalAssessed = ko.pureComputed(function () {

    //    self.monthlyExpenseFoodAndClothingAssessed = parseFloat(self.MonthlyExpenseFoodAndClothingAssessed()) || 0;
    //    self.monthlyExpenseTransportationAssessed = parseFloat(self.MonthlyExpenseTransportationAssessed()) || 0;
    //    self.monthlyExpenseEducationAssessed = parseFloat(self.MonthlyExpenseEducationAssessed()) || 0;
    //    self.monthlyExpenseRentAndUtilityAssessed = parseFloat(self.MonthlyExpenseRentAndUtilityAssessed()) || 0;
    //    self.monthlyExpenseInstallmentsAssessed = parseFloat(self.MonthlyExpenseInstallmentsAssessed()) || 0;
    //    self.monthlyExpenseOthersAssessed = parseFloat(self.MonthlyExpenseOthersAssessed()) || 0;

    //    return (self.monthlyExpenseFoodAndClothingAssessed
    //        + self.monthlyExpenseTransportationAssessed
    //        + self.monthlyExpenseEducationAssessed
    //        + self.monthlyExpenseRentAndUtilityAssessed
    //        + self.monthlyExpenseInstallmentsAssessed
    //        + self.monthlyExpenseOthersAssessed);
    //});
    self.LoadIncomeStatement = function () {
        if (self.CIF_PersonalId() > 0) {
            $.getJSON("/IPDC/CIF/LoadIncomeStatement/?cifPersonId=" + self.CIF_PersonalId(),
                null,
                function (data) {
                    self.CIF_PersonalId(data.CIF_PersonalId);
                    self.Id(data.Id);
                    self.MonthlySalaryDeclared(data.MonthlySalaryDeclared);
                    //self.MonthlySalaryAssessed(data.MonthlySalaryAssessed);
                    self.MonthlyInterestIncomeDeclared(data.MonthlyInterestIncomeDeclared);
                    //self.MonthlyInterestIncomeAssessed(data.MonthlyInterestIncomeAssessed);
                    self.MonthlyRentalIncomeDeclared(data.MonthlyRentalIncomeDeclared);
                    //self.MonthlyRentalIncomeAssessed(data.MonthlyRentalIncomeAssessed);
                    self.MonthlyOtherIncomeDeclared(data.MonthlyOtherIncomeDeclared);
                    self.OtherIncomeSource(data.OtherIncomeSource);
                    self.MonthlyBusinessIncomeDeclared(data.MonthlyBusinessIncomeDeclared);

                    self.MonthlyIncomeTotalDeclared(data.MonthlyIncomeTotalDeclared);
                    //self.MonthlyBusinessIncomeAssessed(data.MonthlyBusinessIncomeAssessed);

                    //self.MonthlyIncomeTotalDeclared(data.MonthlyIncomeTotalDeclared);
                    //self.MonthlyIncomeTotalAssessed(data.MonthlyIncomeTotalAssessed);

                    //Expense Variables
                    self.MonthlyExpenseFoodAndClothingDeclared(data.MonthlyExpenseFoodAndClothingDeclared);
                    //self.MonthlyExpenseFoodAndClothingAssessed(data.MonthlyExpenseFoodAndClothingAssessed);
                    self.MonthlyExpenseTransportationDeclared(data.MonthlyExpenseTransportationDeclared);
                    //self.MonthlyExpenseTransportationAssessed(data.MonthlyExpenseTransportationAssessed);
                    self.MonthlyExpenseEducationDeclared(data.MonthlyExpenseEducationDeclared);
                    //self.MonthlyExpenseEducationAssessed(data.MonthlyExpenseEducationAssessed);
                    self.MonthlyExpenseRentAndUtilityDeclared(data.MonthlyExpenseRentAndUtilityDeclared);
                    //self.MonthlyExpenseRentAndUtilityAssessed(data.MonthlyExpenseRentAndUtilityAssessed);
                    self.MonthlyExpenseInstallmentsDeclared(data.MonthlyExpenseInstallmentsDeclared);
                    //self.MonthlyExpenseInstallmentsAssessed(data.MonthlyExpenseInstallmentsAssessed);
                    self.MonthlyExpenseOthersDeclared(data.MonthlyExpenseOthersDeclared);
                    //self.MonthlyExpenseOthersAssessed(data.MonthlyExpenseOthersAssessed);
                    self.MonthlyExpenseTotalDeclared(data.MonthlyExpenseTotalDeclared);

                    //self.MonthlyExpenseTotalDeclared(data.MonthlyExpenseTotalDeclared);
                    //self.MonthlyExpenseTotalAssessed(data.MonthlyExpenseTotalAssessed);

                    //Loan Installment Variables
                    self.MonthlyPayableLoanInstallmentDeclared(data.MonthlyPayableLoanInstallmentDeclared);
                  
                    self.IncomeAssessmentDateTxt(data.IncomeAssessmentDateTxt);
                    $.each(data.MonthlyOtherIncomesDeclared,
                            function (index, value) {
                                var aDetail = new CIF_AdditionalIncomeDeclared();
                                if (typeof (value) != 'undefined') {
                                    aDetail.LoadData(value);
                                    self.AdditionalIncomeDeclared.push(aDetail);
                                }

                            });
                });

        }
    }
    //Add New Data
    self.Submit = function () {
        if (self.IsCreateNew() === true) {
            self.Id(0);
        }
        //self.CIFPersonalId();
        if (self.MonthlyIncomeTotalDeclaredComputed() > 0) {
            self.MonthlyIncomeTotalDeclared(self.MonthlyIncomeTotalDeclaredComputed());
        }

        if (self.MonthlyExpenseTotalDeclaredComputer() > 0) {
            self.MonthlyExpenseTotalDeclared(self.MonthlyExpenseTotalDeclaredComputer());
        }
        self.IncomeAssessmentDateTxt($('#txtIncomeAssessmentDate').val());
        var cifIncomeStatementInfo;
        var details = ko.observableArray([]);
        $.each(self.AdditionalIncomeDeclared(),
            function (index, value) {
                details.push({
                    Id: value.Id(),
                    CIF_IncomeStatementId: value.CIF_IncomeStatementId(),
                    SourceOfIncome: value.SourceOfIncome(),
                    IncomeAmount: value.IncomeAmount()
                });
            });
        cifIncomeStatementInfo = {
            Id: self.Id(),
            CIF_PersonalId: self.CIF_PersonalId(),
            MonthlySalaryDeclared: self.MonthlySalaryDeclared(),
            MonthlyInterestIncomeDeclared: self.MonthlyInterestIncomeDeclared(),
            MonthlyRentalIncomeDeclared: self.MonthlyRentalIncomeDeclared(),
            MonthlyOtherIncomeDeclared: self.MonthlyOtherIncomeDeclared(),
            OtherIncomeSource: self.OtherIncomeSource(),
            MonthlyBusinessIncomeDeclared: self.MonthlyBusinessIncomeDeclared(),
            MonthlyIncomeTotalDeclared: self.MonthlyIncomeTotalDeclared(),
            MonthlyExpenseFoodAndClothingDeclared: self.MonthlyExpenseFoodAndClothingDeclared(),
            MonthlyExpenseTransportationDeclared: self.MonthlyExpenseTransportationDeclared(),
            MonthlyExpenseEducationDeclared: self.MonthlyExpenseEducationDeclared(),
            MonthlyExpenseRentAndUtilityDeclared: self.MonthlyExpenseRentAndUtilityDeclared(),
            MonthlyExpenseInstallmentsDeclared: self.MonthlyExpenseInstallmentsDeclared(),
            MonthlyExpenseOthersDeclared: self.MonthlyExpenseOthersDeclared(),
            MonthlyExpenseTotalDeclared: self.MonthlyExpenseTotalDeclared(),
            MonthlyPayableLoanInstallmentDeclared: self.MonthlyPayableLoanInstallmentDeclared(),
            IncomeAssessmentDateTxt: self.IncomeAssessmentDateTxt(),
            MonthlyOtherIncomesDeclared: details,
            RemovedAdditionalIncomeDeclared: self.RemovedAdditionalIncomeDeclared()
        };

        $.ajax({
            url: '/IPDC/CIF/SaveCIFIncomeStatement',
            //cache: false,
            type: 'POST',
            contentType: 'application/json',
            data: ko.toJSON(cifIncomeStatementInfo),
            success: function (data) {
                $('#ISsuccessModal').modal('show');
                $('#ISsuccessModalText').text(data.Message);
                //self.Reset();
            },
            error: function (error) {
                alert(error.status + "<1--and--> " + error.statusText);
            }
        });
    }
    self.Reset = function () {
        self.Id("");
        self.MonthlySalaryDeclared("");
        //self.MonthlySalaryAssessed("");
        self.MonthlyInterestIncomeDeclared("");
        //self.MonthlyInterestIncomeAssessed("");
        self.MonthlyRentalIncomeDeclared("");
        //self.MonthlyRentalIncomeAssessed("");
        self.MonthlyOtherIncomeDeclared("");
        //self.MonthlyOtherIncomeAssessed("");
        self.OtherIncomeSource("");
        self.MonthlyBusinessIncomeDeclared("");
        //self.MonthlyBusinessIncomeAssessed("");

        //Expense Variables
        self.MonthlyExpenseFoodAndClothingDeclared("");
        ////////self.MonthlyExpenseFoodAndClothingAssessed("");
        self.MonthlyExpenseTransportationDeclared("");
        //////self.MonthlyExpenseTransportationAssessed("");
        self.MonthlyExpenseEducationDeclared("");
        //self.MonthlyExpenseEducationAssessed("");
        self.MonthlyExpenseRentAndUtilityDeclared("");
        ////self.MonthlyExpenseRentAndUtilityAssessed("");
        self.MonthlyExpenseInstallmentsDeclared("");
        //self.MonthlyExpenseInstallmentsAssessed("");
        self.MonthlyExpenseOthersDeclared("");
        //self.MonthlyExpenseOthersAssessed("");

        //Loan Installment Variables
        self.MonthlyPayableLoanInstallmentDeclared("");
        //self.MonthlyPayableLoanInstallmentAssessed("");

    }
};

//var alVm = new CIFIncomeStatementViewModel();
////var qValue = alVm.queryString("CIFPersonalId");
////alVm.CIFPersonalId(qValue);
//ko.applyBindings(alVm, document.getElementById("cifIncomeStatement")[0]);

//})
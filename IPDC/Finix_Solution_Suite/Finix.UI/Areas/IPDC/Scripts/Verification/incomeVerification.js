$(function () {
    $('#txtIncomeAssessmentDate').datetimepicker({ format: 'DD/MM/YYYY' });
    $('#VarificationDateId').datetimepicker({ format: 'DD/MM/YYYY' });
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
//IncomeVerificationAdditionalIncomeAssessed
function IncomeVerificationAdditionalIncomeAssessed(data) {
    var self = this;
 
    self.Id = ko.observable();
    self.IncomeVerificationId = ko.observable(data ? data.IncomeVerificationId : 0);
    self.AdditionalIncomeDeclaredId = ko.observable(data && data.AdditionalIncomeDeclaredId ? data.AdditionalIncomeDeclaredId : null);
    self.SourceOfIncome = ko.observable(data ? data.SourceOfIncome : "");
    self.IncomeAmount = ko.observable(data ? data.IncomeAmount : 0);
    self.IsConsidered = ko.observable(data ? data.IsConsidered :true);
    self.LoadData = function (data) {
        self.Id(data ? data.Id : '');
        self.IncomeVerificationId(data && data.IncomeVerificationId ? data.IncomeVerificationId : '');
        self.AdditionalIncomeDeclaredId(data && data.AdditionalIncomeDeclaredId ? data.AdditionalIncomeDeclaredId : null);
        self.SourceOfIncome(data ? data.SourceOfIncome : "");
        self.IncomeAmount(data ? data.IncomeAmount : 0);
        self.IsConsidered(data ? data.IsConsidered :true);
    }
}
$(document).ready(function () {
    var CIFIncomeStatementVerificationVM = function () {
        var self = this;
        self.CIF_PersonalId = ko.observable('');
        self.CifNo = ko.observable();
        self.CifName = ko.observable();
        self.Id = ko.observable('');
        self.ApplicationId = ko.observable();
        var currentDate = (new Date()).toISOString().split('T')[0];
        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }

        //Income Variables
        self.VerificationDateText = ko.observable();
        self.VerificationDate = ko.observable();
        self.VerificationStatus = ko.observable('0');
        self.VerificationState = ko.observable('0');
        self.MonthlySalaryDeclared = ko.observable();
        self.MonthlySalaryAssessed = ko.observable();
        self.MonthlyInterestIncomeDeclared = ko.observable();
        self.MonthlyInterestIncomeAssessed = ko.observable();
        self.MonthlyRentalIncomeDeclared = ko.observable();
        self.MonthlyRentalIncomeAssessed = ko.observable();
        self.MonthlyOtherIncomeDeclared = ko.observable();
        self.MonthlyOtherIncomeAssessed = ko.observable();
        self.OtherIncomeSource = ko.observable();
        self.MonthlyBusinessIncomeDeclared = ko.observable();
        self.MonthlyBusinessIncomeAssessed = ko.observable();
        //Expense Variables
        self.MonthlyExpenseFoodAndClothingDeclared = ko.observable();
        self.MonthlyExpenseFoodAndClothingAssessed = ko.observable();
        self.MonthlyExpenseTransportationDeclared = ko.observable();
        self.MonthlyExpenseTransportationAssessed = ko.observable();
        self.MonthlyExpenseEducationDeclared = ko.observable();
        self.MonthlyExpenseEducationAssessed = ko.observable();
        self.MonthlyExpenseRentAndUtilityDeclared = ko.observable();
        self.MonthlyExpenseRentAndUtilityAssessed = ko.observable();
        self.MonthlyExpenseInstallmentsDeclared = ko.observable();
        self.MonthlyExpenseInstallmentsAssessed = ko.observable();
        self.MonthlyExpenseOthersDeclared = ko.observable();
        self.MonthlyExpenseOthersAssessed = ko.observable();
        self.AdditionalIncomeDeclared = ko.observableArray([]);

        self.IncomeHistory = function () {
            var parameters = [{
                Name: 'AppId',
                Value: self.ApplicationId()
            }, {
                Name: 'CIFPId',
                Value: self.CIF_PersonalId()
            }, {
                Name: 'Id',
                Value: self.Id()
            }];
            var menuInfo = {
                //Id: urlId++,
                Menu: 'Income Verification History',
                Url: '/IPDC/Verification/IncomeVerificationHistory',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        //Loan Installment Variables
        ///////////////////////////////////////Assesed//////////////////////////////
        self.VerificationPersonRole = ko.observable();
        self.VerificationPersonRoleFrom = ko.observable();
        self.MonthlySalaryAssessedConsidered = ko.observable(true);
        self.MonthlyInterestIncomeAssessedConsidered = ko.observable(true);
        self.MonthlyRentalIncomeAssessedConsidered = ko.observable(true);
        self.MonthlyBusinessIncomeAssessedConsidered = ko.observable(true);
        self.MonthlyIncomeTotalAssessedConsidered = ko.observable(true);
        self.ISId = ko.observable();

        self.MonthlyPayableLoanInstallmentDeclared = ko.observable();
        self.MonthlyPayableLoanInstallmentAssessed = ko.observable();

        self.IncomeAssessmentDate = ko.observable('');
        self.IncomeAssessmentDateTxt = ko.observable(moment(currentDate).format("DD/MM/YYYY"));
        self.TotalIncome = ko.observable(false);

        self.AssesedAdditionalIncome = ko.observableArray([]);
        self.AddAdditionalIncome = function () {
            var aDetail = new IncomeVerificationAdditionalIncomeAssessed();
            self.AssesedAdditionalIncome.push(aDetail);
        }
        self.RemovedAdditionalIncomeAssesed = ko.observableArray([]);
        self.RemoveAdditionalIncome = function (line) {
            if (line.Id() > 0)
                self.RemovedAdditionalIncomeAssesed.push(line.Id());
            self.AssesedAdditionalIncome.remove(line);
        };

        self.MonthlyIncomeTotalDeclared = ko.observable();

        self.EnableFieldsTotalincome = function () {

            if (self.MonthlyIncomeTotalDeclaredComputed() > 0)
                return false;
            else
                return true;
        }

        self.IsCreateNew = ko.observable(false);
        self.IsNew = function () {
            self.IsCreateNew(true);
            self.Submit();
        }
        self.MonthlyIncomeTotalAssessed = ko.observable();
        self.MonthlyIncomeTotalAssessedComputed = ko.pureComputed(function () {

            self.monthlySalaryAssessed = parseFloat(self.MonthlySalaryAssessed()) || 0;
            self.monthlyInterestIncomeAssessed = parseFloat(self.MonthlyInterestIncomeAssessed()) || 0;
            self.monthlyRentalIncomeAssessed = parseFloat(self.MonthlyRentalIncomeAssessed()) || 0;
            self.monthlyOtherIncomeAssessed = parseFloat(self.MonthlyOtherIncomeAssessed()) || 0;
            self.monthlyBusinessIncomeAssessed = parseFloat(self.MonthlyBusinessIncomeAssessed()) || 0;
            var totalAdditionalIncome = 0.0;
            $.each(self.AssesedAdditionalIncome(),
             function (index, value) {
                 totalAdditionalIncome = totalAdditionalIncome + parseFloat(value.IncomeAmount());
             });
            return (self.monthlySalaryAssessed + self.monthlyInterestIncomeAssessed + self.monthlyRentalIncomeAssessed + self.monthlyOtherIncomeAssessed + self.monthlyBusinessIncomeAssessed + totalAdditionalIncome);
        });

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

        self.MonthlyExpenseTotalAssessed = ko.pureComputed(function () {

            self.monthlyExpenseFoodAndClothingAssessed = parseFloat(self.MonthlyExpenseFoodAndClothingAssessed()) || 0;
            self.monthlyExpenseTransportationAssessed = parseFloat(self.MonthlyExpenseTransportationAssessed()) || 0;
            self.monthlyExpenseEducationAssessed = parseFloat(self.MonthlyExpenseEducationAssessed()) || 0;
            self.monthlyExpenseRentAndUtilityAssessed = parseFloat(self.MonthlyExpenseRentAndUtilityAssessed()) || 0;
            self.monthlyExpenseInstallmentsAssessed = parseFloat(self.MonthlyExpenseInstallmentsAssessed()) || 0;
            self.monthlyExpenseOthersAssessed = parseFloat(self.MonthlyExpenseOthersAssessed()) || 0;

            return (self.monthlyExpenseFoodAndClothingAssessed
                + self.monthlyExpenseTransportationAssessed
                + self.monthlyExpenseEducationAssessed
                + self.monthlyExpenseRentAndUtilityAssessed
                + self.monthlyExpenseInstallmentsAssessed
                + self.monthlyExpenseOthersAssessed);
        });
        self.LoadIncomeStatement = function () {
            
            var url = "/IPDC/Verification/LoadIncomeStatementVerification?";
            if (self.ApplicationId() > 0)
                url += "AppId=" + self.ApplicationId() + "&";

            if (self.CIF_PersonalId() > 0) {
                url += "CIFPId=" + self.CIF_PersonalId() + "&";
            }
            if (self.Id() > 0)
                url += "Id=" + self.Id();
            $.getJSON(url, null,
            function (data) {
                self.CIF_PersonalId(data.IncomeStatement.CIF_PersonalId);
                self.CifNo(data.CifNo);
                self.CifName(data.CifName);
                self.Id(data.Id);
                ////////////////////////////////////Declared//////////////////////////////////////////////////////////////
                self.MonthlySalaryDeclared(data.IncomeStatement.MonthlySalaryDeclared);
                self.ISId(data.IncomeStatement.Id);
                self.MonthlySalaryAssessed(data.MonthlySalaryAssessed);
                self.MonthlyInterestIncomeDeclared(data.IncomeStatement.MonthlyInterestIncomeDeclared);
                self.MonthlyInterestIncomeAssessed(data.MonthlyInterestIncomeAssessed);
                self.MonthlyRentalIncomeDeclared(data.IncomeStatement.MonthlyRentalIncomeDeclared);
                self.MonthlyRentalIncomeAssessed(data.MonthlyRentalIncomeAssessed);
                self.MonthlyOtherIncomeDeclared(data.IncomeStatement.MonthlyOtherIncomeDeclared);
                self.OtherIncomeSource(data.IncomeStatement.OtherIncomeSource);
                self.MonthlyBusinessIncomeDeclared(data.IncomeStatement.MonthlyBusinessIncomeDeclared);
                self.MonthlyIncomeTotalDeclared(data.IncomeStatement.MonthlyIncomeTotalDeclared);
                self.MonthlyExpenseFoodAndClothingDeclared(data.IncomeStatement.MonthlyExpenseFoodAndClothingDeclared);
                self.MonthlyExpenseFoodAndClothingAssessed(data.MonthlyExpenseFoodAndClothingAssessed);
                self.MonthlyExpenseTransportationDeclared(data.IncomeStatement.MonthlyExpenseTransportationDeclared);
                self.MonthlyExpenseTransportationAssessed(data.MonthlyExpenseTransportationAssessed);
                self.MonthlyExpenseEducationDeclared(data.IncomeStatement.MonthlyExpenseEducationDeclared);
                self.MonthlyExpenseEducationAssessed(data.MonthlyExpenseEducationAssessed);
                self.MonthlyExpenseRentAndUtilityDeclared(data.IncomeStatement.MonthlyExpenseRentAndUtilityDeclared);
                self.MonthlyExpenseRentAndUtilityAssessed(data.MonthlyExpenseRentAndUtilityAssessed);
                self.MonthlyExpenseInstallmentsDeclared(data.IncomeStatement.MonthlyExpenseInstallmentsDeclared);
                self.MonthlyExpenseInstallmentsAssessed(data.MonthlyExpenseInstallmentsAssessed);
                self.MonthlyExpenseOthersDeclared(data.IncomeStatement.MonthlyExpenseOthersDeclared);
                self.MonthlyExpenseOthersAssessed(data.MonthlyExpenseOthersAssessed);
                self.MonthlyExpenseTotalDeclared(data.IncomeStatement.MonthlyExpenseTotalDeclared);
                self.MonthlySalaryAssessedConsidered(data.MonthlySalaryAssessedConsidered);
                self.MonthlyInterestIncomeAssessedConsidered(data.MonthlyInterestIncomeAssessedConsidered);
                self.MonthlyRentalIncomeAssessedConsidered(data.MonthlyRentalIncomeAssessedConsidered);
                self.MonthlyBusinessIncomeAssessedConsidered(data.MonthlyBusinessIncomeAssessedConsidered);
                self.MonthlyIncomeTotalAssessedConsidered(data.MonthlyIncomeTotalAssessedConsidered);
                self.MonthlyOtherIncomeAssessed(data.MonthlyOtherIncomeAssessed);
                self.MonthlyBusinessIncomeAssessed(data.MonthlyBusinessIncomeAssessed);
                $.each(data.IncomeStatement.MonthlyOtherIncomesDeclared,
                  function (index, value) {
                      var aDetail = new CIF_AdditionalIncomeDeclared();
                      
                      if (typeof (value) !== 'undefined') {
                          aDetail.LoadData(value);
                          self.AdditionalIncomeDeclared.push(aDetail);
                      }

                  });
                ///////////////////////////////////////////Assesed//////////////////////////////////////////////////////////////
                $.each(data.MonthlyOtherIncomesAssessed,
              function (index, value) {
                  var aDetail = new IncomeVerificationAdditionalIncomeAssessed();
                  
                  if (typeof (value) !== 'undefined') {
                      aDetail.LoadData(value);
                      self.AssesedAdditionalIncome.push(aDetail);
                  }

              });


                //Loan Installment Variables
                self.MonthlyPayableLoanInstallmentDeclared(data.MonthlyPayableLoanInstallmentDeclared);
                self.MonthlyPayableLoanInstallmentAssessed(data.MonthlyPayableLoanInstallmentAssessed);
                self.VerificationState(data.VerificationState.toString());
                
                if(data.Id > 0)
                    self.VerificationPersonRole(data.VerificationPersonRole + '');
                //RefId: receiveDetail.Id
                self.IncomeAssessmentDateTxt(data.IncomeAssessmentDateTxt);

            });


        }
        self.Details = function (data) {
            var parameters = [{
                Name: 'AppId',
                Value: self.ApplicationId()
            },
            {
                Name: 'CIFPId',
                Value: self.CIF_PersonalId()
            }];
            var menuInfo = {
                Id: 93,
                Menu: 'Verification',
                Url: '/IPDC/Verification/IncomeVerificationHistory',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
        //Add New Data
        self.Submit = function () {
            //self.CIFPersonalId();
            if (self.IsCreateNew() === true) {
                self.Id('');
                self.VerificationPersonRole(self.VerificationPersonRoleFrom());
            }
            if (self.MonthlyIncomeTotalAssessedComputed() > 0) {
                self.MonthlyIncomeTotalAssessed(self.MonthlyIncomeTotalAssessedComputed());
            }

            if (self.MonthlyExpenseTotalDeclaredComputer() > 0) {
                self.MonthlyExpenseTotalDeclared(self.MonthlyExpenseTotalDeclaredComputer());
            }
            self.IncomeAssessmentDateTxt($('#txtIncomeAssessmentDate').val());

            var details = ko.observableArray([]);
            $.each(self.AssesedAdditionalIncome(),
                function (index, value) {
                    details.push({
                        Id: value.Id(),
                        IncomeVerificationId: value.IncomeVerificationId(),
                        SourceOfIncome: value.SourceOfIncome(),
                        IncomeAmount: value.IncomeAmount(),
                        AdditionalIncomeDeclaredId: value.AdditionalIncomeDeclaredId(),
                        IsConsidered :value.IsConsidered()
                    });
                });
            var cifIncomeStatementInfo;
            cifIncomeStatementInfo = {
                Id: self.Id(),
                ISId: self.ISId(),
                ApplicationId: self.ApplicationId(),
                CifId: self.CIF_PersonalId(),
                CifNo: self.CifNo(),
                CifName: self.CifName(),
                VerificationPersonRole :self.VerificationPersonRole(),
                MonthlySalaryAssessed :self.MonthlySalaryAssessed(),
                MonthlyInterestIncomeAssessed:self.MonthlyInterestIncomeAssessed(),
                MonthlyRentalIncomeAssessed:self.MonthlyRentalIncomeAssessed(),
                MonthlyOtherIncomeAssessed:self.MonthlyOtherIncomeAssessed(),
                MonthlyBusinessIncomeAssessed:self.MonthlyBusinessIncomeAssessed(),
                MonthlyIncomeTotalAssessed:self.MonthlyIncomeTotalAssessed(),
                MonthlyOtherIncomesAssessed : details,
                //expense fields
                MonthlyExpenseFoodAndClothingAssessed:self.MonthlyExpenseFoodAndClothingAssessed(),
                MonthlyExpenseTransportationAssessed:self.MonthlyExpenseTransportationAssessed(),
                MonthlyExpenseEducationAssessed:self.MonthlyExpenseEducationAssessed(),
                MonthlyExpenseRentAndUtilityAssessed:self.MonthlyExpenseRentAndUtilityAssessed(),
                MonthlyExpenseInstallmentsAssessed:self.MonthlyExpenseInstallmentsAssessed(),
                MonthlyExpenseOthersAssessed:self.MonthlyExpenseOthersAssessed(),
                MonthlyExpenseTotalAssessed:self.MonthlyExpenseTotalAssessed(),
                //loan installment
                MonthlyPayableLoanInstallmentAssessed:self.MonthlyPayableLoanInstallmentAssessed(),
                IncomeAssessmentDate:self.IncomeAssessmentDate(),
                IncomeAssessmentDateTxt:self.IncomeAssessmentDateTxt(),
                VerificationState:self.VerificationState(),
                //IncomeAssessmentDateTxt: self.IncomeAssessmentDateTxt(),
                MonthlySalaryAssessedConsidered:self.MonthlySalaryAssessedConsidered(),
                MonthlyInterestIncomeAssessedConsidered:self.MonthlyInterestIncomeAssessedConsidered(),
                MonthlyRentalIncomeAssessedConsidered:self.MonthlyRentalIncomeAssessedConsidered(),
                MonthlyBusinessIncomeAssessedConsidered:self.MonthlyBusinessIncomeAssessedConsidered(),
                MonthlyIncomeTotalAssessedConsidered:self.MonthlyIncomeTotalAssessedConsidered()
               
            };

            $.ajax({
                url: '/IPDC/Verification/SaveIncomeVerification',
                //cache: false,
                type: 'POST',
                contentType: 'application/json',
                data: ko.toJSON(cifIncomeStatementInfo),
                success: function(data) {
                    $('#ISsuccessModal').modal('show');
                    $('#ISsuccessModalText').text(data.Message);
                    //self.Reset();
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.Initialize = function () {
            self.LoadIncomeStatement();
        }
        self.Reset = function () {
            self.Id("");
            //self.MonthlySalaryDeclared("");
            self.MonthlySalaryAssessed("");
            //self.MonthlyInterestIncomeDeclared("");
            self.MonthlyInterestIncomeAssessed("");
            //self.MonthlyRentalIncomeDeclared("");
            self.MonthlyRentalIncomeAssessed("");
            //self.MonthlyOtherIncomeDeclared("");
            self.MonthlyOtherIncomeAssessed("");
            //self.OtherIncomeSource("");
            //self.MonthlyBusinessIncomeDeclared("");
            self.MonthlyBusinessIncomeAssessed("");

            //Expense Variables
            //self.MonthlyExpenseFoodAndClothingDeclared("");
            self.MonthlyExpenseFoodAndClothingAssessed("");
            //self.MonthlyExpenseTransportationDeclared("");
            self.MonthlyExpenseTransportationAssessed("");
            //self.MonthlyExpenseEducationDeclared("");
            self.MonthlyExpenseEducationAssessed("");
            //self.MonthlyExpenseRentAndUtilityDeclared("");
            self.MonthlyExpenseRentAndUtilityAssessed("");
            //self.MonthlyExpenseInstallmentsDeclared("");
            self.MonthlyExpenseInstallmentsAssessed("");
            //self.MonthlyExpenseOthersDeclared("");
            self.MonthlyExpenseOthersAssessed("");

            //Loan Installment Variables
            //self.MonthlyPayableLoanInstallmentDeclared("");
            self.MonthlyPayableLoanInstallmentAssessed("");

        }
    };

    var isvvm = new CIFIncomeStatementVerificationVM();
    var qValue = isvvm.queryString("CIFPId");
    isvvm.CIF_PersonalId(qValue);
    isvvm.ApplicationId(isvvm.queryString("AppId"));
    isvvm.Id(isvvm.queryString("Id"));
    //console.log('Id' + isvvm.Id());
    isvvm.VerificationPersonRole(isvvm.queryString("VerificationAs"));
    isvvm.VerificationPersonRoleFrom(isvvm.VerificationPersonRole());
    isvvm.Initialize();
    ko.applyBindings(isvvm, document.getElementById("cifIncomeStatementVerification")[0]);

})
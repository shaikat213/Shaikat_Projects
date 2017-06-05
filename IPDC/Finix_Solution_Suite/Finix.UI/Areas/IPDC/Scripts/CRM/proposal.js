

function StressRateTest(interestRate, recomendedLoan, term, dbr, flatDbr, proposedLoanEmi, factor, liabilityTotalEMI, totalCreditCard, ConsideredIncome, rentalIncome, id, expectedRentPercentage) {
    
    var interestAndFactor = parseFloat(interestRate ? interestRate : 0) + parseFloat(factor ? factor : 0);
    var divisor = interestAndFactor / 100 / 12;
    var EMI = (Math.round(((recomendedLoan) / ((1 - (1 / (Math.pow((1 + divisor), term)))) / (divisor)))));
   
    var Increase = EMI - proposedLoanEmi;
    var perOfCreditCard = totalCreditCard * .05;
    var dbrAmount = 0;
    var dbrPurchaseAmount = 0;
    var rentalIncomeAmount = 0;
    if (ConsideredIncome > 0) {
        var total = (EMI + liabilityTotalEMI + perOfCreditCard);
        rentalIncomeAmount = (parseFloat(rentalIncome * expectedRentPercentage) / 100);      
        var totalIncome = parseFloat(ConsideredIncome ? ConsideredIncome : 0) + parseFloat(rentalIncomeAmount ? rentalIncomeAmount : 0);
        dbrAmount = (total / ConsideredIncome) * 100;
        dbrPurchaseAmount = (total / totalIncome) * 100;// dbr + ((rentalIncome * expectedRentPercentage) / 100);
    }
    return { Id: id, InterestRate: interestAndFactor, EMI: EMI, Increase: Increase, DBR: dbrAmount.toFixed(2), DBRFlatPurchase: dbrPurchaseAmount.toFixed(2) }
};

$(document).ready(function () {
    $(document).on('click', '.panel-heading.address', function (e) {
        if ($(this).data('toggle') == 'collapse') {
            
            $(this).next().toggle();
        }

        e.preventDefault();
    });
    $(function () {
        //$('#ApplicationDateId').datetimepicker({ format: 'DD/MM/YYYY' });
        $('#ExpiryDate').datetimepicker({ format: 'DD/MM/YYYY' });
        $('#FirstDisbursementExpDateTxt').datetimepicker({ format: 'DD/MM/YYYY' });
        $('#SubmissionDeadlineId').datetimepicker({ format: 'DD/MM/YYYY' });

    });

    ko.validation.init({
        errorElementClass: 'has-error',
        errorMessageClass: 'help-block',
        decorateInputElement: true,
        grouping: { deep: true, observable: true }
    });

    function clientProfile() {
        var self = this;
        self.Id = ko.observable();
        self.ProposalId = ko.observable();
        self.CIFPId = ko.observable();
        self.CIFOId = ko.observable();
        self.ApplicantRole = ko.observable();
        self.ApplicantRoleName = ko.observable();
        self.CIFNo = ko.observable();
        self.Name = ko.observable();
        self.NID = ko.observable();
        self.RelationshipWithApplicant = ko.observable();
        self.RelationshipWithApplicantName = ko.observable();
        self.DateOfBirth = ko.observable();
        self.Age = ko.observable();
        //self.AgeFormatted = ko.pureComputed({
        //    read: function () {

        //    },
        //    write: function () {
        //    }
        //});
        self.AgeInMonths = ko.pureComputed({
            read: function () {
                return (parseInt(self.Age()) % 12).toFixed(0);
            },
            write: function (value) {
                self.Age(parseInt(self.AgeInYears() * 12) + parseInt(value));
            }
        });
        self.AgeInYears = ko.pureComputed({
            read: function () {
                return Math.trunc(parseInt(self.Age()) / 12);
            },
            write: function (value) {
                self.Age(parseInt(self.AgeInYears() * 12) + parseInt(value));
            }
        });
        self.DateOfBirthTxt = ko.observable();
        self.AcademicQualification = ko.observable();
        self.ProfessionName = ko.observable();
        self.Designation = ko.observable();
        self.OrganizationName = ko.observable();
        self.ExperienceDetails = ko.observable();
        self.ResidenceStatus = ko.observable();
        self.OfficeAddressId = ko.observable();
        self.OfficeAddress = new address();
        self.PresentAddressId = ko.observable();
        self.PresentAddress = new address();
        self.PermanentAddressId = ko.observable();
        self.PermanentAddress = new address();
        self.LoadData = function (data) {
            self.Id(data.Id != null ? data.Id : 0);
            self.ProposalId(data.ProposalId);
            self.CIFPId(data.CIFPId);
            self.CIFOId(data.CIFOId);
            self.ApplicantRole(data.ApplicantRole);
            self.ApplicantRoleName(data.ApplicantRoleName);
            self.CIFNo(data.CIFNo);
            self.Name(data.Name);
            self.NID(data.NID);
            self.RelationshipWithApplicant(data.RelationshipWithApplicant);
            self.RelationshipWithApplicantName(data.RelationshipWithApplicantName);
            self.DateOfBirth(data.DateOfBirth);
            self.Age(data.Age);
            self.DateOfBirthTxt(data.DateOfBirthTxt);
            self.AcademicQualification(data.AcademicQualification);
            self.ProfessionName(data.ProfessionName);
            self.Designation(data.Designation);
            self.OrganizationName(data.OrganizationName);
            self.ExperienceDetails(data.ExperienceDetails);
            self.ResidenceStatus(data.ResidenceStatus);
            self.OfficeAddressId(data.OfficeAddressId);
            self.PresentAddressId(data.PresentAddressId);
            self.PresentAddress.LoadAddress(data.PresentAddress != null ? data.PresentAddress : "");
            self.PermanentAddressId(data.PermanentAddressId);
            self.PermanentAddress.LoadAddress(data.PermanentAddress != null ? data.PermanentAddress : "");
            self.OfficeAddress.LoadAddress(data.OfficeAddress != null ? data.OfficeAddress : "");

        }
    }

    function income() {
        var self = this;
        self.Id = ko.observable();
        self.ProposalId = ko.observable();
        self.CIFNo = ko.observable();
        self.Name = ko.observable();
        self.ApplicantRole = ko.observable();
        self.ApplicantRoleName = ko.observable();
        self.IsConsidered = ko.observable();
        self.IncomeSource = ko.observable();
        self.IncomeAmount = ko.observable();
        self.IncomeAmountFormatted = ko.pureComputed({
            read: function () {
                return self.IncomeAmount().toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.IncomeAmount(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.ConsideredPercentage = ko.observable().extend({ min: 0, max: 100 });
        //    {//    message: "maxlength", //    params: 100,//    onlyIf: function() {//        return self.IsValid(); //    } //}

        self.ConsideredAmount = ko.computed(function () {
            if (self.IncomeAmount()) {
                var total = 0;
                if (self.ConsideredPercentage()) {
                    total = Math.round((parseFloat(self.ConsideredPercentage()) / 100) * parseFloat(self.IncomeAmount()), 2);
                }
                return total;
            }
        });
        self.ConsideredAmountFormatted = ko.pureComputed({
            read: function () {
                if (self.ConsideredAmount() > 0) {
                    return parseFloat(self.ConsideredAmount() ? self.ConsideredAmount() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.ConsideredAmount(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.Remarks = ko.observable();
        self.LoadData = function (data) {
            self.Id(data.Id);
            self.ProposalId(data.ProposalId);
            self.CIFNo(data.CIFNo);
            self.Name(data.Name);
            self.ApplicantRole(data.ApplicantRole);
            self.ApplicantRoleName(data.ApplicantRoleName);
            self.IsConsidered(data.IsConsidered);
            self.IncomeSource(data.IncomeSource);
            self.IncomeAmount(data.IncomeAmount);
            self.Remarks(data.Remarks);
            self.ConsideredPercentage(data.ConsideredPercentage);
        }
    }

    function netWorth() {
        var self = this;
        self.Id = ko.observable();
        self.ProposalId = ko.observable();
        self.CIFNo = ko.observable();
        self.Name = ko.observable();
        self.ClientRole = ko.observable();
        self.ClientRoleName = ko.observable();
        self.TotalAssetOfApplicant = ko.observable();
        self.TotalAssetOfApplicantFormatted = ko.pureComputed({
            read: function () {
                if (self.TotalAssetOfApplicant() > 0) {
                    return parseFloat(self.TotalAssetOfApplicant() ? self.TotalAssetOfApplicant() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.TotalAssetOfApplicant(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.TotalLiabilityOfApplicant = ko.observable();
        self.TotalLiabilityOfApplicantFormatted = ko.pureComputed({
            read: function () {
                if (self.TotalLiabilityOfApplicant() > 0) {
                    return parseFloat(self.TotalLiabilityOfApplicant() ? self.TotalLiabilityOfApplicant() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.TotalLiabilityOfApplicant(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.NetWorthOfApplicant = ko.observable();
        self.NetWorthOfApplicantFormatted = ko.pureComputed({
            read: function () {
                if (self.NetWorthOfApplicant() > 0) {
                    return parseFloat(self.NetWorthOfApplicant() ? self.NetWorthOfApplicant() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.NetWorthOfApplicant(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.LoadData = function (data) {
            self.Id(data.Id);
            self.ProposalId(data.ProposalId);
            self.CIFNo(data.CIFNo);
            self.Name(data.Name);
            self.ClientRole(data.ClientRole);
            self.ClientRoleName(data.ClientRoleName);
            self.TotalAssetOfApplicant(data.TotalAssetOfApplicant);
            self.TotalLiabilityOfApplicant(data.TotalLiabilityOfApplicant);
            self.NetWorthOfApplicant(data.NetWorthOfApplicant);
        }

    }

    function cibs() {
        var self = this;
        self.Id = ko.observable();
        self.ProposalId = ko.observable();
        self.CIFNo = ko.observable();
        self.ClientRole = ko.observable();
        self.ClientRoleName = ko.observable();
        self.Name = ko.observable();
        self.CIBStatus = ko.observable();
        self.CIBStatusName = ko.observable();
        self.CIBDate = ko.observable();
        self.CIBDateTxt = ko.observable();
        self.TotalOutstandingAsBorrower = ko.observable();
        self.ClassifiedAmountAsBorrower = ko.observable();
        self.TotalEMIAsBorrower = ko.observable();
        self.LoadData = function (data) {
            self.Id(data.Id);
            self.ProposalId(data.ProposalId);
            self.CIFNo(data.CIFNo);
            self.ClientRole(data.ClientRole);
            self.ClientRoleName(data.ClientRoleName);
            self.Name(data.Name);
            self.CIBStatus(data.CIBStatus);
            self.CIBStatusName(data.CIBStatusName);
            self.CIBDate(data.CIBDate);
            self.CIBDateTxt(data.CIBDateTxt);
            self.TotalOutstandingAsBorrower(data.TotalOutstandingAsBorrower);
            self.ClassifiedAmountAsBorrower(data.ClassifiedAmountAsBorrower);
            self.TotalEMIAsBorrower(data.TotalEMIAsBorrower);
        }
    }

    function liabilities() {
        var self = this;
        self.Id = ko.observable();
        self.ProposalId = ko.observable();
        self.Name = ko.observable();
        self.FacilityType = ko.observable();
        self.InstituteName = ko.observable();
        self.Limit = ko.observable();
        self.LimitFormatted = ko.pureComputed({
            read: function () {
                if (self.Limit() > 0) {
                    return parseFloat(self.Limit() ? self.Limit() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.Limit(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.Outstanding = ko.observable();
        self.OutstandingFormatted = ko.pureComputed({
            read: function () {
                if (self.Outstanding() > 0) {
                    return parseFloat(self.Outstanding() ? self.Outstanding() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.Outstanding(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.EMI = ko.observable();
        self.EMIFormatted = ko.pureComputed({
            read: function () {
                if (self.EMI() > 0) {
                    return parseFloat(self.EMI() ? self.EMI() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.EMI(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.PaymentRecord = ko.observable();
        self.StartingDate = ko.observable();
        self.StartingDateTxt = ko.observable();
        self.ExpiryDate = ko.observable();
        self.ExpiryDateTxt = ko.observable();
        self.LoadData = function (data) {
            self.Id(data.Id);
            self.ProposalId(data.ProposalId);
            self.Name(data.Name);
            self.FacilityType(data.FacilityType);
            self.InstituteName(data.InstituteName);
            self.Limit(data.Limit);
            self.Outstanding(data.Outstanding);
            self.EMI(data.EMI);
            self.PaymentRecord(data.PaymentRecord);
            self.StartingDate(data.StartingDate ? moment(data.StartingDate) : moment());
            self.StartingDateTxt(data.StartingDateTxt);
            self.ExpiryDate(data.ExpiryDate ? moment(data.ExpiryDate) : moment());
            self.ExpiryDateTxt(data.ExpiryDateTxt);
        }
    }

    function guarantors() {
        var self = this;

        self.Id = ko.observable();
        self.ProposalId = ko.observable();
        self.GuarantorCif = ko.observable();
        self.GuarantorCifId = ko.observable();
        self.GuarantorCif.subscribe(function () {
            self.GuarantorCifId(self.GuarantorCif().key);
        })
        self.Name = ko.observable();
        self.ProfessionName = ko.observable();
        self.CompanyName = ko.observable();
        self.Designation = ko.observable();
        self.RelationshipWithApplicant = ko.observable();
        self.RelationshipWithApplicantName = ko.observable();
        self.Age = ko.observable();
        self.MonthlyIncome = ko.observable();
        self.MonthlyIncomeFormatted = ko.pureComputed({
            read: function () {
                if (self.MonthlyIncome() > 0) {
                    return parseFloat(self.MonthlyIncome() ? self.MonthlyIncome() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.MonthlyIncome(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.OccupationTypeName = ko.observable();
        self.LoadData = function (data) {
            self.Id(data.Id);
            self.ProposalId(data.ProposalId);
            if (data.GuarantorCIF) {
            self.GuarantorCif(data.GuarantorCIF);
            self.GuarantorCifId(data.GuarantorCifId);
            }
            self.Name(data.Name);
            self.ProfessionName(data.ProfessionName);
            self.CompanyName(data.CompanyName);
            self.Designation(data.Designation);
            self.RelationshipWithApplicant(data.RelationshipWithApplicant);
            self.RelationshipWithApplicantName(data.RelationshipWithApplicantName);
            self.Age(data.Age);
            self.MonthlyIncome(data.MonthlyIncome);
        }

    }

    function texts() {
        var self = this;

        self.Id = ko.observable();
        self.ProposalId = ko.observable();
        self.Type = ko.observable();
        self.Text = ko.observable();
        self.IsPrintable = ko.observable();
        self.PrinterFiltering = ko.observable();
        self.LoadData = function (data) {
            self.Id(data.Id);
            self.ProposalId(data.ProposalId);
            self.Type(data.Type);
            self.Text(data.Text);
            self.IsPrintable(data.IsPrintable);
            self.PrinterFiltering(data.PrinterFiltering);
        }
    }

    function otherCosts() {
        var self = this;

        self.Id = ko.observable();
        self.ProposalId = ko.observable();
        self.Details = ko.observable();
        self.Amount = ko.observable();
        self.AmountFormatted = ko.pureComputed({
            read: function () {
                if (self.Amount() > 0) {
                    return parseFloat(self.Amount() ? self.Amount() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.Amount(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.Other = ko.observable();
        self.LoadData = function (data) {
            self.Id(data.Id);
            self.ProposalId(data.ProposalId);
            self.Details(data.Details);
            self.Amount(data.Amount);
            self.Other(data.Amount);
        }
    }

    function overallAssessment() {
        var self = this;

        self.Id = ko.observable();
        self.ProposalId = ko.observable();
        self.AssessmentParticulars = ko.observable();
        self.VerificationStatus = ko.observable();
        self.CIFName = ko.observable();
        self.DoneBy = ko.observable();
        self.AssessmentDate = ko.observable();
        self.AssessmentDateTxt = ko.observable();
        self.Remarks = ko.observable();
        self.LoadData = function (data) {
            self.Id(data.Id);
            self.ProposalId(data.ProposalId);
            self.AssessmentParticulars(data.AssessmentParticulars);
            self.VerificationStatus(data.VerificationStatus);
            self.CIFName(data.CIFName);
            self.DoneBy(data.DoneBy);
            self.AssessmentDate(data.AssessmentDate ? moment(data.AssessmentDate) : "");
            self.AssessmentDateTxt(data.AssessmentDateTxt);
            self.Remarks(data.Remarks);
        }
    }

    function stressRate() {
        var self = this;
        self.Id = ko.observable();
        self.ProposalId = ko.observable();
        self.InterestRate = ko.observable();
        self.AppliedLoanTerm = ko.observable();
        self.RecomendedLoanAmountFromIPDC = ko.observable();
        self.EMIofProposedLoan = ko.observable();
        self.EMI = ko.observable();
        self.Increase = ko.observable();
        self.DBR = ko.observable();
        self.DBRFlatPurchase = ko.observable();
        self.LoadData = function (data) {
            self.Id(data.Id);
            self.ProposalId(data.ProposalId);
            self.InterestRate(data.InterestRate);
            self.AppliedLoanTerm(data.AppliedLoanTerm);
            self.RecomendedLoanAmountFromIPDC(data.RecomendedLoanAmountFromIPDC);
            self.EMIofProposedLoan(data.EMIofProposedLoan);
            self.EMI(data.EMI);
            self.Increase(data.Increase);
            self.DBR(data.DBR);
            self.DBRFlatPurchase(data.DBRFlatPurchase);
        }
    }

    function fdr() {
        var self = this;

        self.Id = ko.observable();
        self.ProposalId = ko.observable();
        self.InstituteName = ko.observable();
        self.BranchName = ko.observable();
        self.FDRAccountNo = ko.observable();
        self.Amount = ko.observable();
        self.Rate = ko.observable();
        self.DepositorName = ko.observable();
        self.MaturityDate = ko.observable();
        self.MaturityDateTxt = ko.observable();
        self.LoadData = function (data) {
            self.Id(data.Id);
            self.ProposalId(data.ProposalId);
            self.InstituteName(data.InstituteName);
            self.BranchName(data.BranchName);
            self.FDRAccountNo(data.FDRAccountNo);
            self.Amount(data.Amount);
            self.Rate(data.Rate);
            self.DepositorName(data.DepositorName);
            self.MaturityDate(data.MaturityDate ? moment(data.MaturityDate) : moment());
            self.MaturityDateTxt(data.MaturityDateTxt);
        }
    }

    function signatory() {
        var self = this;

        self.Id = ko.observable();
        self.ProposalId = ko.observable();
        self.Name = ko.observable();

        self.SignatoryId = ko.observable();
        self.LoadData = function (data) {
            self.Id(data.Id);
            self.ProposalId(data.ProposalId);
            self.Name(data.Name);
            self.SignatoryId(data.SignatoryId);

        }
    }

    function securityDetail() {
        var self = this;
        self.Id = ko.observable();
        self.ProposalId = ko.observable();
        self.SecurityType = ko.observable();
        self.SecurityTypeName = ko.observable();
        self.Details = ko.observable();
        self.LoadData = function (data) {
            self.Id(data.Id);
            self.ProposalId(data.ProposalId);
            self.SecurityType(data.SecurityType);
            self.SecurityTypeName(data.SecurityTypeName);
            self.Details(data.Details);
        }
    }

    function proposalCrdCard() {
        var self = this;
        self.Id = ko.observable();
        self.ProposalId = ko.observable();
        self.CIFId = ko.observable();
        self.CreditCardId = ko.observable();
        self.CreditCardNo = ko.observable().extend({digit :true,minLength :15});
        self.CreditCardIssuersName = ko.observable();
        self.CreditCardIssueDate = ko.observable();
        self.CreditCardIssueDateTxt = ko.observable();
        self.CreditCardLimit = ko.observable();
        self.CreditCardLimitFormatted = ko.pureComputed({
            read: function () {
                if (self.CreditCardLimit() > 0) {
                    return parseFloat(self.CreditCardLimit() ? self.CreditCardLimit() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.CreditCardLimit(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.LoadData = function (data) {
            self.Id(data != null ? data.Id : 0);
            self.ProposalId(data != null ? data.ProposalId : 0);
            self.CIFId(data.CIFId);
            self.CreditCardId(data.CreditCardId);
            self.CreditCardNo(data != null ? data.CreditCardNo : "");
            self.CreditCardIssuersName(data != null ? data.CreditCardIssuersName : "");
            self.CreditCardIssueDate(data.CreditCardIssueDate ? moment(data.CreditCardIssueDate) : "");
            //self.CreditCardIssueDateTxt(data != null ? data.CreditCardIssueDateTxt : "");
            self.CreditCardLimit(data != null ? data.CreditCardLimit : 0);
        }
    }

    function proposalVM() {
        var self = this;
        self.Id = ko.observable();
        self.ApplicationId = ko.observable();
        self.ApplicationNo = ko.observable();
        self.Link1 = ko.observable();
        self.Link2 = ko.observable();
        self.Link3 = ko.observable();
        self.Title1 = ko.observable('PDF');
        self.Title2 = ko.observable('Excel');
        self.Title3 = ko.observable('Word');
        //initial info
        self.ApplicationReceiveDate = ko.observable();
        self.ApplicationReceiveDateText = ko.observable();
        self.CRMReceiveDate = ko.observable();
        self.CRMReceiveDateText = ko.observable();
        self.ProposalDate = ko.observable('');
        self.ProposalDateText = ko.observable();
        self.RMName = ko.observable();
        self.RMCode = ko.observable();
        self.BranchName = ko.observable();
        self.FacilityType = ko.observable();
        self.FacilityTypeName = ko.observable();
        self.RecomendedLoanAmountFromIPDC = ko.observable();
        self.TotalExpences = ko.observable();
        //loan related info
        self.AppliedLoanAmount = ko.observable();
        self.AppliedLoanAmountFormatted = ko.pureComputed({
            read: function () {
                if (self.AppliedLoanAmount() != undefined) {
                    return self.AppliedLoanAmount().toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.AppliedLoanAmount(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.AppliedLoanTerm = ko.observable();
        self.AppliedLoanTermApplication = ko.observable();
        self.CurrentExposureWithIPDC = ko.observable();
        self.TLComment = ko.observable();
        self.BMComment = ko.observable();
        self.TotalExposureWithIPDC = ko.computed(function () {
            var total = 0;
            var current = parseFloat(self.CurrentExposureWithIPDC() ? self.CurrentExposureWithIPDC() : 0);
            var loanAmt = parseFloat(self.RecomendedLoanAmountFromIPDC() ? self.RecomendedLoanAmountFromIPDC() : 0);
            total = current + loanAmt;
            return total;
        }); // calculated value = currentExposureWithIPDC + LoanAmount
        self.TotalExposureWithIPDCFormatted = ko.pureComputed({
            read: function () {
                if (self.TotalExposureWithIPDC() != undefined) {
                    return self.TotalExposureWithIPDC().toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
                },
                write: function (value) {
                    value = parseFloat(value.replace(/,/g, ""));
                    self.TotalExposureWithIPDC(isNaN(value) ? 0 : value);
                },
                owner: self
        });
        self.InterestRateCard = ko.observable();
        self.InterestRateOffered = ko.observable();
        self.RateVariance = ko.observable(); //interestRateCard - InterestRateOffered
        self.LoanRemarks = ko.observable();
        self.ProcessingFeeAndDocChargesCardRate = ko.observable();

        ////client profile

        self.RelationshipsWithApplicant = ko.observableArray([]);
        //var test = new clientProfile();
        //test.ApplicantRole('2');
        self.ClientProfiles = ko.observableArray([]);
        self.AddClientProfile = function () {
            self.ClientProfiles.push(new clientProfile());
        }
        self.HomeOwnerships = ko.observableArray([]);
        self.GetHomeOwnerships = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/GetHomeOwnerships',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.HomeOwnerships(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        ////loan purpose
        self.LoanPurpose = ko.observable();
        self.SecurityDetails = ko.observableArray([]);


        ////income assessment

        self.Incomes = ko.observableArray([]);//considered and not considered
        self.ConsideredIncome = ko.observableArray([]);

        self.NotConsideredIncome = ko.observableArray([]);

        self.TotalMonthlyIncomeConsidered = ko.computed(function () {
            var total = 0;
            $.each(self.ConsideredIncome(), function (index, value) {
                if (value.ConsideredAmount()) {
                    total += parseFloat(value.ConsideredAmount() != null ? value.ConsideredAmount() : 0);
                }
            });
            return total.toFixed(2);
        });
        self.TotalMonthlyIncomeConsideredFormatted = ko.pureComputed({
            read: function () {
                if (self.TotalMonthlyIncomeConsidered() > 0) {
                    return parseFloat(self.TotalMonthlyIncomeConsidered() ? self.TotalMonthlyIncomeConsidered() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.TotalMonthlyIncomeConsidered(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.OtherValuationCosts = ko.observableArray([]);
        self.TotalOtherValuationCost = ko.computed(function () {
            var total = 0;
            $.each(self.OtherValuationCosts(), function (index, value) {
                if (value.Amount()) {
                    total += parseFloat(value != null ? value.Amount() : 0);
                }
            });
            return total.toFixed(2);
        });
        self.TotalOtherValuationCostFormatted = ko.pureComputed({
            read: function () {
                if (self.TotalOtherValuationCost() > 0) {
                    return parseFloat(self.TotalOtherValuationCost() ? self.TotalOtherValuationCost() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.TotalOtherValuationCost(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.TotalMonthlyIncomeNotConsidered = ko.computed(function () {
            var total = 0;
            $.each(self.NotConsideredIncome(), function (index, value) {
                if (value.IncomeAmount()) {
                    total += value.IncomeAmount();
                }
            });

            return total;
        });
        self.TotalMonthlyIncomeNotConsideredFormatted = ko.pureComputed({
            read: function () {
                if (self.TotalMonthlyIncomeNotConsidered() > 0) {
                    return parseFloat(self.TotalMonthlyIncomeNotConsidered() ? self.TotalMonthlyIncomeNotConsidered() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.TotalMonthlyIncomeNotConsidered(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        ////public decimal LiabilityTotalEMI = ko.observable();
        ////public decimal EMIofProposedLoan = ko.observable();

        ////public decimal DBR = ko.observable();
        //self.IncomeAssessmentRemarks = ko.observable();
        self.IncomeConsideredRemarks = ko.observable();
        self.IncomeNotConsideredRemarks = ko.observable();

        ////net worth
        self.NetWorths = ko.observableArray([]);
        self.TotalAssetOfApplicants = ko.computed(function () {
            var totalAsset = 0;
            $.each(self.NetWorths(), function (index, value) {
                //
                if (value.ClientRole() === 1 || value.ClientRole() === 2)
                    totalAsset += parseFloat(value ? value.TotalAssetOfApplicant() : 0);
            });
            return totalAsset;
        });
        self.TotalAssetOfApplicantsFormatted = ko.pureComputed({
            read: function () {
                if (self.TotalAssetOfApplicants() > 0) {
                    return parseFloat(self.TotalAssetOfApplicants() ? self.TotalAssetOfApplicants() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.TotalAssetOfApplicants(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.TotalLiabilitiesOfApplicants = ko.computed(function () {
            var totalLiability = 0;
            $.each(self.NetWorths(), function (index, value) {
                if (value.ClientRole() === 1 || value.ClientRole() === 2)
                    totalLiability += parseFloat(value ? value.TotalLiabilityOfApplicant() : 0);
            });
            return totalLiability;
        });
        self.TotalLiabilitiesOfApplicantsFormatted = ko.pureComputed({
            read: function () {
                if (self.TotalLiabilitiesOfApplicants() > 0) {
                    return parseFloat(self.TotalLiabilitiesOfApplicants() ? self.TotalLiabilitiesOfApplicants() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.TotalLiabilitiesOfApplicants(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.TotalNetWorthOfApplicants = ko.computed(function () {
            var totalNetWorth = 0;
            $.each(self.NetWorths(), function (index, value) {
                if (value.ClientRole() === 1 || value.ClientRole() === 2)
                    totalNetWorth += parseFloat(value ? value.NetWorthOfApplicant() : 0);
            });
            return totalNetWorth;
        });
        self.TotalNetWorthOfApplicantsFormatted = ko.pureComputed({
            read: function () {
                if (self.TotalNetWorthOfApplicants() > 0) {
                    return parseFloat(self.TotalNetWorthOfApplicants() ? self.TotalNetWorthOfApplicants() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.TotalNetWorthOfApplicants(isNaN(value) ? 0 : value);
            },
            owner: self
        });

        ////CIB Status
        self.CIBs = ko.observableArray([]);
        self.CIBClassificationStatuses = ko.observableArray([]);
        self.GetCIBClassificationStatuses = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/GetCIBClassificationStatus',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CIBClassificationStatuses(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.NetWorthsRemarks = ko.observable();
        ////Liability Details
        self.Liabilities = ko.observableArray([]);
        self.TotalLimit = ko.computed(function () {
            var totalLimit = 0;
            $.each(self.Liabilities(), function (index, value) {
                totalLimit += parseFloat(value ? value.Limit() : 0);
            });
            return totalLimit;
        });
        self.TotalLimitFormatted = ko.pureComputed({
            read: function () {
                if (self.TotalLimit() > 0) {
                    return parseFloat(self.TotalLimit() ? self.TotalLimit() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.TotalLimit(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.TotalOutstanding = ko.computed(function () {
            var totalOutstanding = 0;
            $.each(self.Liabilities(), function (index, value) {
                totalOutstanding += parseFloat(value ? value.Outstanding() : 0);
            });
            return totalOutstanding;
        });
        self.TotalOutstandingFormatted = ko.pureComputed({
            read: function () {
                if (self.TotalOutstanding() > 0) {
                    return parseFloat(self.TotalOutstanding() ? self.TotalOutstanding() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.TotalOutstanding(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.LiabilityTotalEMI = //ko.observable();
            ko.pureComputed(function () {
                var totalEMI = 0;
                $.each(self.Liabilities(), function (index, value) {
                    totalEMI += parseFloat(value ? value.EMI() : 0);
                });
                return totalEMI;
            });
        self.LiabilityTotalEMIFormatted = ko.pureComputed({
            read: function () {
                if (self.LiabilityTotalEMI() > 0) {
                    return parseFloat(self.LiabilityTotalEMI() ? self.LiabilityTotalEMI() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.LiabilityTotalEMI(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.TotalCreditCard = //ko.observable();
           ko.pureComputed(function () {
               var totalCreditCard = 0;
               $.each(self.ProposalCreditCards(), function (index, value) {
                   totalCreditCard += parseFloat(value ? value.CreditCardLimit() : 0);
               });
               return totalCreditCard;
           });
        self.TotalCreditCardFormatted = ko.pureComputed({
            read: function () {
                if (self.TotalCreditCard() > 0) {
                    return parseFloat(self.TotalCreditCard() ? self.TotalCreditCard() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.TotalMonthlyIncomeConsidered(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.LiabilityRemarks = ko.observable();


        ////personal guarantor information
        self.Guarantors = ko.observableArray([]);
        self.RemovedGuarantors = ko.observableArray([]);
        self.AddGuarantor = function () {
            self.Guarantors.push(new guarantors());
        }
        self.RemoveGuarantor = function (line) {
            if (line.Id() > 0)
                self.RemovedGuarantors.push(line.Id());
            self.Guarantors.remove(line);
        }
        self.GuarantorRemarks = ko.observable();

        ////comment on bank statement
        self.CommentOnBankStatement = ko.observable();


        ////large text field inclueds asset backup, Strength, Weakness, Disbursement Conditions, Exceptions
        self.AssetBackupTexts = ko.observableArray([]); //texts with type = 1
        self.AddAssetBackup = function () {
            var assetBackup = new texts();
            assetBackup.Type(1);
            self.AssetBackupTexts.push(assetBackup);
        }
        self.RemovedAssetBackup = ko.observableArray([]);
        self.RemoveAssetBackup = function (line) {
            if (line.Id() > 0)
                self.RemovedAssetBackup.push(line.Id());
            self.AssetBackupTexts.remove(line);
        }
        self.AssetBackupRemarks = ko.observable();


        self.AddSecurityDetails = function () {
            var security = new securityDetail();
            security.SecurityType(1);
            self.SecurityDetails.push(security);
        }
        self.RemovedSecurityDetails = ko.observableArray([]);
        self.RemoveSecurityDetails = function (line) {
            if (line.Id() > 0)
                self.RemovedSecurityDetails.push(line.Id());
            self.SecurityDetails.remove(line);
        }

        //// PDC / EFTN details
        self.PDCBankName = ko.observable();
        self.PDCBankBranch = ko.observable();
        self.PDCRoutingNo = ko.observable();
        self.PDCAccountTitle = ko.observable();
        self.PDCAccountType = ko.observable();
        self.PDCAccountNo = ko.observableArray();

        ////strengths and weakness
        self.Strengths = ko.observableArray([]);
        self.AddStrengths = function () {
            var strength = new texts();
            strength.Type(2);
            self.Strengths.push(strength);
        }
        //self.RemovedStrengths = ko.observableArray([]);
        self.RemoveStrengths = function (line) {
            if (line.Id() > 0)
                self.RemovedAssetBackup.push(line.Id());
            self.Strengths.remove(line);
        }
        self.AssetBackupRemarks = ko.observable();
        self.Weakness = ko.observableArray([]);
        self.AddWeakness = function () {
            var weakness = new texts();
            weakness.Type(3);
            self.Weakness.push(weakness);
        }
        //self.RemovedWeakness = ko.observableArray([]);
        self.RemoveWeakness = function (line) {
            if (line.Id() > 0)
                self.RemovedAssetBackup.push(line.Id());
            self.Weakness.remove(line);
        }
        self.AssetBackupRemarks = ko.observable();


        ////Property/Vehicle/FDR/Consumer Goods Details
        self.Product = ko.observable(); //4


        ////if product = vehicle
        self.Vehicle_Name = ko.observable();
        self.Vehicle_ModelYear = ko.observable();
        self.Vehicle_VendorName = ko.observable();
        self.Vehicle_QuotedPrice = ko.observable();
        self.CC = ko.observable();
        self.Colour = ko.observable();
        self.ChassisNo = ko.observable();
        self.EngineNo = ko.observable();
        self.PresentMarketValue = ko.observable();

        //    //public decimal? RecomendedLoanAmountFromIPDC = ko.observable(); // non editable  //loan amount / preset market value %

        self.Product_Remarks = ko.observable();

        ////if product = FDR
        self.FDRs = ko.observableArray([]);
        self.AddFDRs = function () {
            self.FDRs.push(new fdr());
        }
        self.RemovedFDRs = ko.observableArray([]);
        self.RemoveFDRs = function (line) {
            if (line.Id() > 0)
                self.RemovedFDRs.push(line.Id());
            self.FDRs.remove(line);
        }

        //todo
        ////public decimal? LTVonTotalPresentMarketValue = ko.observable(); // Loan amount / fdr total amount %
        ////public decimal? RecomendedLoanAmountFromIPDC = ko.observable();
        self.FDR_Remarks = ko.observable();

        ////if product = Consumer goods
        self.CG_Item = ko.observable();
        self.CG_Brand = ko.observable();
        self.CG_DealerName = ko.observable();//dealer or showroom
        self.CG_Price = ko.observable();
        //    //public decimal? PresentMarketValue = ko.observable();
        //    //public decimal? RecomendedLoanAmountFromIPDC = ko.observable();
        //    //public decimal? LTVonTotalPresentMarketValue = ko.observable();
        //    //Product_Remarks = ko.observable();

        ////if product = property
        self.PropertyType = ko.observable().extend({ required: true,message :"Please Provide Property Type" });
        //    //if propertyType = Flat
        self.SellersName = ko.observable();
        self.DevelopersName = ko.observable();
        self.ProjectName = ko.observable();
        self.ProjectStatus = ko.observable();
        self.ProjectAddressId = ko.observable();

        self.ProjectAddress = new address();
        self.DeveloperCategory = ko.observable();
        self.TotalNumberOfFloors = ko.observable();
        self.TotalNumberOfSlabsCasted = ko.observable();
        self.TotalLandArea = ko.observable();
        self.TotalLandAreaFormatted = ko.pureComputed({
            read: function () {
                if (self.TotalLandArea() > 0) {
                    return parseFloat(self.TotalLandArea() ? self.TotalLandArea() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.TotalLandArea(isNaN(value) ? 0 : value);
            },
            owner: self
        });

        self.FlatDetails = ko.observable();
        self.NumberOfCarParking = ko.observable();
        self.FlatSize = ko.observable();
        self.PropertyOwnershipType = ko.observable();

        self.IsPermissionRequired = ko.observable(false);
        self.PermissionRemarks = ko.observable();

        ////if propertyType = Own Construction
        self.PropertyOwnerName = ko.observable();
        ////ProjectName = ko.observable();
        ////public long? ProjectAddressId = ko.observable();
        ////[ForeignKey("ProjectAddressId")]
        ////public Address ProjectAddress = ko.observable();
        ////public decimal? TotalLandArea = ko.observable();
        self.OwnConstructionLoanPurpose = ko.observable();
        ////public LandType? PropertyOwnershipType = ko.observable();
        ////public bool? IsPermissionRequired = ko.observable();
        ////PermissionRemarks = ko.observable();


        ////landed property valuation
        ////if product = property
        ////if propertyType = Flat
        ////public decimal? FlatSize = ko.observable();
        self.PricePerSQF = ko.observable();
        self.ConstructionCostFinancingPlan = ko.observable();
        self.MarketPriceOfFlat = ko.computed(function () {
            var flatSize = 0;
            var pricePerSQF = 0;
            var total = 0;
            if (self.FlatSize())
                flatSize += parseFloat(self.FlatSize() != null ? self.FlatSize() : 0);
            if (self.PricePerSQF())
                pricePerSQF += parseFloat(self.PricePerSQF() != null ? self.PricePerSQF() : 0);
            total = pricePerSQF * flatSize;
            return total;
        });//= ko.observable();
        self.CarParkingPrice = ko.observable();
        self.RegistrationCost = ko.observable();

        self.PerKathaPriceAsPerRAJUK = ko.observable();
        self.PerKathaPriceAsPerRAJUKFormatted = ko.pureComputed({
            read: function () {
                if (self.PerKathaPriceAsPerRAJUK() > 0) {
                    return parseFloat(self.PerKathaPriceAsPerRAJUK() ? self.PerKathaPriceAsPerRAJUK() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.PerKathaPriceAsPerRAJUK(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.EstimatedConstructionCostApproved = ko.observable();
        self.EstimatedConstructionCostApprovedFormatted = ko.pureComputed({
            read: function () {
                if (self.EstimatedConstructionCostApproved() > 0) {
                    return parseFloat(self.EstimatedConstructionCostApproved() ? self.EstimatedConstructionCostApproved() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.EstimatedConstructionCostApproved(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.TotalMarketValue = ko.computed(function () {
            var marketPriceOfFlat = parseFloat(self.MarketPriceOfFlat() != null ? self.MarketPriceOfFlat() : 0);
            var carParkingPrice = parseFloat(self.CarParkingPrice() != null ? self.CarParkingPrice() : 0);
            var registrationCost = parseFloat(self.RegistrationCost() != null ? self.RegistrationCost() : 0);
            var totalOtherValuationCost = parseFloat(self.TotalOtherValuationCost() != null ? self.TotalOtherValuationCost() : 0);
            var total = marketPriceOfFlat + carParkingPrice + registrationCost + totalOtherValuationCost;
            return total;
        }); //ko.observable(); // carParkingPrice + MarketValeOfFlat
        self.TotalMarketValueFormatted = ko.pureComputed({
            read: function () {
                if (self.TotalMarketValue() > 0) {
                    return parseFloat(self.TotalMarketValue() ? self.TotalMarketValue() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.TotalMarketValue(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.MarketValueOfLandAsPerRAJUK = ko.computed(function () {

            var netAmount = 0;
            var totalLandArea = 0;
            var perKathaPrice = 0;
            if (self.TotalLandArea())
                totalLandArea += parseFloat(self.TotalLandArea() != null ? self.TotalLandArea() : 0);

            if (self.PerKathaPriceAsPerRAJUK())
                perKathaPrice += parseFloat(self.PerKathaPriceAsPerRAJUK() != null ? self.PerKathaPriceAsPerRAJUK() : 0);

            netAmount = totalLandArea * perKathaPrice;

            return netAmount;

        });
        self.MarketValueOfLandAsPerRAJUKFormatted = ko.pureComputed({
            read: function () {
                if (self.MarketValueOfLandAsPerRAJUK() > 0) {
                    return parseFloat(self.MarketValueOfLandAsPerRAJUK() ? self.MarketValueOfLandAsPerRAJUK() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.MarketValueOfLandAsPerRAJUK(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.DistressedValue = ko.computed(function () {
            
            var tlAmount = 0;
            if (self.PropertyType() === '2') {
                var mrkAmt = parseFloat(self.MarketValueOfLandAsPerRAJUK() ? self.MarketValueOfLandAsPerRAJUK() : 0);
                var constCost = parseFloat(self.EstimatedConstructionCostApproved() ? self.EstimatedConstructionCostApproved() : 0);
                tlAmount = mrkAmt + constCost;
            } if (self.PropertyType() === '1') {
                var marketPriceOfFlat = parseFloat(self.MarketPriceOfFlat() != null ? self.MarketPriceOfFlat() : 0);
                var carParkingPrice = parseFloat(self.CarParkingPrice() != null ? self.CarParkingPrice() : 0);
                var registrationCost = parseFloat(self.RegistrationCost() != null ? self.RegistrationCost() : 0);
                tlAmount = marketPriceOfFlat + carParkingPrice + registrationCost;
            }

            return (tlAmount * .7).toFixed(2);
        });
        self.DistressedValueFormatted = ko.pureComputed({
            read: function () {
                if (self.DistressedValue() > 0) {
                    return parseFloat(self.DistressedValue() ? self.DistressedValue() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.DistressedValue(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        //70% of present market value
        //    //public decimal? RecomendedLoanAmountFromIPDC = ko.observable();
        //    //public decimal? LTVonTotalPresentMarketValue = ko.observable();
        self.LandedPropertyValuationRemarks = ko.observable();
        self.LTVonTotalPresentMarketValue = ko.computed(function () {
            var ltvAmount = 0;
            var loanAmt = parseFloat(self.RecomendedLoanAmountFromIPDC() ? self.RecomendedLoanAmountFromIPDC() : 0);
            var marktValue = 0;
            if (self.FacilityType() === 1) {
                //var otherCost = parseFloat(self.TotalOtherValuationCost() ? self.TotalOtherValuationCost() : 0);
                if (self.PropertyType() === '2')
                    marktValue = parseFloat(self.TotalAmount() ? self.TotalAmount() : 0);
                else if (self.PropertyType() === '1')
                    marktValue = parseFloat(self.TotalMarketValue() ? self.TotalMarketValue() : 0);
                ltvAmount = (loanAmt / marktValue) * 100;
            } else if (self.FacilityType() === 2) {
                marktValue = parseFloat(self.PresentMarketValue() ? self.PresentMarketValue() : 0);
                ltvAmount = (loanAmt / marktValue) * 100;
            } else if (self.FacilityType() === 3) {
                marktValue = parseFloat(self.TotalMonthlyIncomeConsidered() ? self.TotalMonthlyIncomeConsidered() : 0);
                ltvAmount = loanAmt / marktValue;
            }
            return ltvAmount.toFixed(2);
        });
        ////if propertyType = Own Construction
        ////public decimal? TotalLandArea = ko.observable();


        self.LPVConstructionDetails = ko.observable();
        self.TotalConstructionAmountDetails = ko.observable();

        //    //public decimal? DistressedValue = ko.observable();
        //    //public decimal? RecomendedLoanAmountFromIPDC = ko.observable();
        //    //public decimal? LTVonTotalPresentMarketValue = ko.observable();
        //    //LandedPropertyValuationRemarks = ko.observable();


        //    //Landed property financing plan
        //    //if product = property
        //    //if propertyType = Flat
        //    //public decimal? TotalMarketValue = ko.observable(); //it is purchase price
        //    //public decimal? RegistrationCost = ko.observable();
        self.OtherCosts = ko.observableArray([]);
        //new otherCosts();
        self.AddOtherCosts = function () {
            self.OtherCosts.push(new otherCosts());
        }
        self.RemovedOtherCosts = ko.observableArray([]);
        self.RemoveOtherCosts = function (line) {
            if (line.Id() > 0)
                self.RemovedOtherCosts.push(line.Id());
            self.OtherCosts.remove(line);
        }

        self.AddValuationOtherCosts = function () {
            self.OtherValuationCosts.push(new otherCosts());
        }
        self.RemovedValuationOtherCosts = ko.observableArray([]);
        self.RemoveValuationOtherCosts = function (line) {
            if (line.Id() > 0)
                self.RemovedValuationOtherCosts.push(line.Id());
            self.OtherValuationCosts.remove(line);
        }

        //self.OtherValuationCosts = ko.observableArray([]);
        self.AddSignatories = function () {
            self.SignatoryList.push(new signatory());
        }
        self.RemovedSignatories = ko.observableArray([]);
        self.RemoveSignatories = function (line) {
            if (line.Id() > 0)
                self.RemovedSignatories.push(line.Id());
            self.SignatoryList.remove(line);
        }

        self.TotalAmount = ko.computed(function () {
            var totalAmount = 0;
            if (self.EstimatedConstructionCostApproved())
                totalAmount += parseFloat(self.EstimatedConstructionCostApproved() != null ? self.EstimatedConstructionCostApproved() : 0);
            if (self.TotalOtherValuationCost())
                totalAmount += parseFloat(self.TotalOtherValuationCost() != null ? self.TotalOtherValuationCost() : 0);
            if (self.MarketValueOfLandAsPerRAJUK())
                totalAmount += parseFloat(self.MarketValueOfLandAsPerRAJUK() != null ? self.MarketValueOfLandAsPerRAJUK() : 0);
            //$.each(self.OtherCosts(), function (index, value) {
            //    if (value.Amount())
            //        totalAmount += parseFloat(value.Amount());
            //});
            return totalAmount.toFixed(2);
        });
        self.TotalAmountFormatted = ko.pureComputed({
            read: function () {
                if (self.TotalAmount() > 0) {
                    return parseFloat(self.TotalAmount() ? self.TotalAmount() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.TotalAmount(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.LoanPaymentToDeveloper = ko.observable();
        self.LoanPaymentToDeveloperFormatted = ko.pureComputed({
            read: function () {
                if (self.LoanPaymentToDeveloper() > 0) {
                    return parseFloat(self.LoanPaymentToDeveloper() ? self.LoanPaymentToDeveloper() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.LoanPaymentToDeveloper(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.FlatFinanceTotalAmount = ko.computed(function () {
            var totalAmount = 0;
            if (self.PropertyType() === '2') {
                if (self.ConstructionCostFinancingPlan())
                    totalAmount += parseFloat(self.ConstructionCostFinancingPlan() != null ? self.ConstructionCostFinancingPlan() : 0);
                $.each(self.OtherCosts(), function (index, value) {
                    if (value.Amount())
                        totalAmount += parseFloat(value.Amount() != null ? value.Amount() : 0);
                });
                //$.each(self.OtherValuationCosts(), function (index, value) {
                //    if (value.Amount())
                //        totalAmount += parseFloat(value.Amount() != null ? value.Amount() : 0);
                //});
            }
            if (self.PropertyType() === '1') {
                //if (self.TotalMarketValue())
                //    totalAmount += parseFloat(self.TotalMarketValue() != null ? self.TotalMarketValue() : 0);
                $.each(self.OtherCosts(), function (index, value) {
                    if (value.Amount())
                        totalAmount += parseFloat(value.Amount() != null ? value.Amount() : 0);
                });
            }
            return totalAmount.toFixed(2);
        });
        self.FlatFinanceTotalAmountFormatted = ko.pureComputed({
            read: function () {
                if (self.FlatFinanceTotalAmount() > 0) {
                    return parseFloat(self.FlatFinanceTotalAmount() ? self.FlatFinanceTotalAmount() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.FlatFinanceTotalAmount(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.LPFPAlreadyPaid = ko.observable();
        self.LPFPAlreadyPaidFormatted = ko.pureComputed({
            read: function () {
                if (self.LPFPAlreadyPaid() > 0) {
                    return parseFloat(self.LPFPAlreadyPaid() ? self.LPFPAlreadyPaid() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.LPFPAlreadyPaid(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        //    //public decimal? RecomendedLoanAmountFromIPDC = ko.observable();
        self.AdditionalEquity2bInvestedByApplicant = ko.computed(function () {
            var totalAmount = 0;
            if (self.FlatFinanceTotalAmount())
                totalAmount += parseFloat(self.FlatFinanceTotalAmount() != null ? self.FlatFinanceTotalAmount() : 0);
            if (self.LPFPAlreadyPaid())
                totalAmount -= parseFloat(self.LPFPAlreadyPaid() != null ? self.LPFPAlreadyPaid() : 0);
            if (self.RecomendedLoanAmountFromIPDC())
                totalAmount -= parseFloat(self.RecomendedLoanAmountFromIPDC() != null ? self.RecomendedLoanAmountFromIPDC() : 0);

            return totalAmount.toFixed(2);
        });
        self.AdditionalEquity2bInvestedByApplicantFormatted = ko.pureComputed({
            read: function () {
                if (self.AdditionalEquity2bInvestedByApplicant() > 0) {
                    return parseFloat(self.AdditionalEquity2bInvestedByApplicant() ? self.AdditionalEquity2bInvestedByApplicant() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.AdditionalEquity2bInvestedByApplicant(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.LPFinancingPlanRemarks = ko.observable();
        self.PropertyRemarks = ko.observable();
        self.SecurityRemarks = ko.observable();
        ////if propertyType = Own Construction
        self.LPFPConstructionDetails = ko.observable();
        ////public decimal? EstimatedConstructionCostApproved = ko.observable();
        ////public List<Proposal_OtherCosts> OtherCosts = ko.observable();
        ////public decimal? TotalAmount = ko.observable();
        ////public decimal? LPFPAlreadyPaid = ko.observable();
        ////public decimal? RecomendedLoanAmountFromIPDC = ko.observable();
        self.LoanAmount2bUtilized4Construction = ko.observable();
        self.LoanAmount2bUtilized4ConstructionFormatted = ko.pureComputed({
            read: function () {
                if (self.LoanAmount2bUtilized4Construction() > 0) {
                    return parseFloat(self.LoanAmount2bUtilized4Construction() ? self.LoanAmount2bUtilized4Construction() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.LoanAmount2bUtilized4Construction(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.NetAmountForConstruction = ko.computed(function () {
            var netAmount = 0;
            if (self.RecomendedLoanAmountFromIPDC())
                netAmount += parseFloat(self.RecomendedLoanAmountFromIPDC() != null ? self.RecomendedLoanAmountFromIPDC() : 0);
            if (self.FlatFinanceTotalAmount())
                netAmount -= parseFloat(self.FlatFinanceTotalAmount() != null ? self.FlatFinanceTotalAmount() : 0);
            return netAmount;

        });
        self.NetAmountForConstructionFormatted = ko.pureComputed({
            read: function () {
                if (self.NetAmountForConstruction() > 0) {
                    return parseFloat(self.NetAmountForConstruction() ? self.NetAmountForConstruction() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.NetAmountForConstruction(isNaN(value) ? 0 : value);
            },
            owner: self
        });


        self.errors = ko.validation.group(self);
        self.IsValid = ko.computed(function () {
            var err = self.errors().length;
            if (err == 0)
                return true;
            return false;
        });



        //TotalAmount - Sum of Other costs
        //    //public decimal? AdditionalEquity2bInvestedByApplicant = ko.observable();
        //    //LPFinancingPlanRemarks = ko.observable();


        //    //overall assessment
        //self.GetAuthority
        self.ProposalCreditCards = ko.observableArray([]);
        //self.GetAuthority = function () {
        //    return $.ajax({
        //        type: "GET",
        //        url: '/IPDC/CRM/GetRelationshipWithApplicant',
        //        contentType: "application/json; charset=utf-8",
        //        dataType: "json",
        //        success: function (data) {
        //            self.RelationshipsWithApplicant(data); //Put the response in ObservableArray
        //        },
        //        error: function (error) {
        //            alert(error.status + "<--and--> " + error.statusText);
        //        }
        //    });
        //}
        self.OverallAssessments = ko.observableArray([]);
        self.AssessmentStatusList = ko.observableArray([]);
        self.AddOverallAssessment = function () {
            self.OverallAssessments.push(new overallAssessment());
        }
        self.RemovedOverallAssessments = ko.observableArray([]);
        self.RemoveOverallAssessment = function (line) {
            if (line.Id() > 0)
                self.RemovedOverallAssessments.push(line.Id());
            self.OverallAssessments.remove(line);
        };

        self.ProcessingFeeAndDocChargesPercentage = ko.observable();
        self.ProcessingFeeAndDocChargesAmount = ko.observable();

        self.CalculateFeeAmt = ko.computed(function () {
            if (self.ProcessingFeeAndDocChargesPercentage()>0 && self.RecomendedLoanAmountFromIPDC() >0 ) {
                var prct = parseFloat(self.ProcessingFeeAndDocChargesPercentage() ? self.ProcessingFeeAndDocChargesPercentage() : 0);
                var loanAmt = parseFloat(self.RecomendedLoanAmountFromIPDC() ? self.RecomendedLoanAmountFromIPDC() : 0);
                var result = ((prct * loanAmt) / 100).toFixed(2);
                self.ProcessingFeeAndDocChargesAmount(result);
            }
        });
        self.EMIofProposedLoan = ko.computed(function () {
            var tenor = self.AppliedLoanTerm();
            var interestRate = self.InterestRateOffered();
            var recommendedLoan = self.RecomendedLoanAmountFromIPDC();
            var rate = interestRate / 100 / 12;
            return Math.round(((recommendedLoan) / ((1 - (1 / (Math.pow((1 + rate), tenor)))) / (rate))));
        });
        self.EMIofProposedLoanFormatted = ko.pureComputed({
            read: function () {
                if (self.EMIofProposedLoan() > 0) {
                    return parseFloat(self.EMIofProposedLoan() ? self.EMIofProposedLoan() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.EMIofProposedLoan(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.DBR = ko.computed(function () {
            var dbrAmount = 0;
            var total = 0;
            var emiPropAmt = parseFloat(self.EMIofProposedLoan() ? self.EMIofProposedLoan() : 0);
            var fetchedEmiPropAmt = parseFloat(self.LiabilityTotalEMI() ? self.LiabilityTotalEMI() : 0);
            var totalCreditCrd = parseFloat(self.TotalCreditCard() ? self.TotalCreditCard() : 0);
            var perOfCreditCard = totalCreditCrd * .05;
            if (self.TotalMonthlyIncomeConsidered() > 0) {
                total = (emiPropAmt + fetchedEmiPropAmt + perOfCreditCard);
                var considered = parseFloat(self.TotalMonthlyIncomeConsidered() ? self.TotalMonthlyIncomeConsidered() : 0);
                dbrAmount = (total / considered) * 100;
            }
            return dbrAmount.toFixed(2);
        });
        self.DBRFormatted = ko.pureComputed({
            read: function () {
                if (self.DBR() > 0) {
                    return parseFloat(self.DBR() ? self.DBR() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.DBR(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.RentalIncome = ko.observable();
        self.ExpectedRentPercentage = ko.observable();
        //({min: { params: 1, message: "Please enter Valid NID number" },maxLength: 100});
        self.DBRFlatPurchase = ko.computed(function () {
            var dbrRentAmount = 0;
            //var dbr = parseFloat(self.DBR() ? self.DBR() : 0);
            var rent = parseFloat(self.RentalIncome() ? self.RentalIncome() : 0);
            var percentage = parseFloat(self.ExpectedRentPercentage() ? self.ExpectedRentPercentage() : 0);
            dbrRentAmount = ((rent * percentage) / 100); //percentage;//
            //return total;
            var dbrAmount = 0;
            var total = 0;
            var emiPropAmt = parseFloat(self.EMIofProposedLoan() ? self.EMIofProposedLoan() : 0);
            var fetchedEmiPropAmt = parseFloat(self.LiabilityTotalEMI() ? self.LiabilityTotalEMI() : 0);
            var totalCreditCrd = parseFloat(self.TotalCreditCard() ? self.TotalCreditCard() : 0);
            var perOfCreditCard = totalCreditCrd * .05;
            if (self.TotalMonthlyIncomeConsidered() > 0) {
                total = (emiPropAmt + fetchedEmiPropAmt + perOfCreditCard);
                var considered = parseFloat(self.TotalMonthlyIncomeConsidered() ? self.TotalMonthlyIncomeConsidered() : 0);
                dbrAmount = (total / (considered + dbrRentAmount)) * 100;
            }
            return dbrAmount.toFixed(2);
        });
        ////public decimal? LTVonTotalPresentMarketValue = ko.observable();
        self.FacilityRecomendation = ko.observable();
        //self.FacilityRecomendation = ko.computed(function () {
        //    var favoringNames = "";
        //    $.each(self.ClientProfiles(), function (index, value) {
        //        if (favoringNames.length > 0)
        //            favoringNames += ", ";
        //        favoringNames += value.Name();
        //    });
        //    if (self.FacilityType() === 4)
        //        return "Based n the avove terms and conditions, approval is sought for Retail Loan Secured (RLS) up to BDT. " + self.RecomendedLoanAmountFromIPDC() + " favoring " + favoringNames + " against total deposit of BDT. " + self.TotalAmountForFdr() + ".";
        //    else
        //        return "";
        //});
        self.FRRemarks = ko.observable();

        ////stress testing
        self.StressRates = ko.observableArray([]);//stressRate
        self.AddStressRate = function () {
            self.StressRates.push(new stressRate());
        };
        self.RemovedStressRates = ko.observableArray([]);
        self.RemoveStressRate = function (line) {
            if (line.Id() > 0)
                self.RemovedStressRates.push(line.Id());
            self.StressRates.remove(line);
        };
        //self.StressRate1 = ko.observable(new stressRate());
        //self.AddOneStressRate = function () {
        //    var i = 0;
        //    for (i = 0; i < 5 ; i++) {
        //        var aStress = new stressRate();
        //        var rate = parseFloat(self.InterestRateOffered() ? self.InterestRateOffered() : 0) + 1;
        //        aStress.InterestRate(rate);
        //        //aStress.EMI = ko.observable();
        //        //var increase = 0;
        //        aStress.RecomendedLoanAmountFromIPDC(self.RecomendedLoanAmountFromIPDC());
        //        aStress.AppliedLoanTerm(self.AppliedLoanTerm());
        //        aStress.EMIofProposedLoan(self.EMIofProposedLoan());
        //        //increase += parseFloat(value.Amount() ? value.Amount() : 0);
        //        //aStress.Increase = ko.observable();
        //        aStress.DBR = ko.observable();
        //        aStress.DBRFlatPurchase = ko.observable();
        //        self.StressRates.push(aStress);
        //    }
        //};
        self.StressRateTest1Id = ko.observable('');
        self.StressRateTest1 = ko.computed(function () {
            var test = StressRateTest((self.InterestRateOffered()),
                self.RecomendedLoanAmountFromIPDC(),
                self.AppliedLoanTerm(),
                self.DBR(),
                self.DBRFlatPurchase(),
                self.EMIofProposedLoan(),
                1, self.LiabilityTotalEMI(),
                self.TotalCreditCard(),
                self.TotalMonthlyIncomeConsidered(),
                self.RentalIncome(),
                self.StressRateTest1Id(), self.ExpectedRentPercentage());
            return test;
        });
        self.StressRateTest2Id = ko.observable('');
        self.StressRateTest2 = ko.computed(function () {
            var test = StressRateTest((self.InterestRateOffered()),
                self.RecomendedLoanAmountFromIPDC(),
                self.AppliedLoanTerm(),
                self.DBR(),
                self.DBRFlatPurchase(),
                self.EMIofProposedLoan(),
                2, self.LiabilityTotalEMI(),
                self.TotalCreditCard(),
                self.TotalMonthlyIncomeConsidered(),
                self.RentalIncome(),
                self.StressRateTest2Id(), self.ExpectedRentPercentage());
            return test;
        });
        self.StressRateTest3Id = ko.observable('');
        self.StressRateTest3 = ko.computed(function () {
            var test = StressRateTest((self.InterestRateOffered()),
                self.RecomendedLoanAmountFromIPDC(),
                self.AppliedLoanTerm(),
                self.DBR(),
                self.DBRFlatPurchase(),
                self.EMIofProposedLoan(),
                3, self.LiabilityTotalEMI(),
                self.TotalCreditCard(),
                self.TotalMonthlyIncomeConsidered(),
                self.RentalIncome(),
                self.StressRateTest3Id(), self.ExpectedRentPercentage());
            return test;
        });
        self.StressRateTest4Id = ko.observable('');
        self.StressRateTest4 = ko.computed(function () {
            var test = StressRateTest((self.InterestRateOffered()),
                self.RecomendedLoanAmountFromIPDC(),
                self.AppliedLoanTerm(),
                self.DBR(),
                self.DBRFlatPurchase(),
                self.EMIofProposedLoan(),
                4, self.LiabilityTotalEMI(),
                self.TotalCreditCard(),
                self.TotalMonthlyIncomeConsidered(),
                self.RentalIncome(),
                self.StressRateTest4Id(), self.ExpectedRentPercentage());
            return test;
        });
        self.StressRateTest5Id = ko.observable('');
        self.StressRateTest5 = ko.computed(function () {
            var test = StressRateTest((self.InterestRateOffered()),
                self.RecomendedLoanAmountFromIPDC(),
                self.AppliedLoanTerm(),
                self.DBR(),
                self.DBRFlatPurchase(),
                self.EMIofProposedLoan(),
                5, self.LiabilityTotalEMI(),
                self.TotalCreditCard(),
                self.TotalMonthlyIncomeConsidered(),
                self.RentalIncome(),
                self.StressRateTest5Id(), self.ExpectedRentPercentage());
            return test;
        });

        self.StressRemarks = ko.observable();
        self.RecomendedLoanAmountCommaSeperated = ko.pureComputed({
            read: function () {
                if (self.RecomendedLoanAmountFromIPDC() > 0)
                    return self.RecomendedLoanAmountFromIPDC().toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');         
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.RecomendedLoanAmountFromIPDC(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        //self.RecomendedLoanAmountFromIPDC(self.CommaSeperatedAmount());
        self.ApproveProposal = function () {
            if (self.Id() > 0) {
                var submitData = {
                    Id: self.Id()
                };
                $.ajax({
                    url: '/IPDC/CRM/SaveProposalApproval',
                    type: 'POST',
                    contentType: 'application/json',
                    data: ko.toJSON(submitData),
                    success: function (data) {
                        $('#lonSuccessModal').modal('show');
                        $('#lonSuccessModalText').text(data.Message);

                    },
                    error: function (error) {
                        alert(error.status + "<--and--> " + error.statusText);
                    }
                });
            } else {
                $('#lonSuccessModal').modal('show');
                $('#lonSuccessModalText').text("Please Save Proposal First");
            }
        }
        self.Print = function () {
            if (self.Id() > 0) {
                var url = "";
                if (self.FacilityType() === 4)
                    url = "/IPDC/CRM/RLSReport?reportTypeId=PDF&ProposalId=" + self.Id();
                else
                    url = "/IPDC/CRM/CreditMemoReport?reportTypeId=PDF&ProposalId=" + self.Id();
                window.open(url, '_blank');
            } else {
                $('#lonSuccessModal').modal('show');
                $('#lonSuccessModalText').text("Enable to find proposal");
            }
        };
        self.setUrl = ko.computed(function () {
            if (self.Id() > 0) {
                self.Link1('/IPDC/CRM/CreditMemoReport?reportTypeId=PDF&ProposalId=' + self.Id());
                self.Link2('/IPDC/CRM/CreditMemoReport?reportTypeId=Excel&ProposalId=' + self.Id());
                self.Link3('/IPDC/CRM/CreditMemoReport?reportTypeId=Word&ProposalId=' + self.Id());
            }
        });
        self.PreparedBy = ko.observable();
        self.ReviewedBy = ko.observable();
        self.RecommendedBy = ko.observable();

        self.ExpiryDate = ko.observable();
        self.ExpiryDateTxt = ko.observable();
        self.LoantoDepositRatio = ko.observable();
        self.TotalAmountForFdr = ko.computed(function () {
            var totalAmount = 0;
            $.each(self.FDRs(), function (index, value) {
                totalAmount += parseFloat(value.Amount() ? value.Amount() : 0);
            });
            return totalAmount;
        });
        self.LoanForRLS = ko.computed(function () {
            var result = 0;
            var amt = parseFloat(self.TotalAmountForFdr() ? self.TotalAmountForFdr() : 0);
            var ratio = parseFloat(self.LoantoDepositRatio() ? self.LoantoDepositRatio() : 0);
            if (ratio > 0) {
                result = (amt * ratio) / 100;
                self.RecomendedLoanAmountFromIPDC(Math.round(result));
            }
            
        });
        self.WeightedAverageRate = ko.computed(function () {
            var totalAmount = 0;
            var amountAndRate = 0;
            var result = 0.0;
            $.each(self.FDRs(), function (index, value) {
                totalAmount += parseFloat(value.Amount() ? value.Amount() : 0);
                amountAndRate += parseFloat((value.Amount() ? value.Amount() : 0) * (value.Rate() ? value.Rate() : 0));
            });
            result = amountAndRate / totalAmount;
            return result.toFixed(2);
        });
        self.Spread = ko.computed(function () {
            return parseFloat(self.InterestRateOffered()) - parseFloat(self.WeightedAverageRate());
        });
        self.DulyDischargedFdr = ko.observable(false);
        self.LetterOfLienSetOff = ko.observable(false);
        self.DemandPromissoryNote = ko.observable(false);
        self.SingedLoanApplication = ko.observable(false);
        self.Imperfections = ko.observable();
        self.PDCRemarks = ko.observable();
        self.ExistingEMI = ko.observable();

        //disbursement conditions
        self.DisbursementConditionsTexts = ko.observableArray([]);
        self.AddDisbursementCondition = function () {
            var disbursementCondition = new texts();
            disbursementCondition.Type(5);
            self.DisbursementConditionsTexts.push(disbursementCondition);
        }
        //self.RemovedDisbursementConditions = ko.observableArray([]);
        self.RemoveDisbursementCondition = function (line) {
            if (line.Id() > 0)
                self.RemovedAssetBackup.push(line.Id());
            self.DisbursementConditionsTexts.remove(line);
        }

        //disbursement mode
        self.ModeOfDisbursementTexts = ko.observableArray([]);
        self.AddDisbursementMode = function () {
            var disbursementMode = new texts();
            disbursementMode.Type(4);
            self.ModeOfDisbursementTexts.push(disbursementMode);
        }
        //self.RemovedDisbursementModes = ko.observableArray([]);
        self.RemoveDisbursementMode = function (line) {
            if (line.Id() > 0)
                self.RemovedAssetBackup.push(line.Id());
            self.ModeOfDisbursementTexts.remove(line);
        }

        self.ExceptionsTexts = ko.observableArray([]);
        self.AddExceptionsText = function () {
            var exceptionsText = new texts();
            exceptionsText.Type(6);
            self.ExceptionsTexts.push(exceptionsText);
        }
        //self.RemovedExceptionsTexts = ko.observableArray([]);
        self.RemoveExceptionsText = function (line) {
            if (line.Id() > 0)
                self.RemovedAssetBackup.push(line.Id());
            self.ExceptionsTexts.remove(line);
        }

        self.AddCreditCard = function () {
            var crd = new proposalCrdCard();
            self.ProposalCreditCards.push(crd);
        }
        self.RemovedCreditCards = ko.observableArray([]);
        self.RemoveCreditCard = function (line) {
            if (line.Id() > 0)
                self.RemovedCreditCards.push(line.Id());
            self.ProposalCreditCards.remove(line);
        }
        self.ApprovalAuthorityLevel = ko.observable();
        self.AuthorityLevel = ko.observable();
        self.AuthorityLevelList = ko.observableArray([]);
        self.GetApprovalAuthorityLevel = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/CRM/GetAuthorityLevel',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.AuthorityLevelList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.FacilityList = ko.observableArray([]);
        self.GetFacilityType = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/CRM/GetFacilityType',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.FacilityList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.SignatoryListForProp = ko.observableArray([]);
        self.GetAllSignatories = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/CRM/GetAllSignatories',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.SignatoryListForProp(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.FacilityRecommandationText = ko.computed(function () {

            var text = "We are requesting for final approval of ";
            if (self.FacilityTypeName() != null) {
                text += self.FacilityTypeName();
            }
            text += " of BDT ";
            if (self.RecomendedLoanAmountFromIPDC() >0) {
                text += self.RecomendedLoanAmountFromIPDC();
            }
            text += " @";
            if (self.InterestRateOffered() > 0) {
                text += self.InterestRateOffered();
            }
            text += "% for ";
            if (self.AppliedLoanTerm() >0) {
                text += self.AppliedLoanTerm();
            }
            text += " months in favor of ";
            var len = self.ClientProfiles().length;

           
            if (self.ClientProfiles().length > 0) {
                $.each(self.ClientProfiles(), function (index, value) {
                    if (typeof (value) != 'undefined') {
                        len = len - 1;
                        text += value.Name();
                        if (len > 1) {
                            text += " ,";
                        } if (len === 1) {
                            text += " & ";
                        }
                    }
                });
            }

            self.FacilityRecomendation(text);
        });

        self.CountryList = ko.observableArray([]);
        self.GetCountry = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Address/GetCountries',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CountryList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }


        self.DeveloperCatagoryList = ko.observableArray([]);
        self.GetDeveloperCatagory = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/CRM/GetDeveloperCategory',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.DeveloperCatagoryList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.GetVerificationStates = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/CRM/GetVerificationStates',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.AssessmentStatusList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.GetRelationshipWithApplicant = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/CRM/GetRelationshipWithApplicant',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.RelationshipsWithApplicant(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.SignatoryList = ko.observableArray([]);
        self.PrinterFilteringList = ko.observableArray([]);
        self.GetPrinterFiltering = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/CRM/GetPrinterFiltering',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.PrinterFilteringList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.ApprovalAuthoritySignatoryId = ko.observable();
        self.ExistingCifIds = ko.computed(function () {
            var list = [];
            $.each(self.ClientProfiles(), function (index, value) {
                if (value.CIFPId() > 0)
                    list.push(value.CIFPId());
            });
            $.each(self.Guarantors(), function (index, value) {
                if (value.GuarantorCifId() > 0)
                    list.push(value.GuarantorCifId());
            });

            return list;
        });

        self.GetCIFList = function (searchTerm, callback) {
            var submitData = {
                prefix: searchTerm,
                exclusionList: self.ExistingCifIds()
            };
            $.ajax({
                type: "POST",
                url: '/IPDC/CIF/GetCIFPListForAutoFill',
                data: ko.toJSON(submitData),
                contentType: "application/json",
                success: function () {
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            }).done(callback);
        };
        self.RejectProposal = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/CRM/RejectProposal?id=' + self.Id(),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    $('#lonSuccessModal').modal('show');
                    $('#lonSuccessModalText').text(data.Message);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.ViewCardRate = function () {
            return '/IPDC/Application/Download';//+ '_blank';
            //window.open(url, '_blank');
        }
        self.DiscardProposal = function() {
            if (self.Id() > 0) {
                return $.ajax({
                    type: "GET",
                    url: '/IPDC/CRM/DiscardCreditMemo?id='+self.Id(),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        $('#lonSuccessModal').modal('show');
                        $('#lonSuccessModalText').text(data.Message);
                        if (data.Id > 0) {
                            self.Id('');
                            self.ApplicationId(data.Id);
                            self.LoadProposaLData();
                        }
                    },
                    error: function (error) {
                        alert(error.status + "<--and--> " + error.statusText);
                    }
                });
                //if (self.ApplicationId() >0) {
                //    self.LoadProposaLData();
                //}
            }
        }
        self.LoadProposaLData = function () {
            if (self.ApplicationId() > 0 || self.Id() > 0) {
                $.getJSON("/IPDC/CRM/LoadProposalByAppId/?AppId=" + self.ApplicationId() + '&Id=' + self.Id(),
                    null,
                    function (data) {
                        self.SecurityDetails([]);
                        self.ProposalCreditCards([]);
                        self.ConsideredIncome([]);
                        self.NotConsideredIncome([]);
                        self.NetWorths([]);
                        self.CIBs([]);
                        self.Liabilities([]);
                        self.Guarantors([]);
                        self.AssetBackupTexts([]);
                        self.Strengths([]);
                        self.Weakness([]);
                        self.ModeOfDisbursementTexts([]);
                        self.ExceptionsTexts([]);
                        self.DisbursementConditionsTexts([]);
                        self.FDRs([]);
                        self.OtherCosts([]);
                        self.OtherValuationCosts([]);
                        self.OverallAssessments([]);
                        self.SignatoryList([]);
                        self.ClientProfiles([]);


                        self.Id(data.Id);
                        self.PropertyType(data.PropertyType + "");
                        self.ApplicationId(data.ApplicationId);
                        self.ApplicationReceiveDate(moment(data.ApplicationReceiveDate));
                        self.ApprovalAuthoritySignatoryId(data.ApprovalAuthoritySignatoryId);
                        self.ApplicationNo(data.ApplicationNo);
                        self.ProposalDate(moment(data.ProposalDate));
                        self.CRMReceiveDate(moment(data.CRMReceiveDate));
                        self.RMName(data.RMName);
                        self.BranchName(data.BranchName);//self.GetFacilityType()
                        self.LoanPaymentToDeveloper(data.LoanPaymentToDeveloper);
                        $.when(self.GetFacilityType()).done(function () {
                            self.FacilityType(data.FacilityType);
                            self.FacilityTypeName(data.FacilityTypeName);
                        });
                        self.AppliedLoanAmount(data.AppliedLoanAmount);
                        self.AppliedLoanTerm(data.AppliedLoanTerm);
                        self.AppliedLoanTermApplication(data.AppliedLoanTermApplication);
                        self.CurrentExposureWithIPDC(data.CurrentExposureWithIPDC);
                        self.TLComment(data.TLComment);
                        self.BMComment(data.BMComment);
                        //self.TotalExposureWithIPDC(data.TotalExposureWithIPDC);
                        self.InterestRateCard(data.InterestRateCard);
                        self.InterestRateOffered(data.InterestRateOffered);
                        self.SecurityRemarks(data.SecurityRemarks);
                        self.RateVariance(data.RateVariance);
                        self.LoanRemarks(data.LoanRemarks);
                        self.NetWorthsRemarks(data.NetWorthsRemarks);
                        self.ProcessingFeeAndDocChargesCardRate(data.ProcessingFeeAndDocChargesCardRate);
                        //ClientProfiles: clientDetails,
                        $.when(self.GetRelationshipWithApplicant()).done(function () {
                            $.when(self.GetCountry()).done(function () {
                                $.when(self.GetHomeOwnerships()).done(function () {
                                    $.each(data.ClientProfiles,
                                        function (index, value) {
                                            var aDetail = new clientProfile();
                                            if (typeof (value) != 'undefined') {
                                                aDetail.LoadData(value);
                                                self.ClientProfiles.push(aDetail);
                                            }
                                        });
                                    self.ProjectAddress.LoadAddress(data.ProjectAddress != null ? data.ProjectAddress : "");
                                });
                            });
                            $.each(data.Guarantors,
                                function (index, value) {
                                    var aDetail = new guarantors();
                                    if (typeof (value) != 'undefined') {
                                        aDetail.LoadData(value);
                                        self.Guarantors.push(aDetail);
                                    }
                                });
                        });
                        //self.SecurityDetails.
                        $.each(data.SecurityDetails,
                                    function (index, value) {
                                        var aDetail = new securityDetail();
                                        if (typeof (value) != 'undefined') {
                                            aDetail.LoadData(value);
                                            self.SecurityDetails.push(aDetail);
                                        }
                                    });
                        $.each(data.ProposalCreditCards,
                                 function (index, value) {
                                     var aDetail = new proposalCrdCard();
                                     if (typeof (value) != 'undefined') {
                                         aDetail.LoadData(value);
                                         self.ProposalCreditCards.push(aDetail);
                                     }
                                 });
                        self.LoanPurpose(data.LoanPurpose);
                        //Incomes: incomeDetails,
                        $.each(data.Incomes,
                                         function (index, value) {
                                             var aDetail = new income();
                                             if (typeof (value) != 'undefined') {
                                                 aDetail.LoadData(value);
                                                 if (value.IsConsidered === true) {
                                                     self.ConsideredIncome.push(aDetail);
                                                 } else {
                                                     self.NotConsideredIncome.push(aDetail);
                                                 }
                                             }

                                         });
                        //self.TotalMonthlyIncomeConsidered(data.TotalMonthlyIncomeConsidered);
                        //self.TotalMonthlyIncomeNotConsidered(data.TotalMonthlyIncomeNotConsidered);
                        //self.LiabilityTotalEMI(data.LiabilityTotalEMI);
                        //self.EMIofProposedLoan(data.EMIofProposedLoan);
                        //self.FreeCash(data.FreeCash);
                        //self.DBR(data.DBR); ko computed
                        //self.IncomeAssessmentRemarks(data.IncomeAssessmentRemarks);
                        //self.NetWorths: netWorthDetails,
                        $.each(data.NetWorths,
                                      function (index, value) {
                                          var aDetail = new netWorth();
                                          if (typeof (value) != 'undefined') {
                                              aDetail.LoadData(value);
                                              self.NetWorths.push(aDetail);
                                          }
                                      });
                        //CIBs: cibDetails,
                        $.when(self.GetCIBClassificationStatuses()).done(function () {
                            $.each(data.CIBs,
                            function (index, value) {
                                var aDetail = new cibs();
                                if (typeof (value) != 'undefined') {
                                    aDetail.LoadData(value);
                                    self.CIBs.push(aDetail);
                                }
                            });
                        });

                        //Liabilities: liabilitiesDetails,
                        $.when(self.GetFacilityType()).done(function () {
                            $.each(data.Liabilities,
                            function (index, value) {
                                var aDetail = new liabilities();
                                if (typeof (value) != 'undefined') {
                                    aDetail.LoadData(value);
                                    self.Liabilities.push(aDetail);
                                }
                            });
                        });
                        //self.TotalLimit(data.TotalLimit);
                        //self.TotalOutstanding(data.TotalOutstanding);

                        self.LiabilityRemarks(data.LiabilityRemarks);
                        //Guarantors(data.guarantorsDetails);

                        self.GuarantorRemarks(data.GuarantorRemarks);
                        //Texts(data.AssetBackupTexts);self.GetPrinterFiltering
                        $.when(self.GetPrinterFiltering()).done(function () {
                            $.each(data.Texts,
                            function (index, value) {
                                var aDetail = new texts();
                                if (typeof (value) != 'undefined') {
                                    aDetail.LoadData(value);
                                    if (value.Type === 1) {
                                        self.AssetBackupTexts.push(aDetail);
                                    }
                                    if (value.Type === 2) {
                                        self.Strengths.push(aDetail);
                                    }
                                    if (value.Type === 3) {
                                        self.Weakness.push(aDetail);
                                    }
                                    if (value.Type === 4) {
                                        self.ModeOfDisbursementTexts.push(aDetail);
                                    }
                                    if (value.Type === 6) {
                                        self.ExceptionsTexts.push(aDetail);
                                    }
                                    if (value.Type === 5) {
                                        self.DisbursementConditionsTexts.push(aDetail);
                                    }
                                }
                            });
                        });
                        self.AssetBackupRemarks(data.AssetBackupRemarks);
                        self.CommentOnBankStatement(data.CommentOnBankStatement);
                        self.PDCBankName(data.PDCBankName);
                        self.PDCBankBranch(data.PDCBankBranch);
                        self.PDCRoutingNo(data.PDCRoutingNo);
                        self.PDCAccountTitle(data.PDCAccountTitle);
                        self.PDCAccountType(data.PDCAccountType);
                        self.PDCAccountNo(data.PDCAccountNo);
                        self.Product(data.Product);
                        self.Vehicle_Name(data.Vehicle_Name);
                        self.Vehicle_ModelYear(data.Vehicle_ModelYear);
                        self.Vehicle_VendorName(data.Vehicle_VendorName);
                        self.Vehicle_QuotedPrice(data.Vehicle_QuotedPrice);
                        self.CC(data.CC);
                        self.Colour(data.Colour);
                        self.ChassisNo(data.ChassisNo);
                        self.EngineNo(data.EngineNo);
                        self.PresentMarketValue(data.PresentMarketValue);
                        self.RecomendedLoanAmountFromIPDC(data.RecomendedLoanAmountFromIPDC); // non editable
                        //self.LTVonTotalPresentMarketValue(data.LTVonTotalPresentMarketValue);
                        self.Product_Remarks(data.Product_Remarks);
                        self.PropertyRemarks(data.PropertyRemarks);
                        //FDRs: fdrDetails,
                        $.each(data.FDRs,
                         function (index, value) {
                             var aDetail = new fdr();
                             if (typeof (value) != 'undefined') {
                                 aDetail.LoadData(value);
                                 self.FDRs.push(aDetail);
                             }
                         });
                        self.CG_Item(data.CG_Item);
                        self.CG_Brand(data.CG_Brand);
                        self.CG_DealerName(data.CG_DealerName); //dealer or showroom
                        self.CG_Price(data.CG_Price);
                        self.FDR_Remarks(data.FDR_Remarks);
                        self.SellersName(data.SellersName);
                        self.DevelopersName(data.DevelopersName);
                        self.ProjectName(data.ProjectName);
                        self.ProjectStatus(data.ProjectStatus);
                        $.when(self.GetDeveloperCatagory()).done(function () {
                            self.DeveloperCategory(data.DeveloperCategory);
                        });
                        self.TotalNumberOfFloors(data.TotalNumberOfFloors);
                        self.TotalNumberOfSlabsCasted(data.TotalNumberOfSlabsCasted);
                        self.TotalLandArea(data.TotalLandArea);
                        self.FlatDetails(data.FlatDetails);
                        self.NumberOfCarParking(data.NumberOfCarParking);
                        self.FlatSize(data.FlatSize);
                        self.PropertyOwnershipType(data.PropertyOwnershipType + '');
                        self.IsPermissionRequired(data.IsPermissionRequired);
                        self.PermissionRemarks(data.PermissionRemarks);
                        self.PropertyOwnerName(data.PropertyOwnerName);
                        self.OwnConstructionLoanPurpose(data.OwnConstructionLoanPurpose);

                        self.PricePerSQF(data.PricePerSQF);
                        //self.MarketPriceOfFlat(data.MarketPriceOfFlat);
                        self.CarParkingPrice(data.CarParkingPrice);
                        self.RegistrationCost(data.RegistrationCost);
                        //self.TotalMarketValue(data.TotalMarketValue);
                        //self.DistressedValue(data.DistressedValue);
                        self.LandedPropertyValuationRemarks(data.LandedPropertyValuationRemarks);
                        self.PerKathaPriceAsPerRAJUK(data.PerKathaPriceAsPerRAJUK);
                        //self.MarketValueOfLandAsPerRAJUK(data.MarketValueOfLandAsPerRAJUK);
                        self.LPVConstructionDetails(data.LPVConstructionDetails);
                        self.EstimatedConstructionCostApproved(data.EstimatedConstructionCostApproved);
                        self.TotalConstructionAmountDetails(data.TotalConstructionAmountDetails);
                        //TotalAmount:self.TotalAmount(),
                        self.LPFPConstructionDetails(data.LPFPConstructionDetails);
                        //OtherCosts: otherCostDetails,ProposalCreditCards
                        $.each(data.OtherCosts,
                        function (index, value) {
                            var aDetail = new otherCosts();
                            if (typeof (value) != 'undefined') {
                                aDetail.LoadData(value);
                                self.OtherCosts.push(aDetail);
                            }
                        });
                        $.each(data.ValuationOtherCosts,
                       function (index, value) {
                           var aDetail = new otherCosts();
                           if (typeof (value) != 'undefined') {
                               aDetail.LoadData(value);
                               self.OtherValuationCosts.push(aDetail);
                           }
                       });
                        //self.FlatFinanceTotalAmount(data.FlatFinanceTotalAmount);
                        self.LPFPAlreadyPaid(data.LPFPAlreadyPaid);
                        //RecomendedLoanAmountFromIPDC :self
                        //self.AdditionalEquity2bInvestedByApplicant(data.AdditionalEquity2bInvestedByApplicant);
                        self.LPFinancingPlanRemarks(data.LPFinancingPlanRemarks);
                        self.ConstructionCostFinancingPlan(data.ConstructionCostFinancingPlan);
                        //self.NetAmountForConstruction(data.NetAmountForConstruction);
                        //OverallAssessments: overallAssessmentDetails,
                        $.when(self.GetVerificationStates()).done(function () {
                            $.each(data.OverallAssessments,
                            function (index, value) {
                                var aDetail = new overallAssessment();
                                if (typeof (value) != 'undefined') {
                                    aDetail.LoadData(value);
                                    self.OverallAssessments.push(aDetail);
                                }
                            });
                        });
                        //public ProposalFacilityType? FacilityType :self
                        //public int AppliedLoanTerm :self
                        //public decimal InterestRateOffered :self
                        self.ProcessingFeeAndDocChargesPercentage(data.ProcessingFeeAndDocChargesPercentage);
                        self.ProcessingFeeAndDocChargesAmount(data.ProcessingFeeAndDocChargesAmount);
                        self.TotalExpences(data.TotalExpences);
                        //self.DBRFlatPurchase(data.DBRFlatPurchase);
                        self.ExpectedRentPercentage(data.ExpectedRentPercentage);
                        self.FacilityRecomendation(data.FacilityRecomendation);
                        self.FRRemarks(data.FRRemarks);
                        if (data.StressRates) {
                            if (data.StressRates[0])
                                self.StressRateTest1Id(data.StressRates[0].Id);
                            if (data.StressRates[1])
                                self.StressRateTest2Id(data.StressRates[1].Id);
                            if (data.StressRates[2])
                                self.StressRateTest3Id(data.StressRates[2].Id);
                            if (data.StressRates[3])
                                self.StressRateTest4Id(data.StressRates[3].Id);
                            if (data.StressRates[4])
                                self.StressRateTest5Id(data.StressRates[4].Id);
                        }
                        //StressRates: stressRateDetails,
                        //$.each(data.StressRates,
                        //function (index, value) {
                        //    var aDetail = new stressRate();
                        //    if (typeof (value) != 'undefined') {
                        //        aDetail.LoadData(value);
                        //        self.StressRates.push(aDetail);
                        //    }
                        //});
                        self.StressRemarks(data.StressRemarks);
                        $.when(self.GetApprovalAuthorityLevel()).done(function () {
                            self.ApprovalAuthorityLevel(data.ApprovalAuthorityLevel);
                        });
                        //signatory SignatoryListForProp GetAllSignatories
                        $.when(self.GetAllSignatories()).done(function () {
                            $.each(data.Signatories,
                           function (index, value) {
                               var aDetail = new signatory();
                               if (typeof (value) != 'undefined') {
                                   aDetail.LoadData(value);
                                   self.SignatoryList.push(aDetail);
                               }
                           });
                        });
                        self.RentalIncome(data.RentalIncome);
                        self.PreparedBy(data.PreparedBy);
                        self.ReviewedBy(data.ReviewedBy);
                        self.RecommendedBy(data.RecommendedBy);

                        //self.Spread(data.Spread);
                        self.ExpiryDate(data.ExpiryDate ? moment(data.ExpiryDate) : moment());
                        self.ExpiryDateTxt(data.ExpiryDateTxt);
                        self.LoantoDepositRatio(data.LoantoDepositRatio);
                        self.DulyDischargedFdr(data.DulyDischargedFdr);
                        self.LetterOfLienSetOff(data.LetterOfLienSetOff);
                        self.DemandPromissoryNote(data.DemandPromissoryNote);
                        self.SingedLoanApplication(data.SingedLoanApplication);
                        self.Imperfections(data.Imperfections);
                        self.PDCRemarks(data.PDCRemarks);
                        self.ExistingEMI(data.ExistingEMI);
                        //$.when(self.GetGuarantorCifLists())
                        //.done(function () {
                        //      $.when(self.GetRelationships())
                        //          .done(function() {
                        //              $.each(data.Guarantors,
                        //                  function(index, value) {
                        //                      var aDetail = new guarantor();
                        //                      if (typeof (value) != 'undefined') {
                        //                          aDetail.LoadData(value);
                        //                          self.Guarantors.push(aDetail);
                        //                      }

                        //                  });
                        //          });
                        //  });
                        //$.when(self.GetProjectsByDevelopers())
                        //    .done(function () {
                        //        self.ProjectId(data.LPPrimarySecurity.ProjectId);
                        //    });
                    });
            };

        };
        self.SaveProposal = function () {
            self.ExpiryDateTxt($("#ExpiryDate").val());
            var clientDetails = ko.observableArray([]);
            $.each(self.ClientProfiles(),
            function (index, value) {
                clientDetails.push({
                    Id: value.Id(),
                    ProposalId: value.ProposalId(),
                    CIFPId: value.CIFPId(),
                    CIFOId: value.CIFOId(),
                    ApplicantRole: value.ApplicantRole(),
                    ApplicantRoleName: value.ApplicantRoleName(),
                    CIFNo: value.CIFNo(),
                    Name: value.Name(),
                    NID: value.NID(),
                    RelationshipWithApplicant: value.RelationshipWithApplicant(),
                    RelationshipWithApplicantName: value.RelationshipWithApplicantName(),
                    DateOfBirth: value.DateOfBirth(),
                    Age: value.Age(),
                    DateOfBirthTxt: moment(value.DateOfBirth()).format("DD/MM/YYYY"),
                    AcademicQualification: value.AcademicQualification(),
                    ProfessionName: value.ProfessionName(),
                    Designation: value.Designation(),
                    OrganizationName: value.OrganizationName(),
                    ExperienceDetails: value.ExperienceDetails(),
                    ResidenceStatus: value.ResidenceStatus(),
                    OfficeAddress: value.OfficeAddress,
                    PresentAddress: value.PresentAddress,
                    PermanentAddress: value.PermanentAddress
                });
            });
            var incomeDetails = ko.observableArray([]);
            $.each(self.ConsideredIncome(),
            function (index, value) {
                incomeDetails.push({
                    Id: value.Id(),
                    ProposalId: value.ProposalId(),
                    CIFNo: value.CIFNo(),
                    Name: value.Name(),
                    ApplicantRole: value.ApplicantRole(),
                    ApplicantRoleName: value.ApplicantRoleName(),
                    ConsideredPercentage: value.ConsideredPercentage(),
                    ConsideredAmount: value.ConsideredAmount(),
                    IsConsidered: value.IsConsidered(),
                    IncomeSource: value.IncomeSource(),
                    IncomeAmount: value.IncomeAmount(),
                    Remarks: value.Remarks()
                });
            });
            $.each(self.NotConsideredIncome(),
               function (index, value) {
                   incomeDetails.push({
                       Id: value.Id(),
                       ProposalId: value.ProposalId(),
                       CIFNo: value.CIFNo(),
                       Name: value.Name(),
                       ApplicantRole: value.ApplicantRole(),
                       ApplicantRoleName: value.ApplicantRoleName(),
                       IsConsidered: value.IsConsidered(),
                       IncomeSource: value.IncomeSource(),
                       IncomeAmount: value.IncomeAmount()
                   });
               });
            var netWorthDetails = ko.observableArray([]);
            $.each(self.NetWorths(),
            function (index, value) {
                netWorthDetails.push({
                    Id: value.Id(),
                    ProposalId: value.ProposalId(),
                    CIFNo: value.CIFNo(),
                    Name: value.Name(),
                    ClientRole: value.ClientRole(),
                    ClientRoleName: value.ClientRoleName(),
                    TotalAssetOfApplicant: value.TotalAssetOfApplicant(),
                    TotalLiabilityOfApplicant: value.TotalLiabilityOfApplicant(),
                    NetWorthOfApplicant: value.NetWorthOfApplicant()
                });
            });

            var securities = ko.observableArray([]);
            $.each(self.SecurityDetails(),
            function (index, value) {
                securities.push({
                    Id: value.Id(),
                    ProposalId: value.ProposalId(),
                    SecurityType: value.SecurityType(),
                    SecurityTypeName: value.SecurityTypeName(),
                    Details: value.Details()
                });
            });

            var cibDetails = ko.observableArray([]);
            $.each(self.CIBs(),
            function (index, value) {
                cibDetails.push({
                    Id: value.Id(),
                    ProposalId: value.ProposalId(),
                    CIFNo: value.CIFNo(),
                    ClientRole: value.ClientRole(),
                    ClientRoleName: value.ClientRoleName(),
                    Name: value.Name(),
                    CIBStatus: value.CIBStatus(),
                    CIBStatusName: value.CIBStatusName(),
                    CIBDate: value.CIBDate(),
                    //CIBDateTxt: value.CIBDateTxt(),
                    CIBDateTxt: moment(value.CIBDate()).format("DD/MM/YYYY"),
                    TotalOutstandingAsBorrower: value.TotalOutstandingAsBorrower(),
                    ClassifiedAmountAsBorrower: value.ClassifiedAmountAsBorrower(),
                    TotalEMIAsBorrower: value.TotalEMIAsBorrower()
                });
            });
            var liabilitiesDetails = ko.observableArray([]);
            $.each(self.Liabilities(),
            function (index, value) {
                liabilitiesDetails.push({
                    Id: value.Id(),
                    ProposalId: value.ProposalId(),
                    Name: value.Name(),
                    FacilityType: value.FacilityType(),
                    InstituteName: value.InstituteName(),
                    Limit: value.Limit(),
                    Outstanding: value.Outstanding(),
                    EMI: value.EMI(),
                    PaymentRecord: value.PaymentRecord(),
                    StartingDate: value.StartingDate(),
                    StartingDateTxt: moment(value.StartingDate()).format("DD/MM/YYYY"),
                    //StartingDateTxt: value.StartingDateTxt(),
                    ExpiryDate: value.ExpiryDate(),
                    //ExpiryDateTxt: value.ExpiryDateTxt()
                    ExpiryDateTxt: moment(value.ExpiryDate()).format("DD/MM/YYYY")
                });
            });
            var guarantorsDetails = ko.observableArray([]);
            $.each(self.Guarantors(),
            function (index, value) {
                guarantorsDetails.push({
                    Id: value.Id(),
                    ProposalId: value.ProposalId(),
                    GuarantorCifId: value.GuarantorCifId(),
                    Name: value.Name(),
                    ProfessionName: value.ProfessionName(),
                    CompanyName: value.CompanyName(),
                    Designation: value.Designation(),
                    RelationshipWithApplicant: value.RelationshipWithApplicant(),
                    RelationshipWithApplicantName: value.RelationshipWithApplicantName(),
                    Age: value.Age(),
                    MonthlyIncome: value.MonthlyIncome()
                });
            });
            var textsDetails = ko.observableArray([]);
            $.each(self.AssetBackupTexts(),
          function (index, value) {
              textsDetails.push({
                  Id: value.Id(),
                  ProposalId: value.ProposalId(),
                  Type: value.Type(),
                  Text: value.Text(),
                  IsPrintable: value.IsPrintable(),
                  PrinterFiltering: value.PrinterFiltering()
              });
          });
            $.each(self.Strengths(),
         function (index, value) {
             textsDetails.push({
                 Id: value.Id(),
                 ProposalId: value.ProposalId(),
                 Type: value.Type(),
                 Text: value.Text(),
                 IsPrintable: value.IsPrintable(),
                 PrinterFiltering: value.PrinterFiltering()
             });
         });
            $.each(self.Weakness(),
        function (index, value) {
            textsDetails.push({
                Id: value.Id(),
                ProposalId: value.ProposalId(),
                Type: value.Type(),
                Text: value.Text(),
                IsPrintable: value.IsPrintable(),
                PrinterFiltering: value.PrinterFiltering()
            });
        });
            $.each(self.ModeOfDisbursementTexts(),
                 function (index, value) {
                     textsDetails.push({
                         Id: value.Id(),
                         ProposalId: value.ProposalId(),
                         Type: value.Type(),
                         Text: value.Text(),
                         IsPrintable: value.IsPrintable(),
                         PrinterFiltering: value.PrinterFiltering()
                     });
                 });
            $.each(self.ExceptionsTexts(),
         function (index, value) {
             textsDetails.push({
                 Id: value.Id(),
                 ProposalId: value.ProposalId(),
                 Type: value.Type(),
                 Text: value.Text(),
                 IsPrintable: value.IsPrintable(),
                 PrinterFiltering: value.PrinterFiltering()
             });
         });
            $.each(self.DisbursementConditionsTexts(),
                function (index, value) {
                    textsDetails.push({
                        Id: value.Id(),
                        ProposalId: value.ProposalId(),
                        Type: value.Type(),
                        Text: value.Text(),
                        IsPrintable: value.IsPrintable(),
                        PrinterFiltering: value.PrinterFiltering()
                    });
                });
            //self.OtherCosts
            var otherCostDetails = ko.observableArray([]);
            $.each(self.OtherCosts(),
          function (index, value) {
              otherCostDetails.push({
                  Id: value.Id(),
                  ProposalId: value.ProposalId(),
                  Details: value.Details(),
                  Amount: value.Amount(),
                  Other: value.Other
              });
          });
            var otherValuationCostDetails = ko.observableArray([]);
            $.each(self.OtherValuationCosts(),
          function (index, value) {
              otherValuationCostDetails.push({
                  Id: value.Id(),
                  ProposalId: value.ProposalId(),
                  Details: value.Details(),
                  Amount: value.Amount(),
                  Other: value.Other
              });
          });
            var overallAssessmentDetails = ko.observableArray([]);
            $.each(self.OverallAssessments(),
              function (index, value) {
                  overallAssessmentDetails.push({
                      Id: value.Id(),
                      ProposalId: value.ProposalId(),
                      AssessmentParticulars: value.AssessmentParticulars(),
                      VerificationStatus: value.VerificationStatus(),
                      CIFName: value.CIFName(),
                      DoneBy: value.DoneBy(),
                      AssessmentDate: value.AssessmentDate(),
                      AssessmentDateTxt: moment(value.AssessmentDate()).format("DD/MM/YYYY"),
                      Remarks: value.Remarks()
                  });
              });
            //stressRate
            var stressRateDetails = ko.observableArray([]);
            stressRateDetails.push(self.StressRateTest1);
            stressRateDetails.push(self.StressRateTest2);
            stressRateDetails.push(self.StressRateTest3);
            stressRateDetails.push(self.StressRateTest4);
            stressRateDetails.push(self.StressRateTest5);
            //  $.each(self.StressRates(),
            //function (index, value) {
            //    stressRateDetails.push({
            //        Id: value.Id(),
            //        ProposalId: value.ProposalId(),
            //        InterestRate: value.InterestRate(),
            //        EMI: value.EMI(),
            //        Increase: value.Increase(),
            //        DBR: value.DBR(),
            //        DBRFlatPurchase: value.DBRFlatPurchase()
            //    });
            //});

            var fdrDetails = ko.observableArray([]);
            $.each(self.FDRs(),
          function (index, value) {
              fdrDetails.push({
                  Id: value.Id(),
                  ProposalId: value.ProposalId(),
                  InstituteName: value.InstituteName(),
                  BranchName: value.BranchName(),
                  FDRAccountNo: value.FDRAccountNo(),
                  Amount: value.Amount(),
                  DepositorName: value.DepositorName(),
                  MaturityDate: value.MaturityDate(),
                  Rate: value.Rate(),
                  //MaturityDateTxt: value.MaturityDateTxt()
                  MaturityDateTxt: moment(value.MaturityDate()).format("DD/MM/YYYY")
              });
          });//signatoryDetails
            var signatoryDetails = ko.observableArray([]);
            $.each(self.SignatoryList(),
          function (index, value) {
              signatoryDetails.push({
                  Id: value.Id(),
                  ProposalId: value.ProposalId,
                  Name: value.Name,
                  //valAuthoritySignatoryName: value.ApprovalAuthoritySignatoryName,
                  SignatoryId: value.SignatoryId
              });
          });//signatoryDetails
            var creditCards = ko.observableArray([]);
            $.each(self.ProposalCreditCards(),
            function (index, value) {
                creditCards.push({
                    Id: value.Id(),
                    ProposalId: value.ProposalId(),
                    CIFId: value.CIFId(),
                    CreditCardId: value.CreditCardId(),
                    CreditCardNo: value.CreditCardNo(),
                    CreditCardIssuersName: value.CreditCardIssuersName(),
                    CreditCardIssueDateTxt: moment(value.CreditCardIssueDate()).format("DD/MM/YYYY"),
                    CreditCardLimit: value.CreditCardLimit()
                });
            });
            var submitData = {
                Id: self.Id(),
                ApplicationId: self.ApplicationId(),
                ApplicationReceiveDate: self.ApplicationReceiveDate(),
                ApplicationReceiveDateText: moment(self.ApplicationReceiveDate()).format("DD/MM/YYYY"),
                ApplicationNo: self.ApplicationNo(),
                ProposalDate: self.ProposalDate(),
                ProposalDateText: moment(self.ProposalDate()).format("DD/MM/YYYY"),
                CRMReceiveDate: self.CRMReceiveDate(),
                CRMReceiveDateText: moment(self.CRMReceiveDate()).format("DD/MM/YYYY"),
                RMName: self.RMName(),
                BranchName: self.BranchName(),
                FacilityType: self.FacilityType(),
                AppliedLoanAmount: self.AppliedLoanAmount(),
                AppliedLoanTerm: self.AppliedLoanTerm(),
                AppliedLoanTermApplication :self.AppliedLoanTermApplication(),
                CurrentExposureWithIPDC: self.CurrentExposureWithIPDC(),
                TotalExposureWithIPDC: self.TotalExposureWithIPDC(),
                InterestRateCard: self.InterestRateCard(),
                InterestRateOffered: self.InterestRateOffered(),
                RateVariance: self.RateVariance(),
                LoanRemarks: self.LoanRemarks(),
                NetWorthsRemarks: self.NetWorthsRemarks(),
                PropertyRemarks: self.PropertyRemarks(),
                ProcessingFeeAndDocChargesCardRate: self.ProcessingFeeAndDocChargesCardRate(),
                ClientProfiles: clientDetails,
                LoanPurpose: self.LoanPurpose(),
                Incomes: incomeDetails,
                TotalMonthlyIncomeConsidered: self.TotalMonthlyIncomeConsidered(),
                TotalMonthlyIncomeNotConsidered: self.TotalMonthlyIncomeNotConsidered(),
                LiabilityTotalEMI: self.LiabilityTotalEMI(),
                EMIofProposedLoan: self.EMIofProposedLoan(),
                FreeCash: self.FreeCash(),
                DBR: self.DBR(),
                RentalIncome: self.RentalIncome(),
                //IncomeAssessmentRemarks: self.IncomeAssessmentRemarks(),
                NetWorths: netWorthDetails,
                CIBs: cibDetails,
                Liabilities: liabilitiesDetails,
                TotalLimit: self.TotalLimit(),
                TotalOutstanding: self.TotalOutstanding(),
                //LiabilityTotalEMI : self.LiabilityTotalEMI(),
                LiabilityRemarks: self.LiabilityRemarks(),
                Guarantors: guarantorsDetails,
                RemovedGuarantors: self.RemovedGuarantors(),
                GuarantorRemarks: self.GuarantorRemarks(),
                Texts: textsDetails, //self.AssetBackupTexts(),
                AssetBackupRemarks: self.AssetBackupRemarks(),
                CommentOnBankStatement: self.CommentOnBankStatement(),
                PDCBankName: self.PDCBankName(),
                PDCBankBranch: self.PDCBankBranch(),
                PDCRoutingNo: self.PDCRoutingNo(),
                PDCAccountTitle: self.PDCAccountTitle(),
                PDCAccountType: self.PDCAccountType(),
                PDCAccountNo: self.PDCAccountNo(),
                Product: self.Product(),
                Vehicle_Name: self.Vehicle_Name(),
                Vehicle_ModelYear: self.Vehicle_ModelYear(),
                Vehicle_VendorName: self.Vehicle_VendorName(),
                Vehicle_QuotedPrice: self.Vehicle_QuotedPrice(),
                CC: self.CC(),
                Colour: self.Colour(),
                ChassisNo: self.ChassisNo(),
                EngineNo: self.EngineNo(),
                PresentMarketValue: self.PresentMarketValue(),
                RecomendedLoanAmountFromIPDC: self.RecomendedLoanAmountFromIPDC(), // non editable
                LTVonTotalPresentMarketValue: self.LTVonTotalPresentMarketValue(),
                Product_Remarks: self.Product_Remarks(),
                FDRs: fdrDetails,
                CG_Item: self.CG_Item(),
                CG_Brand: self.CG_Brand(),
                CG_DealerName: self.CG_DealerName(), //dealer or showroom
                CG_Price: self.CG_Price(),
                PropertyType: self.PropertyType(),
                FDR_Remarks: self.FDR_Remarks(),
                SellersName: self.SellersName(),
                DevelopersName: self.DevelopersName(),
                ProjectName: self.ProjectName(),
                ProjectStatus: self.ProjectStatus(),
                DeveloperCategory: self.DeveloperCategory(),
                TotalNumberOfFloors: self.TotalNumberOfFloors(),
                TotalNumberOfSlabsCasted: self.TotalNumberOfSlabsCasted(),
                TotalLandArea: self.TotalLandArea(),
                FlatDetails: self.FlatDetails(),
                NumberOfCarParking: self.NumberOfCarParking(),
                FlatSize: self.FlatSize(),
                PropertyOwnershipType: self.PropertyOwnershipType(),
                IsPermissionRequired: self.IsPermissionRequired(),
                PermissionRemarks: self.PermissionRemarks(),
                PropertyOwnerName: self.PropertyOwnerName(),
                OwnConstructionLoanPurpose: self.OwnConstructionLoanPurpose(),
                ProjectAddress: self.ProjectAddress,
                PricePerSQF: self.PricePerSQF(),
                MarketPriceOfFlat: self.MarketPriceOfFlat(),
                CarParkingPrice: self.CarParkingPrice(),
                RegistrationCost: self.RegistrationCost(),
                TotalMarketValue: self.TotalMarketValue(),
                DistressedValue: self.DistressedValue(),
                LandedPropertyValuationRemarks: self.LandedPropertyValuationRemarks(),
                PerKathaPriceAsPerRAJUK: self.PerKathaPriceAsPerRAJUK(),
                MarketValueOfLandAsPerRAJUK: self.MarketValueOfLandAsPerRAJUK(),
                LPVConstructionDetails: self.LPVConstructionDetails(),
                EstimatedConstructionCostApproved: self.EstimatedConstructionCostApproved(),
                TotalConstructionAmountDetails: self.TotalConstructionAmountDetails(),
                TotalAmount: self.TotalAmount(),
                LPFPConstructionDetails: self.LPFPConstructionDetails(),
                OtherCosts: otherCostDetails,
                ValuationOtherCosts: otherValuationCostDetails,
                FlatFinanceTotalAmount: self.FlatFinanceTotalAmount(),
                LPFPAlreadyPaid: self.LPFPAlreadyPaid(),
                //RecomendedLoanAmountFromIPDC :self
                AdditionalEquity2bInvestedByApplicant: self.AdditionalEquity2bInvestedByApplicant(),
                LPFinancingPlanRemarks: self.LPFinancingPlanRemarks(),
                ConstructionCostFinancingPlan: self.ConstructionCostFinancingPlan(),
                NetAmountForConstruction: self.NetAmountForConstruction(),
                OverallAssessments: overallAssessmentDetails,
                ApprovalAuthoritySignatoryId: self.ApprovalAuthoritySignatoryId(),
                //public ProposalFacilityType? FacilityType :self
                //public int AppliedLoanTerm :self
                //public decimal InterestRateOffered :self
                ProcessingFeeAndDocChargesPercentage: self.ProcessingFeeAndDocChargesPercentage(),
                ProcessingFeeAndDocChargesAmount: self.ProcessingFeeAndDocChargesAmount(),
                DBRFlatPurchase: self.DBRFlatPurchase(),
                ExpectedRentPercentage: self.ExpectedRentPercentage(),
                //public decimal? LTVonTotalPresentMarketValue :self
                FacilityRecomendation: self.FacilityRecomendation(),
                FRRemarks: self.FRRemarks(),
                StressRates: stressRateDetails,
                StressRemarks: self.StressRemarks(),
                //ApprovalAuthorityLevel: self.ApprovalAuthorityLevel(),
                PreparedBy: self.PreparedBy(),
                ReviewedBy: self.ReviewedBy(),
                RecommendedBy: self.RecommendedBy(),
                SecurityDetails: securities,
                SecurityRemarks: self.SecurityRemarks(),
                Signatories: signatoryDetails,
                Spread: self.Spread(),
                ExpiryDate: self.ExpiryDate(),
                ExpiryDateTxt: moment(self.ExpiryDate()).format("DD/MM/YYYY"),
                LoantoDepositRatio: self.LoantoDepositRatio(),
                TotalAmountForFdr: self.TotalAmountForFdr(),
                WeightedAverageRate: self.WeightedAverageRate(),
                DulyDischargedFdr: self.DulyDischargedFdr(),
                LetterOfLienSetOff: self.LetterOfLienSetOff(),
                DemandPromissoryNote: self.DemandPromissoryNote(),
                SingedLoanApplication: self.SingedLoanApplication(),
                Imperfections: self.Imperfections(),
                PDCRemarks: self.PDCRemarks(),
                ExistingEMI: self.ExistingEMI(),
                ApprovalAuthorityLevel: self.ApprovalAuthorityLevel(),
                ProposalCreditCards: creditCards,
                RemovedAssetBackup: self.RemovedAssetBackup(),
                RemovedOtherCosts: self.RemovedOtherCosts(),
                RemovedValuationOtherCosts: self.RemovedValuationOtherCosts(),
                RemovedOverallAssessments: self.RemovedOverallAssessments(),
                RemovedSignatories: self.RemovedSignatories(),
                RemovedCreditCards : self.RemovedCreditCards(),
                TotalExpences: self.TotalExpences(),
                TotalAssetOfApplicants: self.TotalAssetOfApplicants(),
                TotalLiabilitiesOfApplicants: self.TotalLiabilitiesOfApplicants(),
                TotalNetWorthOfApplicants: self.TotalNetWorthOfApplicants(),
                IncomeConsideredRemarks: self.IncomeConsideredRemarks(),
                IncomeNotConsideredRemarks: self.IncomeNotConsideredRemarks(),
                LoanPaymentToDeveloper: self.LoanPaymentToDeveloper()
            }
            if (self.IsValid()) {
                $.ajax({
                    type: "POST",
                    url: '/IPDC/CRM/SaveProposal',
                    data: ko.toJSON(submitData),
                    contentType: "application/json",
                    success: function (data) {
                        $('#lonSuccessModal').modal('show');
                        $('#lonSuccessModalText').text(data.Message);
                        if (data.Id > 0) {
                            self.Id(data.Id);
                            self.LoadProposaLData();
                        }

                    },
                    error: function () {
                        alert(error.status + "<--and--> " + error.statusText);
                    }
                });
            } else {
                self.errors.showAllMessages();
            }

        }
        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));

        }
        self.Initialize = function () {
            self.GetCountry();
            self.GetVerificationStates();
            self.GetDeveloperCatagory();
            //self.GetLandedPropertyValuationType();
            self.GetRelationshipWithApplicant();
            self.GetApprovalAuthorityLevel();
            //self.FacilityTypeList();
            self.GetFacilityType();
            self.GetPrinterFiltering();
            self.GetCIBClassificationStatuses();
            self.GetAllSignatories();
            self.GetHomeOwnerships();
            if (self.ApplicationId() > 0 || self.Id() > 0) {
                self.LoadProposaLData();
            }


        }

        self.FreeCash = ko.computed(function () {//TotalMonthlyIncome - EMIofExistingLoans - EMIofProposedLoan self.TotalCreditCard
            var monthlyIncome = parseFloat(self.TotalMonthlyIncomeConsidered() ? self.TotalMonthlyIncomeConsidered() : 0);
            var emiExisting = parseFloat(self.LiabilityTotalEMI() ? self.LiabilityTotalEMI() : 0);
            var emiNew = parseFloat(self.EMIofProposedLoan() ? self.EMIofProposedLoan() : 0);
            var totalExpences = parseFloat(self.TotalExpences() ? self.TotalExpences() : 0);
            var prctgOfCreditCard = parseFloat(self.TotalCreditCard() ? self.TotalCreditCard() : 0);
            return (monthlyIncome - (emiExisting + emiNew + totalExpences + (prctgOfCreditCard * .05)));

        });
        self.FreeCashFormatted = ko.pureComputed({
            read: function () {
                if (self.FreeCash() > 0) {
                    return parseFloat(self.FreeCash() ? self.FreeCash() : 0).toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
                }
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.FreeCash(isNaN(value) ? 0 : value);
            },
            owner: self
        });
    }

    var proposal = new proposalVM();

    var qValue = proposal.queryString('AppId');
    proposal.ApplicationId(qValue);
    var qValueId = proposal.queryString('Id');
    proposal.Id(qValueId);
    proposal.Initialize();

    //ko.applyBindings(proposal, document.getElementById('proposalCreate'));
    ko.applyBindings(proposal, $('#proposalCreate')[0]);

});
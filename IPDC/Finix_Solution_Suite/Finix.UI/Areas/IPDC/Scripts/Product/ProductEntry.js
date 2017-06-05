var customerType = [{ 'Id': 1, 'Name': 'Individual' }, { 'Id': 2, 'Name': 'Organizational' }]
var collectionStage = [{ 'Id': 1, 'Name': 'Application' }, { 'Id': 2, 'Name': 'Operation' }]
var depositType = [{ 'Id': 1, 'Name': 'Fixed' }, { 'Id': 2, 'Name': 'Recurring' }]
var productType = [{ 'Id': 1, 'Name': 'Deposit' }, { 'Id': 2, 'Name': 'Loan' }]
var proposalProd = [{ 'Id': 1, 'Name': 'Vehicle' }, { 'Id': 2, 'Name': 'FDR' }, { 'Id': 3, 'Name': 'Consumer Goods' }, { 'Id': 4, 'Name': 'Landed Property' }, { 'Id': 6, 'Name': 'Others' }]
var facility = [{ 'Id': 1, 'Name': 'Home Loan' }, { 'Id': 2, 'Name': 'Auto Loan' }, { 'Id': 3, 'Name': 'Personal Loan' }, { 'Id': 4, 'Name': 'RLS' }, { 'Id': 5, 'Name': 'Fixed Deposit' }, { 'Id': 6, 'Name': 'Recurrent Deposit' }]
$(document).ready(function () {
    ko.validation.init({
        errorElementClass: 'has-error',
        errorMessageClass: 'help-block',
        decorateInputElement: true,
        grouping: { deep: true, observable: true }
    });

    function ProductRate() {
        var self = this;
        self.Id = ko.observable();
        self.ProductId = ko.observable();
        self.CardRate = ko.observable();
        self.PositiveVariance = ko.observable();
        self.NegativeVariance = ko.observable();
        self.EffectiveDate = ko.observable();
        self.EffectiveDateTxt = ko.observable();
        self.LoadData = function (data) {
            self.Id(data.Id);
            self.ProductId(data.ProductId);
            self.CardRate(data ? data.CardRate : 0);
            self.PositiveVariance(data ? data.PositiveVariance : 0);
            self.NegativeVariance(data ? data.NegativeVariance : 0);
            self.EffectiveDate(moment(data.EffectiveDate));
            self.EffectiveDateTxt(data.EffectiveDateTxt);
        }
    }
    function ProductSpclRate() {
        var self = this;
        self.Id = ko.observable();
        self.ProductId = ko.observable();
        self.Deviation = ko.observable();
        self.AuthorizedBy = ko.observable();
        self.EffectiveDate = ko.observable();
        self.EffectiveDateTxt = ko.observable();
        self.LoadData = function (data) {
            self.Id(data.Id);
            self.ProductId(data.ProductId);
            self.Deviation(data ? data.Deviation : 0);
            self.AuthorizedBy(data.AuthorizedBy);
            self.EffectiveDate(moment(data.EffectiveDate));
            self.EffectiveDateTxt(data.EffectiveDateTxt);
        }
    }

    function DPSMaturity() {
        var self = this;
        self.Id = ko.observable();
        self.ProductId = ko.observable();
        self.EffectiveDate = ko.observable();
        self.EffectiveDateTxt = ko.observable();
        self.InitialDeposit = ko.observable();
        self.InstallmentAmount = ko.observable();
        self.Term = ko.observable();
        self.MaturityAmount = ko.observable();
        self.LoadData = function (data) {
            self.Id(data.Id);
            self.ProductId(data.ProductId);
            self.InitialDeposit(data ? data.InitialDeposit : 0);
            self.InstallmentAmount(data.InstallmentAmount);
            self.Term(data ? data.Term : 0);
            self.MaturityAmount(data.MaturityAmount);
            self.EffectiveDate(moment(data.EffectiveDate));
            self.EffectiveDateTxt(data.EffectiveDateTxt);
        }
    }

    function DocSetup() {
        var self = this;
        self.Id = ko.observable();
        self.ProductId = ko.observable();
        self.DocName = ko.observable();
        self.IsMandatory = ko.observable();
        //self.AppicableFor = ko.observable();
        self.DocCollectionStage = ko.observable();
        self.DocId = ko.observable();
        self.CustomerType = ko.observable();
        self.CompanyLegalStatus = ko.observable();
        self.LoadData = function (data) {
            self.Id(data.Id);
            self.ProductId(data.ProductId);
            self.DocName(data ? data.DocName : 0);
            self.IsMandatory(data.IsMandatory);
            //self.AppicableFor(data ? data.AppicableFor : 0);
            self.DocCollectionStage(data.DocCollectionStage);
            self.DocId(data.DocId);
            self.CustomerType(data.CustomerType);
            self.CompanyLegalStatus(data.CompanyLegalStatus);
        }
    }

    function prodSecurity() {
        var self = this;
        self.Id = ko.observable();
        self.ProductId = ko.observable();
        self.SecurityDescription = ko.observable();
        self.IsMandatory = ko.observable();
        self.LoadData = function (data) {
            console.log(data);
            self.Id(data.Id);
            self.ProductId(data.ProductId);
            self.SecurityDescription(data ? data.SecurityDescription : 0);
            self.IsMandatory(data.IsMandatory);
        }
    }
    var ProductEntryViewModel = function () {
        var self = this;
        self.Id = ko.observable();
        self.Name = ko.observable();
        self.ShortName = ko.observable();
        self.Prefix = ko.observable();
        self.ProductType = ko.observable();
        self.ProductTypes = ko.observableArray(productType);
        self.DepositType = ko.observable();
        self.MinTerm = ko.observable();
        self.MaxTerm = ko.observable();
        self.MinAmount = ko.observable();
        self.MaxAmount = ko.observable();
        self.ApplicationFee = ko.observable();
        self.MaxProcessingFeeRate = ko.observable();
        self.MaxProcessingFeeAmount = ko.observable();
        self.MaxDocFeeRate = ko.observable();
        self.MaxDocFeeAmount = ko.observable();
        self.MinCIBCharge = ko.observable();
        self.JointAccountAllowed = ko.observable();
        self.FlexAmountAllowed = ko.observable();
        self.ProductRates = ko.observableArray([]);
        self.ProductSpecialRate = ko.observableArray([]);
        self.DPSMaturitySchedule = ko.observableArray([]);
        self.DocumentSetups = ko.observableArray([]);
        self.ProductSecurity = ko.observableArray([]);
        self.ProposalProduct = ko.observable();
        self.FacilityType = ko.observable();

        //List//
        self.CustomerTypes = ko.observableArray(customerType);
        self.DocCollectionStages = ko.observableArray(collectionStage);
        self.DepositTypes = ko.observableArray(depositType);
        self.ProposalProducts = ko.observableArray(proposalProd);
        self.FacilityTypes = ko.observableArray(facility);
        self.Documents = ko.observableArray([]);
        self.CompanyLegalStatuses = ko.observableArray([]);
        ///////
        self.GetDocuments = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Product/GetAllDocuments',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.Documents(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetCompanyLegalStatuses = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/CIF/GetAllCifOrgLegalStatus',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CompanyLegalStatuses(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.AddSecurities = function () {
            self.ProductSecurity.push(new prodSecurity());
        }
        self.RemovedSecurities = ko.observableArray([]);
        self.RemoveSecurities = function (line) {
            if (line.Id() > 0)
                self.RemovedSecurities.push(line.Id());
            self.ProductSecurity.remove(line);
        }
        self.AddProductRates = function () {
            self.ProductRates.push(new ProductRate());
        }
        self.RemovedProductRates = ko.observableArray([]);
        self.RemoveProductRates = function (line) {
            if (line.Id() > 0)
                self.RemovedProductRates.push(line.Id());
            self.ProductRates.remove(line);
        }
        self.AddSpclProductRates = function () {
            self.ProductSpecialRate.push(new ProductSpclRate());
        }
        self.RemovedSpclProductRates = ko.observableArray([]);
        self.RemoveSpclProductRates = function (line) {
            if (line.Id() > 0)
                self.RemovedSpclProductRates.push(line.Id());
            self.ProductSpecialRate.remove(line);
        }

        self.AddDPSMaturitySchedule = function () {
            self.DPSMaturitySchedule.push(new DPSMaturity());
        }
        self.RemovedDPSMaturitySchedule = ko.observableArray([]);
        self.RemoveDPSMaturitySchedule = function (line) {
            if (line.Id() > 0)
                self.RemovedDPSMaturitySchedule.push(line.Id());
            self.DPSMaturitySchedule.remove(line);
        }

        self.AddDocumentSetup = function () {
            self.DocumentSetups.push(new DocSetup());
        }
        self.RemovedDocumentSetup = ko.observableArray([]);
        self.RemoveDocumentSetup = function (line) {
            if (line.Id() > 0)
                self.RemovedDocumentSetup.push(line.Id());
            self.DocumentSetups.remove(line);
        }

        self.GetProductTypes = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetProductTypes',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ProductTypes(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.GetProducts = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetAllProducts',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.Products(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.LoadApplicationData = function () {
            if (self.Id() > 0) {
                $.getJSON("/IPDC/Product/LoadProductById/?id=" + self.Id(),
                    null,
                    function (data) {
                        self.ProductRates([]);
                        self.ProductSpecialRate([]);
                        self.DPSMaturitySchedule([]);
                        self.DocumentSetups([]);
                        self.ProductSecurity([]);
                        self.Id(data.Id),
                        self.Name(data.Name),
                        self.ShortName(data.ShortName),
                        self.Prefix(data.Prefix),
                        //self.ProductId(data.ProductId),
                        self.ProductType(data.ProductType),
                        self.DepositType(data.DepositType),
                        self.MinTerm(data.MinTerm),
                       self.MaxTerm(data.MaxTerm),
                       self.MinAmount(data.MinAmount),
                        self.MaxAmount(data.MaxAmount),
                        self.ApplicationFee(data.ApplicationFee),
                        self.MaxProcessingFeeRate(data.MaxProcessingFeeRate),
                        self.MaxProcessingFeeAmount(data.MaxProcessingFeeAmount),
                        self.MaxDocFeeRate(data.MaxDocFeeRate),
                        self.MaxDocFeeAmount(data.MaxDocFeeAmount),
                        self.MinCIBCharge(data.MinCIBCharge),
                        self.JointAccountAllowed(data.JointAccountAllowed),
                        self.FlexAmountAllowed(data.FlexAmountAllowed),
                        self.ProposalProduct(data.ProposalProduct),
                        self.FacilityType(data.FacilityType),

                        $.each(data.ProductRates, function (index, value) {
                            var aDetail = new ProductRate();
                            if (typeof (value) != 'undefined') {
                                aDetail.LoadData(value);
                                self.ProductRates.push(aDetail);
                            }
                        });

                        $.each(data.ProductSpecialRate, function (index, value) {
                            var aDetail = new ProductSpclRate();
                            if (typeof (value) != 'undefined') {
                                aDetail.LoadData(value);
                                self.ProductSpecialRate.push(aDetail);
                            }
                        });

                        $.each(data.DPSMaturitySchedule, function (index, value) {
                            var aDetail = new DPSMaturity();
                            if (typeof (value) != 'undefined') {
                                aDetail.LoadData(value);
                                self.DPSMaturitySchedule.push(aDetail);
                            }
                        });
                        $.when(self.GetDocuments()).done(function () {
                            $.when(self.GetCompanyLegalStatuses()).done(function () {
                                $.each(data.DocumentSetups, function (index, value) {
                                    var aDetail = new DocSetup();
                                    if (typeof (value) != 'undefined') {
                                        aDetail.LoadData(value);
                                        self.DocumentSetups.push(aDetail);
                                    }
                                });
                            });
                        });

                        $.each(data.ProductSecurity, function (index, value) {
                            var aDetail = new prodSecurity();
                            if (typeof (value) != 'undefined') {
                                aDetail.LoadData(value);
                                self.ProductSecurity.push(aDetail);
                            }
                        });
                    });

            }
        }
        self.errors = ko.validation.group(self);
        self.IsValid = ko.computed(function () {
            if (self.errors().length === 0)
                return true;
            else {
                return false;
            }
        });
        self.Submit = function () {
            var rate = ko.observableArray([]);
            var specialRate = ko.observableArray([]);
            var maturityRate = ko.observableArray([]);
            var doc = ko.observableArray([]);
            var security = ko.observableArray([]);
            $.each(self.ProductRates(),
                function (index, value) {
                    rate.push({
                        Id: value.Id,
                        CardRate: value.CardRate,
                        PositiveVariance: value.PositiveVariance,
                        NegativeVariance: value.NegativeVariance,
                        ProductId: value.ProductId,
                        EffectiveDate: value.EffectiveDate,
                        EffectiveDateTxt: moment(value.EffectiveDate()).format("DD/MM/YYYY")
                    });
                });
            $.each(self.ProductSpecialRate(),
              function (index, value) {
                  specialRate.push({
                      Id: value.Id,
                      Deviation: value.Deviation,
                      ProductId: value.ProductId,
                      EffectiveDate: value.EffectiveDate,
                      EffectiveDateTxt: moment(value.EffectiveDate()).format("DD/MM/YYYY")
                  });
              });
            $.each(self.DPSMaturitySchedule(),
            function (index, value) {
                maturityRate.push({
                    Id: value.Id,
                    ProductId: value.ProductId,
                    EffectiveDate: value.EffectiveDate,
                    EffectiveDateTxt: moment(value.EffectiveDate()).format("DD/MM/YYYY"),
                    InitialDeposit: value.InitialDeposit,
                    InstallmentAmount: value.InstallmentAmount,
                    Term: value.Term,
                    MaturityAmount: value.MaturityAmount
                });
            });
            $.each(self.DocumentSetups(),
         function (index, value) {
             doc.push({
                 Id: value.Id,
                 ProductId: value.ProductId,
                 IsMandatory: value.IsMandatory,
                 DocName: value.DocName,
                 //AppicableFor: value.AppicableFor,
                 DocCollectionStage: value.DocCollectionStage,
                 DocId: value.DocId,
                 CustomerType: value.CustomerType,
                 CompanyLegalStatus: value.CompanyLegalStatus
             });
         });
            $.each(self.ProductSecurity(),
        function (index, value) {
            security.push({
                Id: value.Id,
                ProductId: value.ProductId,
                IsMandatory: value.IsMandatory,
                SecurityDescription: value.SecurityDescription
            });
        });
            var submitProductData = {
                Id: self.Id(),
                Name: self.Name(),
                ShortName: self.ShortName(),
                Prefix: self.Prefix(),
                //ProductId: self.ProductId(),
                ProductType: self.ProductType(),
                DepositType: self.DepositType(),
                MinTerm: self.MinTerm(),
                MaxTerm: self.MaxTerm(),
                MinAmount: self.MinAmount(),
                MaxAmount: self.MaxAmount(),
                ApplicationFee: self.ApplicationFee(),
                MaxProcessingFeeRate: self.MaxProcessingFeeRate(),
                MaxProcessingFeeAmount: self.MaxProcessingFeeAmount(),
                MaxDocFeeRate: self.MaxDocFeeRate(),
                MaxDocFeeAmount: self.MaxDocFeeAmount,
                MinCIBCharge: self.MinCIBCharge,
                JointAccountAllowed: self.JointAccountAllowed(),
                FlexAmountAllowed: self.FlexAmountAllowed(),
                ProposalProduct: self.ProposalProduct(),
                FacilityType: self.FacilityType(),
                ProductRates: rate,
                ProductSpecialRate: specialRate,
                DPSMaturitySchedule: maturityRate,
                DocumentSetups: doc,
                ProductSecurity: security,
                RemovedSecurities: self.RemovedSecurities(),
                RemovedProductRates: self.RemovedProductRates(),
                RemovedSpclProductRates: self.RemovedSpclProductRates(),
                RemovedDPSMaturitySchedule: self.RemovedDPSMaturitySchedule(),
                RemovedDocumentSetup: self.RemovedDocumentSetup()
            };
            if (self.IsValid()) {
                $.ajax({
                    url: '/IPDC/Product/SaveProduct',
                    type: 'POST',
                    contentType: 'application/json',
                    data: ko.toJSON(submitProductData),
                    success: function (data) {
                        $('#ProductSuccessModal').modal('show');
                        $('#ProductSuccessModalText').text(data.Message);
                        if (typeof (data.Id) != 'undefined' && data.Id > 0)
                            self.Id(data.Id);
                        //self.Reset();
                    },
                    error: function (error) {
                        alert(error.status + "<--and--> " + error.statusText);
                    }
                });
            } else {
                self.errors.showAllMessages();
            }

        };
        self.Initializer = function () {
            if (self.Id() > 0) {
                //self.LoadProduct();
                self.LoadApplicationData();
            } else {
                self.LoadProductTypes();
            }
            self.GetDocuments();
            self.GetCompanyLegalStatuses();
        }
        self.LoadProductTypes = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Product/GetProductTypes',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ProductTypes(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.LoadProduct = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Product/LoadProduct?Id=' + self.Id(),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.Id(data.Id);
                    self.Name(data.Name);
                    self.ShortName(data.ShortName);
                    self.Prefix(data.Prefix);
                    self.ProductType(data.ProductType + "");
                    $.when(self.LoadProductTypes()).done(function () {
                        self.ProductType(data.ProductType);
                    })
                },

                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);

                }
            });
        }
        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    };
    var proVm = new ProductEntryViewModel();
    proVm.Id(proVm.queryString("Id"));
    proVm.Initializer();
    ko.applyBindings(proVm, document.getElementById("ProductEntry")[0]);

});
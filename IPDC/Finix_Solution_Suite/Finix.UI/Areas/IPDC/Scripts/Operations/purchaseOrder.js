var statuses = [{ 'Id': 2, 'Name': 'Obtained' },
                { 'Id': 3, 'Name': 'Deferred' },
                { 'Id': 4, 'Name': 'Waived' }];
$(document).ready(function () {
    $(function () {
        //$('#OfferLetterDate').datetimepicker({ format: 'DD/MM/YYYY' });
        //$('#NewDateOfBirth').datetimepicker({ format: 'DD/MM/YYYY' });
    });
    //var appvm;
    ko.validation.init({
        errorElementClass: 'has-error',
        errorMessageClass: 'help-block',
        decorateInputElement: true,
        grouping: { deep: true, observable: true }
    });
    function poDocument(data) {
        var self = this;
        self.Id = ko.observable(data ? data.Id : 0);
        self.Name = ko.observable(data ? data.Name : "");
        self.LoadData = function (data) {
            self.Id(data ? data.Id : 0);
            self.Name(data ? data.Name : "");
        }
    }
    function PurchaseOrderVm() {
        var self = this;
        self.Id = ko.observable();
        self.ApplicationId = ko.observable();
        self.ProposalId = ko.observable();
        self.IsVendor = ko.observable();
        self.SellersName = ko.observable();
        self.SellersAddress = new address();
        self.RecomendedLoanAmountFromIPDC = ko.observable();
        self.QuotationDate = ko.observable('');
        self.QuotationDateTxt = ko.observable();
        self.QuotationPrice = ko.observable();
        self.ApplicationTitle = ko.observable();
        self.Documents = ko.observableArray([]);
        self.CustomerName = ko.observable();
        self.CustomerAddress = new address();
        self.VehicleBrand = ko.observable();
        self.ChassisNo = ko.observable();
        self.EngineNo = ko.observable();
        self.ManufacturingYear = ko.observable();
        self.Colour = ko.observable();
        self.CC = ko.observable();
        self.Valitity = ko.observable();
        self.CountryList = ko.observableArray([]);
        self.Link1 = ko.observable();
        self.Link2 = ko.observable();
        self.Link3 = ko.observable();
        self.Title1 = ko.observable('PDF');
        self.Title2 = ko.observable('Excel');
        self.Title3 = ko.observable('Word');
        self.AddDocuments = function () {
            self.Documents.push(new poDocument());
        }
        self.RemovedDocuments = ko.observableArray([]);
        self.RemoveDocuments = function (line) {
            if (line.Id() > 0)
                self.RemovedDocuments.push(line.Id());
            self.Documents.remove(line);
        }
        self.SavePrint = function () {
            //self.Id(' ');
            $.when(self.SavePO()).done(function () {
                self.setUrl();
            });
        }
        self.setUrl = function () {
            //console.log(self.Id());
            //console.log(typeof (self.Id()));
            //console.log(typeof (self.Id()) != 'undefined');
            if ( typeof(self.Id()) != 'undefined') {
                window.open('/IPDC/Operations/PurchaseOrderReport?reportTypeId=PDF&poId=' + self.Id(), '_blank');
            }
        };
        self.GetGroupCountry = function () {
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
        self.SavePO = function () {
            //self.OfferLetterDateTxt($("#OfferLetterDate").val());
            var documentInfo = ko.observableArray([]);           
            $.each(self.Documents(),
              function (index, value) {
                  documentInfo.push({
                      Id:value.Id,
                      Name:value.Description
                  });
              });
            var submitData = {
                Id : self.Id(),
                ApplicationId: self.ApplicationId(),
                ProposalId: self.ProposalId(),
                IsVendor: self.IsVendor(),
                SellersName: self.SellersName(),
                //SellersAddress:self.SellersAddress(),
                SellersAddress: self.SellersAddress,
                RecomendedLoanAmountFromIPDC: self.RecomendedLoanAmountFromIPDC(),
                QuotationDate: self.QuotationDate(),
                QuotationDateTxt: moment(self.QuotationDate()).format("DD/MM/YYYY"),
                QuotationPrice: self.QuotationPrice(),
                ApplicationTitle: self.ApplicationTitle(),
                Documents : self.Documents,
                CustomerName: self.CustomerName(),
                CustomerAddress: self.CustomerAddress,
                //CustomerAddress:self.CustomerAddress(),
                VehicleBrand: self.VehicleBrand(),
                ChassisNo: self.ChassisNo(),
                EngineNo: self.EngineNo(),
                ManufacturingYear: self.ManufacturingYear(),
                Colour: self.Colour(),
                CC: self.CC(),
                Valitity: self.Valitity(),
                RemovedDocuments : self.RemovedDocuments()
            }
            return $.ajax({
                type: "POST",
                url: '/IPDC/Operations/SavePurchaseOrder',
                data: ko.toJSON(submitData),
                contentType: "application/json",
                success: function (data) {
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);
                    self.Id(data.Id);
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.LoadPurchaseOrder = function () {
            if (self.ProposalId() > 0 || self.ApplicationId() > 0 || self.Id() > 0) {
                return $.ajax({
                    type: "GET",
                    url: '/IPDC/Operations/LoadPurchaseOrder/?proposalId=' + self.ProposalId() + '&appId=' + self.ApplicationId() + '&id=' + self.Id(),
                    contentType: "application/json",
                    dataType: "json",
                    success: function (data) {
                        self.Id(data.Id);
                        self.ApplicationId(data.ApplicationId);
                        self.ProposalId(data.ProposalId);
                        self.IsVendor(data.IsVendor);
                        self.SellersName(data.SellersName);
                        //self.SellersAddress(data.SellersAddress);
                        $.when(self.GetGroupCountry()).done(function() {
                            self.SellersAddress.LoadAddress(data.SellersAddress != null ? data.SellersAddress : "");
                            self.CustomerAddress.LoadAddress(data.CustomerAddress != null ? data.CustomerAddress : "");
                        });
                        self.RecomendedLoanAmountFromIPDC(data.RecomendedLoanAmountFromIPDC);
                        self.QuotationDate(data.QuotationDate ? moment(data.QuotationDate) : moment());
                        self.QuotationPrice(data.QuotationPrice);
                        self.ApplicationTitle(data.ApplicationTitle);
                        //self.Documents = ko.observableArray([]);
                        self.CustomerName(data.CustomerName);
                        //self.CustomerAddress(data.CustomerAddress);
                        self.VehicleBrand(data.VehicleBrand);
                        self.ChassisNo(data.ChassisNo);
                        self.EngineNo(data.EngineNo);
                        self.ManufacturingYear(data.ManufacturingYear);
                        self.Colour(data.Colour);
                        self.CC(data.CC);
                        self.Valitity(data.Valitity);
                        $.each(data.Documents, function (index, value) {
                            var doc = new poDocument();
                            if (typeof (value) != 'undefined') {
                                doc.LoadData(value);
                                self.Documents.push(doc);
                            }
                        });
                    }
                });
            }
        };
        self.Initialize = function () {
            self.GetGroupCountry();
            if (self.ProposalId() > 0 || self.ApplicationId() > 0 || self.Id()>0) {
                self.LoadPurchaseOrder();
            }

        }
        //self.GetDocumentStatusList = function () {
        //    return $.ajax({
        //        type: "GET",
        //        url: '/IPDC/Operations/GetDocumentStatusList',
        //        contentType: "application/json; charset=utf-8",
        //        dataType: "json",
        //        success: function (data) {
        //            self.DocumentStatusList(data); //Put the response in ObservableArray
        //        },
        //        error: function (error) {
        //            alert(error.status + "<--and--> " + error.statusText);
        //        }
        //    });
        //}
        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }

    var oflvm = new PurchaseOrderVm();
    var appId = oflvm.queryString('ApplicationId');
    oflvm.ApplicationId(appId);
    var propId = oflvm.queryString('ProposalId');
    oflvm.ProposalId(propId);
    var id = oflvm.queryString('Id');
    oflvm.Id(id);
    oflvm.Initialize();
    ko.applyBindings(oflvm, $('#PurchaseOrderVW')[0]);

});
$(function () {
    //$('#ReceivedDateTxt').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
    //$('#ReceivedDateTxt').datetimepicker({ format: 'DD/MM/YYYY', maxDate: moment() });
});
$(document).ready(function () {
    function DocumentCheckVm() {
        var self = this;
        self.Id = ko.observable();
        self.test = ko.observable('');
        self.LoadData = ko.observableArray(userInfo);
        self.EntryId = ko.observable();
        self.ApplicationId = ko.observable();
        self.Comment = ko.observable();
        self.HardCopyReceived = ko.observable(false);
        self.HardCopyReceiveDateText = ko.observable();
        self.HardCopyReceiveDate = ko.observable();
        self.ProductTypeId = ko.observable();
        self.QuotationDate = ko.observable();
        self.QuotationDateTxt = ko.observable();
        self.Documents = ko.observableArray([]);
        self.Exceptions = ko.observableArray([]);
        self.IsVisible = ko.observable(false);
        self.Link1 = ko.observable();
        self.Title1 = ko.observable('PDF');
        self.PersonType = ko.observable();
        self.PoId = ko.observable();
        self.RejectionReason = ko.observable();
        self.SaveApprovalDate = function () {

            var submitPoData = {
                ApplicationId : self.ApplicationId(),
                PersonType: self.PersonType(),
                QuotationDate: self.QuotationDate(),
                QuotationDateTxt: moment(self.QuotationDate()).format("DD/MM/YYYY"),
                PoId: self.PoId()
            };

            $.ajax({
                url: '/IPDC/Operations/SavePOApproval',
                //cache: false,
                type: 'POST',
                contentType: 'application/json',
                data: ko.toJSON(submitPoData),
                success: function (data) {
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);
                    //self.Reset();
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.PrintPO = function (line) {
            if (line.PoId > 0) {
                window.open('/IPDC/Operations/PurchaseOrderReport?reportTypeId=PDF&poId=' + line.PoId, '_blank');
            }
        }
        self.setVendor = function (line) {
            //console.log(line);
            self.ApplicationId(line.Id);
            self.PoId(line.PoId);
            self.PersonType(2);
            //self.SaveApprovalDate();
        }
        self.setCustomer = function (line) {
            self.ApplicationId(line.Id);
            self.PoId(line.PoId);
            self.PersonType(1);
            //self.SaveApprovalDate();
        }
        self.setPoApp = function (line) {
            self.ApplicationId(line.Id);
            self.PoId(line.PoId);
            self.PersonType(3);
            //self.SaveApprovalDate();
        }
        self.setPoEntry = function (line) {
            self.Id(line.Id);
            self.PoId(line.PoId);
            self.setUrl();
        }
        self.setUrl = function () {
            //if (self.Id() != 'undefined') {
            //    window.open('/IPDC/Operations/LoadPurchaseOrder?appId=' + self.Id(), '_blank');
            //}
            var parameters = [{
                Name: 'ApplicationId',
                Value: self.Id()
            },
            {
                Name: 'Id',
                Value: self.PoId()
            }];
            var menuInfo = {
                Id: 96,
                Menu: 'Purchase Order',
                Url: '/IPDC/Operations/PurchaseOrder',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
        self.SendMessage = function (data) {
            var parameters = [{
                Name: 'applicationId',
                Value: data.Id
            }];
            var menuInfo = {
                //Id: 89,
                Menu: 'New Message',
                Url: '/IPDC/Messaging/NewMessage',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
        self.setId = function (line) {
            self.Id(line.Id);
        }
        self.CancelApplication = function () {
            
            var submitData = {
                Id: self.Id(),
                toApplicationStage: -6, // rejected by operations
                RejectionReason: self.RejectionReason()
            };
            $.ajax({
                url: '/IPDC/Application/CloseApplication',
                type: 'POST',
                contentType: 'application/json',
                data: ko.toJSON(submitData),
                success: function (data) {
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
    }

    var aabmvm = new DocumentCheckVm();
    //var qValue = aabmvm.queryString("entryId");
    //aabmvm.EntryId(qValue);
    //var appId = aabmvm.queryString("AppId");
    //aabmvm.ApplicationId(appId);
    ko.applyBindings(aabmvm, document.getElementById("DocumentCheckVW"));
});
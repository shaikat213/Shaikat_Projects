$(document).ready(function () {
    function DocumentCheckVm() {
        var self = this;
        self.Id = ko.observable();
        self.test = ko.observable('');
        self.LoadData = ko.observableArray(userInfo);
        self.EntryId = ko.observable();
        self.EntryAppId = ko.observable();
        self.ProposalId = ko.observable();
        self.RejectionReason = ko.observable();
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
        self.DclId = ko.observable();

        self.CifSummary = function (data) {
            var parameters = [{
                Name: 'AppId',
                Value: data.Id
            }];
            var menuInfo = {
                Id: 89,
                Menu: 'Application CIF List',
                Url: '/IPDC/Operations/ApplicationCIFList',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        self.SavePrintAppSumm = function (line) {
            self.EntryAppId(line.Id);
            self.ProposalId(line.ProposalId);
            self.setUrlAppsum();
        }

        self.PrintDCL = function (data) {
            self.DclId(data.DclId);
            //console.log('dclId = ' + ko.toJSON(self.DclId()));
            if (typeof (self.DclId()) != 'undefined' && self.DclId() > 0) {
                self.setDclPrintUrl();
            }
        };

        self.setDclPrintUrl = function () {
            
            //if (typeof (self.Id()) != 'undefined') {
            window.open('/IPDC/Operations/DCLLoanReport?reportTypeId=PDF&dclId=' + self.DclId(), '_blank');
            //}
        };

        self.setUrlAppsum = function () {
            if (typeof (self.EntryAppId()) != 'undefined') {
                window.open('/IPDC/Operations/AppSummeryReportLoan?reportTypeId=PDF&AppId=' + self.EntryAppId() + '&ProposalId=' + self.ProposalId(), '_blank');
            }
        };

        self.CbsInfo = function (data) {
            var parameters = [{
                Name: 'AppId',
                Value: data.Id
            }];
            var menuInfo = {
                Id: 89,
                Menu: 'CBS Info',
                Url: '/IPDC/Operations/CBSInfo',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
        self.DisMemo = function (data) {
            var parameters = [
                {
                    Name: 'AppId',
                    Value: data.Id
                }
            ];
            var menuInfo = {
                Id: 93,
                Menu: 'Operation Approval',
                Url: '/IPDC/Operations/OperationApproval',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);

        }

        self.setUrl = function () {
            var parameters = [{
                Name: 'ApplicationId',
                Value: self.Id()
            },
            {
                Name: 'Id',
                Value: self.PoId()
            }];
            var menuInfo = {
                Id: 93,
                Menu: 'Application',
                Url: '/IPDC/Operations/PurchaseOrder',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        self.setEntyId = function (line) {
            self.EntryId(line.Id);
        }
        self.CancelApplication = function () {
            var submitData = {
                Id: self.EntryId(),
                toApplicationStage: -4, // rejected by bm
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

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }

    var aabmvm = new DocumentCheckVm();
    ko.applyBindings(aabmvm, document.getElementById("approvedRlsApplications"));
});
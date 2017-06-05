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
       
        self.DisMemo = function (data) {
                var parameters = [
                    {
                        Name: 'ApplicationId',
                        Value: data.Id
                    },
                    {
                        Name: 'ProposalId',
                        Value: data.ProposalId
                    },
                    {
                        Name: 'Id',
                        Value: data.DmId
                    }
                ];
                var menuInfo = {
                    Id: 93,
                    Menu: 'Disbursment Memo',
                    Url: '/IPDC/Operations/DisbursementMemo',
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

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }

    var aabmvm = new DocumentCheckVm();
    //var qValue = aabmvm.queryString("entryId");
    //aabmvm.EntryId(qValue);
    //var appId = aabmvm.queryString("AppId");
    //aabmvm.ApplicationId(appId);
    ko.applyBindings(aabmvm, document.getElementById("DocumentCheckVW"));
});
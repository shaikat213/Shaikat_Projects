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
        self.EntryAppId = ko.observable();
        self.ApplicationId = ko.observable();
        self.Comment = ko.observable();
        self.IsApproved = ko.observable(false);
        self.HardCopyReceived = ko.observable(false);
        self.HardCopyReceiveDateText = ko.observable();
        self.HardCopyReceiveDate = ko.observable();
        self.ProductTypeId = ko.observable();
        self.Documents = ko.observableArray([]);
        self.Exceptions = ko.observableArray([]);
        self.IsVisible = ko.observable(false);
        self.Link1 = ko.observable();
        self.Title1 = ko.observable('PDF');
        self.Details = function (data) {
            var parameters = [{
                Name: 'AppId',
                Value: data.ApplicationId
            }];
            var menuInfo = {
                //Id: 89,
                Menu: 'Received Funds',
                Url: '/IPDC/Operations/FundConfirmation',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        self.Search = function (data) {
            self.Documents([]);
            self.Exceptions([]);
            self.IsVisible(true);
            return $.ajax({
                type: "GET",
                url: '/IPDC/Operations/LoadDocumentCheckListById/?id=' + data.Id,
                contentType: "application/json",
                dataType: "json",
                success: function (data) {
                    
                    self.EntryId(data.Id);
                    self.EntryAppId(data.ApplicationId);
                    self.Documents(data.Documents);
                    self.Exceptions(data.Exceptions);
                }
            });
        }
        self.setEntyId = function (line) {
            self.EntryId(line.Id);
            //self.EntryAppId(line.ApplicationId);
            self.ProductTypeId(line.ProductTypeId);
            self.setUrl();
        }
        self.setUrl = function () {
            if (self.EntryId() != 'undefined') {
               if (self.ProductTypeId()===1) {
                    window.open('/IPDC/Operations/DCLReport?reportTypeId=PDF&dclId=' + self.EntryId(),'_blank');
               } else if (self.ProductTypeId() === 2) {
                   window.open('/IPDC/Operations/DCLLoanReport?reportTypeId=PDF&dclId=' + self.EntryId(), '_blank');
               }
               else {
                   window.open('/IPDC/Operations/DCLReport?reportTypeId=PDF&dclId=' + self.EntryId(), '_blank');
               }
               
            }  
        }

        self.SubmitAsApproved = function() {
            self.IsApproved(true);
            self.SaveDocCheckList();
        }
        self.SubmitAsNotApproved = function () {
            self.IsApproved(false);
            self.SaveDocCheckList();
        }

        self.SaveDocCheckList = function () {
            var submitData = {
                Id: self.EntryId(),
                ApplicationId: self.EntryAppId(),
                IsApproved: self.IsApproved()
            }
            return $.ajax({
                type: "POST",
                url: '/IPDC/Operations/SaveApprovedDocumentCheckList',
                data: ko.toJSON(submitData),
                contentType: "application/json",
                success: function (data) {
                    $('#docSuccessModal').modal('show');
                    $('#docSuccessModalText').text(data.Message);
                    self.Id(data.Id);
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.SendMessage = function (data) {
            var parameters = [{
                Name: 'applicationId',
                Value: data.ApplicationId
                //Value: data.Id
            }];
            var menuInfo = {
                //Id: 89,
                Menu: 'New Message',
                Url: '/IPDC/Messaging/NewMessage',
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
    }

    var aabmvm = new DocumentCheckVm();
    //var qValue = aabmvm.queryString("entryId");
    //aabmvm.EntryId(qValue);
    //var appId = aabmvm.queryString("AppId");
    //aabmvm.ApplicationId(appId);
    ko.applyBindings(aabmvm, document.getElementById("DocumentCheckVW"));
});
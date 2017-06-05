/// <reference path="../knockout-3.4.0.debug.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../finix.util.js" />
/// <reference path="~/Scripts/knockout.validation.min.js" />
//$(function () {
//    $('#NextFollowUp').datetimepicker({ format: 'DD/MM/YYYY'});
//    $('#NewCommCertIssueDate').datetimepicker({ format: 'DD/MM/YYYY'});
//    $('#NewDateOfBirth').datetimepicker({ format: 'DD/MM/YYYY' });
//});
$(document).ready(function () {

    ko.validation.init({
        errorElementClass: 'has-error',
        errorMessageClass: 'help-block',
        decorateInputElement: true,
        grouping: { deep: true, observable: true }
    });
    function address() {
        var self = this;
        self.ThanaId = ko.observable('');
        self.ThanaName = ko.observable('');
        self.DistrictId = ko.observable('');
        self.DistrictName = ko.observable('');
        self.DivisionId = ko.observable('');
        self.DivisionName = ko.observable('');
        self.CountryId = ko.observable('');
        self.CountryName = ko.observable('');
        self.AddressLine1 = ko.observable('');
        self.AddressLine2 = ko.observable('');
        self.AddressLine3 = ko.observable('');
        self.PostalCode = ko.observable('');
        self.PhoneNo = ko.observable('');
        self.CellPhoneNo = ko.observable('');
        self.Email = ko.observable('');
    }
    function followUpScheduleSRVm() {

        var self = this;
        self.SalesLeadList = ko.observableArray([]);


        self.LoadDetailData = function (data) {
            detailVm.LoadDetailsData(data);
        };
        self.LoadCIFData = function (data) {
            cifpVm.LoadData();
        }
        self.getData = function () {
            $.ajax({
                type: "GET",
                url: '/IPDC/SalesLead/GetProspectiveAssignedSalesLeads',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    
                    self.SalesLeadList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }



    }
    function leadDetailsData() {
        var self = this;

        self.Id = ko.observable('');
        self.Name = ko.observable("");
        self.Address = ko.observable("");
        self.ThanaName = ko.observable("");
        self.DistrictName = ko.observable("");
        self.FollowUpCallTime = ko.observable("");
        self.FollowUpCallTimeText = ko.observable("");
        self.LeadTypeName = ko.observable("");
        self.LoanTypeName = ko.observable("");
        self.CustomerSensitivityName = ko.observable("");
        self.CustomerPriorityName = ko.observable('');
        self.ProductName = ko.observable('');
        self.LeadStatusName = ko.observable('');
        self.CallLog = ko.observableArray([]);
        self.ApplicationId = ko.observable();
        self.LeadPriorityName = ko.observable();
        self.Amount = ko.observable(0);
        self.AmountFormatted = ko.pureComputed({
            read: function () {
                return self.Amount().toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.Amount(isNaN(value) ? 0 : value);
            },
            owner: self
        });
    

        self.LoadDetailsData = function (data) {

            self.Id(data.Id);
            self.Name(data.CustomerName);
            self.ApplicationId(data.ApplicationId);
            self.Address(data.CustomerAddress ?
                (data.CustomerAddress.AddressLine1 ? (data.CustomerAddress.AddressLine1 + ', ') : '')
                + (data.CustomerAddress.AddressLine2 ? (data.CustomerAddress.AddressLine2 + ', ') : '')
                + (data.CustomerAddress.AddressLine3 ? (data.CustomerAddress.AddressLine3 + ', ') : '')
                + (data.CustomerAddress.ThanaName ? (data.CustomerAddress.ThanaName + ', ') : '')
                + (data.CustomerAddress.DistrictName ? (data.CustomerAddress.DistrictName + ', ') : '')
                + (data.CustomerAddress.DivisionName ? (data.CustomerAddress.DivisionName + ', ') : '')
                + (data.CustomerAddress.CountryName ? (data.CustomerAddress.CountryName) : '')
                : '');
            self.ThanaName(data.ThanaName);
            self.DistrictName(data.DistrictName);
            self.FollowUpCallTime(moment(data.FollowupTime).format('DD/MM/YYYY hh:mm A'));
            self.FollowUpCallTimeText(data.FollowupTimeText);
            self.LeadTypeName(data.LeadTypeName);
            self.LoanTypeName(data.LoanTypeName);
            self.CustomerSensitivityName(data.CustomerSensitivityName);
            self.CustomerPriorityName(data.CustomerPriorityName);
            self.ProductName(data.ProductName);
            self.LeadStatusName(data.LeadStatusName);
            self.LoadCallLog();
            self.Amount(data.Amount);
            self.LeadPriorityName(data.LeadPriorityName);
        }

        self.LoadCallLog = function () {
            $.ajax({
                type: "GET",
                url: '/IPDC/SalesLead/GetCallLogBySLNo?SlNo=' + self.Id(),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CallLog(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.AddCIF = function () {
            var parameters = [{
                //Name: 'cifpid',
                //Value: self.Id()
            }];
            var menuInfo = {
                Id: 93,
                Menu: 'CIF Personal',
                Url: '/IPDC/CIF/CIF_Personal',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
            //window.parent.iFrameResize();
        }

        self.AddCIFOrg = function () {
            var parameters = [{
                //Name: 'cifpid',
                //Value: self.Id()
            }];
            var menuInfo = {
                Id: 91,
                Menu: 'CIF Organizational',
                Url: '/IPDC/CIF/CIFOraganizational',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
            //window.parent.iFrameResize();
        }

        self.AddApplication = function () {
            var parameters = [{
                Name: 'leadId',
                Value: self.Id()
            }];
            var menuInfo = {
                Id: 89,
                Menu: 'Application',
                Url: '/IPDC/Application/Application',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
            //window.parent.iFrameResize();
        }

        self.EditApplication = function () {
            var parameters = [{
                Name: 'leadId',
                Value: self.Id()
            },
            {
                Name: 'applicationId',
                Value: self.ApplicationId()
            }];
            
            var menuInfo = {
                Id: 89,
                Menu: 'Application',
                Url: '/IPDC/Application/Application',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
            //window.parent.iFrameResize();
        }

        self.SubmitApplication = function () {
            var submitData = {
                ApplicationId: self.ApplicationId()
            }
            return $.ajax({
                        type: "POST",
                        url: '/IPDC/Application/SubmitApplicationToBm',
                        data: ko.toJSON(submitData),
                        contentType: "application/json",
                        success: function (data) {
                            self.Id(data.Id);
                            $('#appForwardSuccessModal').modal('show');
                            $('#appForwardSuccessModalText').text(data.Message);
                        },
                        error: function () {
                            alert(error.status + "<--and--> " + error.statusText);
                        }
                    });
        }

        self.SendMessage = function () {
            var parameters = [{
                Name: 'applicationId',
                Value: self.ApplicationId()
            }];
            var menuInfo = {
                //Id: 89,
                Menu: 'New Message',
                Url: '/IPDC/Messaging/NewMessage',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
    };
    var vm = new followUpScheduleSRVm();
    vm.getData();
    ko.applyBindings(vm, document.getElementById('followUpScheduleSR'));//$('#followUpScheduleSR')[0]);
    var detailVm = new leadDetailsData();
    ko.applyBindings(detailVm, document.getElementById('leadDetailData'));//$('#leadDetailData')[0]);
    //
    

});





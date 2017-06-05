/// <reference path="../knockout-3.4.0.debug.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../finix.util.js" />
/// <reference path="~/Scripts/knockout.validation.min.js" />
$(function () {
    $('#NextFollowUp').datetimepicker({ format: 'DD/MM/YYYY', minDate: moment() });
    $('#NextFollowUpPre').datetimepicker({ format: 'DD/MM/YYYY HH:mm', minDate: moment() });
    $('#NextFollowUpSet').datetimepicker({ format: 'DD/MM/YYYY HH:mm', minDate: moment() });
});
$(document).ready(function () {
    ko.validation.init({
        errorElementClass: 'has-error',
        errorMessageClass: 'help-block',
        decorateInputElement: true
    });

    function salesLeadListVm() {

        var self = this;
        //self.errors = ko.validation.group(self);
        self.SalesLeadList = ko.observableArray([]);
        self.Employees = ko.observableArray([]);
        self.salesAssignmentList = ko.observableArray([]);

        self.Id = ko.observable('');
        self.Name = ko.observable('');
        self.Address = ko.observable('');
        self.ThanaName = ko.observable('');
        self.DistrictName = ko.observable('');
        self.FollowUpCallTime = ko.observable('');
        self.LeadTypeName = ko.observable('');
        self.LoanTypeName = ko.observable('');
        self.CustomerSensitivityName = ko.observable('');
        self.EmployeeId = ko.observable('').extend({ required: true });
        self.FollowUpTimeTxt = ko.observable('');
        self.FollowUpTimeTxtPre = ko.observable('');
        self.FollowUpTimeTxtLock = ko.observable('');


        self.LoadData = function (data) {
            console.log(ko.toJSON(data));
            self.Id(data.Id);
            self.Name(data.Name);
            self.Address(data.Address);
            self.ThanaName(data.ThanaName);
            self.DistrictName(data.DistrictName);
            self.FollowUpCallTime(moment(data.FollowUpCallTime).format("HH:mm"));
            self.FollowUpTimeTxt(moment(data.FollowUpCallTime).format("DD/MM/YYYY HH:mm"));
            self.FollowUpTimeTxtPre(moment(data.FollowUpCallTime).format("DD/MM/YYYY HH:mm"));
            self.FollowUpTimeTxtLock(moment(data.FollowUpCallTime).format("DD/MM/YYYY"));
            self.LeadTypeName(data.LeadTypeName);
            self.LoanTypeName(data.LoanTypeName);
            self.CustomerSensitivityName(data.CustomerSensitivityName);
            self.EmployeeId(data.EmployeeId);
            //
            self.GetDataForSR();

           
        };

        self.submit = function () {
            
            self.FollowUpTimeTxt($('#NextFollowUpSet').val());
            var quesDetails;
            quesDetails = {     
                AssignedTo :self.EmployeeId(),
                FollowUpTimeTxt: self.FollowUpTimeTxt(),
                //FollowUpTime :  self.FollowUpTimeTxtLock(),
                SalesLeadId : self.Id()
            };
            console.log("quesDetails" + ko.toJSON(quesDetails));
            $.ajax({
                type: "POST",
                url: '/IPDC/SalesLead/SaveSalesLeadAssignment',
                data: ko.toJSON(quesDetails),
                contentType: "application/json",
                success: function (data) {
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.getDateWiseTimeSchedule = function () {
            self.FollowUpTimeTxt($('#NextFollowUpSet').val());
            self.FollowUpTimeTxtLock(self.FollowUpTimeTxt()); //moment(self.FollowUpTimeTxt()).format("DD/MM/YYYY")
            //
            //console.log(self);
            if (self.errors().length == 0) {
                $.ajax({
                    type: "GET",
                    url: '/IPDC/SalesLead/GetDateWiseTimeSchedule/?id=' + self.EmployeeId() + '&dateString=' + self.FollowUpTimeTxt(),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        console.log(data);

                        self.salesAssignmentList(data); //Put the response in ObservableArray
                        //console.log(ko.toJSON(data));
                    },
                    error: function (error) {
                        alert(error.status + "<--and--> " + error.statusText);
                    }
                });
            }
            else {
                self.errors.showAllMessages();
            }
        }

        self.GetDataForSR = function () {
            if (self.errors().length == 0) {
                $.ajax({
                    type: "GET",
                    url: '/IPDC/SalesLead/GeSalesLeadAssignment/?id=' + self.Id() + '&assaignto=' + self.EmployeeId(),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function(data) {
                        console.log(data);

                        self.salesAssignmentList(data); //Put the response in ObservableArray
                        //console.log(ko.toJSON(data));
                    },
                    error: function(error) {
                        alert(error.status + "<--and--> " + error.statusText);
                    }
                });
            }
            else {
                self.errors.showAllMessages();
            }
        }


        self.getData = function () {
            $.ajax({
                type: "GET",
                url: '/IPDC/SalesLead/GetDataForLeadAssignment',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.SalesLeadList(data); //Put the response in ObservableArray

                    //console.log(ko.toJSON(data));
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.GetSR = function () {
            $.ajax({
                type: "GET",
                url: '/IPDC/SalesLead/GetAllSR',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.Employees(data); //Put the response in ObservableArray

                    //console.log(ko.toJSON(data));
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
            
        }

        self.errors = ko.validation.group(self);
        self.IsValid = ko.computed(function () {
            return true;
        });
        //Edit Start
        //self.getEditUrl = function (data) {
        //    return '/Employee/Edit?id=' + data.Id;
        //}

        //self.getTitle = function (data) {
        //    return self.titleEdit('Edit');
        //}
        //End Edid Code
    }
    var vm = new salesLeadListVm();
    vm.getData();
    vm.GetSR();
    ko.applyBindings(vm, $('#leadAssignment')[0]);
});



                                                               
$(function () {
    $('#NextFollowUp').datetimepicker({ format: 'DD/MM/YYYY HH:mm', minDate: moment() });
    $('#NextFollowUpEdit').datetimepicker({ format: 'DD/MM/YYYY HH:mm', minDate: moment() });
    $('#NextFollowUpDetails').datetimepicker({ format: 'DD/MM/YYYY HH:mm', minDate: moment() });
    //$('#CallTimeTxt').datetimepicker({ format: 'DD/MM/YYYY HH:mm', date: moment() });
});


$(document).ready(function () {
    var urlId = 1050;
    var detailVm;
    var questionSequence = 1;
    function Question(data) {
        var self = this;
        self.Sequence = ko.observable(questionSequence++); //++questionSequence;
        self.SalesLeadId = ko.observable(data.SalesLeadId ? data.SalesLeadId : 0);
        self.SalesLeadName = ko.observable(data.SalesLeadName ? data.SalesLeadName : '');
        self.QuestionId = ko.observable(data.QuestionId ? data.QuestionId : 0).extend({ digit: true });
        self.QuestionText = ko.observable(data.QuestionText ? data.QuestionText : 0);
        self.Answer = ko.observable(data.Answer ? data.Answer : '');
        self.QuestionedBy = ko.observable(data.QuestionedBy ? data.QuestionedBy : 0);
    };
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
            self.FollowUpCallTime(moment(data.FollowupTime).format("DD/MM/YYYY HH:mm A"));
            self.FollowUpCallTimeText(moment(data.FollowupTime).format("DD/MM/YYYY HH:mm A"));
            self.LeadTypeName(data.CallTypeName);
            self.LoanTypeName(data.LoanTypeName);
            self.CustomerSensitivityName(data.CustomerSensitivityName);
            self.CustomerPriorityName(data.CustomerPriorityName);
            self.ProductName(data.ProductName);
            self.LeadStatusName(data.LeadStatusName);
            self.LoadCallLog();
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
    };
    function SalesLeadList() {
        var self = this;
        self.errors = ko.validation.group(self);
        self.SalesLeadId = ko.observable('');
        self.CurrentFollowUp = ko.observable('');
        self.CallTime = ko.observable('');
        self.CallTimeTxt = ko.observable('');
        self.FollowupTypes = ko.observableArray([]);
        self.FollowupType = ko.observable('');
        self.proSalesLeadList = ko.observableArray([]);
        self.QuestionAnswerSet = ko.observableArray([]);
        self.NextFollowUpTxt = ko.observable('');
        self.Remarks = ko.observable('');
        self.shouldShowDetail = ko.observable(false);
        //Edit///
        self.Id = ko.observable('');
        self.CustomerName = ko.observable('');
        self.Phone = ko.observable('');
        self.Email = ko.observable('');
        self.AddressLine1 = ko.observable('');
        self.AddressLine2 = ko.observable('');
        self.AddressLine3 = ko.observable('');
        self.CountryId = ko.observable('');
        self.DivisionId = ko.observable('');
        self.ThanaId = ko.observable('');
        self.DistrictId = ko.observable('');
        self.ProductId = ko.observable('');
        self.LeadType = ko.observable('');
        self.OrganizationId = ko.observable('');
        self.Designation = ko.observable('');
        self.Age = ko.observable('');
        self.LeadStatus = ko.observable('');
        self.LeadStatusName = ko.observable();
        self.CustomerPriority = ko.observable();
        self.CustomerPriorityName = ko.observable();
        self.CustomerPriorities = ko.observableArray([]);
        self.FollowUpCallTimeText = ko.observable('');
        self.FollowUpCallTime = ko.observable('');
        self.CountryList = ko.observableArray([]);
        self.DivisionList = ko.observableArray([]);
        self.Thanas = ko.observableArray([]);
        self.Districts = ko.observableArray([]);
        self.Products = ko.observableArray([]);
        self.LeadTypes = ko.observableArray([]);
        self.Organizations = ko.observableArray([]);
        self.LeadStatuses = ko.observableArray([]);

        self.Initialize = function () {
            self.LoadCountry();
            self.LoadDivisionByCountry();
            self.LoadDistrictByDivision();
            self.LoadThanaByDistrict();
            self.LoadLeadStatuses();
            self.LoadFollowupTypes();
            self.LoadCustomerPriority();
        };

        self.LoadCustomerPriority = function () {
            return $.getJSON("/IPDC/SalesLead/GetCustomerPriorityList",
                null,
                function (data) {
                    self.CustomerPriorities(data);
                });
        }

        self.LoadFollowupTypes = function () {
            return $.getJSON("/IPDC/SalesLead/GetFollowupTypesList",
                null,
                function (data) {
                    self.FollowupTypes(data);
                });
        }
        //self.LoadDistrict = function () {
        //    return $.getJSON("/IPDC/SalesLead/GetAllDistricts",
        //        null,
        //        function (data) {
        //            self.Districts(data);
        //        });
        //}
        //self.LoadThanaByDistrict = function (districtId) {
        //    return $.getJSON("/IPDC/SalesLead/GetThanasByDistrict/?districtId=" + districtId, null, function (data) {
        //        self.Thanas(data);
        //    });
        //}
        self.LoadProducts = function () {
            return $.getJSON("/IPDC/SalesLead/GetAllProducts", null, function (data) {
                self.Products(data);
            });
        }

        self.LoadOrganizations = function () {
            return $.getJSON("/IPDC/SalesLead/GetAllOrganaizations", null, function (data) {
                self.Organizations(data);
            });
        }

        self.LoadLeadTypes = function () {
            return $.getJSON("/IPDC/SalesLead/GetAllLeadTypes", null, function (data) {
                self.LeadTypes(data);
            });
        }

        self.LoadLeadStatuses = function () {
            return $.getJSON("/IPDC/SalesLead/GetAllLeadStatusRM", null, function (data) {
                self.LeadStatuses(data);
            });
        }
        self.Search = function () {
            $.getJSON("/IPDC/SalesLead/GetAssignedSalesLeads", null, function (data) {
                self.shouldShowDetail(true);
                
                self.proSalesLeadList(data);

            });
        };

        self.QuestionSet = function () {
            self.QuestionAnswerSet([]);
            $.getJSON("/IPDC/SalesLead/GetQuestionSetForLead/?leadId=" + self.SalesLeadId(), null, function (data) {
                $.each(data, function (index, value) {
                    self.QuestionAnswerSet.push(new Question(value));
                });
            });
        };

        self.LoadCountry = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Address/GetCountries',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CountryList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.LoadDivisionByCountry = function () {
            //
            if (self.CountryId() > 0) {
                return $.ajax({
                    type: "GET",
                    url: '/IPDC/OfficeDesignationArea/GetDivisionByCountry?id=' + self.CountryId(),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        self.DivisionList(data);
                    },
                    error: function (error) {
                        alert(error.status + "<--and--> " + error.statusText);
                    }
                });
            } else {
                return false;
            }
        }

        self.LoadDistrictByDivision = function () {
            //
            if (self.DivisionId() > 0) {
                return $.ajax({
                    type: "GET",
                    url: '/IPDC/Address/GetDistrictsByDivision?divisionId=' + self.DivisionId(),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        self.Districts(data);
                    },
                    error: function (error) {
                        alert(error.status + "<--and--> " + error.statusText);
                    }
                });
            } else {
                return false;
            }
        }

        self.LoadThanaByDistrict = function () {
            if (self.DistrictId() > 0) {
                return $.ajax({
                    type: "GET",
                    url: '/IPDC/Address/GetThanasByDistrict?districtId=' + self.DistrictId(),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        self.Thanas(data);
                    },
                    error: function (error) {
                        alert(error.status + "<--and--> " + error.statusText);
                    }
                });
            } else {
                return false;
            }
        }

        self.EditSalesLead = function () {
            //self.Initialize();
            $.getJSON("/IPDC/SalesLead/GetSalesLeadForEdit/?leadId=" + self.SalesLeadId(), null, function (data) {
                self.Id(data.Id);
                self.CustomerName(data.CustomerName);
                self.Phone(data.Phone);
                self.Email(data.Email);
                self.AddressLine1(data.AddressLine1);
                self.AddressLine2(data.AddressLine2);
                self.AddressLine3(data.AddressLine3);
                self.Designation(data.Designation);
                self.Age(data.Age);
                self.FollowUpCallTimeText(data.FollowUpCallTimeText);
                self.FollowUpCallTime(data.FollowUpCallTime);
                self.CustomerPriority(data.CustomerPriority);
                self.Remarks(data.Remarks);

                //$.when(self.LoadCountry()).done(function () {
                //    self.CountryId(data.CountryId);
                //    $.when(self.LoadThanaByDistrict(self.CountryId())).done(function () {
                //        self.ThanaId(data.ThanaId);

                //    });
                //});

                //$.when(self.LoadDistrict()).done(function () {
                //    self.DistrictId(data.DistrictId);
                //    $.when(self.LoadThanaByDistrict(self.DistrictId())).done(function () {
                //        self.ThanaId(data.ThanaId);

                //    });
                //});
                //
                $.when(self.LoadCountry()).done(function () {
                    self.CountryId(data.CountryId);
                    $.when(self.LoadDivisionByCountry()).done(function () {
                        self.DivisionId(data.DivisionId);
                        $.when(self.LoadDistrictByDivision()).done(function () {
                            self.DistrictId(data.DistrictId);
                            $.when(self.LoadThanaByDistrict()).done(function () {
                                self.ThanaId(data.ThanaId);
                            });
                        });
                    });
                });

                $.when(self.LoadProducts(), self.LoadLeadTypes(), self.LoadOrganizations(), self.LoadLeadStatuses()).done(function () {
                    self.ProductId(data.ProductId);
                    self.LeadType(data.LeadType);
                    self.OrganizationId(data.OrganizationId);
                    self.LeadStatus(data.LeadStatus);
                    self.LeadStatusName(data.LeadStatusName);
                });
            });
        };

        self.questionnaire = function (details) {

            self.SalesLeadId(details.Id);
            self.QuestionSet();
            $('#questionnaire').modal('show');
        };

        self.followupTime = function (details) {

            self.SalesLeadId(details.Id);
            self.CallTimeTxt(moment(details.FollowupTime));
            //self.CallTimeTxt()
            $('#followup').modal('show');
        };

        //self.edit = function (details) {
        //    self.SalesLeadId(details.Id);
        //    self.EditSalesLead();
        //    $('#edit').modal('show');
        //};

        self.edit = function (details) {
            var parameters = [{
                Name: 'leadEntryId',
                Value: details.Id
            }];
            var menuInfo = {
                Id: urlId++,
                Menu: 'Sales Lead Edit',
                Url: '/IPDC/SalesLead/SalesLeadEntry',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        self.details = function (details) {
            detailVm.LoadDetailsData(details);
            $('#leadDetailDataModal').modal('show');
        };

        self.SubmitQuestionnaire = function () {
            var quesDetails = ko.observableArray([]);
            $.each(self.QuestionAnswerSet(), function (index, value) {
                quesDetails.push({
                    SalesLeadId: value.SalesLeadId,
                    SalesLeadName: value.SalesLeadName,
                    QuestionId: value.QuestionId,
                    QuestionText: value.QuestionText,
                    Answer: value.Answer,
                    QuestionedBy: value.QuestionedBy
                });
            });
            $.ajax({
                type: "POST",
                url: '/IPDC/SalesLead/SaveQuestionnaire',
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

        self.Submit = function () {
            self.NextFollowUpTxt($("#NextFollowUp").val());
            var postData = {
                SalesLeadId: self.SalesLeadId(),
                FollowupType: self.FollowupType(),
                NextFollowUpTxt: self.NextFollowUpTxt(),
                Remarks: self.Remarks(),
                CallTimeTxt: moment(self.CallTimeTxt()).format('DD/MM/YYYY HH:mm')
            }
            $.ajax({
                type: "POST",
                url: '/IPDC/SalesLead/SaveFollowUpTime',
                data: ko.toJSON(postData),
                contentType: "application/json",
                success: function (data) {
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
            ////Ends Here  
        };

        self.EditLead = function () {
            self.FollowUpCallTimeText($("#NextFollowUpEdit").val());
            var quesDetails;
            quesDetails = {
                Id: self.Id(),
                CustomerName: self.CustomerName(),
                Phone: self.Phone(),
                Email: self.Email(),
                AddressLine1: self.AddressLine1(),
                AddressLine2: self.AddressLine2(),
                AddressLine3: self.AddressLine3(),
                CountryId:self.CountryId(),
                DivisionId: self.DivisionId(),
                DistrictId: self.DistrictId(),
                ThanaId: self.ThanaId(),
                ProductId: self.ProductId(),
                LeadType: self.LeadType(),
                OrganizationId: self.OrganizationId(),
                Designation: self.Designation(),
                Age: self.Age(),
                LeadStatus: self.LeadStatus(),
                LeadStatusName: self.LeadStatusName(),
                CustomerPriority: self.CustomerPriority(),
                Remarks: self.Remarks(),
                FollowUpCallTimeText: self.FollowUpCallTimeText(),
                FollowUpCallTime: self.FollowUpCallTime()
            };
            $.ajax({
                type: "POST",
                url: '/IPDC/SalesLead/SaveSalesLead',
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
            ////Ends Here EditLead 
        };
    }

    var vm = new SalesLeadList();
    vm.Search();
    vm.Initialize();
   
    ko.applyBindings(vm, document.getElementById("leadFollowup"));
    detailVm = new leadDetailsData();
    ko.applyBindings(detailVm, document.getElementById('leadDetailDataModal'));
});
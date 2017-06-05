
$(function () {
    $('#NextFollowUp').datetimepicker({ format: 'DD/MM/YYYY HH:mm', minDate: moment() });
    $('#NextFollowUpEdit').datetimepicker({ format: 'DD/MM/YYYY HH:mm', minDate: moment() });
    $('#NextFollowUpDetails').datetimepicker({ format: 'DD/MM/YYYY HH:mm', minDate: moment() });
    $('#CallTimeTxt').datetimepicker({ format: 'DD/MM/YYYY HH:mm', date: moment() });
});


$(document).ready(function () {
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
        self.Name = ko.observable('');
        self.Phone = ko.observable('');
        self.Email = ko.observable('');
        self.Address = ko.observable('');
        self.ThanaId = ko.observable('');
        self.DistrictId = ko.observable('');
        self.ProductId = ko.observable('');
        self.LeadType = ko.observable('');
        self.OrganizationId = ko.observable('');
        self.Designation = ko.observable('');
        self.Age = ko.observable('');
        self.LeadStatus = ko.observable('');
        self.FollowUpCallTimeText = ko.observable('');
        self.FollowUpCallTime = ko.observable('');
        self.Thanas = ko.observableArray([]);
        self.Districts = ko.observableArray([]);
        self.Products = ko.observableArray([]);
        self.LeadTypes = ko.observableArray([]);
        self.Organizations = ko.observableArray([]);
        self.LeadStatuses = ko.observableArray([]);

        self.Initialize = function () {
           
            
            
        };
        self.LoadFollowupTypes = function () {
            return $.getJSON("/IPDC/SalesLead/GetFollowupTypesList",
                null,
                function (data) {
                    self.FollowupTypes(data);
                });
        }
        self.LoadDistrict = function () {
            return $.getJSON("/IPDC/SalesLead/GetAllDistricts",
                null,
                function (data) {
                    self.Districts(data);
                });
        }
        self.LoadThanaByDistrict = function (districtId) {
            return $.getJSON("/IPDC/SalesLead/GetThanasByDistrict/?districtId=" + districtId, null, function (data) {
                self.Thanas(data);
            });
        }
        self.LoadProducts = function() {
            return $.getJSON("/IPDC/SalesLead/GetAllProducts", null, function (data) {
                self.Products(data);
            });
        }

        self.LoadOrganizations = function() {
            return $.getJSON("/IPDC/SalesLead/GetAllOrganaizations", null, function (data) {
                self.Organizations(data);
            });
        }

        self.LoadLeadTypes = function() {
            return $.getJSON("/IPDC/SalesLead/GetAllLeadTypes", null, function (data) {
                //console.log(data);
                self.LeadTypes(data);
            });
        }

        self.LoadLeadStatuses = function() {
            return $.getJSON("/IPDC/SalesLead/GetAllLeadStatus", null, function (data) {
                //console.log(data);
                self.LeadStatuses(data);
            });
        }
        //////// 

        self.Search = function () {
            $.getJSON("/IPDC/SalesLead/GetProspectiveSalesLeads", null, function (data) {
                //console.log(data);
                self.shouldShowDetail(true);
                self.proSalesLeadList(data);

            });
        };

        self.QuestionSet = function () {
            self.QuestionAnswerSet([]);
            $.getJSON("/IPDC/SalesLead/GetQuestionSetForLead/?leadId=" + self.SalesLeadId(), null, function (data) {
                //console.log(data);
                //self.shouldShowDetail(true);
                $.each(data, function (index, value) {
                    console.log(value);
                    self.QuestionAnswerSet.push(new Question(value));
                });
                //self.QuestionAnswerSet(data);

            });
        };
        self.EditSalesLead = function () {
            
            //self.QuestionAnswerSet([]);
            self.Initialize();
            $.getJSON("/IPDC/SalesLead/GetSalesLeadForEdit/?leadId=" + self.SalesLeadId(), null, function (data) {
                
                console.log("selected lead data " + data);
                self.Id(data.Id);
                self.Name(data.Name);
                self.Phone(data.Phone);
                self.Email(data.Email);
                self.Address(data.Address);
                self.Designation(data.Designation);
                self.Age(data.Age);
                self.FollowUpCallTimeText(data.FollowUpCallTimeText);
                self.FollowUpCallTime(data.FollowUpCallTime);

                $.when(self.LoadDistrict()).done(function () {
                    //console.log("districtId" + data.DistrictId);
                    self.DistrictId(data.DistrictId);
                    //console.log("district id assigned - " + self.DistrictId());
                    $.when(self.LoadThanaByDistrict(self.DistrictId())).done(function() {
                        self.ThanaId(data.ThanaId);
                        $.when(self.LoadProducts(), self.LoadLeadTypes(), self.LoadOrganizations(), self.LoadLeadStatuses()).done(function() {
                            self.ProductId(data.ProductId);
                            self.LeadType(data.LeadType);
                            self.OrganizationId(data.OrganizationId);
                            self.LeadStatus(data.LeadStatus);
                        });
                    });
                });
            });
        };
        self.questionnaire = function (details) {

            self.SalesLeadId(details.Id);
            self.QuestionSet();
            $('#questionnaire').modal('show');

            //console.log("det " + details.Name); Submit
            //window.location.href = "/Somity/Member/MemberSavings?memberId=" + details.MemberIdNo + "&memberName=" + details.Name;
        };
        self.followupTime = function (details) {

            self.SalesLeadId(details.Id);
            self.CurrentFollowUp(moment(details.FollowUpCallTime).format('DD/MMM/YYYY'));

            $('#followup').modal('show');
        };
        self.edit = function (details) {
            //
            self.SalesLeadId(details.Id);
            self.EditSalesLead();
            $('#edit').modal('show');


        };
        self.details = function (details) {
            self.SalesLeadId(details.Id);
            self.EditSalesLead();
            $('#details').modal('show');
        };
        self.SubmitQuestionnaire = function () {
            //console.log(self.QuestionAnswerSet());
            var quesDetails = ko.observableArray([]);
            $.each(self.QuestionAnswerSet(), function (index, value) {
                //
                //console.log("value: " + value);
                quesDetails.push({
                    SalesLeadId: value.SalesLeadId,
                    SalesLeadName: value.SalesLeadName,
                    QuestionId: value.QuestionId,
                    QuestionText: value.QuestionText,
                    Answer: value.Answer,
                    QuestionedBy: value.QuestionedBy
                });
            });
            //console.log(ko.toJSON(self));
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
            self.CallTimeTxt($("#CallTimeTxt").val());
            //OrderDate: moment(self.OrderDate()).format('DD/MM/YYYY'),
            //$.each(self.SODetails(), function (key, value) {
            //    value.Status = 1;
            //});
            //self.OrderDate(moment(self.OrderDate()).format('DD/MM/YYYY'));
            //console.log(ko.toJSON(self));
            var postData = {
                SalesLeadId: self.SalesLeadId(),
                FollowupType: self.FollowupType(),
                NextFollowUpTxt: self.NextFollowUpTxt(),
                Remarks: self.Remarks(),
                FollowupType: self.FollowupType(),
                CallTimeTxt: self.CallTimeTxt()
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
                Id : self.Id(),
                Name: self.Name(),
                Phone: self.Phone(),
                Email: self.Email(),
                Address: self.Address(),
                ThanaId: self.ThanaId(),
                DistrictId: self.DistrictId(),
                ProductId: self.ProductId(),
                LeadType: self.LeadType(),
                OrganizationId: self.OrganizationId(),
                Designation: self.Designation(),
                Age: self.Age(),
                LeadStatus: self.LeadStatus(),
                FollowUpCallTimeText: self.FollowUpCallTimeText(),
                FollowUpCallTime: self.FollowUpCallTime()
            };
            //console.log(ko.toJSON(self));
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
    vm.LoadFollowupTypes();
    ko.applyBindings(vm, document.getElementById("memberSearch"));
});
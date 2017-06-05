$(function () {
    $('#NextFollowUp').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
    $('#NextFollowUpEdit').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
    $('#NextFollowUpDetails').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
});
$(document).ready(function () {

    function receiveDetail(data) {
        var self = this;
        //self.SalesLeadId = ko.observable(data.SalesLeadId ? data.SalesLeadId : 0);
        //self.SalesLeadName = ko.observable(data.SalesLeadName ? data.SalesLeadName : '');
        //self.QuestionId = ko.observable(data.QuestionId ? data.QuestionId : 0).extend({ digit: true });
        //self.QuestionText = ko.observable(data.QuestionText ? data.QuestionText : 0);
        //self.Answer = ko.observable(data.Answer ? data.Answer : '');
        //self.QuestionedBy = ko.observable(data.QuestionedBy ? data.QuestionedBy : 0);
    };
    function SalesLeadList() {
        var self = this;
        self.OfficeLayerId = ko.observable('');
        self.OfficeId = ko.observable('');
        self.DesignationId = ko.observable('');
        self.DivisionId = ko.observable('');
        self.DistrictId = ko.observable('');
        self.UpazilaId = ko.observable('');
        self.area = ko.observable('');

        self.OfficeLayers = ko.observableArray([]);
        self.Offices = ko.observableArray([]);
        self.Designations = ko.observableArray([]);
        self.Upazilas = ko.observableArray([]);
        self.Districts = ko.observableArray([]);
        self.Divisions = ko.observableArray([]);
        self.LoadData = ko.observableArray([]);
        self.SngArea = ko.observableArray([]);

        self.Initialize = function () {
        };
        self.getOfficeLayers = function () {
           
            $.ajax({
                type: "GET",
                url: '/IPDC/OfficeLayer/GetOfficeLayers',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.OfficeLayers(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.getOfficeByOfficeLayer = function () {
            var officeLayer = self.OfficeLayerId() ? self.OfficeLayerId() : 0;

            $.ajax({
                type: "GET",
                url: '/IPDC/Office/GetOfficeByLayer?officelayerid=' + officeLayer,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.Offices(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
         self.getDesignationByOffice = function () {
             var office = self.OfficeId() ?self.OfficeId() : 0;

            $.ajax({
                type: "GET",
                url: '/IPDC/OfficeDesignationSetting/GetDesignationSettingListByOffice?OfficeId=' + office,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.Designations(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

         self.getOfficeDesgArea = function () {
             var setng = self.DesignationId() ? self.DesignationId() : 0;

             $.ajax({
                 type: "GET",
                 url: '/IPDC/OfficeDesignationArea/GetOfficeDesgArea?id=' + setng,
                 contentType: "application/json; charset=utf-8",
                 dataType: "json",
                 success: function (data) {
                     self.SngArea(data);
                 },
                 error: function (error) {
                     alert(error.status + "<--and--> " + error.statusText);
                 }
             });
         }


         self.removeDetail = function (receiveDetail) {
             //self.ReceiveDetails.remove(receiveDetail);
             $.getJSON("/IPDC/OfficeDesignationArea/UpdateOfficeDesignationArea/?id=" + receiveDetail.Id, null, function (data) {
                 
             });
             self.Reset();
         };

         self.addDetail = function (receiveDetail) {
             //self.ReceiveDetails.remove(receiveDetail);
             var quesDetails;
             quesDetails = {
                 OfficeDesignationSettingId: self.DesignationId(),
                 UpozilaOrThana: self.area(),
                 RefId: receiveDetail.Id
         };
             $.ajax({
                 type: "POST",
                 url: '/IPDC/OfficeDesignationArea/SaveOfficeDesignationArea',
                 data: ko.toJSON(quesDetails),
                 contentType: "application/json",
                 success: function (data) {
                     $('#successModal').modal('show');
                     $('#successModalText').text(data.Message);
                     self.Reset();

                 },
                 error: function () {
                     alert(error.status + "<--and--> " + error.statusText);
                 }
             });
         };
        // self.LoadDivision = function () { /IPDC/OfficeDesignationSetting/GetDesignationSettingListByOffice?OfficeId=" + self.OfficeId()
        //     return $.getJSON("/IPDC/OfficeDesignationArea/GetAllDivisions",
        //        null,
        //        function (data) {
        //            self.Divisions(data);
        //        });
        //} getDistrictByDivision

        self.LoadDivision = function () {
            $.ajax({
                type: "GET",
                url: '/IPDC/OfficeDesignationArea/GetAllDivisions',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.Divisions(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
          
        }
        self.getDistrictByDivision = function () {
            var divisionId = self.DivisionId() ? self.DivisionId() : 0;

            $.ajax({
                type: "GET",
                url: '/IPDC/OfficeDesignationArea/GetDistrictByDivision?id=' + divisionId,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.Districts(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.getUpzilaByDistrict = function () {
            var districtId = self.DistrictId() ? self.DistrictId() : 0;

            $.ajax({
                type: "GET",
                url: '/IPDC/OfficeDesignationArea/GetUpzilaByDistrict?id=' + districtId,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.Upazilas(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        //getByOfficeDesignationMap
        self.getByOfficeDesignationMap = function () {
            //
            var area = self.area() ? self.area() : 0;
            if (self.DistrictId()) {
                $.ajax({
                    type: "GET",
                    url: '/IPDC/OfficeDesignationArea/GetByOfficeDesignationMap?upazilaOrThana=' + area + '&dist=' + self.DistrictId() + '&settingId=' + self.DesignationId(),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        self.LoadData(data);
                    },
                    error: function (error) {
                        alert(error.status + "<--and--> " + error.statusText);
                    }
                });
            }
            
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
        self.LoadProducts = function () {
            return $.getJSON("/IPDC/SalesLead/GetAllProducts", null, function (data) {
                self.Designations(data);
            });
        }


        //self.Search = function () {
        //    $.getJSON("/IPDC/SalesLead/GetProspectiveSalesLeads", null, function (data) {
        //        self.shouldShowDetail(true);
        //        self.proSalesLeadList(data);

        //    });
        //};

        //self.QuestionSet = function () {
        //    self.QuestionAnswerSet([]);
        //    $.getJSON("/IPDC/SalesLead/GetQuestionSetForLead/?leadId=" + self.SalesLeadId(), null, function (data) {
        //        //self.shouldShowDetail(true);
        //        $.each(data, function (index, value) {
        //            self.QuestionAnswerSet.push(new receiveDetail(value));
        //        });
        //        //self.QuestionAnswerSet(data);

        //    });
        //};
        //self.EditSalesLead = function () {
        //    //self.QuestionAnswerSet([]);
        //    self.Initialize();
        //    $.getJSON("/IPDC/SalesLead/GetSalesLeadForEdit/?leadId=" + self.SalesLeadId(), null, function (data) {
        //        self.Id(data.Id);
        //        self.Name(data.Name);
        //        self.Phone(data.Phone);
        //        self.Email(data.Email);
        //        self.Address(data.Address);
        //        self.Designation(data.Designation);
        //        self.Age(data.Age);
        //        self.FollowUpCallTimeText(data.FollowUpCallTimeText);
        //        self.FollowUpCallTime(data.FollowUpCallTime);

        //        $.when(self.LoadDistrict()).done(function () {
        //            self.DistrictId(data.DistrictId);
        //            $.when(self.LoadThanaByDistrict(self.DistrictId())).done(function () {
        //                self.ThanaId(data.ThanaId);
        //                $.when(self.LoadProducts(), self.LoadLeadTypes(), self.LoadOrganizations(), self.LoadLeadStatuses()).done(function () {
        //                    self.ProductId(data.ProductId);
        //                    self.LeadType(data.LeadType);
        //                    self.OrganizationId(data.OrganizationId);
        //                    self.LeadStatus(data.LeadStatus);
        //                });
        //            });
        //        });
        //    });
        //};
        //self.questionnaire = function (details) {

        //    self.SalesLeadId(details.Id);
        //    self.QuestionSet();
        //    $('#questionnaire').modal('show');

        //    //window.location.href = "/Somity/Member/MemberSavings?memberId=" + details.MemberIdNo + "&memberName=" + details.Name;
        //};
        //self.followupTime = function (details) {

        //    self.SalesLeadId(details.Id);
        //    self.CurrentFollowUp(moment(details.FollowUpCallTime).format('DD/MMM/YYYY'));
        //    $('#followup').modal('show');
        //};
        //self.edit = function (details) {
        //    self.SalesLeadId(details.Id);
        //    self.EditSalesLead();
        //    $('#edit').modal('show');


        //};
        //self.details = function (details) {
        //    self.SalesLeadId(details.Id);
        //    self.EditSalesLead();
        //    $('#details').modal('show');
        //};
        //self.SubmitQuestionnaire = function () {
        //    var quesDetails = ko.observableArray([]);
        //    $.each(self.QuestionAnswerSet(), function (index, value) {
        //        quesDetails.push({
        //            SalesLeadId: value.SalesLeadId,
        //            SalesLeadName: value.SalesLeadName,
        //            QuestionId: value.QuestionId,
        //            QuestionText: value.QuestionText,
        //            Answer: value.Answer,
        //            QuestionedBy: value.QuestionedBy
        //        });
        //    });
        //    $.ajax({
        //        type: "POST",
        //        url: '/IPDC/SalesLead/SaveQuestionnaire',
        //        data: ko.toJSON(quesDetails),
        //        contentType: "application/json",
        //        success: function (data) {
        //            $('#successModal').modal('show');
        //            $('#successModalText').text(data.Message);
        //        },
        //        error: function () {
        //            alert(error.status + "<--and--> " + error.statusText);
        //        }
        //    });
        //}
        //self.Submit = function () {
        //    self.NextFollowUpTxt($("#NextFollowUp").val());
        //    //OrderDate: moment(self.OrderDate()).format('DD/MM/YYYY'),
        //    //$.each(self.SODetails(), function (key, value) {
        //    //    value.Status = 1;
        //    //});
        //    //self.OrderDate(moment(self.OrderDate()).format('DD/MM/YYYY'));
        //    $.ajax({
        //        type: "POST",
        //        url: '/IPDC/SalesLead/SaveFollowUpTime',
        //        data: ko.toJSON(self),
        //        contentType: "application/json",
        //        success: function (data) {
        //            $('#successModal').modal('show');
        //            $('#successModalText').text(data.Message);
        //        },
        //        error: function () {
        //            alert(error.status + "<--and--> " + error.statusText);
        //        }
        //    });
        //    ////Ends Here  
        //};
        self.Save = function () {
            self.FollowUpCallTimeText($("#NextFollowUpEdit").val());
            var quesDetails;
            quesDetails = {
                Id: self.Id(),
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
            $.ajax({
                type: "POST",
                url: '/IPDC/SalesLead/SaveSalesLead',
                data: ko.toJSON(quesDetails),
                contentType: "application/json",
                success: function (data) {
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);
                    self.Reset();
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
            ////Ends Here EditLead 
        };
        self.Search = function () {

        }
        self.Reset = function () {
            self.SngArea([]);
            self.LoadData([]);
            self.getOfficeDesgArea();
            self.getByOfficeDesignationMap();
            //self.OfficeLayerId('');
            //self.OfficeId('');
            //self.DesignationId('');
            //self.DivisionId('');
            //self.DistrictId('');
            //self.UpazilaId('');
        }

    }

    var vm = new SalesLeadList();
    vm.Search();
    vm.LoadDivision();
    vm.getOfficeLayers();
    ko.applyBindings(vm, document.getElementById("memberSearch"));
});
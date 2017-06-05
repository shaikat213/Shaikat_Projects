
$(document).ready(function () {

    function receiveDetail(data) {
        var self = this;
    };
    function SalesLeadList() {
        var self = this;
        self.OfficeLayerId = ko.observable('');
        self.OfficeId = ko.observable('');
        self.DesignationId = ko.observable('');
        self.RoleId = ko.observable('');
        self.DistrictId = ko.observable('');
        self.UpazilaId = ko.observable('');
        self.area = ko.observable('');

        self.OfficeLayers = ko.observableArray([]);
        self.Offices = ko.observableArray([]);
        self.Designations = ko.observableArray([]);
        self.Upazilas = ko.observableArray([]);
        self.Districts = ko.observableArray([]);
        self.Roles = ko.observableArray([]);
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
            var office = self.OfficeId() ? self.OfficeId() : 0;

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
                url: '/IPDC/Designation/GetDesignationRoleMapping?id=' + setng,
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
            $.getJSON("/IPDC/Designation/RemoveDesignationRoleMap/?id=" + receiveDetail.Id, null, function (data) {

            });
            self.Reset();
        };

        self.addDetail = function (receiveDetail) {
            
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
 
        self.LoadRoles = function () {
            $.ajax({
                type: "GET",
                url: '/IPDC/Designation/GetAllRoles',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.Roles(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });

        }

        self.getByOfficeDesignationMap = function () {
            
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
        self.addData = function () {
            var quesDetails;
            quesDetails = {
                OfficeDesignationSettingId: self.DesignationId(),
                RoleId: self.RoleId()
            };
            $.ajax({
                type: "POST",
                url: '/IPDC/Designation/SaveDesignationRoleMapping',
                data: ko.toJSON(quesDetails),
                contentType: "application/json",
                success: function (data) {
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);
                    self.getOfficeDesgArea();
                    //self.Reset();
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
            self.OfficeLayerId('');
            self.OfficeId('');
            self.DesignationId('');
        
        }

    }

    var vm = new SalesLeadList();
    vm.Search();
    vm.LoadRoles();
    vm.getOfficeLayers();
    ko.applyBindings(vm, document.getElementById("memberSearch"));
});
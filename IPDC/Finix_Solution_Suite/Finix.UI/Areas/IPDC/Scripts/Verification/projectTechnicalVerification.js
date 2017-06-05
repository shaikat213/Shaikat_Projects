ko.validation.rules['dateValidation'] = {
    validator: function (val, validate) {
        return ko.validation.utils.isEmptyVal(val) || moment(val, 'DD/MM/YYYY').isValid();
    },
    message: 'Invalid date'
};
ko.validation.registerExtenders();

$(document).ready(function () {

    var appvm;
    ko.validation.init({
        errorElementClass: 'has-error',
        errorMessageClass: 'help-block',
        decorateInputElement: true,
        grouping: { deep: true, observable: true }
    });


    function ProjectTechVm() {

        var self = this;
        self.Id = ko.observable();
        self.ProjectId = ko.observable();
        //self.Project = ko.observable();
        self.PlanApprovalAuthority = ko.observable();
        self.ProjectPropertyType = ko.observable();
        self.AreaOfLandPP = ko.observable();
        //self.AreaOfLandPPUomId = ko.observable();

        //self.AreaOfLandPPUOM = ko.observable();
        self.AccessRoadWidth = ko.observable();
        self.ApprovedNoOfFloors = ko.observable();
        self.ApprovedNoOfUnitsPerFloor = ko.observable();
        self.ApprovedTotalNoOfUnits = ko.observable();
        self.ApprovedTotalFloorArea = ko.observable();
        self.DeviatedFromApproved = ko.observable(false);
        self.YesDeviatedFromApproved = ko.observable(false);
        self.NoDeviatedFromApproved = ko.observable(false);
        self.YesDeviatedFromApproved.subscribe(function () {
            self.DeviatedFromApproved(true);
        });
        self.NoDeviatedFromApproved.subscribe(function () {
            self.DeviatedFromApproved(false);
        });
        self.ActualNoOfFloors = ko.observable();
        self.ActualNoOfUnitsPerFloor = ko.observable();
        self.ActualTotalNoOfUnits = ko.observable();
        self.ActualTotalFloorArea = ko.observable();
        self.TotalDeviationInFloorArea = ko.computed(function () {
            var approve = 0;
            var actual = 0;
            var result = 0;
            actual = parseFloat(self.ActualTotalFloorArea() ? self.ActualTotalFloorArea() : 0);
            approve = parseFloat(self.ApprovedTotalFloorArea() ? self.ApprovedTotalFloorArea() : 0);
            result = (actual - approve);
            return result;
        });
        self.TotalDeviationPercentage = ko.computed(function () {
            var approve = 0;
            var total = 0;
            var result = 0;
            if (self.ApprovedTotalFloorArea()>0) {
                total = parseFloat(self.TotalDeviationInFloorArea() ? self.TotalDeviationInFloorArea() : 0);
                approve = parseFloat(self.ApprovedTotalFloorArea() ? self.ApprovedTotalFloorArea() : 0);
                result = (total / approve) * 100;
            }
            return result;
        });
        //= ko.observable();
        self.NorthBound = ko.observable();
        self.SouthBound = ko.observable();
        self.EastBound = ko.observable();
        self.WestBound = ko.observable();
        self.ProjectCompletionPercentage = ko.observable();
        //public  List<ProjectImages> Photos = ko.observable();
        self.TechnicalApprovalStatus = ko.observable();
        self.ProjectTechnicalVerificationFile = ko.observable();
        self.ProjectTechnicalVerificationFileName = ko.observable();
        self.ProjectTechnicalVerificationPath = ko.observable();
        self.Remarks = ko.observable();
        self.getEditUrl = function () {
            return '/IPDC/Verification/Download?fileBytes=' + self.ProjectTechnicalVerificationPath() + '&fileName=' + self.ProjectTechnicalVerificationFileName();
        }

        self.errors = ko.validation.group(self);
        self.IsValid = ko.computed(function () {
            console.log("error count - " + self.errors().length);
            if (self.errors().length === 0)
                return true;
            else {
                return false;
            }

        });

        ////////////////////////////////////////////////Latest///////////////////////////////////////////////////////////
        self.ProjectApprovalAuthorities = ko.observableArray([]);
        self.PropertyTypes = ko.observableArray([]);
        self.PropertyBounds = ko.observableArray([]);
        self.ApprovalStatuses = ko.observableArray([]);
        self.GetProjectApprovalAuthority = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/GetProjectApprovalAuthority',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ProjectApprovalAuthorities(data); //Put the response in ObservableArray

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.GetPropertyType = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/GetPropertyType',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.PropertyTypes(data); //Put the response in ObservableArray

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.GetPropertyBounds = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/GetPropertyBounds',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.PropertyBounds(data); //Put the response in ObservableArray

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.GetApprovalStatus = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/GetApprovalStatus',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ApprovalStatuses(data); //Put the response in ObservableArray

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        self.LoadProjectTechnicalData = function () {
            debugger;
            console.log(self.Id());
            if (self.Id() > 0) {
                $.getJSON("/IPDC/Verification/LoadProjectTechnicalData/?id=" + self.Id(),//+ '&CifId=' + self.CifId() + '&IsnId=' + self.Id()
                    null,
                    function (data) {
                        console.log(data);
                        self.Id(data.Id),
                        self.ProjectId(data.ProjectId),
                        $.when(self.GetProjectApprovalAuthority())
                            .done(function () {
                                self.PlanApprovalAuthority(data.PlanApprovalAuthority);
                            });
                        $.when(self.GetPropertyType())
                            .done(function () {
                                self.ProjectPropertyType(data.ProjectPropertyType);
                            });
                        $.when(self.GetPropertyBounds())
                            .done(function () {
                                self.NorthBound(data.NorthBound);
                                self.SouthBound(data.SouthBound);
                                self.EastBound(data.EastBound);
                                self.WestBound(data.WestBound);
                            });
                        $.when(self.GetApprovalStatus())
                            .done(function () {
                                self.TechnicalApprovalStatus(data.TechnicalApprovalStatus);
                            });
                        self.AreaOfLandPP(data.AreaOfLandPP);
                        self.AccessRoadWidth(data.AccessRoadWidth);
                        self.ApprovedNoOfFloors(data.ApprovedNoOfFloors);
                        self.ApprovedNoOfUnitsPerFloor(data.ApprovedNoOfUnitsPerFloor);
                        self.ApprovedTotalNoOfUnits(data.ApprovedTotalNoOfUnits);
                        self.ApprovedTotalFloorArea(data.ApprovedTotalFloorArea);
                        self.DeviatedFromApproved(data.DeviatedFromApproved + "");
                        self.ActualNoOfFloors(data.ActualNoOfFloors);
                        self.ActualNoOfUnitsPerFloor(data.ActualNoOfUnitsPerFloor);
                        self.ActualTotalNoOfUnits(data.ActualTotalNoOfUnits);
                        self.ActualTotalFloorArea(data.ActualTotalFloorArea);
                        //debugger;
                        //self.TotalDeviationInFloorArea(data.TotalDeviationInFloorArea);
                        self.ProjectCompletionPercentage(data.ProjectCompletionPercentage);
                        self.Remarks(data.Remarks);
                        //self.ProjectTechnicalVerificationFile(data.ProjectTechnicalVerificationFile);
                        self.ProjectTechnicalVerificationFileName(data.ProjectTechnicalVerificationFileName);
                        self.ProjectTechnicalVerificationPath(data.ProjectTechnicalVerificationPath);
                       

                    });
            }
        }
        self.IsEdit = ko.observable(false);
        self.Create = function () {
            self.IsEdit(false);
            self.Submit();
        }
        self.Edit = function () {
            
            self.IsEdit(true);
            self.Submit();
            console.log(self.IsEdit());
        }
        self.Submit = function () {
            var file_data;
            if (typeof (self.ProjectTechnicalVerificationFile()) != 'undefined') {
                file_data = $('#ProjectTechnicalVerificationFile').prop('files')[0];
            }
            if (self.IsEdit() === false) {
                self.Id(0);
            }
            var formData = new FormData();
            formData.append('Id', self.Id());
            formData.append('ProjectId', self.ProjectId());
            formData.append('PlanApprovalAuthority', self.PlanApprovalAuthority());
            formData.append('ProjectPropertyType', self.ProjectPropertyType());
            formData.append('AreaOfLandPP', self.AreaOfLandPP());
            formData.append('AccessRoadWidth', self.AccessRoadWidth());
            formData.append('ApprovedNoOfFloors', self.ApprovedNoOfFloors());
            formData.append('ApprovedNoOfUnitsPerFloor', self.ApprovedNoOfUnitsPerFloor());
            formData.append('ApprovedTotalNoOfUnits', self.ApprovedTotalNoOfUnits());
            formData.append('ApprovedTotalFloorArea', self.ApprovedTotalFloorArea());
            formData.append('DeviatedFromApproved', self.DeviatedFromApproved());
            formData.append('ActualNoOfFloors', self.ActualNoOfFloors());
            formData.append('ActualNoOfUnitsPerFloor', self.ActualNoOfUnitsPerFloor());
            formData.append('ActualTotalNoOfUnits', self.ActualTotalNoOfUnits());
            formData.append('ActualTotalFloorArea', self.ActualTotalFloorArea());
            formData.append('TotalDeviationInFloorArea', self.TotalDeviationInFloorArea());
            formData.append('TotalDeviationPercentage', self.TotalDeviationPercentage());
            formData.append('NorthBound', self.NorthBound());
            formData.append('SouthBound', self.SouthBound());
            formData.append('EastBound', self.EastBound());
            formData.append('WestBound', self.WestBound());
            formData.append('ProjectCompletionPercentage', self.ProjectCompletionPercentage());
            formData.append('TechnicalApprovalStatus', self.TechnicalApprovalStatus());
            formData.append('ProjectTechnicalVerificationFile', file_data);
            formData.append('ProjectTechnicalVerificationFileName', self.ProjectTechnicalVerificationFileName());
            formData.append('ProjectTechnicalVerificationFile', self.ProjectTechnicalVerificationPath());            
            formData.append('Remarks', self.Remarks());
            $.ajax({
                type: "POST",
                url: '/IPDC/Verification/SaveProjectTechnicalVerification',
                data: formData,
                contentType: false,
                processData: false,
                cache: false,
                success: function (data) {
                    $('#SuccessModal').modal('show');
                    $('#SuccessModalText').text(data.Message);
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        };

        self.Initialize = function () {
            
            self.GetProjectApprovalAuthority();
            self.GetPropertyType();
            self.GetPropertyBounds();
            self.GetApprovalStatus();
            console.log(self.Id());
            if (self.Id() > 0) {
                self.LoadProjectTechnicalData();
            }
        }
        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }
    appvm = new ProjectTechVm();
    var qValue = appvm.queryString('projectId');
    appvm.ProjectId(qValue);
    var qValueId = appvm.queryString('id');
    appvm.Id(qValueId);
    appvm.Initialize();
    ko.applyBindings(appvm, $('#ProjectTechVw')[0]);
});




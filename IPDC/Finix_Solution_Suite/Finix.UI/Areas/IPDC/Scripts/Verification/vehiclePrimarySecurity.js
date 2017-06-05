
$(function () {
    $('#VarificationDateId').datetimepicker({ format: 'DD/MM/YYYY' });

});
$(document).ready(function () {

    var appvm;
    ko.validation.init({
        errorElementClass: 'has-error',
        errorMessageClass: 'help-block',
        decorateInputElement: true,
        grouping: { deep: true, observable: true }
    });

    function applicationCIFs(data) {
        var self = this;
        self.Id = ko.observable(data ? data.Id : 0);
        self.ApplicationId = ko.observable(data ? data.ApplicationId : 0);
        self.CIF_PersonalId = ko.observable(data ? data.CIF_PersonalId : 0);
        self.ApplicantRole = ko.observable(data ? data.ApplicantRole : 0);
        self.ApplicantRoleName = ko.observable(data ? data.ApplicantRoleName : '');
        self.CIF_OrganizationalId = ko.observable(data ? data.CIF_OrganizationalId : 0);
        self.ApplicantName = ko.observable(data ? data.ApplicantName : '');
        self.Age = ko.observable(data ? data.Age : 0);
        self.ProfessionName = ko.observable(data ? data.ProfessionName : '');
        self.MonthlyIncome = ko.observable(data ? data.MonthlyIncome : 0);
        self.CIFNo = ko.observable(data ? data.CIFNo : 0);

        self.IsAddOption = ko.observable(data ? data.ApplicantRole === 1 || data.ApplicantRole === 2 ? true : false : false);
        self.checkCoApp = function () {
            //
            if (self.ApplicantRole() === 1) {
                self.IsAddOption(true);
            }
            if (self.ApplicantRole() === 2) {
                self.IsAddOption(true);
            }
        }
        self.LoadData = function (data) {
            self.Id(data ? data.Id : 0);
            self.ApplicationId(data ? data.ApplicationId : 0);
            self.CIF_PersonalId(data ? data.CIF_PersonalId : 0);
            self.ApplicantRole(data ? data.ApplicantRole : 0);
            self.ApplicantRoleName(data ? data.ApplicantRoleName : 0);
            self.CIF_OrganizationalId(data ? data.CIF_OrganizationalId : 0);
            self.ApplicantName(data ? data.ApplicantName : '');
            self.Age(data ? data.Age : 0);
            self.ProfessionName(data ? data.ProfessionName : '');
            self.MonthlyIncome(data ? data.MonthlyIncome : 0);
            self.CIFNo(data ? data.CIFNo : 0);
            self.checkCoApp();
        }

    }
    function address() {
        var self = this;
        self.IsChanged = ko.observable(false);
        self.Id = ko.observable();
        self.CountryId = ko.observable().extend({ required: true });
        self.CountryName = ko.observable('');
        //self.CountryName.subscribe(function () { self.IsChanged(true); });
        self.ThanaId = ko.observable();
        self.ThanaName = ko.observable('');
        self.DistrictId = ko.observable();
        self.DistrictName = ko.observable('');
        self.DivisionId = ko.observable();
        self.DivisionName = ko.observable('');

        self.AddressLine1 = ko.observable('');
        self.AddressLine1.subscribe(function () { self.IsChanged(true); });
        self.AddressLine2 = ko.observable('');
        self.AddressLine2.subscribe(function () { self.IsChanged(true); });
        self.AddressLine3 = ko.observable('');
        self.AddressLine3.subscribe(function () { self.IsChanged(true); });
        self.PostalCode = ko.observable('');
        self.PostalCode.subscribe(function () { self.IsChanged(true); });
        self.PhoneNo = ko.observable('');
        self.PhoneNo.subscribe(function () { self.IsChanged(true); });
        self.CellPhoneNo = ko.observable('');
        self.CellPhoneNo.subscribe(function () { self.IsChanged(true); });
        self.Email = ko.observable('');
        self.Email.subscribe(function () { self.IsChanged(true); });
        self.ThanaId.subscribe(function () { self.IsChanged(true); });

        self.DivisionList = ko.observableArray([]);
        self.DistrictList = ko.observableArray([]);
        self.ThanaList = ko.observableArray([]);


        self.CountryId.subscribe(function () {
            self.IsChanged(true);
            //var officecountryId = self.CountryId();
            if (self.CountryId() > 0) {
                self.LoadDivisionByCountry();
            }

        });

        self.DivisionId.subscribe(function () {
            self.IsChanged(true);
            //var officecountryId = self.CountryId();
            if (self.DivisionId() > 0) {
                self.LoadDistrictByDivision();
            }

        });

        self.DistrictId.subscribe(function () {
            self.IsChanged(true);
            //var officecountryId = self.CountryId();
            if (self.DistrictId() > 0) {
                self.LoadThanaByDistrict();
            }
        });

        self.LoadDivisionByCountry = function () {
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
            if (self.DivisionId() > 0) {
                return $.ajax({
                    type: "GET",
                    url: '/IPDC/Address/GetDistrictsByDivision?divisionId=' + self.DivisionId(),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        self.DistrictList(data);
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
                        self.ThanaList(data);
                    },
                    error: function (error) {
                        alert(error.status + "<--and--> " + error.statusText);
                    }
                });
            } else {
                return false;
            }
        }

        self.LoadAddress = function (data) {
            if (data != null) {
                self.Id(data.Id);
                self.CountryId(data.CountryId);
                self.CountryName(data.CountryName);
                self.ThanaName(data.ThanaName);
                self.DistrictName(data.DistrictName);
                self.DivisionName(data.DivisionName);

                self.AddressLine1(data.AddressLine1);
                self.AddressLine2(data.AddressLine2);
                self.AddressLine3(data.AddressLine3);
                self.PostalCode(data.PostalCode);
                self.PhoneNo(data.PhoneNo);
                self.CellPhoneNo(data.CellPhoneNo);
                self.Email(data.Email);
                $.when(self.LoadDivisionByCountry()).done(function () {
                    self.DivisionId(data.DivisionId);
                    self.IsChanged(false);
                    $.when(self.LoadDistrictByDivision()).done(function () {
                        self.DistrictId(data.DistrictId);
                        self.IsChanged(false);
                        $.when(self.LoadThanaByDistrict()).done(function () {
                            self.ThanaId(data.ThanaId);
                            self.IsChanged(false);
                        });
                    });
                });
                //
                self.IsChanged(false);
            }

        }
    }
    function VehiclePrimarySecurityVm() {

        var self = this;
        var currentDate = (new Date()).toISOString().split('T')[0];
        //self.FromDate = ko.observable(currentDate);
        var i = 0;
        for (i = new Date().getFullYear() + 50 ; i > new Date().getFullYear() - 100; i--) {
            $('#MnufacturingYear').append($('<option />').val(i).html(i));
            $('#YearModel').append($('<option />').val(i).html(i));
            $('#RegistrationYear').append($('<option />').val(i).html(i));
        }
        self.AppId = ko.observable();
        self.Id = ko.observable();
        self.ApplicationNo = ko.observable();
        self.VehiclePrimarySecurityId = ko.observable();
        self.VehiclePrimarySecurity = ko.observable();
        self.VerificationDate = ko.observable();
        self.VerifiedByUserId = ko.observable();
        self.VerifiedByEmpDegMapId = ko.observable();

        self.VerifiedPrice = ko.observable();
        self.VerificationMethod = ko.observable();
        self.Remarks = ko.observable();
        self.VerificationState = ko.observable();
        /*Vehicle*/
        self.VehicleStatus = ko.observable();
        self.VehicleStatusName = ko.observable();
        self.VendorType = ko.observable();
        self.VerificationDateText = ko.observable(moment(currentDate).format("DD/MM/YYYY"));
        self.IsSeller = ko.observable(false);
        self.IsVendor = ko.observable(true);
        self.VendorType.subscribe(function () {
            if (self.VendorType() === 1) {
                self.IsSeller(true);
                self.IsVendor(false);
            } else {
                self.IsSeller(false);
                self.IsVendor(true);
            }
        });
        self.VendorTypeName = ko.observable();
        self.VehicleId = ko.observable();
        self.SellersName = ko.observable();
        self.SellersAddressId = ko.observable();
        self.SellersAddress = new address();
        self.SellersPhone = ko.observable();
        self.VendorId = ko.observable();
        self.VendorName = ko.observable();
        self.VehicleType = ko.observable();
        self.VehicleTypeName = ko.observable();
        self.Manufacturer = ko.observable();
        self.MnufacturingYear = ko.observable();
        self.YearModel = ko.observable();
        self.RegistrationYear = ko.observable();
        self.RegistrationNo = ko.observable();
        self.CC = ko.observable();
        self.Colour = ko.observable();
        self.ChassisNo = ko.observable();
        self.EngineNo = ko.observable();
        self.Price = ko.observable();
        
        self.GuarantorCifList = ko.observableArray([]);
        self.CountryIdList = ko.observableArray([]);
        self.VerificationStates = ko.observableArray([]);
        self.ValuationTypes = ko.observableArray([]);
        self.GuarantorCifList = ko.observableArray([]);
        self.CIFList = ko.observableArray([]);
        self.GetVerificationStates = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/GetVerificationStates',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    ko.toJSON(data);
                    self.VerificationStates(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.LoadCountryIDList = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Address/GetCountries',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CountryIdList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetValuationType = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/GetValuationType',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ValuationTypes(data); //Put the response in ObservableArray

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        self.LoadVehiclePrimarySecurity = function () {
            if (self.VehiclePrimarySecurityId() > 0) {
                $.getJSON("/IPDC/Verification/LoadVehiclePrimarySecurity/?id=" + self.VehiclePrimarySecurityId(),
                    null,
                    function (data) {
                        //console.log(data.VerificationDateText);
                        if (data.Id > 0)
                            self.Id(data.Id);
                        if (data.VehiclePrimarySecurityId > 0)
                            self.VehiclePrimarySecurityId(data.VehiclePrimarySecurityId);
                        self.VerificationDate(data.VerificationDate);
                        self.VerifiedByUserId(data.VerifiedByUserId);
                        self.VerifiedByEmpDegMapId(data.VerifiedByEmpDegMapId);
                        if (data.VerificationDateText != null) {
                            self.VerificationDateText(data.VerificationDateText);
                        }
                        self.VerifiedPrice(data.VerifiedPrice);
                        self.VerificationMethod(data.VerificationMethod);
                        self.Remarks(data.Remarks);
                        $.when(self.GetVerificationStates())
                            .done(function() {
                                self.VerificationState(data.VerificationState);
                            });
                    });
            }
        }

        self.LoadApplicationData = function () {
            if (self.AppId() > 0) {
                $.getJSON("/IPDC/Application/LoadLoanApplicationByAppId/?AppId=" + self.AppId(),
                    null,
                    function (data) {
                        
                        //console.log(ko.toJSON(data));
                        self.ApplicationNo(data.ApplicationNo);
                        $.each(data.CIFList, function (index, value) {
                            var aDetail = new applicationCIFs();
                            if (typeof (value) != 'undefined') {
                                aDetail.LoadData(value);
                                self.CIFList.push(aDetail);
                            }
                        });
                        //
                        if (typeof (data.VehiclePrimarySecurity) !== 'undefined') {
                            //console.log('vehsec2:' + ko.toJSON(data.VehiclePrimarySecurity.Id));
                            self.VehiclePrimarySecurityId(data.VehiclePrimarySecurity.Id);
                            self.VehicleStatusName(data.VehiclePrimarySecurity.VehicleStatusName);
                            self.VendorTypeName(data.VehiclePrimarySecurity.VendorTypeName);
                            self.VendorName(data.VehiclePrimarySecurity.VendorName);
                            self.VendorType(data.VehiclePrimarySecurity.VehicleType);
                            self.VehicleTypeName(data.VehiclePrimarySecurity.VehicleTypeName);
                            self.SellersName(data.VehiclePrimarySecurity.SellersName);
                            self.SellersAddressId(data.VehiclePrimarySecurity.SellersAddressId);
                            self.SellersPhone(data.VehiclePrimarySecurity.SellersPhone);
                            self.Manufacturer(data.VehiclePrimarySecurity.Manufacturer);
                            self.MnufacturingYear(data.VehiclePrimarySecurity.MnufacturingYear);
                            self.YearModel(data.VehiclePrimarySecurity.YearModel);
                            self.RegistrationYear(data.VehiclePrimarySecurity.RegistrationYear);
                            self.RegistrationNo(data.VehiclePrimarySecurity.RegistrationNo);
                            self.CC(data.VehiclePrimarySecurity.CC);
                            self.Colour(data.VehiclePrimarySecurity.Colour);
                            self.ChassisNo(data.VehiclePrimarySecurity.ChassisNo);
                            self.EngineNo(data.VehiclePrimarySecurity.EngineNo);
                            self.Price(data.VehiclePrimarySecurity.Price);
                            //self.VehicleId(data.VehiclePrimarySecurity.Id);
                            //self.VerificationDateText(data.VehiclePrimarySecurity.VerificationDateText);
                            if (data.VehiclePrimarySecurity.SellersAddress != null || data.VehiclePrimarySecurity.SellersAddress != undefined) {
                                self.SellersAddress.LoadAddress(data.VehiclePrimarySecurity.SellersAddress);
                            }
                        }
                        //if (self.VehiclePrimarySecurityId() > 0) {
                            self.LoadVehiclePrimarySecurity();
                        //}

                    });
            }
        }

        self.Submit = function () {
            self.VerificationDateText($("#VarificationDateId").val());
            var submitData = {
                Id: self.Id(),
                VehiclePrimarySecurityId: self.VehiclePrimarySecurityId(),
                VerificationDateText: self.VerificationDateText(), 
                VerifiedPrice: self.VerifiedPrice(),
                VerificationMethod: self.VerificationMethod(),
                Remarks: self.Remarks(),
                VerificationState: self.VerificationState()
            }
            $.ajax({
                type: "POST",
                url: '/IPDC/Verification/SaveVehiclePrimarySecurityValuation',
                data: ko.toJSON(submitData),
                contentType: "application/json",
                success: function (data) {
                    $('#lonSuccessModal').modal('show');
                    $('#lonSuccessModalText').text(data.Message);
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.SubmitNew = function() {
            self.VerificationDateText($("#VarificationDateId").val());
            var submitData = {
                VehiclePrimarySecurityId: self.VehiclePrimarySecurityId(),
                VerificationDateText: self.VerificationDateText(),
                VerifiedPrice: self.VerifiedPrice(),
                VerificationMethod: self.VerificationMethod(),
                Remarks: self.Remarks(),
                VerificationState: self.VerificationState()
            }
            $.ajax({
                type: "POST",
                url: '/IPDC/Verification/SaveVehiclePrimarySecurityValuation',
                data: ko.toJSON(submitData),
                contentType: "application/json",
                success: function (data) {
                    $('#lonSuccessModal').modal('show');
                    $('#lonSuccessModalText').text(data.Message);
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.Initialize = function () {
            self.LoadCountryIDList();
            self.GetVerificationStates();
            self.GetValuationType();
            
            if (self.AppId() > 0) {
                self.LoadApplicationData();
            }
        }

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }
    appvm = new VehiclePrimarySecurityVm();
    var qValue = appvm.queryString('applicationId');
    appvm.AppId(qValue);
    appvm.Initialize();
    ko.applyBindings(appvm, $('#VehiclePrimarySecurityVw')[0]);
});




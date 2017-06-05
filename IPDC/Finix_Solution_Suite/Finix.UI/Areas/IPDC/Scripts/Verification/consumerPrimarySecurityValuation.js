
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
                        //console.log(data);
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
                        //console.log(data);
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
                        //console.log(data);
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
            console.log(data);
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

        self.AppId = ko.observable();
        self.Id = ko.observable();
        self.ApplicationNo = ko.observable();
        self.ConsumerGoodsPrimarySecurityId = ko.observable('');
        self.ConsumerGoodsPrimarySecurity = ko.observable('');
        self.VerificationDate = ko.observable('');
        self.VerificationDateText = ko.observable('');
        self.VerifiedByUserId = ko.observable('');
        self.VerifiedByEmpDegMapId = ko.observable('');

        self.VerifiedPrice = ko.observable('');
        self.VerificationMethod = ko.observable('');
        self.Remarks = ko.observable('');
        self.VerificationState = ko.observable('');
        self.IsVendor = ko.observable(false);
        self.IsSeller = ko.observable(false);
    
        self.LoanApplicationId = ko.observable('');
        self.Item = ko.observable('');
        self.Brand = ko.observable('');
        self.Dealer = ko.observable('');
        self.DealerAddressId = ko.observable('');
        self.ShowRoomName = ko.observable('');
        self.VendorName = ko.observable('');
        self.DealerAddress = new address();
        self.ShowRoomId = ko.observable('');
        self.ShowRoomId.subscribe(function () {
            if (self.ShowRoomId() >= 1) {
                self.IsVendor(true);
                self.IsSeller(false);
            } else {
                self.IsVendor(false);
                self.IsSeller(true);
            }
        });


        self.Price = ko.observable('');
        self.IsDealer = ko.observable(false);
        self.ConsumerGoodsId = ko.observable('');

        self.CountryIdList = ko.observableArray([]);
        self.VerificationStates = ko.observableArray([]);
        self.ValuationTypes = ko.observableArray([]);
        self.CIFList = ko.observableArray([]);
        self.GetVerificationStates = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/GetVerificationStates',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {

                    console.log(data);
                    ko.toJSON(data);
                    console.log(ko.toJSON(data));
                    self.VerificationStates(data); //Put the response in ObservableArray
                    console.log(self.VerificationStates());
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
        

        ////////////////////////////////////////////////Latest///////////////////////////////////////////////////////////


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
        self.LoadConsumerPrimarySecurity = function () {
            
            //console.log(self.AppId());
            if (self.ConsumerGoodsPrimarySecurityId() > 0) {
                $.getJSON("/IPDC/Verification/LoadConsumerPrimarySecurity/?id=" + self.ConsumerGoodsPrimarySecurityId(),
                    null,
                    function (data) {
                        
                        self.Id(data.Id);
                        self.ConsumerGoodsPrimarySecurityId(self.ConsumerGoodsPrimarySecurityId());
                        self.VerificationDate(data.VerificationDate);
                        self.VerifiedByUserId(data.VerifiedByUserId);
                        self.VerifiedByEmpDegMapId(data.VerifiedByEmpDegMapId);
                        self.VerificationDateText(data.VerificationDateText);
                        self.VerifiedPrice(data.VerifiedPrice);
                        self.VerificationMethod(data.VerificationMethod);
                        self.Remarks(data.Remarks);
                        // self.GetVerificationStates()
                        $.when(self.GetVerificationStates())
                            .done(function () {
                                self.VerificationState(data.VerificationState);
                            });

                    });
            }
        }
        self.LoadApplicationData = function () {
            //console.log(self.AppId());
            if (self.AppId() > 0) {
                $.getJSON("/IPDC/Application/LoadLoanApplicationByAppId/?AppId=" + self.AppId(),
                    null,
                    function (data) {

                        //console.log(data);
                        self.ApplicationNo(data.ApplicationNo);
                        $.each(data.CIFList, function (index, value) {
                            var aDetail = new applicationCIFs();
                            if (typeof (value) != 'undefined') {
                                aDetail.LoadData(value);
                                self.CIFList.push(aDetail);
                            }
                        });
                        if (data.ConsumerGoodsPrimarySecurity != null) {
                            if (data.ConsumerGoodsPrimarySecurity.DealerAddress != null || data.ConsumerGoodsPrimarySecurity.DealerAddress != undefined) {
                                self.IsDealer(true);
                                self.DealerAddress.LoadAddress(data.ConsumerGoodsPrimarySecurity.DealerAddress);
                            }
                            //self.ConsumerGoodsId = ko.observable('');
                            self.ConsumerGoodsPrimarySecurityId(data.ConsumerGoodsPrimarySecurity.Id);
                            self.Item(data.ConsumerGoodsPrimarySecurity.Item);
                            self.Brand(data.ConsumerGoodsPrimarySecurity.Brand);
                            self.Dealer(data.ConsumerGoodsPrimarySecurity.Dealer);
                            self.DealerAddressId(data.ConsumerGoodsPrimarySecurity.DealerAddressId);
                            self.ShowRoomName(data.ConsumerGoodsPrimarySecurity.ShowRoomId);
                            self.Price(data.ConsumerGoodsPrimarySecurity.Price);
                            if (data.ConsumerGoodsPrimarySecurity.ShowRoomId == null) {
                                self.IsSeller(true);
                                self.IsVendor(false);
                            } else {
                                self.IsSeller(false);
                                self.IsVendor(true);
                            }
                        }
                        
                        if (self.ConsumerGoodsPrimarySecurityId() > 0) {
                            //console.log(self.ConsumerGoodsId());
                            self.LoadConsumerPrimarySecurity();
                        }
                        //console.log(data);

                    });
            }
        }

        self.Submit = function () {

            self.VerificationDateText($("#VarificationDateId").val());
            var submitData = {
                Id: self.Id(),
                ConsumerGoodsPrimarySecurityId: self.ConsumerGoodsPrimarySecurityId(),
                //VerificationDate :
                //VerifiedByUserId :
                //VerifiedByEmpDegMapId  :
                VerificationDateText: self.VerificationDateText(),
                VerifiedPrice: self.VerifiedPrice(),
                VerificationMethod: self.VerificationMethod(),
                Remarks: self.Remarks(),
                VerificationState: self.VerificationState()

            }
            console.log(ko.toJSON(submitData));
            $.ajax({
                type: "POST",
                url: '/IPDC/Verification/SaveConsumerPrimarySecurityValuation',
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
        self.SubmitNew = function () {

            self.VerificationDateText($("#VarificationDateId").val());
            var submitData = {

                ConsumerGoodsPrimarySecurityId: self.ConsumerGoodsPrimarySecurityId(),
                VerificationDateText: self.VerificationDateText(),
                VerifiedPrice: self.VerifiedPrice(),
                VerificationMethod: self.VerificationMethod(),
                Remarks: self.Remarks(),
                VerificationState: self.VerificationState()
            }
            console.log(ko.toJSON(submitData));
            $.ajax({
                type: "POST",
                url: '/IPDC/Verification/SaveConsumerPrimarySecurityValuation',
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
            //self.GetValuationType();

            console.log(self.AppId());
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
        //var appvm = new ApplicationVm();
    }
    appvm = new VehiclePrimarySecurityVm();
    var qValue = appvm.queryString('applicationId');
    appvm.AppId(qValue);
    //var leadId = appvm.queryString('leadId');
    //appvm.SalesLeadId(leadId);
    appvm.Initialize();
    ko.applyBindings(appvm, $('#VehiclePrimarySecurityVw')[0]);
});




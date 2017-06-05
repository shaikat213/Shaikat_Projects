
$(document).ready(function () {
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
            //debugger;
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
                    })
                })
            });
            //debugger;
            self.IsChanged(false);
        }
    }


    var CallEntryGeneralEmpViewModel = function () {
        var self = this;
       
        //Call Entry Variables
        self.Id = ko.observable();
        self.Call_EntyId = ko.observable('');
        self.CallSourceList = ko.observableArray([]);
        self.CallSource = ko.observable();
        self.CallSourceName = ko.observable();
        self.CustomerName = ko.observable().extend({ required: true });
        self.CustomerPhone = ko.observable().extend({ digit: true, minLength: 11 });
        self.CustomerAddressId = ko.observable();
        self.ProductList = ko.observableArray([]);
        self.ProductId = ko.observable();

        self.TicketSize = ko.observable();
        self.CallTypeList = ko.observableArray([]);
        self.CallType = ko.observable();
        self.CallTypeName = ko.observable();
        //self.CallStatusList = ko.observableArray([]);
        self.CallStatus = ko.observable();
        self.ReferredToDegList = ko.observableArray([]);
        self.ReferredTo = ko.observable();
        //self.Remarks = ko.observable();
        self.CustomerCountryList = ko.observableArray([]);
        self.CustomerCountryId = ko.observable();
        self.CustomerCountryName = ko.observable();

        self.CustomerAddress = new address();

        //chosenItems: ko.observableArray()

        //Add New Data
        self.SubmitUnfinished = function () {
            self.CallStatus(0);
            self.Submit();
        };
        //self.SubmitUnSuccessful = function () {
        //    self.CallStatus(1);
        //    self.Submit();
        //}
        //self.SubmitSuccessful = function () {
        //    self.CallStatus(2);
        //    self.Submit();
        //}

        self.Submit = function () {

            //debugger
            var submitCallData = {
                Id: self.Id(),
                CustomerName: self.CustomerName(),
                CustomerPhone: self.CustomerPhone(),
                CallSource:self.CallSource(),
                CallSourceName:self.CallSourceName(),
                CallType: self.CallType(),
                CallTypeName: self.CallTypeName(),
                ProductId: self.ProductId(),
                TicketSize: self.TicketSize(),
                CallStatus: self.CallStatus(),
                ReferredTo: self.ReferredTo(),
                //Remarks: self.Remarks(),
                CustomerAddressId: self.CustomerAddressId(),
                CustomerAddress: self.CustomerAddress
                
            };
            $.ajax({
                url: '/IPDC/Call/SaveCallEntry',
                //cache: false,
                type: 'POST',
                contentType: 'application/json',
                data: ko.toJSON(submitCallData),
                success: function (data) {
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);
                    //self.Reset();
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }        

        //self.LoadCallEntry
        self.LoadCallEntry = function () {
            //debugger;
            if (self.Call_EntyId() > 0) {
                $.getJSON("/IPDC/Call/GetCallEntry/?callEntryId=" + self.Call_EntyId(),
                    null,
                    function (data) {
                        //console.log(data);
                        //debugger;
                        self.Id(data.Id);
                        self.CallSource(data.CallSource),
                        self.CustomerName(data.CustomerName),
                        self.CustomerPhone(data.CustomerPhone),
                        self.ProductId(data.ProductId),
                        self.TicketSize(data.TicketSize),
                        self.CallType(data.CallType),
                        self.CallStatus(data.CallStatus),
                        self.ReferredTo(data.ReferredTo),
                        $.when(self.LoadCallSource())
                            .done(function () {                                
                                self.CallSource(data.CallSource);
                            });

                        $.when(self.LoadCallType())
                            .done(function () {
                                self.CallType(data.CallType);
                            });

                        $.when(self.LoadProduct())
                            .done(function () {
                                self.ProductId(data.ProductId)
                                //self.Pro(data.CallSource);
                            });

                        $.when(self.LoadReferredToDeg())
                            .done(function () {
                                self.ReferredTo(data.ReferredTo);
                            });

                        $.when(self.LoadCountry())
                                .done(function () {
                                    self.CustomerAddressId(data.CustomerAddressId);
                                    self.CustomerAddress.LoadAddress(data.CustomerAddress);
                                });
                        //self.CustomerAddressId(data.CustomerAddressId),
                        
                    });
            }
        }
        //debugger;
        self.LoadCountry = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Address/GetCountries',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CustomerCountryList(data); //Put the response in ObservableArray
                    //console.log(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.LoadCallSource = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Call/GetAllCallSource',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CallSourceList(data); //Put the response in ObservableArray
                    ////console.log(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.LoadCallType = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Call/GetAllCallTypeCC',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CallTypeList(data); //Put the response in ObservableArray
                    ////console.log(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        //self.LoadCallStatus = function () {
        //    return $.ajax({
        //        type: "GET",
        //        url: '/IPDC/Call/GetAllCallStatus',
        //        contentType: "application/json; charset=utf-8",
        //        dataType: "json",
        //        success: function (data) {
        //            self.CallStatusList(data);
        //        },
        //        error: function (error) {
        //            alert(error.status + "<--and--> " + error.statusText);
        //        }
        //    });
        //}

        self.LoadReferredToDeg = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/OfficeDesignationSetting/GetOffDegSettingsForAssignment',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ReferredToDegList(data); //Put the response in ObservableArray
                    ////console.log(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.LoadProduct = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Call/GetAllProduct',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ProductList(data); //Put the response in ObservableArray
                    //console.log("CIF_PersonalList" + ko.toJSON(self.CIF_PersonalList()));
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.Reset = function () {
            //self.Name("");
            //self.Designation("");
            //self.Department("");
            //self.EnlistedOrganization("");
            //self.OrganizationName("");
            //self.OrganizationId("");
            //self.OfficeAddressId("");
            //self.RelationshipWithApplicant("");
            //self.ResidenceAddressId("");
            //self.PermanentAddressId("");
        }
        self.Initialize = function () {
            self.LoadCountry();
            self.LoadCallType();
            self.LoadProduct();
            self.LoadReferredToDeg()
            self.LoadCallSource();
            //self.LoadCallStatus();
        }

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    };

    var alVm = new CallEntryGeneralEmpViewModel();

    var qValue = alVm.queryString("callEntryId");
    alVm.Call_EntyId(qValue);
    alVm.Initialize();
    alVm.LoadCallEntry();
    ko.applyBindings(alVm, document.getElementById("callEntryGeneralEmp")[0]);

})
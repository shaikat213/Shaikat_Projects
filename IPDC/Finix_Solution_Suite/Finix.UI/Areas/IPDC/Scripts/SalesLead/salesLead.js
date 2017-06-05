
$(document).ready(function () {
    ko.validation.init({
        errorElementClass: 'has-error',
        errorMessageClass: 'help-block',
        decorateInputElement: true,
        grouping: { deep: true, observable: true }
    });
    function address() {
        var self = this;

        self.IsChanged = ko.observable(false);

        self.Id = ko.observable();
        self.CountryId = ko.observable();//.extend({ required: true });
        self.CountryName = ko.observable('');
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
                $.when(self.LoadDistrictByDivision()).done(function() {
                    self.DistrictId(data.DistrictId);
                    self.IsChanged(false);
                    $.when(self.LoadThanaByDistrict()).done(function() {
                        self.ThanaId(data.ThanaId);
                        self.IsChanged(false);
                    });
                });
            });
            self.IsChanged(false);
        }
    }


    var LeadEntryViewModel = function () {
        var self = this;
        //Call Entry Variables
        self.Id = ko.observable();
        self.CallOriginator = ko.observable();
        self.IsOrganization = ko.observable("false");
        self.ContactPerson = ko.observable();
        self.ContactPersonDesignation = ko.observable();
        
        self.CallSourceList = ko.observableArray([]);
        self.IncomeRanges = ko.observableArray([]);
        self.IncomeRange = ko.observable();
        self.CallSourceText = ko.observable();
        self.CustomerSensitivity = ko.observable().extend({ required: true });
        self.CustomerName = ko.observable().extend({ required: true });
        self.CustomerOrganization = ko.observable();
        self.CustomerDesignation = ko.observable();
        self.Gender = ko.observable(); //.extend({ required: { message: 'Please give gender information' } })
        self.GenderName = ko.observable('');
        self.CallCategory = ko.observable();
        self.DateOfBirth = ko.observable('');
        self.DateOfBirthText = ko.observable('');
        self.ProfessionId = ko.observable('');
        self.OtherProfession = ko.observable('');
        self.showErrors = ko.observable(false);
        self.NatureOfBusiness = ko.observable('');
        self.CustomerPhone = ko.observable().extend({ digit: true, minLength: 11 });
        self.CustomerAddressId = ko.observable();
        self.ProductList = ko.observableArray([]);
        self.CallStatus = ko.observable();
        self.ProductId = ko.observable().extend({
            required: {
                message: "Product is required.",
                onlyIf: function() {
                    return self.CallStatus() === 2;
                }
            }
        });
        self.LeadStatus = ko.observable();
        self.FollowUpCallTime = ko.observable();
        self.FollowUpCallTimeText = ko.observable();
        self.CallFailReasonList = ko.observableArray([]);
        self.CallFailReason = ko.observable().extend({
            required: {
                message: "Reason is required.",
                onlyIf: function () {
                    return self.LeadStatus() === 2;
                }
            }
        });
        self.CallFailReasonName = ko.observable();
        self.Amount = ko.observable().extend({
            required: {
                message: "Amount is required.",
                onlyIf: function () {
                    return self.CallStatus() === 2;
                }
            }
        });
        self.AmountFormatted = ko.pureComputed({
            read: function () {
                if(self.Amount() > 0)
                    return self.Amount().toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.Amount(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.CallTypeList = ko.observableArray([]);
        self.CallType = ko.observable();
        self.CallType.subscribe(function () {
            if (self.CallType() == 1) {
                self.CallCategory(1);
            }
        });
        self.CallTypeName = ko.observable();
        self.GenderList = ko.observableArray([]);
        self.ProfessionList = ko.observableArray([]);
        self.AgeRange = ko.observable();
        self.AgeRanges = ko.observableArray([]);
        self.CustomerPriorities = ko.observableArray([]);
        self.CustomerPriority = ko.observable();
        self.CallModes = ko.observableArray([]);
        self.CallMode = ko.observable();
        self.MaritalStatuses = ko.observableArray([]);
        self.MaritalStatus = ko.observable();
        //self.IsAutoAssign = ko.observable(false);
        
        //self.CallStatusList = ko.observableArray([]);
        
        self.ReferredToDegList = ko.observableArray([]);
        self.ReferredTo = ko.observable();
        self.Remarks = ko.observable();
        self.FailedRemarks = ko.observable().extend({
            required: {
                message: "Remarks is required.",
                onlyIf: function () {
                    return self.LeadStatus() === 2;
                }
            }
        });
        self.CustomerCountryList = ko.observableArray([]);
        self.CustomerCountryId = ko.observable();
        self.CustomerCountryName = ko.observable();
        self.CustomerAddress = new address();

        
        self.LeadPriority = ko.observable().extend({ required: true });
        self.LeadPriorities = ko.observableArray([]);

        //Add New Data
        self.errors = ko.validation.group(self);
        self.IsValid = ko.computed(function () {
            if (self.errors().length === 0)// && self.addressErrors().length === 0)
                return true;
            else {
                return false;
            }
        });
        self.SubmitUnfinished = function () {
            
            self.LeadStatus(0);
            self.Submit();
        };
        self.SubmitUnSuccessful = function () {
            self.LeadStatus(2);
            self.Submit();
        }
        self.SubmitProspective = function () {
            self.LeadStatus(4);
            if (self.ProductId() > 0) {
                self.Submit();
            }
        }
        self.FollowupTime = ko.observable();
        self.GetGender = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Employee/GetGender',
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.GenderList(data);
                },
                error: function (error) {
                    alert(error.status + "<-----and----->" + error.statusText);
                }
            });
        }

        self.GetMaritalStatuses = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Call/GetMaritalStatuses',
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.MaritalStatuses(data);
                },
                error: function (error) {
                    alert(error.status + "<-----and----->" + error.statusText);
                }
            });
        }
        self.GetAgeRange = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Call/GetCustomerAgeRange',
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.AgeRanges(data);
                },
                error: function (error) {
                    alert(error.status + "<-----and----->" + error.statusText);
                }
            });
        }
        self.GetIncomeRange = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Call/GetIncomeRange',
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.IncomeRanges(data);
                },
                error: function (error) {
                    alert(error.status + "<-----and----->" + error.statusText);
                }
            });
        }
        self.GetProfessionList = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Profession/GetAllProfession',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ProfessionList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetCustomerPriorities = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Call/GetCustomerPriorities',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CustomerPriorities(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetCallModes = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Call/GetCallModes',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CallModes(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.Submit = function () {
            var submitCallData = {
                Id: self.Id(),
                IsOrganization: self.IsOrganization(),
                CustomerName: self.CustomerName(),
                CustomerPhone: self.CustomerPhone(),
                Gender: self.Gender(),
                AgeRange: self.AgeRange(),
                MaritalStatus:self.MaritalStatus(),
                ContactPerson: self.ContactPerson(),
                ContactPersonDesignation: self.ContactPersonDesignation(),
                ProfessionId: self.ProfessionId(),
                OtherProfession: self.OtherProfession(),
                IncomeRange: self.IncomeRange(),
                CustomerPriority: self.CustomerPriority(),
                CallCategory: self.CallCategory(),
                CallMode: self.CallMode(),
                CallSourceText: self.CallSourceText(),
                CallFailReason:self.CallFailReason(),
                CallType: self.CallType(),
                ProductId: self.ProductId(),
                Amount: self.Amount(),
                CallStatus: self.CallStatus(),
                ReferredTo: self.ReferredTo(),
                Remarks: self.Remarks(),
                FailedRemarks: self.FailedRemarks(),
                CustomerAddressId: self.CustomerAddressId(),
                CustomerAddress: self.CustomerAddress,
                NatureOfBusiness: self.NatureOfBusiness(),
                CustomerOrganization: self.CustomerOrganization(),
                CustomerDesignation: self.CustomerDesignation(),
                LeadStatus: self.LeadStatus(),
                FollowupTime: self.FollowupTime(),
                FollowupTimeText: moment(self.FollowupTime()).format('DD/MM/YYYY HH:mm'),
                CustomerSensitivity: self.CustomerSensitivity(),
                LeadPriority: self.LeadPriority()
            };
            if (self.IsValid()) {
            $.ajax({
                url: '/IPDC/SalesLead/SaveSalesLead',
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
            } else {
                self.errors.showAllMessages();
            }
        }           
        //self.LoadCallEntry
        self.LoadCallEntry = function () {
            
            if (self.Id() > 0) {
                $.getJSON("/IPDC/SalesLead/GetSalesLeadForEdit/?leadId=" + self.Id(),
                    null,
                    function (data) {
                        self.Id(data.Id);
                        self.IsOrganization(data.IsOrganization + '');
                        self.CallSourceText(data.CallSourceText);
                        self.CustomerName(data.CustomerName);
                        self.ContactPerson(data.ContactPerson),
                        self.ContactPersonDesignation(data.ContactPersonDesignation),
                        self.CustomerPhone(data.CustomerPhone);
                        self.Amount(data.Amount);
                        self.CallStatus(data.CallStatus);
                        self.Remarks(data.Remarks);
                        self.FailedRemarks(data.FailedRemarks);
                        self.NatureOfBusiness(data.NatureOfBusiness);
                        self.CallOriginator(data.CallCreatorName);
                        self.CustomerOrganization(data.CustomerOrganization);
                        self.CustomerDesignation(data.CustomerDesignation);
                        self.FollowupTime(moment(data.FollowupTime));
                        $.when(self.GetAgeRange())
                            .done(function () {
                                self.AgeRange(data.AgeRange);
                            });
                        $.when(self.GetGender())
                            .done(function () {
                                self.Gender(data.Gender);
                            });
                        $.when(self.GetMaritalStatuses())
                            .done(function () {
                                self.MaritalStatus(data.MaritalStatus);
                            });
                        $.when(self.GetProfessionList())
                            .done(function () {
                                self.ProfessionId(data.ProfessionId);
                            });
                        self.OtherProfession(data.OtherProfession);
                        $.when(self.LoadCallSource())
                            .done(function () {                                
                                self.CallCategory(data.CallCategory);
                            });
                        $.when(self.LoadCallType())
                            .done(function () {
                                self.CallType(data.CallType);
                            });
                        $.when(self.LoadProduct())
                            .done(function () {
                                self.ProductId(data.ProductId)
                                //self.Pro(data.CallCategory);
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
                        $.when(self.GetIncomeRange())
                            .done(function () {
                                self.IncomeRange(data.IncomeRange);
                            });
                        $.when(self.GetCustomerPriorities())
                            .done(function () {
                                self.CustomerPriority(data.CustomerPriority);
                            });
                        $.when(self.GetCallModes())
                            .done(function () {
                                self.CallMode(data.CallMode);
                            });
                        $.when(self.LoadCallFailReason())
                            .done(function () {
                                self.CallFailReason(data.CallFailReason);
                            });
                        $.when(self.LoadCustomerSensitivities())
                            .done(function () {
                                self.CustomerSensitivity(data.CustomerSensitivity);
                            });
                        $.when(self.LoadLeadPriorities())
                            .done(function () {
                                self.LeadPriority(data.LeadPriority);
                            });
                        //
                    });
            }
        }
        self.LoadCountry = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Address/GetCountries',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CustomerCountryList(data); //Put the response in ObservableArray
                    self.CustomerAddress.CountryId(1);
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
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.LoadCallType = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Call/GetAllCallType',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CallTypeList(data); //Put the response in ObservableArray
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
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.LoadCallFailReason = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Call/GetAllCallFailReason',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CallFailReasonList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.CustomerSensitivities = ko.observableArray([]);
        self.LoadCustomerSensitivities = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/SalesLead/GetCustomerSensitivities',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CustomerSensitivities(data); //Put the response in ObservableArray
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
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.LoadLeadPriorities = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Call/GetLeadPriorities',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.LeadPriorities(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        
        self.Initialize = function () {
            self.GetGender();
            self.GetAgeRange();
            self.GetProfessionList();
            self.LoadCountry();
            self.LoadCallType();
            self.LoadProduct();
            self.LoadReferredToDeg();
            self.LoadCallSource();
            self.LoadCallFailReason();
            self.GetIncomeRange();
            self.GetCustomerPriorities();
            self.GetCallModes();
            self.GetMaritalStatuses();
            self.LoadCustomerSensitivities();
            self.LoadLeadPriorities();
        }

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    };

    var alVm = new LeadEntryViewModel();

    var qValue = alVm.queryString("leadEntryId");
    alVm.Id(qValue);
    if (qValue.length < 1) {
        alVm.Initialize();
    }
    alVm.LoadCallEntry();
    ko.applyBindings(alVm, document.getElementById("leadEntry")[0]);

})
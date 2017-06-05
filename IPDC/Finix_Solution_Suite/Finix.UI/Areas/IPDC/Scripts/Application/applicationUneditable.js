/// <reference path="../knockout-3.4.0.debug.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../finix.util.js" />
/// <reference path="~/Scripts/knockout.validation.min.js" />
/// <reference path="~/Scripts/bootstrap-datetimepicker.js" />
/// <reference path="~/Scripts/knockout-3.4.0.debug.js" />
var addressTypes = [{ 'Id': 1, 'Name': 'Present Address' },
                    { 'Id': 2, 'Name': 'Permanent Address' },
                    { 'Id': 3, 'Name': 'Work Address' },
                    { 'Id': 4, 'Name': 'Other Address' }];
var depositAppVM;
var loanAppVM;
var appvm;
$(document).ready(function () {

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
        self.CIF_Personal = ko.observable();
        self.CIFName = ko.observable();
        self.CIF_Personal.subscribe(function () {
            self.CIF_PersonalId(self.CIF_Personal().key);
            self.CIFName(self.CIF_Personal().value);
        });
        //self.NBFI_1_SectorCodeId = ko.observable();
        //self.NBFI1SectorCode = ko.observable();
        //self.NBFI_1_SectorCodeDescription = ko.observable();
        //self.NBFI1SectorCode.subscribe(function () {
        //    self.NBFI_1_SectorCodeId(self.NBFI1SectorCode().Id);
        //    self.NBFI_1_SectorCodeDescription(self.NBFI1SectorCode().Description);
        //});
        self.CIF_PersonalId = ko.observable(data ? data.CIF_PersonalId : '');
        self.CIFName = ko.observable(data ? data.CIFName : "");
        self.ApplicantName = ko.observable('');

        self.ApplicantRole = ko.observable(data ? data.ApplicantRole : 0);
        self.CIF_OrganizationalId = ko.observable(data ? data.CIF_OrganizationalId : '');

        self.IsAddOption = ko.observable(data ? data.ApplicantRole === 1 || data.ApplicantRole === 2 ? true : false : false);
        //self.checkCoApp = function () {
        //if (self.ApplicantRole() === 1) {
        //}
        //if (self.ApplicantRole() === 2) {
        //    //self.IsAddOption(true);
        //}
        //}

        self.LoadData = function (data) {
            self.Id(data ? data.Id : 0);
            self.ApplicationId(data ? data.ApplicationId : 0);
            self.CIF_PersonalId(data ? data.CIF_PersonalId : '');
            self.CIFName(data ? data.CIFName : '');
            if (data.CIF_PersonalId && data.CIF_PersonalId > 0)
                self.CIF_Personal(data.CIF_Personal)
            self.ApplicantName(data ? data.ApplicantName ? data.ApplicantName : '' : '');
            self.ApplicantRole(data ? data.ApplicantRole : 0);
            self.CIF_OrganizationalId(data ? data.CIF_OrganizationalId : '');
            //self.checkCoApp();
        }

    }

    function applicationCheckList(data) {
        var self = this;
        //var currentDate = (new Date()).toISOString().split('T')[0];
        self.Id = ko.observable(data ? data.Id : 0);
        self.ApplicationId = ko.observable(data ? data.ApplicationId : 0);
        self.ProductDocId = ko.observable(data ? data.ProductDocId : 0);
        self.ProductDoc = ko.observable(data ? data.ProductDoc : "");
        self.DocumentStatus = ko.observable(data ? data.DocumentStatus : "");
        self.SubmissionDeadline = ko.observable();
        self.SubmissionDeadlineText = ko.observable();
        self.ApprovalRequired = ko.observable(data ? data.ApprovalRequired : "");
        self.ApprovedById = ko.observable(data ? data.ApprovedById : 0);
        self.ProductId = ko.observable(data ? data.ProductId : 0);
        self.DocName = ko.observable(data ? data.DocName : "");
        //self.SubmissionDeadlineText = ko.observable(data ? data.SubmissionDeadlineText : "");
        self.IsChecked = ko.observable(data ? data.IsChecked : false);
        self.DocumentStatusList = ko.observableArray([]);

        self.IfDererred = ko.observable(data ? data.DocumentStatus === 3 ? true : false : false);
        self.CheckIfDererred = function () {
            if (self.DocumentStatus() === 3) {
                self.IfDererred(true);
            } else {
                self.IfDererred(false);
            }
            if (self.DocumentStatus() > 0) {
                self.IsChecked(true);
            }
        }
        self.LoadData = function (data) {
            self.Id(data ? data.Id : 0);
            self.ApplicationId(data ? data.ApplicationId : 0);
            self.ProductDocId(data ? data.ProductDocId : 0);
            self.ProductDoc(data ? data.ProductDoc : "");
            self.DocumentStatus(data ? data.DocumentStatus : "");
            self.SubmissionDeadline(data.SubmissionDeadline ? moment(data.SubmissionDeadline) : "");
            self.ApprovalRequired(data ? data.ApprovalRequired : "");
            self.ApprovedById(data ? data.ApprovedById : 0);
            self.ProductId(data ? data.ProductId : 0);
            self.DocName(data ? data.DocName : "");
            self.SubmissionDeadlineText(moment(data.SubmissionDeadlineText).format('DD/MM/YYYY'));

            self.IsChecked(data ? data.IsChecked : false);
            self.CheckIfDererred();
        }

    }

    function address() {
        var self = this;
        self.Id = ko.observable('');
        self.ThanaId = ko.observable('');
        self.ThanaName = ko.observable('');
        self.DistrictId = ko.observable('');
        self.DistrictName = ko.observable('');
        self.DivisionId = ko.observable('');
        self.DivisionName = ko.observable('');
        self.CountryId = ko.observable('');
        self.CountryName = ko.observable('');
        self.AddressLine1 = ko.observable('');
        self.AddressLine2 = ko.observable('');
        self.AddressLine3 = ko.observable('');
        self.PostalCode = ko.observable('');
        self.PhoneNo = ko.observable('');
        self.CellPhoneNo = ko.observable('');
        self.Email = ko.observable('');
        self.IsChanged = ko.observable(false);
        self.Initialize = function (data) {
            self.ThanaId(data ? data.ThanaId : "");
            self.ThanaName(data ? data.ThanaName : "");
            self.DistrictId(data ? data.DistrictId : "");
            self.DistrictName(data ? data.DistrictName : "");
            self.DivisionId(data ? data.DivisionId : "");
            self.DivisionName(data ? data.DivisionName : "");
            self.CountryId(data ? data.CountryId : "");
            self.CountryName(data ? data.CountryName : "");
            self.AddressLine1(data ? data.AddressLine1 : "");
            self.AddressLine2(data ? data.AddressLine2 : "");
            self.AddressLine3(data ? data.AddressLine3 : "");
            self.PostalCode(data ? data.PostalCode : "");
            self.PhoneNo(data ? data.PhoneNo : "");
            self.CellPhoneNo(data ? data.CellPhoneNo : "");
            self.Email(data ? data.Email : "");
        }
    }

    function ApplicationVm() {

        var self = this;

        self.CifIdTest = ko.observable();
        self.TLComment = ko.observable();
        self.BMComment = ko.observable();
        self.RejectionReason = ko.observable();
        self.CIFListTest = ko.observableArray([]);
        self.GetCIFListTest = function (searchTerm, callback) {
            var submitData = {
                prefix: searchTerm,
                exclusionList: self.ExistingCifIds()
            };
            $.ajax({
                type: "POST",
                url: '/IPDC/CIF/GetCIFPListForAutoFill',
                data: ko.toJSON(submitData),
                contentType: "application/json",
                success: function () {
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            }).done(callback);
        };

        self.SalesLeadId = ko.observable();
        self.CIF_PersonalList = ko.observableArray([]);
        self.ApplicantRoles = ko.observableArray([]);
        self.Organizations = ko.observableArray([]);
        self.AddressTypes = ko.observableArray(addressTypes);
        self.ContactPersonAddressType = ko.observable();
        self.ApplicationNo = ko.observable();
        self.IsAddPrimary = ko.observable(true);
        //GetOrganizations
        //self.checkCoApp = function (line) {
        //    
        //    var role = ko.toJSON(line.ApplicantRole);

        //    if (role === '1') {
        //        self.IsAddPrimary(false);
        //    }
        //}
        self.GetOrganizations = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/CIF/GetAllCifOrgList',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.Organizations(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        //self.GetCIFs = function () {
        //    return $.ajax({
        //        type: "GET",
        //        url: '/IPDC/CIF/GetAllCif',
        //        contentType: "application/json; charset=utf-8",
        //        dataType: "json",
        //        success: function (data) {
        //            self.CIF_PersonalList(data); //Put the response in ObservableArray
        //        },
        //        error: function (error) {
        //            alert(error.status + "<--and--> " + error.statusText);
        //        }
        //    });
        //}
        self.GetApplicantRoles = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetApplicantRoles',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ApplicantRoles(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.Id = ko.observable('');
        self.ApplicationDate = ko.observable('');
        self.CustomerType = ko.observable().extend({ required: true, message: 'Please Select Customer Type.' });// true });

        self.ApplicationType = ko.observable().extend({ required: true, message: 'Please Select Customer Type.' });
        self.ContactPersonId = ko.observable();
        self.UseConAddAsGrpAdd = ko.observable(false);
        self.IsJoint = ko.observable(false);
        self.jointCheck = function () {
            if (self.ApplicationType() === 2) {
                self.IsJoint(true);
            } else {
                self.IsJoint(false);
            }
        }
        self.UseConAddAsGrpAdd.subscribe(function () {
            //IsAddFieldsEnabled
            if (self.UseConAddAsGrpAdd())
                self.IsAddFieldsEnabled(false);
            else
                self.IsAddFieldsEnabled(true);
        });
        self.ApplicationDateText = ko.observable('');//.extend({required: "Application date is required."});
        self.ContactPersonList = ko.observableArray([]);
        self.GroupAddress = new address();
        self.DocChecklist = ko.observableArray([]);
        self.GroupCountryList = ko.observableArray([]);
        self.GroupDivisionList = ko.observableArray([]);
        self.GroupDistrictList = ko.observableArray([]);
        self.GroupThanaList = ko.observableArray([]);
        self.ProductTypes = ko.observableArray([]);
        self.DocumentStatusList = ko.observableArray([]);
        //self.Products = ko.observableArray([]);
        //self.ApplicationCIFsDto = ko.observableArray([]);
        self.IsGrpAddressChng = function () {
            self.GroupAddress.IsChanged(true);
        }

        self.AccountTitle = ko.observable().extend({ required: true, maxLength: { params: 70, message: "Please enter no more than 70 characters. " } });
        self.AccGroupId = ko.observable();//.extend({ required: true });
        self.ProductId = ko.observable().extend({ required: true });
        self.ProductType = ko.observable().extend({ required: true, message: 'Please Select Product Type.' });
        self.ProductType.subscribe(function () {
            //document.getElementById('depAppTab')
            if (self.ProductType() === 1) {
                $("#depAppTab").show();
                $("#lonAppTab").hide();
            }
            else if (self.ProductType() === 2) {
                $("#depAppTab").hide();
                $("#lonAppTab").show();
            }
        });
        self.LoanApplicationId = ko.observable('');
        self.DepositApplicationId = ko.observable('');
        self.Term = ko.observable('');
        self.IsIndividual = ko.observable(false);
        self.IsOrganizational = ko.observable(false);




        self.IsAddFieldsEnabled = ko.observable(true);

        self.CIFList = ko.observableArray([]);

        self.ApplicationCustomerTypes = ko.observableArray([]);
        self.ApplicationTypes = ko.observableArray([]);
        self.ProductTypes = ko.observableArray([]);

        self.ApplicantLoans = ko.observableArray([]);
        self.Products = ko.observableArray([]);
        //self.ProductId.subscribe(function () {
        //    self.addDocCheckListDetail();
        //});
        self.IsIndividual.subscribe(function () {
            self.addDocCheckListDetail();
        });

        self.CostCenterId = ko.observable();
        self.CostCenterList = ko.observableArray([]);

        self.addDocCheckListDetail = function () {
            self.DocChecklist([]);
            var url = '/IPDC/Application/GetAllDocCheckList?prodId=' + self.ProductId();
            if (self.ProductId() > 0) {
                if (typeof (self.IsIndividual()) != 'undefined') {
                    url += '&IsIndividual=' + self.IsIndividual();

                    if (typeof (self.CIFList()[0]) != 'undefined' && self.CIFList()[0].CIF_OrganizationalId() > 0 && self.IsIndividual() === false) {
                        url += '&CifOrgId=' + self.CIFList()[0].CIF_OrganizationalId();
                    }
                }
                return $.ajax({
                    type: "GET",
                    url: url,
                    contentType: "application/json",
                    success: function (data) {
                        $.each(data, function (index, value) {
                            self.DocChecklist.push(new applicationCheckList(value));
                        });
                    },
                    error: function () {
                        alert(error.status + "<--and-->" + error.statusText);
                    }
                });
            }
        };
        self.GetDocumentStatusList = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetDocumentStatusList',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.DocumentStatusList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.errors = ko.validation.group(self);
        self.ExistingCifIds = ko.observableArray([]);
        self.CifListValidity = ko.computed(function () {
            //
            self.ExistingCifIds([]);
            if (typeof (depositAppVM) != 'undefined') {
                if (depositAppVM.GuiardianCifId() > 0)
                    self.ExistingCifIds.push(depositAppVM.GuiardianCifId());
                if (depositAppVM.Nominees().length > 0) {
                    $.each(depositAppVM.Nominees(), function (index, value) {
                        if (value.NomineeCifId() > 0)
                            self.ExistingCifIds.push(value.NomineeCifId());
                        if (value.GuiardianCifId() > 0)
                            self.ExistingCifIds.push(value.GuiardianCifId());
                    })
                }
            }
            if (typeof (loanAppVM) != 'undefined') {
                if (loanAppVM.Guarantors().length > 0) {
                    $.each(loanAppVM.Guarantors(), function (index, value) {
                        if (value.GuarantorCifId() > 0)
                            self.ExistingCifIds.push(value.GuarantorCifId());
                    })
                }
            }
            var primaryApplicants = 0;
            if (self.CustomerType() === 1 && self.CIFList().length > 0) {
                $.each(self.CIFList(), function (index, value) {
                    if (value.CIF_PersonalId() > 0)
                        self.ExistingCifIds.push(value.CIF_PersonalId());
                    if (value.ApplicantRole() === 1)
                        primaryApplicants++;
                })
                //console.log("self.ExistingCifIds " + self.ExistingCifIds());
                if (primaryApplicants < 1)
                    return { validator: false, message: "At least one primary applicant is mandatory." }
                else if (primaryApplicants > 1)
                    return { validator: false, message: "Multiple primary applicant cannot be accepted." }
                else
                    return { validator: true }
            } else
                return { validator: true }
        });
        self.IsValid = ko.computed(function () {
            var err = self.errors().length;
            //
            if (self.CifListValidity().validator === false)
                err++;
            //console.log("err - " + err);
            if (err == 0)
                return true;
            return false;
        });
        self.addDetailOfClient = function () {
            self.CIFList([]);

            var aDetail = new applicationCIFs();
            if (self.CustomerType() === 1) {
                self.IsIndividual(true);
                self.IsOrganizational(false);
                self.ApplicationType('');
                //aDetail.InitializeClient();
                self.CIFList.push(aDetail);
            }
            if (self.CustomerType() === 2) {
                self.IsIndividual(false);
                self.IsOrganizational(true);
                self.ApplicationType(1);
                //aDetail.InitializeClient();
                self.CIFList.push(aDetail);
            }
        }
        self.RemovedCIFList = ko.observableArray([]);
        self.removeDetialOfClient = function (line) {
            if (line.Id() > 0)
                self.RemovedCIFList.push(line.Id())
            self.CIFList.remove(line);
        }

        self.addDetailOfClientRow = function () {
            var aDetail = new applicationCIFs();
            if (self.CustomerType() === 1) {
                self.IsIndividual(true);
                self.IsOrganizational(false);
                //aDetail.InitializeClient();
                self.CIFList.push(aDetail);
            }
            if (self.CustomerType() === 2) {
                self.IsIndividual(false);
                self.IsOrganizational(true);
                //aDetail.InitializeClient();
                self.CIFList.push(aDetail);
            }
        }

        self.GetApplicationCustomerTypes = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetApplicationCustomerTypes',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ApplicationCustomerTypes(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetApplicationTypes = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetApplicationTypes',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {

                    self.ApplicationTypes(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetProductTypes = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetProductTypes',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ProductTypes(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.GetProducts = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetAllProducts',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.Products(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.GetContactPerson = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/CIF/GetAllCif',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ContactPersonList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetGroupCountry = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Address/GetCountries',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.GroupCountryList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.LoadGroupDivisionByCountry = function () {
            var countryId = self.GroupAddress.CountryId() ? self.GroupAddress.CountryId() : 0;
            return $.ajax({
                type: "GET",
                url: '/IPDC/OfficeDesignationArea/GetDivisionByCountry?id=' + countryId,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.GroupDivisionList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });

        }
        self.getGroupDistrictByDivision = function () {
            var divisionId = self.GroupAddress.DivisionId() ? self.GroupAddress.DivisionId() : 0;

            return $.ajax({
                type: "GET",
                url: '/IPDC/OfficeDesignationArea/GetDistrictByDivision?id=' + divisionId,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.GroupDistrictList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.getGroupUpzilaByDistrict = function () {
            var districtId = self.GroupAddress.DistrictId() ? self.GroupAddress.DistrictId() : 0;

            return $.ajax({
                type: "GET",
                url: '/IPDC/SalesLead/GetThanasByDistrict?districtId=' + districtId,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.GroupThanaList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetProductTypes = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetProductTypes',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ProductTypes(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.GetProductByType = function () {
            var productType = self.ProductType() ? self.ProductType() : 0;

            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetProductByType?typeId=' + productType,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.Products(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.GetCostCenters = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetCostCenters',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CostCenterList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.LoadApplicationData = function () {
            if (self.Id() > 0) {
                $.getJSON("/IPDC/Application/LoadApplicationByAppId?AppId=" + self.Id(),
                    null,
                    function (data) {
                        self.CIFList([]);
                        self.DocChecklist([]);
                        self.Id(data.Id);
                        self.ApplicationNo(data.ApplicationNo);
                        self.TLComment(data.TLComment);
                        self.BMComment(data.BMComment);
                        self.RejectionReason(data.RejectionReason);
                        self.SalesLeadId(data.SalesLeadId);
                        $.when(self.GetApplicationTypes())
                            .done(function () {
                                self.ApplicationType(data.ApplicationType);
                            });
                        self.UseConAddAsGrpAdd(data.UseConAddAsGrpAdd);
                        //self.ApplicationDateText(data.ApplicationDateText);
                        //$('#ApplicationDateId').val(data.ApplicationDateText);
                        self.ApplicationDate(moment(data.ApplicationDate));
                        if (data.GroupAddress != null || data.GroupAddress != undefined) {
                            self.GroupAddress.Id(data.GroupAddress.Id);
                            self.GroupAddress.AddressLine1(data.GroupAddress.AddressLine1);
                            self.GroupAddress.AddressLine2(data.GroupAddress.AddressLine2);
                            self.GroupAddress.AddressLine3(data.GroupAddress.AddressLine3);
                            self.GroupAddress.PostalCode(data.GroupAddress.PostalCode);
                            self.GroupAddress.Email(data.GroupAddress.Email);
                            self.GroupAddress.PhoneNo(data.GroupAddress.PhoneNo);
                            self.GroupAddress.CellPhoneNo(data.GroupAddress.CellPhoneNo);
                            self.GroupAddress.PostalCode(data.GroupAddress.PostalCode);
                            $.when(self.GetGroupCountry())
                                .done(function () {
                                    self.GroupAddress.CountryId(data.GroupAddress.CountryId);
                                    $.when(self.LoadGroupDivisionByCountry())
                                        .done(function () {
                                            self.GroupAddress.DivisionId(data.GroupAddress.DivisionId);
                                            $.when(self.getGroupDistrictByDivision())
                                                .done(function () {
                                                    self.GroupAddress.DistrictId(data.GroupAddress.DistrictId);
                                                    $.when(self.getGroupUpzilaByDistrict())
                                                        .done(function () {
                                                            self.GroupAddress
                                                                .ThanaId(data.GroupAddress.ThanaId);

                                                        });
                                                });
                                        });
                                });
                        }
                        $.when(self.GetApplicationCustomerTypes())
                           .done(function () {
                               self.CustomerType(data.CustomerType);
                               if (data.CustomerType === 1) {
                                   self.IsIndividual(true);
                                   self.IsOrganizational(false);
                               } else {
                                   self.IsIndividual(false);
                                   self.IsOrganizational(true);
                               }
                           });
                        //$.when(self.GetContactPerson())
                        //  .done(function () {
                        //      self.ContactPersonId(data.ContactPersonId);
                        //  });
                        //$.when(self.GetCIFs())
                        //    .done(function () {
                        $.when(self.GetApplicantRoles()).done(function () {
                            $.when(self.GetOrganizations()).done(function () {
                                $.each(data.CIFList, function (index, value) {
                                    var aDetail = new applicationCIFs();
                                    if (typeof (value) != 'undefined') {
                                        aDetail.LoadData(value);
                                        self.CIFList.push(aDetail);
                                    }
                                });
                                self.ContactPersonId(data.ContactPersonId);
                                if (data.ContactPersonId > 0) {
                                    self.IsJoint(true);
                                }
                                $.when(self.GetProductTypes())
                                 .done(function () {
                                     self.ProductType(data.ProductType);
                                     $.when(self.GetProducts())
                                       .done(function () {
                                           self.ProductId(data.ProductId);
                                           $.when(self.GetDocumentStatusList())
                                            .done(function () {
                                                $.each(data.DocChecklist,
                                                     function (index, value) {
                                                         var aDetail = new applicationCheckList();
                                                         if (typeof (value) != 'undefined') {
                                                             aDetail.LoadData(value);
                                                             self.DocChecklist.push(aDetail);
                                                         }

                                                     });
                                            });
                                       });
                                 });
                            });
                        });
                        //});
                        $.when(self.GetCostCenters()).done(function () {
                            self.CostCenterId(data.CostCenterId);
                        });
                        self.ContactPersonAddressType(data.ContactPersonAddressType);
                        self.AccountTitle(data.AccountTitle);
                        self.AccGroupId(data.AccGroupId);
                        self.LoanApplicationId(data.LoanApplicationId);
                        self.DepositApplicationId(data.DepositApplicationId);
                        self.Term(data.Term);
                    });

            }
        }
        self.CIFPDetails = function (data) {
            var parameters = [{
                Name: 'cifpid',
                Value: data.CIF_PersonalId()
            }];
            var menuInfo = {
                Id: 27,
                Menu: 'CIF Personal',
                Url: '/IPDC/CIF/CIF_Personal',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
        self.CIFODetails = function (data) {
            var parameters = [{
                Name: 'cifOrgId',
                Value: data.CIF_OrganizationalId()
            }];
            var menuInfo = {
                Id: 25,
                Menu: 'Organizational CIF',
                Url: '/IPDC/CIF/CIFOraganizational',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
        self.Initialize = function () {
            self.GetApplicationCustomerTypes();
            self.GetApplicationTypes();
            self.GetContactPerson();
            self.GetGroupCountry();
            self.GetProductTypes();
            self.GetDocumentStatusList();
            if (self.Id() > 0) {
                self.LoadApplicationData();
            }
            //self.GetCIFs();
            self.GetApplicantRoles();
            self.GetOrganizations();
            self.GetCostCenters();
        }
        
        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));

        }
        self.addCIF = function () {
            var menuInfo = {
                Id: 99,
                Menu: 'CIF Personal',
                Url: '/IPDC/CIF/CIF_Personal'
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
        self.addCIFOrg = function () {
            var menuInfo = {
                Id: 99,
                Menu: 'CIF Organizational',
                Url: '/IPDC/CIF/CIFOraganizational'
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
    }

    appvm = new ApplicationVm();


    var qValue = appvm.queryString('applicationId');
    appvm.Id(qValue);
    var leadId = appvm.queryString('leadId');
    appvm.SalesLeadId(leadId);
    appvm.Initialize();
    ko.applyBindings(appvm, $('#ApplicationVw')[0]);


    $.getScript("/Areas/IPDC/Scripts/Application/depositApplicationUneditable.js", function () {
        depositAppVM = new DepositApplicationViewModel();
        if (typeof (qValue) != 'undefined') {
            depositAppVM.Application_Id(qValue);
            depositAppVM.LoadDepositAppData();
            depositAppVM.GetApplicantAge();
        }
        depositAppVM.Initializer();
        ko.applyBindings(depositAppVM, document.getElementById('depositApplication'));
    });

    $.getScript("/Areas/IPDC/Scripts/Application/loanApplicationUneditable.js", function () {
        loanAppVM = new LoanApplicationVm();
        if (typeof (qValue) != 'undefined') {
            loanAppVM.Application_Id(qValue);
            loanAppVM.LoadLoanApplicationData();

        }
        loanAppVM.Initialize();
        ko.applyBindings(loanAppVM, document.getElementById('loanApplication'));
    });

});




var alVm;
$(document).ready(function () {
    $('.input-group-addon').click(function (event, object) {
        $(this).siblings('input').focus();
    });

    var FactoryAddressLine = function () {
        var self = this;
        self.Id = ko.observable();
        self.Address = new address();
        self.AddressId = ko.observable();

        self.UseRegisteredAddress = function () {
            if (typeof (alVm) != 'undefined') {
                var id;
                if (self.Address.Id() > 0)
                    id = self.Address.Id();
                //console.log(alVm.RegAddress);
                //console.log(ko.toJSON(alVm.RegAddress));
                var addressCopy = {
                    Id: alVm.RegAddress.Id(),
                    ThanaId: alVm.RegAddress.ThanaId(),
                    DistrictId: alVm.RegAddress.DistrictId(),
                    DivisionId: alVm.RegAddress.DivisionId(),
                    CountryId: alVm.RegAddress.CountryId(),
                    AddressLine1: alVm.RegAddress.AddressLine1(),
                    AddressLine2: alVm.RegAddress.AddressLine2(),
                    AddressLine3: alVm.RegAddress.AddressLine3(),
                    PostalCode: alVm.RegAddress.PostalCode(),
                    PhoneNo: alVm.RegAddress.PhoneNo(),
                    CellPhoneNo: alVm.RegAddress.CellPhoneNo(),
                    Email: alVm.RegAddress.Email(),
                    IsChanged: alVm.RegAddress.IsChanged()
                }
                //console.log(addressCopy);
                self.Address.LoadAddress(addressCopy);
                if (typeof (id) != 'undefined') {
                    self.Address.Id(id);
                    self.AddressId(id);
                }
            }
        }

        self.LoadFactoryAddress = function (data) {
            //var newAddress = new address();
            self.Id(data.Id);
            self.Address.LoadAddress(data.Address);
            self.AddressId(data.AddressId);
        }
    };

    var OwnersLine = function () {
        var self = this;
        self.Id = ko.observable();
        self.CIF_PersonalName = ko.observable();
        self.CIF_PersonalId = ko.observable();
        self.SelectedCIF_Personal = ko.observable();
        self.SelectedCIF_Personal.subscribe(function () {
            self.CIF_PersonalId(self.SelectedCIF_Personal().key);
            
        });
        self.CIF_Org_OwnersRole = ko.observable();

        self.Load = function (data) {
            
            self.Id(data.Id);
            self.CIF_Org_OwnersRole(data.CIF_Org_OwnersRole);
            self.SelectedCIF_Personal(data.CIF_Personal);
        }

    };

    var CIFOrganizationalViewModel = function () {
        var self = this;

        self.Id = ko.observable('');
        self.CIF_OrganizationalId = ko.observable('');
        self.CIFNo = ko.observable();
        self.CBSCIFNo = ko.observable();
        self.IsEnlistedCompany = ko.observable(false);

        self.CompanyNameList = ko.observableArray([]);//
        self.CompanyId = ko.observable();
        self.CompanyName = ko.observable();

        self.FactoryAddress = ko.observableArray([new FactoryAddressLine()]);
        self.AddFactoryAddressLine = function () {
            self.FactoryAddress.push(new FactoryAddressLine());
        }
        self.RemovedFactoryAddress = ko.observableArray([]);
        self.RemoveFactoryAddressLine = function (line) {
            if (line.Id() > 0)
                self.RemovedFactoryAddress.push(line.Id());
            self.FactoryAddress.remove(line);
        }

        self.Owners = ko.observableArray([new OwnersLine()]);
        self.CIF_PersonalList = ko.observableArray([]);
        self.CIF_Org_OwnersRoleList = ko.observableArray([]);
        self.AddOwnersLine = function () {
            self.Owners.push(new OwnersLine());
        }
        self.RemovedOwners = ko.observableArray([]);
        self.RemoveOwnersLine = function (line) {
            if (line.Id() > 0)
                self.RemovedOwners.push(line.Id());
            self.Owners.remove(line);
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

        self.LegalStatus = ko.observable();
        self.LegalStatusName = ko.observable();
        self.LegalStatusList = ko.observableArray([]);//        

        self.TradeLicenceNo = ko.observable().extend({
            pattern: {
                message: 'Trade License can only contain a-z, A-Z, -, (, )',
                params: /^([A-Za-z0-9\-\(\)])+$/
            }
        });
        self.TradeLicenceDate = ko.observable();
        self.TradeLicenceDateTxt = ko.observable('');

        self.DateOfIncorporation = ko.observable();
        //self.DateOfIncorporationTxt = ko.observable('');

        self.TL_IssueAuthority = ko.observable();
        self.RegistrationNo = ko.observable().extend({
            pattern: {
                message: 'Registration No. can only contain a-z, A-Z, -, (, )',
                params: /^([A-Za-z0-9\-\(\)])+$/
            }
        });
        self.RegistrationDate = ko.observable('');
        self.RegistrationDateTxt = ko.observable('');
        self.RegAuthority = ko.observable();

        self.CountryIdList = ko.observableArray([]);//
        self.RegCountryId = ko.observable();
        self.RegCountryName = ko.observable();

        self.ETIN = ko.observable().extend({
            pattern: {
                message: 'ETIN can only contain numeric characters.',
                params: /^([0-9])+$/
            }
        });
        self.VATRegNo = ko.observable().extend({
            pattern: {
                message: 'VAT Registration can only contain a-z, A-Z, -, (, )',
                params: /^([A-Za-z0-9\-\(\)])+$/
            }
        });

        self.ContactPersonName = ko.observable();
        self.ContactPersonDesignation = ko.observable();
        self.ContactpersonPhone = ko.observable();
        self.ContactPersonCellPhone = ko.observable().extend({ digit: true, minLength: 11 });
        self.ContactPersonEmail = ko.observable().extend({ email: true });
        self.Website = ko.observable();

        self.NumberOfEmployees = ko.observable();
        self.TotalAsset = ko.observable();
        self.TotalAssetExcLandAndBuilding = ko.observable();
        self.AnnualTurnover = ko.observable();

        self.OfficeAddressId = ko.observable();
        self.RegAddressId = ko.observable();

        self.OfficeCountryList = ko.observableArray([]);
        self.OfficeDivisionList = ko.observableArray([]);
        self.OfficeDistrictList = ko.observableArray([]);
        self.OfficeThanaList = ko.observableArray([]);

        self.RegCountryList = ko.observableArray([]);
        self.RegDivisionList = ko.observableArray([]);
        self.RegDistrictList = ko.observableArray([]);
        self.RegThanaList = ko.observableArray([]);

        self.BusinessType = ko.observable();
        self.BusinessTypes = ko.observableArray([]);
        self.BusinessSize = ko.observable();
        self.BusinessSizes = ko.observableArray([]);
        self.SectorType = ko.observable().extend({ required: true });
        self.SectorTypes = ko.observableArray([]);

        self.NBFI_1_SectorCodeId = ko.observable().extend({ required: true });
        self.NBFI1SectorCode = ko.observable();
        self.NBFI_1_SectorCodeDescription = ko.observable();
        self.NBFI1SectorCode.subscribe(function () {
            self.NBFI_1_SectorCodeId(self.NBFI1SectorCode().Id);
            self.NBFI_1_SectorCodeDescription(self.NBFI1SectorCode().Description);
        });
        self.NBFI_2_3_SectorCodeId = ko.observable().extend({ required: true });
        self.NBFI_2_3_SectorCodeDescription = ko.observable();
        self.NBFI2SectorCode = ko.observable();
        self.NBFI2SectorCode.subscribe(function () {
            self.NBFI_2_3_SectorCodeId(self.NBFI2SectorCode().Id);
            self.NBFI_2_3_SectorCodeDescription(self.NBFI2SectorCode().Description)
        });
        self.NBDC_SectorCodeId = ko.observable().extend({ required: true });
        self.NBDC_SectorCodeDescription = ko.observable();
        self.NBDCSectorCode = ko.observable();
        self.NBDCSectorCode.subscribe(function () {
            self.NBDC_SectorCodeId(self.NBDCSectorCode().Id);
            self.NBDC_SectorCodeDescription(self.NBDCSectorCode().Description);
        })

        self.OfficeAddress = new address();
        self.RegAddress = new address();

        self.errors = ko.validation.group(self);
        self.IsValid = ko.computed(function () {
            console.log("errors - " + self.errors().length);
            if (self.errors().length === 0)
                return true;
            else {
                return false;
            }

        });

        self.LoadData = function () {
            if (self.CIF_OrganizationalId() > 0) {
                $.getJSON("/IPDC/CIF/GetCIFOrganizational/?cifOrgId=" + self.CIF_OrganizationalId(),
                    null,
                    function (data) {
                        self.FactoryAddress([]);
                        self.Owners([]);
                        self.CIFNo(data.CIFNo);
                        self.CBSCIFNo(data.CBSCIFNo);
                        self.Id(data.Id);
                        self.IsEnlistedCompany(data.IsEnlistedCompany);
                        //if(data.IsEnlistedCompany && data.IsEnlistedCompany === 'true')
                        $.when(self.LoadOffice())
                            .done(function () {
                                self.CompanyId(data.CompanyId);
                                self.CompanyName(data.CompanyName);
                            });
                        self.CompanyName(data.CompanyName);
                        $.when(self.LoadLegalStatusList())
                            .done(function () {
                                self.LegalStatus(data.LegalStatus);
                                //self.LegalStatusName(data.LegalStatusName);
                            });
                        self.TradeLicenceNo(data.TradeLicenceNo);
                        self.TradeLicenceDate(moment(data.TradeLicenceDate));
                        self.TradeLicenceDateTxt(data.TradeLicenceDateTxt);
                        self.TL_IssueAuthority(data.TL_IssueAuthority);

                        self.RegistrationNo(data.RegistrationNo);
                        self.RegistrationDate(moment(data.RegistrationDate));
                        self.RegistrationDateTxt(data.RegistrationDateTxt);
                        self.RegAuthority(data.RegAuthority);


                        self.RegCountryId(data.RegCountryId);
                        self.ETIN(data.ETIN);
                        self.VATRegNo(data.VATRegNo);
                        self.ContactPersonName(data.ContactPersonName);
                        self.ContactPersonDesignation(data.ContactPersonDesignation);
                        self.ContactpersonPhone(data.ContactpersonPhone);
                        self.ContactPersonCellPhone(data.ContactPersonCellPhone);
                        self.ContactPersonEmail(data.ContactPersonEmail);
                        self.Website(data.Website);

                        self.RegAddressId(data.RegAddressId);
                        self.OfficeAddressId(data.OfficeAddressId);

                        self.NumberOfEmployees(data.NumberOfEmployees);
                        self.TotalAsset(data.TotalAsset);
                        self.TotalAssetExcLandAndBuilding(data.TotalAssetExcLandAndBuilding);
                        self.AnnualTurnover(data.AnnualTurnover);

                        self.DateOfIncorporation(moment(data.DateOfIncorporation));
                        $.when(self.LoadCountryIDList())
                            .done(function () {
                                self.RegCountryId(data.RegCountryId);
                                if (data.RegAddress) {
                                    self.RegAddress.LoadAddress(data.RegAddress);
                                }
                                if (data.FactoryAddress.length > 0) {
                                    $.each(data.FactoryAddress, function (index, value) {
                                        var factoryAdd = new FactoryAddressLine();
                                        factoryAdd.LoadFactoryAddress(value);
                                        self.FactoryAddress.push(factoryAdd);
                                    });
                                }
                                if (data.OfficeAddress) {
                                    self.OfficeAddress.LoadAddress(data.OfficeAddress);
                                }

                            });

                        if (data.Owners.length > 0) {
                            $.when(self.LoadRolesList()).done(function () {
                                //$.when(self.LoadCIFPerson()).done(function () {

                                    $.each(data.Owners, function (index, value) {
                                        //var temp;
                                        //$.each(self.CIF_PersonalList(), function (i, v) {
                                        //    if (v.Id === value.CIF_PersonalId)
                                        //        temp = v;
                                        //});

                                        var owner = new OwnersLine();
                                        owner.Load(value);
                                        self.Owners.push(owner);
                                    });
                                //});

                            });
                            //self.Owners(data.Owners)
                        }
                        $.when(self.LoadBusinessTypes()).done(function () {
                            self.BusinessType(data.BusinessType);
                        });
                        $.when(self.LoadBusinessSizes()).done(function () {
                            self.BusinessSize(data.BusinessSize);
                        });
                        $.when(self.LoadSectorTypes()).done(function () {
                            self.SectorType(data.SectorType);
                        });

                        self.NBFI_1_SectorCodeId(data.NBFI_1_SectorCodeId);
                        self.NBFI_1_SectorCodeDescription();
                        if (data.NBFI1SectorCode)
                            self.NBFI1SectorCode(data.NBFI1SectorCode);
                        self.NBFI_2_3_SectorCodeId(data.NBFI_2_3_SectorCodeId);
                        if (data.NBFI2SectorCode)
                            self.NBFI2SectorCode(data.NBFI2SectorCode);
                        self.NBDC_SectorCodeId(data.NBDC_SectorCodeId);
                        if (data.NBDCSectorCode)
                            self.NBDCSectorCode(data.NBDCSectorCode);
                    });
            }

        }

        //Add New Data
        self.Submit = function () {
            self.TradeLicenceDateTxt(moment(self.TradeLicenceDate()).format('DD/MM/YYYY'));
            self.RegistrationDateTxt(moment(self.RegistrationDate()).format('DD/MM/YYYY'));

            var FactoryAddress = ko.observableArray([]);
            var ownersInfo = ko.observableArray([]);

            $.each(self.FactoryAddress(),
                function (index, value) {
                    FactoryAddress.push({
                        Id: value.Id,
                        AddressId: value.AddressId,
                        Address: {
                            Id: value.Address.Id,
                            ThanaId: value.Address.ThanaId,
                            DistrictId: value.Address.DistrictId,
                            DivisionId: value.Address.DivisionId,
                            CountryId: value.Address.CountryId,
                            AddressLine1: value.Address.AddressLine1,
                            AddressLine2: value.Address.AddressLine2,
                            AddressLine3: value.Address.AddressLine3,
                            PostalCode: value.Address.PostalCode,
                            PhoneNo: value.Address.PhoneNo,
                            CellPhoneNo: value.Address.CellPhoneNo,
                            Email: value.Address.Email,
                            IsChanged: value.Address.IsChanged
                        }

                    });
                });
            $.each(self.Owners(),
                function (index, value) {
                    ownersInfo.push({
                        Id: value.Id(),
                        CIF_PersonalId: value.CIF_PersonalId(),
                        CIF_Org_OwnersRole: value.CIF_Org_OwnersRole()
                    });
                });

            var submitOrgData = {
                Id: self.Id(),
                CIFNo: self.CIFNo(),
                CBSCIFNo: self.CBSCIFNo(),
                IsEnlistedCompany: self.IsEnlistedCompany(),
                CompanyId: self.CompanyId(),
                CompanyName: self.CompanyName(),
                FactoryAddress: FactoryAddress,
                Owners: ownersInfo,
                LegalStatus: self.LegalStatus(),
                LegalStatusName: self.LegalStatusName(),
                TradeLicenceNo: self.TradeLicenceNo(),
                TradeLicenceDate: self.TradeLicenceDate(),
                TradeLicenceDateTxt: self.TradeLicenceDateTxt(),
                TL_IssueAuthority: self.TL_IssueAuthority(),
                RegistrationNo: self.RegistrationNo(),
                RegistrationDate: self.RegistrationDate(),
                RegistrationDateTxt: self.RegistrationDateTxt(),
                RegAuthority: self.RegAuthority(),
                RegCountryId: self.RegCountryId(),
                RegCountryName: self.RegCountryName(),
                ETIN: self.ETIN(),
                VATRegNo: self.VATRegNo(),
                ContactPersonName: self.ContactPersonName(),
                ContactPersonDesignation: self.ContactPersonDesignation(),
                ContactpersonPhone: self.ContactpersonPhone(),
                ContactPersonCellPhone: self.ContactPersonCellPhone(),
                ContactPersonEmail: self.ContactPersonEmail(),
                Website: self.Website(),
                NumberOfEmployees: self.NumberOfEmployees(),
                TotalAsset: self.TotalAsset(),
                TotalAssetExcLandAndBuilding: self.TotalAssetExcLandAndBuilding(),
                AnnualTurnover: self.AnnualTurnover(),
                OfficeAddressId: self.OfficeAddressId(),
                OfficeAddress: self.OfficeAddress,
                RegAddressId: self.RegAddressId(),
                RegAddress: self.RegAddress,
                RemovedOwners: self.RemovedOwners(),
                RemovedFactoryAddress: self.RemovedFactoryAddress(),
                BusinessType: self.BusinessType(),
                BusinessSize: self.BusinessSize(),
                SectorType: self.SectorType(),
                NBFI_1_SectorCodeId: self.NBFI_1_SectorCodeId(),
                NBFI_2_3_SectorCodeId: self.NBFI_2_3_SectorCodeId(),
                NBDC_SectorCodeId: self.NBDC_SectorCodeId(),
                DateOfIncorporationTxt: moment(self.DateOfIncorporation()).format('DD/MM/YYYY')
            };
            if (self.IsValid()) {
                $.ajax({
                    url: '/IPDC/CIF/SaveCifOrganizational',
                    type: 'POST',
                    contentType: 'application/json',
                    data: ko.toJSON(submitOrgData),
                    success: function (data) {
                        $('#successModal').modal('show');
                        $('#successModalText').text(data.Message);
                    },
                    error: function (error) {
                        alert(error.status + "<--and--> " + error.statusText);
                    }
                });
            } else {
                self.errors.showAllMessages();
            }
        }

        self.LoadOffice = function () {
            //if(self.EnlistedOrganization)
            return $.ajax({
                type: "GET",
                url: '/IPDC/Organization/GetOrganizations',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CompanyNameList(data);
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

        self.LoadLegalStatusList = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/CIF/GetAllCifOrgLegalStatus',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.LegalStatusList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.LoadRolesList = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/CIF/GetAllCifOrgOwnerRoles',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CIF_Org_OwnersRoleList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.LoadBusinessTypes = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/CIF/GetBusinessTypes',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.BusinessTypes(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.LoadBusinessSizes = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/CIF/GetBusinessSizes',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.BusinessSizes(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.LoadSectorTypes = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/CIF/GetCIF_Org_SectorTypes',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.SectorTypes(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.GetNBFI1SectorCodeList = function (searchTerm, callback) {
            $.ajax({
                dataType: "json",
                url: "/IPDC/CIF/GetSectorCodesForAutoFill",
                data: {
                    prefix: searchTerm,
                    sectorCode: 1
                },
            }).done(callback);
        };
        self.GetNBFI2SectorCodeList = function (searchTerm, callback) {
            $.ajax({
                dataType: "json",
                url: "/IPDC/CIF/GetSectorCodesForAutoFill",
                data: {
                    prefix: searchTerm,
                    sectorCode: 2
                },
            }).done(callback);
        };
        self.GetNBDCSectorCodeList = function (searchTerm, callback) {
            $.ajax({
                dataType: "json",
                url: "/IPDC/CIF/GetSectorCodesForAutoFill",
                data: {
                    prefix: searchTerm,
                    sectorCode: 3
                },
            }).done(callback);
        };

        //self.LoadCIFPerson = function () {
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

        self.Reset = function () {
            //self.CashAtHand("");

        }
        self.ExistingCifIds = ko.computed(function () {
            var list = [];
            $.each(self.Owners(), function (index, value) {
                if (value.CIF_PersonalId() > 0)
                    list.push(value.CIF_PersonalId());
            })
            return list;
        });
        
        self.GetCIFList = function (searchTerm, callback) {
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
        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
        self.Initializer = function () {
            self.LoadOffice();
            self.LoadLegalStatusList();
            self.LoadCountryIDList();
            self.LoadRolesList();
            //self.LoadCIFPerson();
            self.LoadBusinessTypes();
            self.LoadBusinessSizes();
            self.LoadSectorTypes();
        }
    };

    alVm = new CIFOrganizationalViewModel();
    alVm.Initializer();


    var qValue = alVm.queryString("cifOrgId");
    alVm.CIF_OrganizationalId(qValue);
    alVm.LoadData();
    ko.applyBindings(alVm, document.getElementById("cifOrganizational")[0]);

})
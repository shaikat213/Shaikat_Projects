
$(document).ready(function () {

    function Address() {
        var self = this;

        self.Id = ko.observable();
        self.CountryId = ko.observable().extend({ required: true });
        self.CountryName = ko.observable('');
        self.ThanaId = ko.observable('');
        self.ThanaName = ko.observable('');
        self.DistrictId = ko.observable('');
        self.DistrictName = ko.observable('');
        self.DivisionId = ko.observable('');
        self.DivisionName = ko.observable('');

        self.AddressLine1 = ko.observable('');
        self.AddressLine2 = ko.observable('');
        self.AddressLine3 = ko.observable('');
        self.PostalCode = ko.observable('');
        self.PhoneNo = ko.observable('');
        self.CellPhoneNo = ko.observable('');
        self.Email = ko.observable('');
        self.IsChanged = ko.observable(false);

        self.IsAddressChanged = function () {
            //
            self.IsChanged(true);
        }

        self.DivisionList = ko.observableArray([]);
        self.DistrictList = ko.observableArray([]);
        self.ThanaList = ko.observableArray([]);

        self.CountryId.subscribe(function () {
            //var officecountryId = self.CountryId();
            if (self.CountryId() > 0) {
                self.LoadDivisionByCountry();
            }

        });

        self.DivisionId.subscribe(function () {
            //var officecountryId = self.CountryId();
            if (self.DivisionId() > 0) {
                self.LoadDistrictByDivision();
            }

        });

        self.DistrictId.subscribe(function () {
            //var officecountryId = self.CountryId();
            if (self.DistrictId() > 0) {
                self.LoadThanaByDistrict();
            }
        });

        self.LoadDivisionByCountry = function () {
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
        }

        self.LoadDistrictByDivision = function () {
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
        }

        self.LoadThanaByDistrict = function () {
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
                $.when(self.LoadDistrictByDivision())
                    .done(function () {
                        self.DistrictId(data.DistrictId);
                        self.IsChanged(false);
                        $.when(self.LoadThanaByDistrict())
                            .done(function () {
                                self.ThanaId(data.ThanaId);
                                self.IsChanged(false);
                            });
                    });
            });
            //
            self.IsChanged(false);
        }
    }

    var MembersLine = function () {
        var self = this;
        self.Id = ko.observable();
        self.DeveloperId = ko.observable();
        self.Name = ko.observable();
        self.AddressId = ko.observable();

        self.Address = new Address();

        self.LoadData = function (data) {
            self.Id(data ? data.Id : 0);
            self.DeveloperId(data ? data.DeveloperId : 0);
            self.Name(data ? data.Name : "");
            self.AddressId(data ? data.AddressId : 0);

            if (data.Address != null && typeof (data.Address) != 'undefined') {
                self.Address.LoadAddress(data.Address);
            }
        }
    };

    var DirectorsLine = function () {
        var self = this;
        self.Id = ko.observable();
        self.DeveloperId = ko.observable();
        self.Name = ko.observable();
        self.Designation = ko.observable();
        self.AcademicQualification = ko.observable();
        self.SharePercentage = ko.observable();
        self.BusinessExperience = ko.observable();
        self.ContactNumber = ko.observable();
        self.LoadData = function (data) {
            self.Id(data ? data.Id : 0);
            self.DeveloperId(data ? data.DeveloperId : 0);
            self.Name(data ? data.BankName : "");
            self.Designation(data ? data.Designation : "");
            self.AcademicQualification(data ? data.AcademicQualification : "");
            self.SharePercentage(data ? data.SharePercentage : "");
            self.BusinessExperience(data ? data.BusinessExperience : "");
            self.ContactNumber(data ? data.ContactNumber : "");
        }
    };

    var OtherDocumentsLine = function () {
        var self = this;
        self.Id = ko.observable();
        self.DeveloperId = ko.observable();
        self.Name = ko.observable();
        self.DocumentStatus = ko.observable();
        self.LoadData = function (data) {
            self.Id(data ? data.Id : 0);
            self.DeveloperId(data ? data.DeveloperId : 0);
            self.Name(data ? data.Name : "");
            self.DocumentStatus(data ? data.DocumentStatus : 0);
        }
    };


    var DeveloperEntryViewModel = function () {
        var self = this;

        self.Id = ko.observable();

        self.DeveloperType = ko.observable();
        self.DeveloperTypeName = ko.observable();
        self.DeveloperTypeList = ko.observableArray([]);

        self.GroupName = ko.observable();
        self.Website = ko.observable();
        self.YearOfEstablishment = ko.observable();
        self.MemberOfRehab = ko.observable(false);
        self.RehabMemberNo = ko.observable();

        self.ContactPerson = ko.observable();
        self.ContactPersonDesignation = ko.observable();
        self.ContactPersonPhone = ko.observable();
        self.ContactPersonEmail = ko.observable();

        self.ArchitectCount = ko.observable();
        self.EngineerCount = ko.observableArray([]);
        self.MarketingEmpCount = ko.observable();
        self.OtherEmpCount = ko.observable();

        self.TypeOfAccount = ko.observable();
        self.TypeOfAccountName = ko.observable();
        self.TypeOfAccounList = ko.observable();


        self.BankName = ko.observable();
        self.TotalLiabilityAmount = ko.observable();


        self.NumberOfCompleteProject = ko.observable();
        self.NumberOfOngoingProject = ko.observable();
        self.NumberOfUpcomingProject = ko.observable();


        self.DeveloperMEM = ko.observable();


        self.DevelopreART = ko.observable();


        self.TradeLicence = ko.observable();


        self.FormXII = ko.observable();


        self.BoardResForSig = ko.observable();


        self.ContactPersonVCard = ko.observable();



        self.EnlistmentStatus = ko.observable();
        self.EnlistmentStatusName = ko.observable();
        self.EnlistmentStatuses = ko.observableArray([]);

        self.ApprovalCategory = ko.observable();
        self.ApprovalCategoryName = ko.observable();
        self.ApprovalCategoryList = ko.observableArray([]);
        self.ApprovalRemarks = ko.observable();

        self.Members = ko.observableArray([]);
        self.AddMembers = function () {
            self.Members.push(new MembersLine());
        }

        self.RemovedMembers = ko.observableArray([]);
        self.RemoveMembers = function (line) {
            if (line.Id() > 0)
                self.RemovedMembers.push(line.Id());
            self.Members.remove(line);
        }

        self.Directors = ko.observableArray([]);
        self.AddDirectors = function () {
            self.Directors.push(new DirectorsLine());
        }

        self.RemovedDirectors = ko.observableArray([]);
        self.RemoveDirectors = function (line) {
            if (line.Id() > 0)
                self.RemovedDirectors.push(line.Id());
            self.Directors.remove(line);
        }

        self.OtherDocuments = ko.observableArray([]);
        self.DocumentStatusList = ko.observableArray([]);
        self.AddOtherDocument = function () {
            self.OtherDocuments.push(new OtherDocumentsLine());
        }

        self.RemovedOtherDocuments = ko.observableArray([]);
        self.RemoveOtherDocuments = function (line) {
            if (line.Id() > 0)
                self.RemovedOtherDocuments.push(line.Id());
            self.OtherDocuments.remove(line);
        }


        self.CountryList = ko.observableArray([]);

        self.LoadDeveloperType = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Developer/GetDeveloperType',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.DeveloperTypeList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.LoadCategories = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Developer/GetDeveloperCategory',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ApprovalCategoryList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.LoadCountry = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Address/GetCountries',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CountryList(data); //Put the response in ObservableArray
                    //self.Address.CountryId(1);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.LoadEnlistmentStatus = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Developer/GetDeveloperEnlistmentStatuses',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.EnlistmentStatuses(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.LoadBankAccntType = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Developer/GetBankAccountTypes',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.TypeOfAccounList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.LoadDocumentStatus = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Developer/GetDocumentStatusList',
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


        self.Submit = function () {

            var members = ko.observableArray([]);
            var directors = ko.observableArray([]);
            var othersDocs = ko.observableArray([]);

            $.each(self.Members(),
                    function (index, value) {
                        members.push({
                            Id: value.Id(),
                            DeveloperId: value.DeveloperId(),
                            Name: value.Name(),
                            AddressId: value.AddressId(),
                            Address: value.Address
                        });
                    });

            $.each(self.Directors(),
                    function (index, value) {
                        directors.push({
                            Id: value.Id(),
                            DeveloperId: value.DeveloperId(),
                            Name: value.Name(),
                            Designation: value.Designation(),
                            AcademicQualification: value.AcademicQualification(),
                            SharePercentage: value.SharePercentage(),
                            BusinessExperience: value.BusinessExperience(),
                            ContactNumber: value.ContactNumber()
                        });
                    });

            $.each(self.OtherDocuments(),
                    function (index, value) {
                        othersDocs.push({
                            Id: value.Id(),
                            DeveloperId: value.DeveloperId(),
                            Name: value.Name(),
                            DocumentStatus: value.DocumentStatus()
                        });
                    });


            var submitDeveloper = {

                Id: self.Id(),
                DeveloperType: self.DeveloperType(),
                DeveloperTypeName: self.DeveloperType(),
                GroupName: self.GroupName(),
                Website: self.Website(),
                YearOfEstablishment: self.YearOfEstablishment(),
                MemberOfRehab: self.MemberOfRehab(),
                RehabMemberNo: self.RehabMemberNo(),
                ContactPerson: self.ContactPerson(),
                ContactPersonDesignation: self.ContactPersonDesignation(),
                ContactPersonPhone: self.ContactPersonPhone(),
                ContactPersonEmail: self.ContactPersonEmail(),
                ArchitectCount: self.ArchitectCount(),

                EngineerCount: self.EngineerCount(),
                MarketingEmpCount: self.MarketingEmpCount(),
                OtherEmpCount: self.OtherEmpCount(),
                TypeOfAccount: self.TypeOfAccount(),
                TypeOfAccountName: self.TypeOfAccountName(),
                BankName: self.BankName(),

                TotalLiabilityAmount: self.TotalLiabilityAmount(),
                NumberOfCompleteProject: self.NumberOfCompleteProject(),
                NumberOfOngoingProject: self.NumberOfOngoingProject(),
                NumberOfUpcomingProject: self.NumberOfUpcomingProject(),
                DeveloperMEM: self.DeveloperMEM(),
                DevelopreART: self.DevelopreART(),
                TradeLicence: self.TradeLicence(),
                FormXII: self.FormXII(),
                BoardResForSig: self.BoardResForSig(),
                ContactPersonVCard: self.ContactPersonVCard(),

                EnlistmentStatus: self.EnlistmentStatus(),
                EnlistmentStatusName: self.EnlistmentStatusName(),
                ApprovalCategory: self.ApprovalCategory(),
                ApprovalCategoryName: self.ApprovalCategoryName(),
                ApprovalRemarks: self.ApprovalRemarks(),

                Members: members,
                Directors: directors,
                OtherDocuments: othersDocs
            };

            $.ajax({
                url: '/IPDC/Developer/SaveDeveloper',
                type: 'POST',
                contentType: 'application/json',
                data: ko.toJSON(submitDeveloper),

                success: function (data) {
                    $('#DeveloperSuccessModal').modal('show');
                    $('#DeveloperSuccessModalText').text(data.Message);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        };

        self.LoadDeveloper = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Developer/LoadDeveloper?Id=' + self.Id(),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.Id(data.Id);

                    $.when(self.LoadDeveloperType()).done(function () {
                        self.DeveloperType(data.DeveloperType);
                        self.DeveloperTypeName(data.DeveloperTypeName);
                    });

                    self.GroupName(data.GroupName);
                    self.Website(data.Website);

                    self.YearOfEstablishment(data.YearOfEstablishment);

                    self.YearOfEstablishment(data.YearOfEstablishment);
                    self.MemberOfRehab(data.MemberOfRehab);
                    self.RehabMemberNo(data.RehabMemberNo);
                    self.ContactPerson(data.ContactPerson);
                    self.ContactPersonDesignation(data.ContactPersonDesignation);
                    self.ContactPersonPhone(data.ContactPersonPhone);
                    self.ContactPersonEmail(data.ContactPersonEmail);

                    self.ArchitectCount(data.ArchitectCount);
                    self.EngineerCount(data.EngineerCount);
                    self.MarketingEmpCount(data.MarketingEmpCount);
                    self.OtherEmpCount(data.OtherEmpCount);

                    $.when(self.LoadBankAccntType()).done(function () {
                        self.TypeOfAccount(data.TypeOfAccount);
                        self.TypeOfAccountName(data.TypeOfAccountName);
                    });

                    self.BankName(data.BankName);
                    self.TotalLiabilityAmount(data.TotalLiabilityAmount);
                    self.NumberOfCompleteProject(data.NumberOfCompleteProject);
                    self.NumberOfOngoingProject(data.NumberOfOngoingProject);
                    self.NumberOfUpcomingProject(data.NumberOfUpcomingProject);

                    self.DeveloperMEM(data.DeveloperMEM.toString());
                    self.DevelopreART(data.DevelopreART.toString());
                    self.TradeLicence(data.TradeLicence.toString());
                    self.FormXII(data.FormXII.toString());
                    self.BoardResForSig(data.BoardResForSig.toString());
                    self.ContactPersonVCard(data.ContactPersonVCard.toString());


                    $.when(self.LoadEnlistmentStatus()).done(function () {
                        self.EnlistmentStatus(data.EnlistmentStatus);
                        self.EnlistmentStatusName(data.EnlistmentStatusName);
                    });

                    $.when(self.LoadCategories()).done(function () {
                        self.ApprovalCategory(data.ApprovalCategory);
                        self.ApprovalCategoryName(data.ApprovalCategoryName);
                    });

                    self.ApprovalRemarks(data.ApprovalRemarks);

                    $.when(self.LoadDocumentStatus())
                        .done(function () {
                            $.each(data.OtherDocuments,
                                function (index, value) {
                                    var aDetail = new OtherDocumentsLine();
                                    if (typeof (value) != 'undefined') {
                                        aDetail.LoadData(value);
                                        self.OtherDocuments.push(aDetail);
                                    }
                                });
                        });

                    $.when(self.LoadCountry())
                        .done(function () {
                            $.each(data.Members,
                                function (index, value) {
                                    var aDetail = new MembersLine();
                                    if (typeof (value) != 'undefined') {
                                        aDetail.LoadData(value);
                                        self.Members.push(aDetail);
                                    }
                                });
                        });

                    $.each(data.Directors,
                        function (index, value) {
                            var aDetail = new DirectorsLine();
                            if (typeof (value) != 'undefined') {
                                aDetail.LoadData(value);
                                self.Directors.push(aDetail);
                            }
                        });

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.Initializer = function () {
            if (self.Id() > 0) {
                self.LoadDeveloper();
            } else {
                self.LoadDeveloperType();
                self.LoadCountry();
                self.LoadCategories();
                self.LoadEnlistmentStatus();
                self.LoadBankAccntType();

                self.LoadDocumentStatus();
            }
        }

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }

    };

    var deVm = new DeveloperEntryViewModel();

    deVm.Id(deVm.queryString("Id"));
    deVm.Initializer();
    ko.applyBindings(deVm, document.getElementById("DeveloperEntry")[0]);

})
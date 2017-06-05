
//$(document).ready(function () {
function cifRefAddress() {
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
    self.PostalCode = ko.observable().extend({ digit: true });
    self.PhoneNo = ko.observable().extend({ digit: true });
    self.CellPhoneNo = ko.observable().extend({ digit: true, minLength: 11 });
    self.Email = ko.observable().extend({ email: true });
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
                .done(function() {
                    self.DistrictId(data.DistrictId);
                    self.IsChanged(false);
                    $.when(self.LoadThanaByDistrict())
                        .done(function() {
                            self.ThanaId(data.ThanaId);
                            self.IsChanged(false);
                        });
                });
        });
        //
        self.IsChanged(false);
    }
}

var CIFReferenceLine = function () {
    var self = this;
    self.Id = ko.observable();
    self.CIF_PersonalId = ko.observable();
    self.Name = ko.observable().extend({ required: true });
    self.Designation = ko.observable();
    self.Department = ko.observable();
    self.EnlistedOrganization = ko.observable(false);
    self.NotEnlistedOrganization = ko.pureComputed(function () {
        if (self.EnlistedOrganization() === true)
            return false;
        else
            return true;
    });
    self.OrganizationName = ko.observable();
    self.OrganizationId = ko.observable('');
    self.RelationshipWithApplicant = ko.observable().extend({ required: true });

    self.OfficeAddressId = ko.observable();
    self.ResidenceAddressId = ko.observable();
    self.PermanentAddressId = ko.observable();
    //
    //self.OfficeAddress = ko.observable();//new cifRefAddress();
    //self.OfficeAddress(new cifRefAddress());
    self.OfficeAddress = new cifRefAddress();
    self.ResidenceAddress = new cifRefAddress();
    self.PermanentAddress = new cifRefAddress();

    self.Load = function(data) {
        self.Id(data? data.Id :0);
        self.CIF_PersonalId(data? data.CIF_PersonalId : 0);
        self.Name(data? data.Name : "");
        self.Designation(data? data.Designation :"" );
        self.Department(data? data.Department :"");
        self.EnlistedOrganization(data? data.EnlistedOrganization :false);
        //self.NotEnlistedOrganization = ko.pureComputed(function () {
        //    if (self.EnlistedOrganization() === true)
        //        return false;
        //    else
        //        return true;
        //});
        self.OrganizationName(data? data.OrganizationName :"");
        self.OrganizationId(data? data.OrganizationId :0);
        self.RelationshipWithApplicant(data? data.RelationshipWithApplicant : "");

        self.OfficeAddressId(data? data.OfficeAddressId :0);
        self.ResidenceAddressId(data?data.ResidenceAddressId :0);
        self.PermanentAddressId(data ? data.PermanentAddressId : 0);
        if (data.OfficeAddress != null && typeof (data.OfficeAddress) != 'undefined') {
            self.OfficeAddress.LoadAddress(data.OfficeAddress);
        }
        if (data.ResidenceAddress != null && typeof (data.ResidenceAddress) != 'undefined') {
            self.ResidenceAddress.LoadAddress(data.ResidenceAddress);
        }
        if (data.PermanentAddress != null && typeof (data.PermanentAddress) != 'undefined') {
            self.PermanentAddress.LoadAddress(data.PermanentAddress);
        }
        //self.OfficeAddress();
        //self.ResidenceAddress(data?data.ResidenceAddress :null);
        //self.PermanentAddress(data?data.PermanentAddress : null);
    }

    self.UseResidenceAddress = function () {
        var id;
        if (self.ResidenceAddress.Id() > 0)
            id = self.ResidenceAddress.Id();
        var addressCopy = {
            Id: self.ResidenceAddress.Id(),
            ThanaId: self.ResidenceAddress.ThanaId(),
            DistrictId: self.ResidenceAddress.DistrictId(),
            DivisionId: self.ResidenceAddress.DivisionId(),
            CountryId: self.ResidenceAddress.CountryId(),
            AddressLine1: self.ResidenceAddress.AddressLine1(),
            AddressLine2: self.ResidenceAddress.AddressLine2(),
            AddressLine3: self.ResidenceAddress.AddressLine3(),
            PostalCode: self.ResidenceAddress.PostalCode(),
            PhoneNo: self.ResidenceAddress.PhoneNo(),
            CellPhoneNo: self.ResidenceAddress.CellPhoneNo(),
            Email: self.ResidenceAddress.Email(),
            IsChanged: self.ResidenceAddress.IsChanged()
        }
        self.PermanentAddress.LoadAddress(addressCopy);
        if (typeof (id) != 'undefined') {
            self.PermanentAddress.Id(id);
            self.PermanentAddressId(id);
        }
    }


};

var CIFRefenceViewModel = function () {
    var self = this;
    self.CIF_PersonalId = ko.observable();
    //self.queryString = function getParameterByName(name) {
    //    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    //    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
    //        results = regex.exec(location.search);
    //    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    //}

    //Income Variables

    self.CIFReference = ko.observableArray([]);//new CIFReferenceLine()
    self.OrganizationList = ko.observableArray([]);
    //self.RelationshipWithApplicant = ko.observable().extend({ required: true });

    self.CountryList = ko.observableArray([]);
    //self.DivisionList = ko.observableArray([]);
    //self.DistrictList = ko.observableArray([]);
    //self.ThanaList = ko.observableArray([]);

    //self.ResidenceCountryList = ko.observableArray([]);
    //self.ResidenceDivisionList = ko.observableArray([]);
    //self.ResidenceDistrictList = ko.observableArray([]);
    //self.ResidenceThanaList = ko.observableArray([]);

    //self.PermanentCountryList = ko.observableArray([]);
    //self.PermanentDivisionList = ko.observableArray([]);
    //self.PermanentDistrictList = ko.observableArray([]);
    //self.PermanentThanaList = ko.observableArray([]);

    self.AddCIFReferenceLine = function () {
        self.CIFReference.push(new CIFReferenceLine());
    }
    self.RemoveCIFReferenceLine = function (line) {
        //
        if (line.Id() > 0) {
            //$.getJSON("/IPDC/CIF/RemoveRef/?cid=" + line.Id(),
            //   null,
            //   function (data) {
                  
            //   });
            $.ajax({
                type: "GET",
                url: '/IPDC/CIF/RemoveRef/?cid=' + line.Id(),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                  
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.CIFReference.remove(line);
    }



    //self.CIF_PersonalId = ko.computed(function () {
    //    var value = 1;
    //    return value;
    //});

    self.LoadReference = function () {
        if (self.CIF_PersonalId() > 0) {
            $.getJSON("/IPDC/CIF/GetCIFReference/?cifId=" + self.CIF_PersonalId(),
                null,
                function (data) {
                    self.CIFReference([]);
                    $.when(self.LoadCountry())
                        .done(function() {
                            $.each(data,
                                function(index, value) {
                                    var cifRef = new CIFReferenceLine();
                                    cifRef.Load(value);
                                    self.CIFReference.push(cifRef);
                                });

                        });

                });
        }

    }

    //chosenItems: ko.observableArray()

    //Add New Data
    self.Submit = function () {
       //
        var cifReferenceInfo = ko.observableArray([]);
        //var cifRefOfficeAddress;
        //var cifRefResidenceAddress;
        //var cifRefPermanentAddress;
        $.each(self.CIFReference(),
            function (index, value) {
                cifReferenceInfo.push({
                    Id : value.Id,
                    CIF_PersonalId: self.CIF_PersonalId(),
                    Name: value.Name,
                    Designation: value.Designation,
                    Department: value.Department,
                    EnlistedOrganization: value.EnlistedOrganization,
                    OrganizationName: value.OrganizationName,
                    OrganizationId: value.OrganizationId,
                    RelationshipWithApplicant: value.RelationshipWithApplicant,
                    ResidenceAddress: value.ResidenceAddress,
                    PermanentAddress: value.PermanentAddress,
                    OfficeAddress: value.OfficeAddress
                });
            });
        $.ajax({
            url: '/IPDC/CIF/SaveCIFReference',
            //cache: false,
            type: 'POST',
            contentType: 'application/json',
            data: ko.toJSON(cifReferenceInfo),
            success: function (data) {
                $('#successModal').modal('show');
                $('#successModalText').text(data.Message);
                if (data.Id > 0) {
                    self.LoadReference();
                }
                //self.Reset();
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.LoadOffice = function () {
        //if(self.EnlistedOrganization)
        $.ajax({
            type: "GET",
            url: '/IPDC/Organization/GetOrganizations',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.OrganizationList(data);
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });

    }

    // Office
    self.LoadCountry = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Address/GetCountries',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.CountryList(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }


    self.Reset = function () {
        self.Name("");
        self.Designation("");
        self.Department("");
        self.EnlistedOrganization("");
        self.OrganizationName("");
        //self.OrganizationId("");
        //self.OfficeAddressId("");
        self.RelationshipWithApplicant("");
        //self.ResidenceAddressId("");
        //self.PermanentAddressId("");
    }
    self.Initialize = function () {
        self.LoadOffice();
        self.LoadCountry();
    }
};

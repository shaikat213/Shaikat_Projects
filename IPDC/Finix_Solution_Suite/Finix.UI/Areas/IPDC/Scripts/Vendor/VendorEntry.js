
$(document).ready(function () {

    var ShowroomsLine = function () {
        var self = this;
        self.Id = ko.observable();
        self.Name = ko.observable();
        self.VendorId = ko.observable();

        self.ContactPerson = ko.observable();
        self.ContactPersonPhone = ko.observable();
        self.ContactPersonEmail = ko.observable();

        self.AddressId = ko.observable();
        self.Address = new address();
        self.LoadData = function (data) {
            self.Id(data ? data.Id : 0);
            self.Name(data ? data.Name : "");
            self.VendorId(data ? data.DeveloperId : 0);
            self.ContactPerson(data ? data.ContactPerson : "");
            self.ContactPersonPhone(data ? data.ContactPersonPhone : "");
            self.ContactPersonEmail(data ? data.ContactPersonEmail : "");

            self.AddressId(data ? data.AddressId : 0);

            if (data.Address != null && typeof (data.Address) != 'undefined') {
                self.Address.LoadAddress(data.Address);
            }
        }
    };


    var VendorEntryViewModel = function () {
        var self = this;

        self.Id = ko.observable();
        self.Name = ko.observable();
        self.VendorProductType = ko.observable();
        self.VendorProductTypeName = ko.observable();
        self.VendorProductTypeList = ko.observableArray([]);
        self.ContactPerson = ko.observable();
        self.ContactPersonPhone = ko.observable();
        self.ContactPersonEmail = ko.observable();
        self.Website = ko.observable();

        self.VendorAddressId = ko.observable();
        self.CountryList = ko.observableArray([]);

        self.Showrooms = ko.observableArray([]);
        self.AddShowrooms = function () {
            self.Showrooms.push(new ShowroomsLine());
        }
        self.RemovedShowrooms = ko.observableArray([]);
        self.RemoveShowrooms = function (line) {
            if (line.Id() > 0)
                self.RemovedShowrooms.push(line.Id());
            self.Showrooms.remove(line);
        }

        self.VendorAddress = new address();


        self.LoadVendorProductType = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Vendor/GetVendorProductType',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.VendorProductTypeList(data); //Put the response in ObservableArray
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
                    self.CountryList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.Submit = function () {

            var showrooms = ko.observableArray([]);

            $.each(self.Showrooms(),
                    function (index, value) {
                        showrooms.push({
                            Id: value.Id(),
                            Name: value.Name(),
                            VendorId: value.VendorId(),
                            ContactPerson: value.ContactPerson(),
                            ContactPersonPhone: value.ContactPersonPhone(),
                            ContactPersonEmail: value.ContactPersonEmail(),
                            AddressId: value.AddressId(),
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

            var submitVendor = {

                Id: self.Id(),
                VendorProductType: self.VendorProductType(),
                Name: self.Name(),
                VendorProductTypeName: self.VendorProductTypeName(),
                ContactPerson: self.ContactPerson(),
                ContactPersonPhone: self.ContactPersonPhone(),
                ContactPersonEmail: self.ContactPersonEmail(),
                Website: self.Website(),

                VendorAddressId: self.VendorAddressId(),
                VendorAddress: self.VendorAddress,
                Showrooms: showrooms,
                RemovedShowrooms: self.RemovedShowrooms()
            };

            $.ajax({
                url: '/IPDC/Vendor/SaveVendor',
                type: 'POST',
                contentType: 'application/json',
                data: ko.toJSON(submitVendor),

                success: function (data) {
                    $('#DeveloperSuccessModal').modal('show');
                    $('#DeveloperSuccessModalText').text(data.Message);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        };

        self.LoadVendor = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Vendor/LoadVendor?Id=' + self.Id(),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.Id(data.Id);
                    self.Name(data.Name);
                    $.when(self.LoadVendorProductType()).done(function () {
                        self.VendorProductType(data.VendorProductType);
                        self.VendorProductTypeName(data.VendorProductTypeName);
                    });

                    self.ContactPerson(data.ContactPerson);
                    self.ContactPersonPhone(data.ContactPersonPhone);
                    self.ContactPersonEmail(data.ContactPersonEmail);
                    self.Website(data.Website);

                    self.VendorAddressId(data.RegAddressId);

                    $.when(self.LoadCountry())
                        .done(function () {

                            if (data.VendorAddress) {
                                self.VendorAddress.LoadAddress(data.VendorAddress);
                            }

                            if (data.Showrooms.length > 0) {
                                $.each(data.Showrooms,
                                    function (index, value) {
                                        var aDetail = new ShowroomsLine();
                                        if (typeof (value) != 'undefined') {
                                            aDetail.LoadData(value);
                                            self.Showrooms.push(aDetail);
                                        }
                                    });
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
                self.LoadVendor();
            } else {
                self.LoadVendorProductType();
                self.LoadCountry();;
            }
        }

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }

    };

    var deVm = new VendorEntryViewModel();

    deVm.Id(deVm.queryString("Id"));
    deVm.Initializer();
    ko.applyBindings(deVm, document.getElementById("VendorEntry")[0]);

})
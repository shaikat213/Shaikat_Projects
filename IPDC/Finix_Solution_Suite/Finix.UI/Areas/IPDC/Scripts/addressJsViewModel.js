
function address() {
    var self = this;

    self.IsChanged = ko.observable(false);

    self.Id = ko.observable();
    self.CountryId = ko.observable();//.extend({ required: true });
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
    self.PostalCode = ko.observable().extend({ digit: true });
    self.PostalCode.subscribe(function () { self.IsChanged(true); });
    self.PhoneNo = ko.observable().extend({ digit: true });
    self.PhoneNo.subscribe(function () { self.IsChanged(true); });
    self.CellPhoneNo = ko.observable().extend({ digit: true, minLength: 11 });
    self.CellPhoneNo.subscribe(function () { self.IsChanged(true); });
    self.Email = ko.observable().extend({ email: true });
    self.Email.subscribe(function () { self.IsChanged(true); });
    self.ThanaId.subscribe(function () { self.IsChanged(true); });

    self.DivisionList = ko.observableArray([]);
    self.DistrictList = ko.observableArray([]);
    self.ThanaList = ko.observableArray([]);

    self.CountryId.subscribe(function () {
        self.IsChanged(true);
        self.DivisionList([]);
        self.DistrictList([]);
        self.ThanaList([]);

        if (self.CountryId() > 0) {
            self.LoadDivisionByCountry();
        }

    });

    self.DivisionId.subscribe(function () {
        self.IsChanged(true);
        self.DistrictList([]);
        self.ThanaList([]);

        if (self.DivisionId() > 0) {
            self.LoadDistrictByDivision();
        }

    });

    self.DistrictId.subscribe(function () {
        self.IsChanged(true);
        
        self.ThanaList([]);
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
        //
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
        //
        self.IsChanged(false);
    }
}
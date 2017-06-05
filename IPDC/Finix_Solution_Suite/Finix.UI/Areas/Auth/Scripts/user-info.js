$(document).ready(function () {
    function UserInfo() {
        var self = this;
        //self.Id = ko.observable();
        self.UserId = ko.observable('');
        self.CompanyList = ko.observableArray([]);
        self.CompanyProfileId = ko.observable();

        self.UserName = ko.observable().extend({ required: true, pattern: { message: 'Only alphabetical values required.', params: "^[_A-Za-z ]{1,}$", maxLength: "100" } });
        self.Password = ko.observable().extend({ required: true, pattern: { message: 'valid password required.', params: "^[_A-Za-z0-9_*]{6,12}$", maxLength: "100" } });
        self.ConfirmPassword = ko.observable().extend({ required: true, areSame: self.Password });
        self.errors = ko.validation.group(self);
        self.IsValid = ko.computed(function () {
            if (self.errors().length == 0)
                return true;
            return false;
        });
        self.SaveUser = function () {

            if (self.errors().length == 0) {
                //self.EmployeeId = ko.observable(val);

                var postData = {
                    UserId: self.UserId(),
                    CompanyProfileId: self.CompanyProfileId(),
                    UserName: self.UserName(),
                    Password: self.Password()
                };
                $.ajax({
                    type: "POST",
                    url: '/Auth/User/UserRegistration',
                    data: ko.toJSON(postData),
                    contentType: "application/json",
                    success: function (data) {

                        toastr.success(data);
                        self.Reset();
                        //window.location.href = "/Bank/Create";
                    },
                    error: function () {
                        alert(error.status + "<--save and--> " + error.statusText);
                    }
                });
            } else {
                self.errors.showAllMessages();
            }
        }
        self.Reset = function () {
            self.UserName('');
            self.Password('');
            self.ConfirmPassword('');
        }
        self.getAllCompanies = function () {
            if (userCompanyId != null && userCompanyId > 0) {
                self.CompanyProfileId(userCompanyId);
            } else {
                self.CompanyList([]);
                return $.ajax({
                    type: "GET",
                    url: '/Auth/CompanyProfile/GetAllCompanyList',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        self.CompanyList(data);
                    },
                    error: function (error) {
                        alert(error.status + "<--and--> " + error.statusText);
                    }
                });
            }
        }

        self.GetUserInfoDetails = function () {
            return $.ajax({
                type: "GET",
                url: "/IPDC/Employee/GetUserInfoById/?id=" + val,
                dataType: "json",
                success: function (data) {
                    self.Username(data.Username);
                    self.Password(data.Password);
                    self.ConfirmPassword(data.Password);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) { }
            });
        }

    }

    var UserInfovm = new UserInfo();
    ko.applyBindings(UserInfovm, document.getElementById('userEntry'));
});
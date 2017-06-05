$(document).ready(function () {
    function ProxyRolePermissionVM() {
        var self = this;

        self.RoleList = ko.observableArray();
        self.RoleId = ko.observable(0);
        self.CompanyProfileId = ko.observable(userCompanyId);
        self.rolePermissionList = ko.observable();
        self.ProxyUsers = ko.observableArray([]);
        self.ProxyUserId = ko.observable();
        self.FromDate = ko.observable();
        self.ToDate = ko.observable();

        self.getRoles = function () {
            self.RoleList([]);
            $.ajax({
                url: '/Auth/Role/GetRoles',
                type: "GET",
                success: function (roles) {
                    self.RoleList(roles);
                }
            });
        };
        self.getProxyUsers = function () {
            self.ProxyUsers([]);
            $.ajax({
                url: '/Auth/User/GetUsersByCompanyId?CompanyProfileId=' + self.CompanyProfileId(),
                type: "GET",
                success: function (roles) {
                    self.ProxyUsers(roles);
                }
            });
        }
        self.getRolePermissionData = function () {
            var roleId = null;
            if (self.RoleId() > 0)
                roleId = self.RoleId();
            //$('#MenuHierarchy').empty();
            $('#MenuHierarchy').jstree("destroy").empty();
            $.ajax({
                url: '/Auth/Role/GetRolePermissionData',
                type: "GET",
                success: function (modules) {
                    //return modules;

                    $('#MenuHierarchy').jstree({
                        'plugins': ["wholerow", "checkbox"], 'core': {
                            "animation": 0,
                            'data': modules
                        }
                    });
                }
            });
        }

        self.getCurrentRolePermissionData = function () {
            //var roleId = null;
            //if (self.RoleId() > 0)
            //    roleId = self.RoleId();
            //$('#MenuHierarchy').empty();
            //$('#MenuHierarchy').jstree("destroy").empty();
            if (self.RoleId() > 0) {

                $("#MenuHierarchy").jstree(true).deselect_all();
                $("#MenuHierarchy").jstree(true).close_all();
                $.ajax({
                    url: '/Auth/Role/GetCurrentRolePermissionData?RoleId=' + self.RoleId(),
                    type: "GET",
                    success: function (list) {
                        $("#MenuHierarchy").jstree(true).select_node(list);
                    }
                });
            }
        }

        self.RoleId.subscribe(function () {
            if (self.RoleId() > 0) {
                self.getCurrentRolePermissionData();
            }
            //self.getRolePermissionData();
        });

        self.Save = function () {
            var checked_ids = [];

            checked_ids = $("#MenuHierarchy").jstree(true).get_selected();

            var postData = {
                ProxyUserId: self.ProxyUserId(),
                FromDate: moment(self.FromDate()).format('DD/MM/YYYY'),
                ToDate: moment(self.ToDate()).format('DD/MM/YYYY'),
                ids: checked_ids,
                roleId: self.RoleId()
            };
            $.ajax({
                type: "POST",
                url: '/Auth/Role/SaveProxyRolePermissions',
                data: JSON.stringify(postData),
                contentType: "application/json",
                success: function (data) {
                    //window.location.href = "/Accounts/Accounts/GenAccounts";
                    alert(data.Message);
                    //self.isLoading(self.isLoading() - 1);
                },
                error: function () {
                    //self.isLoading(self.isLoading() - 1);
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.Initialize = function () {
            self.getRoles();
            self.getProxyUsers();
            self.getRolePermissionData();
        }
    };

    var vm = new ProxyRolePermissionVM();

    vm.Initialize();
    ko.applyBindings(vm, $('#proxyRolePermissionAssignment')[0]);
});
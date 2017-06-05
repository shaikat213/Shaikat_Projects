$(document).ready(function () {
    var vm;
    function TableDataVM(data) {
        var self = this;
        self.id = data.id;
        self.selected = ko.observable(data.selected);
        self.description = ko.observable(data.description);
    }

    function BasicDataVM(data) {
        var self = this;

        //ddl data
        self.leftDdlData = ko.mapping.fromJS(data.leftDdlData);
        self.leftDdlSelectedItem = ko.observable();
        /* data-soruce from which right ddl data is filtered based on left ddl selelcted value*/
        self.rightDdlAllData = ko.mapping.fromJS(data.rightDdlAllData);
        self.rightDdlData = ko.observableArray([]);
        self.rightDdlSelectedItem = ko.observable();

        //table data
        self.leftTableData = ko.observableArray([]);
        self.rightTableData = ko.observableArray([]);
        self.leftTableSelectedItemId = ko.observable('');
        self.leftTableSelectedItemIdList = ko.observable([]);

        //subscriptions
        self.leftDdlSelectedItem.subscribe(function (value) {
            var arr = self.rightDdlData();
            arr.length = 0;
            for (var k = 0; k < self.rightDdlAllData().length; k++) {
                if (self.rightDdlAllData()[k].leftTable() == value)
                    arr.push(self.rightDdlAllData()[k]);
            }
            self.rightDdlData(arr);
            if (self.rightTableData().length > 0)
                self.rightDdlSelectedItem(self.rightTableData()[0]);

            self.leftTableData([]);
            self.rightTableData([]);
        });
        self.leftTableSelectedItemId.subscribe(function (newVal) {
            var reqParams = { leftTable: self.leftDdlSelectedItem(), rightTable: self.rightDdlSelectedItem(), leftTableRecordId: newVal };
            $.getJSON(
                "/Auth/Assignments/GetM2MRightTableSelectedData/", reqParams,
                function (resp) {
                    var rtd = self.rightTableData();
                    ko.utils.arrayForEach(rtd, function (item) {
                        item.selected(false);
                    });

                    for (var i in resp.data) {
                        ko.utils.arrayForEach(rtd, function (item) {
                            if (item.id == resp.data[i]) {
                                item.selected(true);
                            }
                        });
                    }
                    self.rightTableData(rtd);
                }
            );


        });
        self.selectRightTablesAllData = function (data, event) {
            ko.utils.arrayForEach(self.rightTableData(), function (item) {
                item.selected(event.target.checked);
            });
            return true;
        };

        //load data
        self.loadData = function () {
            self.leftTableSelectedItemId(null);
            var reqParams = { leftTable: self.leftDdlSelectedItem(), rightTable: self.rightDdlSelectedItem() };
            $.getJSON(
                "/Auth/Assignments/GetManyToManyTableData/", reqParams,
                function (resp) {
                    var ltd = [];
                    var rtd = [];
                    ko.utils.arrayForEach(resp.data, function (item) {
                        var tdv = new TableDataVM({ id: item.Id, description: item.Description });
                        if (item.IsLeftTable == true)
                            ltd.push(tdv);
                        else
                            rtd.push(tdv);
                    });
                    self.leftTableData(ltd);
                    self.rightTableData(rtd);
                    //if (self.leftTableData().length > 0)
                    //    self.leftTableSelectedItemId(self.leftTableData()[0].id);
                }
            );
        };

        //save data
        self.saveMany2ManyAssignments = function () {
            var rtIds = [];
            ko.utils.arrayForEach(self.rightTableData(), function (item) {
                if (item.selected())
                    rtIds.push(item.id);
            });
            var reqParams = {
                leftTable: self.leftDdlSelectedItem(),
                rightTable: self.rightDdlSelectedItem(),
                leftTableRecordId: self.leftTableSelectedItemId(),
                rightTableRecordIds: rtIds
            };
            toastr.info("Saving Data...");
            $.ajax({
                url: "/Auth/Assignments/UpdateM2MData/",
                type: 'post',
                traditional: true,
                data: reqParams,
                success: function (resp) {
                    var rtd = self.rightTableData();
                    ko.utils.arrayForEach(rtd, function (item) {
                        item.selected(false);
                    });

                    for (var i in resp.data) {
                        ko.utils.arrayForEach(rtd, function (item) {
                            if (item.id == resp.data[i]) {
                                item.selected(true);
                            }
                        });
                    }
                    self.rightTableData(rtd);
                    toastr.success("Data Saved Successfully");
                },
                error: function (err) {
                    toastr.error(err);
                }

            }
            );

        };
    }
    $.getJSON("/Auth/Assignments/GetManyToManyDdlData", function (data) {

        vm = new BasicDataVM(data);
        ko.applyBindings(vm, $("#dvMany2Many")[0]);
    });


});
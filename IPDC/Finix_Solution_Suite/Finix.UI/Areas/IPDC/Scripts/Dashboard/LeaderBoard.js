var crt = [{ "Name": "Number", "Id": 1 },
    { "Name": "Amount", "Id": 2 }];

$(document).ready(function () {

    function LeaderBoardViewModel() {
        var self = this;
        self.TimeLines = ko.observableArray([
            { "Name": "Today - Yesterday", "Id": 1 },
            { "Name": "MTD - LMTD", "Id": 3 },
            { "Name": "YTD - LYTD", "Id": 5 },
            { "Name": "QTD - LYQTD", "Id": 7 }
        ]);
        self.Criterias = ko.observableArray([
            { "Name": "Number", "Id": 1 },
            { "Name": "Amount", "Id": 2 }
        ]);
        self.Stages = ko.observableArray([
           { "Name": "Call", "Id": 1 },
           { "Name": "Lead", "Id": 2 },
           { "Name": "Application", "Id": 3 },
           { "Name": "Approval", "Id": 4 },
           { "Name": "Rejection", "Id": 5 },
           { "Name": "Disbursement", "Id": 6 }
        ]);
        self.CostCenter = ko.observable();
        self.SelectedCriteria = ko.observable(1);
        self.SelectedTimeLine = ko.observable();
        self.ProductId = ko.observable();
        self.OfficeId = ko.observable();
        self.StageId = ko.observable(1);
        self.CostCenters = ko.observableArray([]);
        self.Products = ko.observableArray([]);
        //self.Stages = ko.observableArray([]);
        self.Offices = ko.observableArray([]);
        self.TopFirst = ko.observableArray([]);
        self.BottomFirst = ko.observableArray([]);
        self.GetAllCostCenters = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetCostCenters',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CostCenters(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetAllProducts = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Product/GetAllProducts',
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
        self.GetAllBranches = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Office/GetAllActiveOffices',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.Offices(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetHighlights = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Dashboard/GetLeaderboardInfo?timeLine=' + self.SelectedTimeLine() + '&stageId=' + self.StageId() + '&criteriaId=' + self.SelectedCriteria() + '&centerId=' + self.CostCenter() + '&productId=' + self.ProductId() + '&branchId=' + self.OfficeId(),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.TopFirst(data.TopFirst);
                    self.BottomFirst(data.BottomFirst);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
       
        //self.GetAllBranches = ko.observableArray([]);
        self.Initialize = function () {
            self.GetAllCostCenters();
            self.GetAllProducts();
            self.GetAllBranches();
        }
    }
    var lbVM = new LeaderBoardViewModel();
    lbVM.Initialize();
    ko.applyBindings(lbVM, document.getElementById("LeaderBoardVW")[0]);
})
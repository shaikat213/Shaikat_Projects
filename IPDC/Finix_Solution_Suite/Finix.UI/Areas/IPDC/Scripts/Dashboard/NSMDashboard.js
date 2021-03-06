﻿var dashboard;
function numberFormatter(value) {
    if (value > 999999)
        return (value / 1000000).toFixed(1) + ' mln';
    else if (value > 999)
        return (value / 1000).toFixed(1) + ' k';
    else
        return value;

}

$(document).ready(function () {
    $('#branchSelection1,#branchSelection2,#branchSelection3,#branchSelection4,#productSelection1').each(function () {
        $(this).multiselect({
            buttonText: function (options, select) {
                if (options.length === 0) {
                    return 'None selected ...';
                }
                else if (options.length > 1) {
                    return 'More than 1 selected!';
                }
                else {
                    var labels = [];
                    options.each(function () {
                        if ($(this).attr('label') !== undefined) {
                            labels.push($(this).attr('label'));
                        }
                        else {
                            labels.push($(this).html());
                        }
                    });
                    return labels.join(', ') + '';
                }
            }
        });
    });

    function nsmDashboardVM() {
        var self = this;

        var brLiability = {};
        var brAsset = {};

        self.FromDate = ko.observable();
        self.FromDateText = ko.observable('');

        self.ToDate = ko.observable();
        self.ToDateText = ko.observable();

        // Call
        self.Call_HomeLoan = ko.observable();
        self.Call_HomeLoanToDay = ko.observable();

        self.TodayHomeLoanAmount = ko.observable();

        self.Call_PersonalLoan = ko.observable();
        self.Call_PersonalLoanToDay = ko.observable();

        self.TodayPersonalLoanAmount = ko.observable();

        self.Call_AutoLoan = ko.observable();
        self.Call_AutoLoanToDay = ko.observable();

        self.TodayAutoLoanAmount = ko.observable();

        self.Call_FixedDeposit = ko.observable();
        self.Call_FixedDepositToDay = ko.observable();

        self.TodayFixedDepositAmount = ko.observable();

        self.Call_RecurrentDeposit = ko.observable();
        self.Call_RecurrentDepositToDay = ko.observable();

        self.TodayRecurrentDepositAmount = ko.observable();

        self.Call_Undefined = ko.observable();
        self.Call_UndefinedToDay = ko.observable();

        self.TodayUndefinedAmount = ko.observable();

        self.Call_Liability = ko.observable();

        // Call

        //Lead

        self.Lead_HomeLoan = ko.observable();
        self.Lead_HomeLoanToDay = ko.observable();

        self.TodayLeadHomeLoanAmount = ko.observable();

        self.Lead_PersonalLoan = ko.observable();
        self.Lead_PersonalLoanToDay = ko.observable();

        self.TodayLeadPersonalLoanAmount = ko.observable();

        self.Lead_AutoLoan = ko.observable();
        self.Lead_AutoLoanToDay = ko.observable();

        self.TodayLeadAutoLoanAmount = ko.observable();

        self.Lead_FixedDeposit = ko.observable();
        self.Lead_FixedDepositToDay = ko.observable();

        self.TodayLeadFixedDepositAmount = ko.observable();

        self.Lead_RecurrentDeposit = ko.observable();
        self.Lead_RecurrentDepositToDay = ko.observable();

        self.TodayLeadRecurrentDepositAmount = ko.observable();

        self.Lead_Liability = ko.observable();

        // Lead

        // File Submited Apps
        self.Application_HomeLoan = ko.observable();
        self.Application_HomeLoanToDay = ko.observable();

        self.TodayApplicationHomeLoanAmount = ko.observable();

        self.Application_PersonalLoan = ko.observable();
        self.Application_PersonalLoanToDay = ko.observable();

        self.TodayApplicationPersonalLoanAmount = ko.observable();

        self.Application_AutoLoan = ko.observable();
        self.Application_AutoLoanToDay = ko.observable();

        self.TodayApplicationAutoLoanAmount = ko.observable();

        self.Application_FixedDeposit = ko.observable();
        self.Application_FixedDepositToDay = ko.observable();

        self.TodayApplicationFixedDepositAmount = ko.observable();

        self.Application_RecurrentDeposit = ko.observable();
        self.Application_RecurrentDepositToDay = ko.observable();

        self.TodayApplicationRecurrentDepositAmount = ko.observable();

        self.Application_Liability = ko.observable();

        // File Submited Apps

        // File Approved Apps

        self.Approved_HomeLoan = ko.observable();
        self.Approved_HomeLoanToDay = ko.observable();

        self.Approved_HomeLoanAmount = ko.observable();
        self.TodayApprovedApplicationHomeLoanAmount = ko.observable();

        self.Approved_AutoLoan = ko.observable();
        self.Approved_AutoLoanToDay = ko.observable();

        self.Approved_AutoLoanAmount = ko.observable();
        self.TodayApprovedApplicationAutoLoanAmount = ko.observable();

        self.Approved_PersonalLoan = ko.observable();
        self.Approved_PersonalLoanToDay = ko.observable();

        self.Approved_PersonalLoanAmount = ko.observable();
        self.TodayApprovedApplicationPersonalLoanAmount = ko.observable();

        // File Approved Apps

        // File DisApproved Apps

        self.Disapproved_HomeLoan = ko.observable();
        self.DisApproved_HomeLoanToDay = ko.observable();

        self.Disapproved_HomeLoanAmount = ko.observable();
        self.TodayDisApprovedApplicationHomeLoanAmount = ko.observable();

        self.Disapproved_AutoLoan = ko.observable();
        self.DisApproved_AutoLoanToDay = ko.observable();

        self.Disapproved_AutoLoanAmount = ko.observable();
        self.TodayDisApprovedApplicationAutoLoanAmount = ko.observable();

        self.Disapproved_PersonalLoan = ko.observable();
        self.DisApproved_PersonalLoanToDay = ko.observable();

        self.Disapproved_PersonalLoanAmount = ko.observable();
        self.TodayDisApprovedApplicationPersonalLoanAmount = ko.observable();

        // File DisApproved Apps

        // Disbursed/Received Apps
        self.Disbursed_HomeLoanToDay = ko.observable();

        self.Disbursed_AutoLoanToDay = ko.observable();

        self.Disbursed_PersonalLoanToDay = ko.observable();

        self.Received_FixedToDay = ko.observable();

        self.Received_RecurrentToDay = ko.observable();

        self.Disbursed_HomeLoanAmount = ko.observable();
        self.Disbursed_HomeLoanAmountToday = ko.observable();
        self.Disbursed_HomeLoanAmountMTD = ko.observable();
        self.Disbursed_HomeLoanAmountLMTD = ko.observable();

        self.HomeLoanWarMTD = ko.observable();
        self.HomeLoanWarLMTD = ko.observable();

        self.OverallLoanCountMTD = ko.observable();
        self.OverallLoanAmountMTD = ko.observable();

        self.Disbursed_AutoLoanAmount = ko.observable();
        self.Disbursed_AutoLoanAmountToday = ko.observable();
        self.Disbursed_AutoLoanAmountMTD = ko.observable();
        self.Disbursed_AutoLoanAmountLMTD = ko.observable();

        self.AutoLoanWarMTD = ko.observable();
        self.AutoLoanWarLMTD = ko.observable();

        self.OverallLoanCountLMTD = ko.observable();
        self.OverallLoanAmountLMTD = ko.observable();

        self.Disbursed_PersonalLoanAmount = ko.observable();
        self.Disbursed_PersonalLoanAmountToday = ko.observable();
        self.Disbursed_PersonalLoanAmountMTD = ko.observable();
        self.Disbursed_PersonalLoanAmountLMTD = ko.observable();

        self.PersonalLoanWarMTD = ko.observable();
        self.PersonalLoanWarLMTD = ko.observable();

        self.FundReceived_FixedAmountToday = ko.observable();
        self.FundReceived_FixedAmountMTD = ko.observable();
        self.FundReceived_FixedAmountLMTD = ko.observable();

        self.FixedDepositWarMTD = ko.observable();
        self.FixedDepositWarLMTD = ko.observable();

        self.OverallDepositCountMTD = ko.observable();
        self.OverallDepositAmountMTD = ko.observable();

        self.FundReceived_RecurrentAmountToday = ko.observable();
        self.FundReceived_RecurrentAmountMTD = ko.observable();
        self.FundReceived_RecurrentAmountLMTD = ko.observable();

        self.ReccurentDepositWarMTD = ko.observable();
        self.ReccurentDepositWarLMTD = ko.observable();

        self.OverallDepositCountLMTD = ko.observable();
        self.OverallDepositAmountLMTD = ko.observable();

        self.ReceivedDepositAmount = ko.observable();

        self.Disbursed_HomeLoanMTD = ko.observable();

        self.Disbursed_AutoLoanMTD = ko.observable();

        self.Disbursed_PersonalLoanMTD = ko.observable();

        self.Received_FixedMTD = ko.observable();

        self.Received_RecurrentMTD = ko.observable();

        self.Disbursed_HomeLoanLMTD = ko.observable();

        self.Disbursed_AutoLoanLMTD = ko.observable();

        self.Disbursed_PersonalLoanLMTD = ko.observable();

        self.Received_FixedLMTD = ko.observable();

        self.Received_RecurrentLMTD = ko.observable();

        // Disbursed/Received Apps

        self.ProductId = ko.observable();
        self.Products = ko.observableArray([]);

        ///
        self.RMHomeLoan1 = ko.observable();
        self.RMHomeLoan1Amount = ko.observable();
        self.RMHomeLoan1Count = ko.observable();
        self.RMHomeLoan2 = ko.observable();
        self.RMHomeLoan2Amount = ko.observable();
        self.RMHomeLoan2Count = ko.observable();
        self.RMPersonalLoan = ko.observable();
        self.RMPersonalLoanAmount = ko.observable();
        self.RMPersonalLoanCount = ko.observable();
        self.RMAutoLoan = ko.observable();
        self.RMAutoLoanAmount = ko.observable();
        self.RMAutoLoanCount = ko.observable();
        self.RMLiability1 = ko.observable();
        self.RMLiability1Amount = ko.observable();
        self.RMLiability1Count = ko.observable();
        self.RMLiability2 = ko.observable();
        self.RMLiability2Amount = ko.observable();
        self.RMLiability2Count = ko.observable();

        ///

        self.AssetCount = ko.observable();
        self.LiabilityCount = ko.observable();
        ///

        self.LoadDisbAppsbyBranch = ko.observableArray([]);

        self.BusinessConAsset = ko.observableArray([]);
        self.BusinessConLiability = ko.observableArray([]);

        self.BranchIds = ko.observableArray([]);
        self.RightBranchIds = ko.observableArray([]);
        self.DisbBranchIds = ko.observableArray([]);
        self.DepoBranchIds = ko.observableArray([]);
        self.HighlightBranchIds = ko.observableArray([]);
        self.BranchName = ko.observable();

        self.BranchList = ko.observableArray([]);

        self.BranchListRight = ko.observableArray([]);

        self.CostCenterId = ko.observable();
        self.CostCenterList = ko.observableArray([]);

        self.OverallLoanWarMTd = ko.observable();
        self.OverallLoanWarLMTd = ko.observable();

        self.OverallDepositWarMTd = ko.observable();
        self.OverallDepositWarLMTd = ko.observable();
        self.BusinessProductId = ko.observable();
        self.AprvlProductId = ko.observable();
        self.AgingProductId = ko.observable();
        self.ApprovalRatioData = ko.observableArray([]);
        self.BusinessContribution = ko.observableArray([]);
        self.AgingData = ko.observableArray([]);
        self.ProductTypes = ko.observableArray([]);
        self.ProductType = ko.observable();

        self.GetAgingByProduct = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Dashboard/GetAgingForNSM?productId=' + self.AgingProductId(),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.AgingData(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.GetAprvlDataByProduct = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Dashboard/GetApprovalRatioNSM?productId=' + self.AprvlProductId(),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ApprovalRatioData(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetBusContDataByProduct = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Dashboard/GetFileApprovedForNSM?productId=' + self.BusinessProductId(),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.BusinessContribution(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.Criteria = ko.observableArray([
            { "Name": "Number", "Id": 1 },
            { "Name": "Amount", "Id": 2 }
        ]);
        self.Satages = ko.observableArray([
            { "Name": "Call", "Id": 1 },
            { "Name": "Lead", "Id": 2 },
            { "Name": "Files Submitted", "Id": 3 },
            { "Name": "Files Approved", "Id": 4 },
            { "Name": "Files Disbursed", "Id": 5 }
        ]);
        self.TimeLines = ko.observableArray([
            { "Name": "Today", "Id": 1 },
            { "Name": "Yesterday", "Id": 2 },
            { "Name": "MTD", "Id": 3 },
            { "Name": "LMTD", "Id": 4 },
            { "Name": "YTD", "Id": 5 },
            { "Name": "LYTD", "Id": 6 },
            { "Name": "QTD", "Id": 7 },
            { "Name": "LYQTD", "Id": 8 }
        ]);

        self.ProductSelection = ko.observableArray([
            { "Name": "Home Loan", "Id": 1 },
            { "Name": "Auto Loan", "Id": 2 },
            { "Name": "Personal Loan", "Id": 3 },
            { "Name": "Fixed Deposit", "Id": 4 },
            { "Name": "Recurrent Deposit", "Id": 5 }
        ]);

        self.TimeLinesRight = ko.observableArray([
            { "Name": "Today", "Id": 1 },
            { "Name": "Yesterday", "Id": 2 },
            { "Name": "MTD", "Id": 3 },
            { "Name": "LMTD", "Id": 4 },
            { "Name": "YTD", "Id": 5 },
            { "Name": "LYTD", "Id": 6 },
            { "Name": "QTD", "Id": 7 },
            { "Name": "LYQTD", "Id": 8 }

        ]);

        self.GetBranches = function () {
            //self.ToEmpList([]);
            return $.ajax({
                type: "GET",
                url: '/IPDC/Office/GetOfficeByLayer?officelayerid=2',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.BranchList(data);
                    self.BranchListRight(data);
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
        self.FileSubmission = ko.observableArray([]);
        self.GetFileSubmissionDataByProduct = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Dashboard/GetFileSubmissionForNSM?productId=' + self.ProductId(),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.FileSubmission(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.SelectedTimeLine = ko.observable(1);
        self.SelectedTimeLineRight = ko.observable(1);

        self.SelectedCriteria = ko.observable(1);
        self.SelectedStage = ko.observable(1);
        self.SelectedProduct = ko.observableArray([1]);
        self.ProductsByType = ko.observableArray([]);
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
        self.GetProductsByProductType = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetProductByType?typeId=' + self.ProductType(),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ProductsByType(data); //Put the response in ObservableArray
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

        self.GetHighlightsNSMTL = function () {
            var submitData = {
                timeLine: self.SelectedTimeLine(),
                branchId: self.BranchIds()
            };
            return $.ajax({
                type: "POST",
                url: '/IPDC/Dashboard/GetNSMTLDashboardHighlights',
                contentType: "application/json;",
                data: ko.toJSON(submitData),
                dataType: "json",
                success: function (data) {
                    self.Call_HomeLoan(data.Call_HomeLoan);
                    self.Call_PersonalLoan(data.Call_PersonalLoan);
                    self.Call_AutoLoan(data.Call_AutoLoan);
                    self.Call_Liability(data.Call_Liability);
                    self.Call_Undefined(data.Call_Undefined);

                    self.Lead_HomeLoan(data.Lead_HomeLoan);
                    self.Lead_PersonalLoan(data.Lead_PersonalLoan);
                    self.Lead_AutoLoan(data.Lead_AutoLoan);
                    self.Lead_Liability(data.Lead_Liability);

                    self.Application_HomeLoan(data.Application_HomeLoan);
                    self.Application_PersonalLoan(data.Application_PersonalLoan);
                    self.Application_AutoLoan(data.Application_AutoLoan);
                    self.Application_Liability(data.Application_Liability);

                    self.Approved_HomeLoan(data.Approved_HomeLoan);
                    self.Approved_PersonalLoan(data.Approved_PersonalLoan);
                    self.Approved_AutoLoan(data.Approved_AutoLoan);
                    self.Approved_HomeLoanAmount(numberFormatter(data.Approved_HomeLoanAmount));
                    self.Approved_PersonalLoanAmount(numberFormatter(data.Approved_PersonalLoanAmount));
                    self.Approved_AutoLoanAmount(numberFormatter(data.Approved_AutoLoanAmount));
                    self.Disapproved_HomeLoan(data.Disapproved_HomeLoan);
                    self.Disapproved_PersonalLoan(data.Disapproved_PersonalLoan);
                    self.Disapproved_AutoLoan(data.Disapproved_AutoLoan);
                    self.Disapproved_HomeLoanAmount(numberFormatter(data.Disapproved_HomeLoanAmount));
                    self.Disapproved_PersonalLoanAmount(numberFormatter(data.Disapproved_PersonalLoanAmount));
                    self.Disapproved_AutoLoanAmount(numberFormatter(data.Disapproved_AutoLoanAmount));

                    self.Disbursed_HomeLoanAmount(numberFormatter(data.Disbursed_HomeLoanAmount));
                    self.Disbursed_PersonalLoanAmount(numberFormatter(data.Disbursed_PersonalLoanAmount));
                    self.Disbursed_AutoLoanAmount(numberFormatter(data.Disbursed_AutoLoanAmount));
                    self.ReceivedDepositAmount(numberFormatter(data.ReceivedDepositAmount));

                    self.RMHomeLoan1(data.RMHomeLoan1);
                    self.RMHomeLoan1Amount(numberFormatter(data.RMHomeLoan1Amount));
                    self.RMHomeLoan1Count(numberFormatter(data.RMHomeLoan1Count));
                    self.RMHomeLoan2(data.RMHomeLoan2);
                    self.RMHomeLoan2Amount(numberFormatter(data.RMHomeLoan2Amount));
                    self.RMHomeLoan2Count(numberFormatter(data.RMHomeLoan2Count));
                    self.RMPersonalLoan(data.RMPersonalLoan);
                    self.RMPersonalLoanAmount(numberFormatter(data.RMPersonalLoanAmount));
                    self.RMPersonalLoanCount(numberFormatter(data.RMPersonalLoanCount));
                    self.RMAutoLoan(data.RMAutoLoan);
                    self.RMAutoLoanAmount(numberFormatter(data.RMAutoLoanAmount));
                    self.RMAutoLoanCount(numberFormatter(data.RMAutoLoanCount));
                    self.RMLiability1(data.RMLiability1);
                    self.RMLiability1Amount(numberFormatter(data.RMLiability1Amount));
                    self.RMLiability1Count(numberFormatter(data.RMLiability1Count));
                    self.RMLiability2(data.RMLiability2);
                    self.RMLiability2Amount(numberFormatter(data.RMLiability2Amount));
                    self.RMLiability2Count(numberFormatter(data.RMLiability2Count));
                    //self.GetActivitySummaryOfTl();
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.GetHighlightsRight = function () {
            var submitData2 = {
                timeLine: self.SelectedTimeLineRight(),
                branchId: self.RightBranchIds()
            };
            
            return $.ajax({
                type: "POST",
                url: '/IPDC/Dashboard/GetMDDashboardHighlightsRight',
                contentType: "application/json;",
                data: ko.toJSON(submitData2),
                dataType: "json",
                success: function (data) {
                    // Disbursed/Received Apps

                    self.Disbursed_HomeLoanToDay(data.Disbursed_HomeLoanToDay);
                    self.Disbursed_AutoLoanToDay(data.Disbursed_AutoLoanToDay);
                    self.Disbursed_PersonalLoanToDay(data.Disbursed_PersonalLoanToDay);

                    self.Received_FixedToDay(data.Received_FixedToDay);

                    self.Received_RecurrentToDay(data.Received_RecurrentToDay);

                    self.Disbursed_HomeLoanAmount(data.Disbursed_HomeLoanAmount);
                    self.Disbursed_HomeLoanAmountToday(data.Disbursed_HomeLoanAmountToday);

                    self.Disbursed_AutoLoanAmount(data.Disbursed_AutoLoanAmount);
                    self.Disbursed_AutoLoanAmountToday(data.Disbursed_AutoLoanAmountToday);

                    self.Disbursed_PersonalLoanAmount(data.Disbursed_PersonalLoanAmount);
                    self.Disbursed_PersonalLoanAmountToday(data.Disbursed_PersonalLoanAmountToday);

                    self.ReceivedDepositAmount(data.ReceivedDepositAmount);

                    self.FundReceived_FixedAmountToday(data.FundReceived_FixedAmountToday);
                    self.FundReceived_RecurrentAmountToday(data.FundReceived_RecurrentAmountToday);

                    // Disbursed/Received Apps

                    self.AssetCount(data.AssetCount);
                    self.LiabilityCount(data.LiabilityCount);

                    google.charts.setOnLoadCallback(drawChartBusiness);

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.GetHighlightsRightBranch = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Dashboard/GetMDDashboardHighlightsRightBranch?timeLine=' + self.SelectedTimeLineRight(),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    // Disbursed/Received Apps

                    self.Disbursed_HomeLoanToDay(data.Disbursed_HomeLoanToDay);
                    self.Disbursed_AutoLoanToDay(data.Disbursed_AutoLoanToDay);
                    self.Disbursed_PersonalLoanToDay(data.Disbursed_PersonalLoanToDay);

                    self.Received_FixedToDay(data.Received_FixedToDay);

                    self.Received_RecurrentToDay(data.Received_RecurrentToDay);

                    self.Disbursed_HomeLoanAmount(data.Disbursed_HomeLoanAmount);
                    self.Disbursed_HomeLoanAmountToday(data.Disbursed_HomeLoanAmountToday);

                    self.Disbursed_AutoLoanAmount(data.Disbursed_AutoLoanAmount);
                    self.Disbursed_AutoLoanAmountToday(data.Disbursed_AutoLoanAmountToday);

                    self.Disbursed_PersonalLoanAmount(data.Disbursed_PersonalLoanAmount);
                    self.Disbursed_PersonalLoanAmountToday(data.Disbursed_PersonalLoanAmountToday);

                    self.ReceivedDepositAmount(data.ReceivedDepositAmount);

                    self.FundReceived_FixedAmountToday(data.FundReceived_FixedAmountToday);
                    self.FundReceived_RecurrentAmountToday(data.FundReceived_RecurrentAmountToday);

                    // Disbursed/Received Apps

                    self.LiabilityCount(data.LiabilityCount);
                    self.AssetCount(data.AssetCount);

                    self.BusinessConAsset(data.BusinessConAsset);
                    self.BusinessConLiability(data.BusinessConLiability);

                    var assetbranchNames = [];
                    var assetaxisValues = [];
                    var assetCount = [];

                    var liabilitybranchNames = [];
                    var liabilityValues = [];
                    var libCount = [];


                    var totalAsset = 0;
                    var totalLiability = 0;



                    $.each(data.BusinessConAsset, function (index, value) {
                        assetbranchNames.push(value.BranchName);
                        assetaxisValues.push(value.Amount);
                        assetCount.push(value.Count);
                        totalAsset += value.Amount;
                    });

                    totalAsset = numberFormatter(totalAsset);

                    brAsset.xAxis = {
                        categories: assetbranchNames,
                        title: {
                            text: null
                        }
                    };
                    brAsset.series = [
                        {
                            name: 'BUSINESS CONTRIBUTION ASSET' + '(TOTAL ' + totalAsset + ')',
                            data: assetaxisValues
                        }];

                    $.each(data.BusinessConLiability, function (index, value) {
                        liabilitybranchNames.push(value.BranchName);
                        liabilityValues.push(value.Amount);
                        libCount.push(value.Count);
                        totalLiability += value.Amount;
                    });

                    totalLiability = numberFormatter(totalLiability);

                    brLiability.xAxis = {
                        categories: liabilitybranchNames,
                        title: {
                            text: null
                        }
                    };
                    brLiability.series = [
                       {
                           name: 'BUSINESS CONTRIBUTION LIABILITY' + '(TOTAL ' + totalLiability + ')',
                           data: liabilityValues
                       }];


                    $('#containerf').highcharts(brAsset);
                    $('#containerG').highcharts(brLiability);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }

            });
        }

        self.CustomFunction = function () {
            self.GetHighlightsRight();
            self.GetHighlightsRightBranch();
        }
        self.ChartData = ko.observableArray([]);
        self.GetHighlights = function () {
            var submitDataHighlights = {
                fromdate: moment(self.FromDate()).format('MMM/YYYY'),
                todate: moment(self.ToDate()).format('MMM/YYYY'),
                stage:self.SelectedStage(),
                criteria:self.SelectedCriteria(),
                costCenterId:self.CostCenterId(),
                product:self.SelectedProduct(),
                branchId:self.HighlightBranchIds()
            };
            
            return $.ajax({
                type: "POST",
                url: '/IPDC/Dashboard/GetPMDashboard',
                contentType: "application/json;",
                data: ko.toJSON(submitDataHighlights),
                dataType: "json",
                success: function (data) {
                    // Call

                    self.Call_HomeLoan(data.Call_HomeLoan);
                    self.Call_HomeLoanToDay(data.Call_HomeLoanToDay);
                    
                    self.ChartData(data.ChartData);

                    self.TodayHomeLoanAmount(data.TodayHomeLoanAmount);
                    
                    self.Call_PersonalLoan(data.Call_PersonalLoan);
                    self.Call_PersonalLoanToDay(data.Call_PersonalLoanToDay);
                    
                    self.TodayPersonalLoanAmount(data.TodayPersonalLoanAmount);
                    
                    self.Call_AutoLoan(data.Call_AutoLoan);
                    self.Call_AutoLoanToDay(data.Call_AutoLoanToDay);


                    self.TodayAutoLoanAmount(data.TodayAutoLoanAmount);


                    self.Call_FixedDeposit(data.Call_FixedDeposit);
                    self.Call_FixedDepositToDay(data.Call_FixedDepositToDay);


                    self.TodayFixedDepositAmount(data.TodayFixedDepositAmount);


                    self.Call_RecurrentDeposit(data.Call_RecurrentDeposit);
                    self.Call_RecurrentDepositToDay(data.Call_RecurrentDepositToDay);


                    self.TodayRecurrentDepositAmount(data.TodayRecurrentDepositAmount);


                    self.Call_Undefined(data.Call_Undefined);
                    self.Call_UndefinedToDay(data.Call_UndefinedToDay);


                    self.TodayUndefinedAmount(data.TodayUndefinedAmount);


                    self.Call_Liability(data.Call_Liability);

                    // Call
                    // Lead
                    self.Lead_HomeLoan(data.Leadl_HomeLoan);
                    self.Lead_HomeLoanToDay(data.Lead_HomeLoanToDay);


                    self.TodayLeadHomeLoanAmount(data.TodayLeadHomeLoanAmount);


                    self.Lead_PersonalLoan(data.Lead_PersonalLoan);
                    self.Lead_PersonalLoanToDay(data.Lead_PersonalLoanToDay);


                    self.TodayLeadPersonalLoanAmount(data.TodayLeadPersonalLoanAmount);


                    self.Lead_AutoLoan(data.Lead_AutoLoan);
                    self.Lead_AutoLoanToDay(data.Lead_AutoLoanToDay);


                    self.TodayLeadAutoLoanAmount(data.TodayLeadAutoLoanAmount);


                    self.Lead_FixedDeposit(data.Lead_FixedDeposit);
                    self.Lead_FixedDepositToDay(data.Lead_FixedDepositToDay);


                    self.TodayLeadFixedDepositAmount(data.TodayLeadFixedDepositAmount);


                    self.Lead_RecurrentDeposit(data.Lead_RecurrentDeposit);
                    self.Lead_RecurrentDepositToDay(data.Lead_RecurrentDepositToDay);


                    self.TodayLeadRecurrentDepositAmount(data.TodayLeadRecurrentDepositAmount);


                    self.Lead_Liability(data.Lead_Liability);
                    // Lead

                    // File Submited Apps
                    self.Application_HomeLoan(data.Application_HomeLoan);
                    self.Application_HomeLoanToDay(data.Application_HomeLoanToDay);


                    self.TodayApplicationHomeLoanAmount(data.TodayApplicationHomeLoanAmount);


                    self.Application_PersonalLoan(data.Application_PersonalLoan);
                    self.Application_PersonalLoanToDay(data.Application_PersonalLoanToDay);


                    self.TodayApplicationPersonalLoanAmount(data.TodayApplicationPersonalLoanAmount);


                    self.Application_AutoLoan(data.Application_AutoLoan);
                    self.Application_AutoLoanToDay(data.Application_AutoLoanToDay);


                    self.TodayApplicationAutoLoanAmount(data.TodayApplicationAutoLoanAmount);


                    self.Application_FixedDeposit(data.Application_FixedDeposit);
                    self.Application_FixedDepositToDay(data.Application_FixedDepositToDay);


                    self.TodayApplicationFixedDepositAmount(data.TodayApplicationFixedDepositAmount);


                    self.Application_RecurrentDeposit(data.Application_RecurrentDeposit);
                    self.Application_RecurrentDepositToDay(data.Application_RecurrentDepositToDay);


                    self.TodayApplicationRecurrentDepositAmount(data.TodayApplicationRecurrentDepositAmount);


                    self.Application_Liability(data.Application_Liability);

                    // File Submited Apps

                    // File Approved Apps
                    self.Approved_HomeLoan(data.Approved_HomeLoan);
                    self.Approved_HomeLoanToDay(data.Approved_HomeLoanToDay);


                    self.Approved_HomeLoanAmount(numberFormatter(data.Approved_HomeLoanAmount));
                    self.TodayApprovedApplicationHomeLoanAmount(data.TodayApprovedApplicationHomeLoanAmount);


                    self.Approved_AutoLoan(data.Approved_AutoLoan);
                    self.Approved_AutoLoanToDay(data.Approved_AutoLoanToDay);


                    self.Approved_AutoLoanAmount(numberFormatter(data.Approved_AutoLoanAmount));
                    self.TodayApprovedApplicationAutoLoanAmount(data.TodayApprovedApplicationAutoLoanAmount);


                    self.Approved_PersonalLoan(data.Approved_PersonalLoan);
                    self.Approved_PersonalLoanToDay(data.Approved_PersonalLoanToDay);


                    self.Approved_PersonalLoanAmount(numberFormatter(data.Approved_PersonalLoanAmount));
                    self.TodayApprovedApplicationPersonalLoanAmount(data.TodayApprovedApplicationPersonalLoanAmount);

                    // File Approved Apps

                    // File DisApproved Apps
                    self.Disapproved_HomeLoan(data.Disapproved_HomeLoan);
                    self.DisApproved_HomeLoanToDay(data.DisApproved_HomeLoanToDay);


                    self.Disapproved_HomeLoanAmount(numberFormatter(data.Disapproved_HomeLoanAmount));
                    self.TodayDisApprovedApplicationHomeLoanAmount(data.TodayDisApprovedApplicationHomeLoanAmount);


                    self.Disapproved_AutoLoan(data.Disapproved_AutoLoan);
                    self.DisApproved_AutoLoanToDay(data.DisApproved_AutoLoanToDay);


                    self.Disapproved_AutoLoanAmount(numberFormatter(data.Disapproved_AutoLoanAmount));
                    self.TodayDisApprovedApplicationAutoLoanAmount(data.TodayDisApprovedApplicationAutoLoanAmount);


                    self.Disapproved_PersonalLoan(data.Disapproved_PersonalLoan);
                    self.DisApproved_PersonalLoanToDay(data.DisApproved_PersonalLoanToDay);


                    self.Disapproved_PersonalLoanAmount(numberFormatter(data.Disapproved_PersonalLoanAmount));
                    self.TodayDisApprovedApplicationPersonalLoanAmount(data.TodayDisApprovedApplicationPersonalLoanAmount);

                    // File DisApproved Apps

                    // Disbursed/Received Apps


                    self.Disbursed_HomeLoanToDay(data.Disbursed_HomeLoanToDay);


                    self.Disbursed_AutoLoanToDay(data.Disbursed_AutoLoanToDay);


                    self.Disbursed_PersonalLoanToDay(data.Disbursed_PersonalLoanToDay);


                    self.Received_FixedToDay(data.Received_FixedToDay);


                    self.Received_RecurrentToDay(data.Received_RecurrentToDay);


                    self.Disbursed_HomeLoanAmount(data.Disbursed_HomeLoanAmount);
                    self.Disbursed_HomeLoanAmountToday(data.Disbursed_HomeLoanAmountToday);


                    self.Disbursed_AutoLoanAmount(data.Disbursed_AutoLoanAmount);
                    self.Disbursed_AutoLoanAmountToday(data.Disbursed_AutoLoanAmountToday);


                    self.Disbursed_PersonalLoanAmount(data.Disbursed_PersonalLoanAmount);
                    self.Disbursed_PersonalLoanAmountToday(data.Disbursed_PersonalLoanAmountToday);


                    self.ReceivedDepositAmount(data.ReceivedDepositAmount);

                    self.FundReceived_FixedAmountToday(data.FundReceived_FixedAmountToday);


                    self.FundReceived_RecurrentAmountToday(data.FundReceived_RecurrentAmountToday);


                    // Disbursed/Received Apps

                    //
                    //console.log('count call - ' + ko.toJSON(self.totalYesterdayCall));
                    self.totalYesterdayCall = self.Call_HomeLoanToDay() + self.Call_AutoLoanToDay() + self.Call_PersonalLoanToDay() + self.Call_FixedDepositToDay() + self.Call_RecurrentDepositToDay() + self.Call_UndefinedToDay();
                    //console.log('count lead -' + ko.toJSON(self.totalYesterdayLead));
                    self.totalYesterdayLead = self.Lead_HomeLoanToDay() + self.Lead_AutoLoanToDay() + self.Lead_PersonalLoanToDay() + self.Lead_FixedDepositToDay() + self.Lead_RecurrentDepositToDay();

                    self.totalYesterdaySubmit = self.Application_HomeLoanToDay() + self.Application_AutoLoanToDay() + self.Application_PersonalLoanToDay() + self.Application_FixedDepositToDay() + self.Application_RecurrentDepositToDay();

                    self.totalYesterdayApproved = self.Approved_HomeLoanToDay() + self.Approved_AutoLoanToDay() + self.Approved_PersonalLoanToDay();

                    self.totalYesterdaydisApproved = self.DisApproved_HomeLoanToDay() + self.DisApproved_AutoLoanToDay() + self.DisApproved_PersonalLoanToDay();

                    self.totalYesterdaydisbursedApproved = self.Disbursed_HomeLoanToDay() + self.Disbursed_AutoLoanToDay() + self.Disbursed_PersonalLoanToDay() + self.Received_FixedToDay() + self.Received_RecurrentToDay();
                    
                    google.charts.setOnLoadCallback(drawChartProductivity);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.GetProdDisbByBranch = function () {
            var submitData3 = {
                timeLine: 3,
                branchId: self.DisbBranchIds()
            };
            
            $.ajax({
                type: "POST",
                url: '/IPDC/Dashboard/GetNSMDisiburesedReceivedMTD',
                contentType: "application/json;",
                data: ko.toJSON(submitData3),
                dataType: "json",
                success: function (data) {
                    self.Disbursed_HomeLoanMTD(data.Disbursed_HomeLoanMTD);
                    self.Disbursed_AutoLoanMTD(data.Disbursed_AutoLoanMTD);
                    self.Disbursed_PersonalLoanMTD(data.Disbursed_PersonalLoanMTD);

                    self.OverallLoanCountMTD(self.Disbursed_HomeLoanMTD() + self.Disbursed_AutoLoanMTD() + self.Disbursed_PersonalLoanMTD());

                    self.Disbursed_HomeLoanAmountMTD(data.Disbursed_HomeLoanAmountMTD);
                    self.Disbursed_AutoLoanAmountMTD(data.Disbursed_AutoLoanAmountMTD);
                    self.Disbursed_PersonalLoanAmountMTD(data.Disbursed_PersonalLoanAmountMTD);

                    self.OverallLoanAmountMTD(self.Disbursed_HomeLoanAmountMTD() + self.Disbursed_AutoLoanAmountMTD() + self.Disbursed_PersonalLoanAmountMTD());

                    self.HomeLoanWarMTD(data.HomeLoanWarMTD.toFixed(2));
                    self.AutoLoanWarMTD(data.AutoLoanWarMTD.toFixed(2));
                    self.PersonalLoanWarMTD(data.PersonalLoanWarMTD.toFixed(2));

                    self.OverallLoanWarMTd((((self.Disbursed_HomeLoanAmountMTD() * self.HomeLoanWarMTD())
                                        + (self.Disbursed_AutoLoanMTD() * self.AutoLoanWarMTD())
                                        + (self.Disbursed_PersonalLoanMTD() * self.PersonalLoanWarMTD())) / (self.Disbursed_HomeLoanAmountMTD()
                                        + self.Disbursed_AutoLoanAmountMTD() + self.Disbursed_PersonalLoanAmountMTD())).toFixed(2));

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
            var submitData4 = {
                timeLine: 4,
                branchId: self.RightBranchIds()
            };
            $.ajax({
                type: "POST",
                url: '/IPDC/Dashboard/GetNSMDisiburesedReceivedLMTD',
                contentType: "application/json;",
                data: ko.toJSON(submitData4),
                dataType: "json",
                success: function (data) {
                    self.Disbursed_HomeLoanLMTD(data.Disbursed_HomeLoanLMTD);
                    self.Disbursed_AutoLoanLMTD(data.Disbursed_AutoLoanLMTD);
                    self.Disbursed_PersonalLoanLMTD(data.Disbursed_PersonalLoanLMTD);

                    self.OverallLoanCountLMTD(self.Disbursed_HomeLoanLMTD() + self.Disbursed_AutoLoanLMTD() + self.Disbursed_PersonalLoanLMTD());

                    self.Disbursed_HomeLoanAmountLMTD(data.Disbursed_HomeLoanAmountLMTD);
                    self.Disbursed_AutoLoanAmountLMTD(data.Disbursed_AutoLoanAmountLMTD);
                    self.Disbursed_PersonalLoanAmountLMTD(data.Disbursed_PersonalLoanAmountLMTD);

                    self.OverallLoanAmountLMTD(self.Disbursed_HomeLoanAmountLMTD() + self.Disbursed_AutoLoanAmountLMTD() + self.Disbursed_PersonalLoanAmountLMTD());

                    self.HomeLoanWarLMTD(data.HomeLoanWarLMTD.toFixed(2));
                    self.AutoLoanWarLMTD(data.AutoLoanWarLMTD.toFixed(2));
                    self.PersonalLoanWarLMTD(data.PersonalLoanWarLMTD.toFixed(2));

                    self.OverallLoanWarLMTd((((self.Disbursed_HomeLoanAmountLMTD() * self.HomeLoanWarLMTD())
                                    + (self.Disbursed_AutoLoanLMTD() * self.AutoLoanWarLMTD())
                                    + (self.Disbursed_PersonalLoanLMTD() * self.PersonalLoanWarLMTD())) / (self.Disbursed_HomeLoanAmountLMTD()
                                    + self.Disbursed_AutoLoanAmountLMTD() + self.Disbursed_PersonalLoanAmountLMTD())).toFixed(2));


                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });

        }

        self.GrowthHomeLoanFile = ko.computed(function () {
            return (((self.Disbursed_HomeLoanMTD() - self.Disbursed_HomeLoanLMTD()) / (self.Disbursed_HomeLoanLMTD())) * 100).toFixed(2);
        });

        self.GrowthAutoLoanFile = ko.computed(function () {
            return (((self.Disbursed_AutoLoanMTD() - self.Disbursed_AutoLoanLMTD()) / (self.Disbursed_AutoLoanLMTD()) * 100)).toFixed(2);
        });

        self.GrowthPersonalLoanFile = ko.computed(function () {
            return (((self.Disbursed_PersonalLoanMTD() - self.Disbursed_PersonalLoanLMTD()) / (self.Disbursed_PersonalLoanLMTD())) * 100).toFixed(2);
        });


        self.GrowthHomeLoanAmount = ko.computed(function () {
            return (((self.Disbursed_HomeLoanAmountMTD() - self.Disbursed_HomeLoanAmountLMTD()) / (self.Disbursed_HomeLoanAmountLMTD())) * 100).toFixed(2);
        });

        self.GrowthAutoLoanAmount = ko.computed(function () {
            return (((self.Disbursed_AutoLoanAmountMTD() - self.Disbursed_AutoLoanAmountLMTD()) / (self.Disbursed_AutoLoanAmountLMTD())) * 100).toFixed(2);
        });

        self.GrowthPersonalLoanAmount = ko.computed(function () {
            return (((self.Disbursed_PersonalLoanAmountMTD() - self.Disbursed_PersonalLoanAmountLMTD()) / (self.Disbursed_PersonalLoanAmountLMTD())) * 100).toFixed(2);
        });


        self.GrowthHomeLoanWar = ko.computed(function () {
            return (((self.HomeLoanWarMTD() - self.HomeLoanWarLMTD()) / (self.HomeLoanWarLMTD())) * 100).toFixed(2);
        });

        self.GrowthAutoLoanWar = ko.computed(function () {

            return (((self.AutoLoanWarMTD() - self.AutoLoanWarLMTD()) / (self.AutoLoanWarLMTD()) * 100)).toFixed(2);
        });

        self.GrowthPersonalLoanWar = ko.computed(function () {

            return (((self.PersonalLoanWarLMTD() - self.PersonalLoanWarLMTD()) / (self.PersonalLoanWarLMTD())) * 100).toFixed(2);

        });


        self.OverallGrowthLoanFiles = ko.computed(function () {
            return (((self.OverallLoanCountMTD() - self.OverallLoanCountLMTD()) / self.OverallLoanCountLMTD()) * 100).toFixed(2);
        });

        self.OverallGrowthLoanAmount = ko.computed(function () {
            return (((self.OverallLoanAmountMTD() - self.OverallLoanAmountLMTD()) / self.OverallLoanAmountLMTD()) * 100).toFixed(2);
        });

        self.OverallGrowthLoanWar = ko.computed(function () {
            return (((self.OverallLoanWarMTd() - self.OverallLoanWarLMTd()) / self.OverallLoanWarLMTd()) * 100).toFixed(2);
        });

        self.GetProdReceivedByBranch = function () {
            var submitData3 = {
                timeLine: 3,
                branchId: self.DepoBranchIds()
            };
            
            $.ajax({
                type: "POST",
                url: '/IPDC/Dashboard/GetNSMDisiburesedReceivedMTD',
                contentType: "application/json;",
                data: ko.toJSON(submitData3),
                dataType: "json",
                success: function (data) {
                    self.Received_FixedMTD(data.Received_FixedMTD);
                    self.Received_RecurrentMTD(data.Received_RecurrentMTD);

                    self.OverallDepositCountMTD(self.Received_FixedMTD() + self.Received_RecurrentMTD());

                    self.FundReceived_FixedAmountMTD(data.FundReceived_FixedAmountMTD);
                    self.FundReceived_RecurrentAmountMTD(data.FundReceived_RecurrentAmountMTD);

                    self.OverallDepositAmountMTD(self.FundReceived_FixedAmountMTD() + self.FundReceived_RecurrentAmountMTD());

                    self.FixedDepositWarMTD(data.FixedDepositWarMTD.toFixed(2));
                    self.ReccurentDepositWarMTD(data.ReccurentDepositWarMTD.toFixed(2));

                    self.OverallDepositWarMTd((((self.FundReceived_FixedAmountMTD() * self.FixedDepositWarMTD())
                                        + (self.FundReceived_RecurrentAmountMTD() * self.ReccurentDepositWarMTD())) / (self.FundReceived_FixedAmountMTD()
                                        + self.FundReceived_RecurrentAmountMTD())).toFixed(2));


                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
            var submitData4 = {
                timeLine: 4,
                branchId: self.RightBranchIds()
            };
            $.ajax({
                type: "POST",
                url: '/IPDC/Dashboard/GetNSMDisiburesedReceivedLMTD',
                contentType: "application/json;",
                data: ko.toJSON(submitData4),
                dataType: "json",
                success: function (data) {

                    self.Received_FixedLMTD(data.Received_FixedLMTD);
                    self.Received_RecurrentLMTD(data.Received_RecurrentLMTD);

                    self.OverallDepositCountLMTD(self.Received_FixedLMTD() + self.Received_RecurrentLMTD());

                    self.FundReceived_FixedAmountLMTD(data.FundReceived_FixedAmountLMTD);
                    self.FundReceived_RecurrentAmountLMTD(data.FundReceived_RecurrentAmountLMTD);

                    self.OverallDepositAmountLMTD(self.FundReceived_FixedAmountLMTD() + self.FundReceived_RecurrentAmountLMTD());

                    self.FixedDepositWarLMTD(data.FixedDepositWarLMTD.toFixed(2));
                    self.ReccurentDepositWarLMTD(data.ReccurentDepositWarLMTD.toFixed(2));

                    self.OverallDepositWarLMTd((((self.FundReceived_FixedAmountLMTD() * self.FixedDepositWarLMTD())
                                       + (self.FundReceived_RecurrentAmountLMTD() * self.ReccurentDepositWarLMTD())) / (self.FundReceived_FixedAmountLMTD()
                                       + self.FundReceived_RecurrentAmountLMTD())).toFixed(2));

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }


        self.GrowthFixedDepositFile = ko.computed(function () {

            return (((self.Received_FixedMTD() - self.Received_FixedLMTD()) / (self.Received_FixedLMTD())) * 100);
        });

        self.GrowthRecurringFile = ko.computed(function () {

            return (((self.Received_RecurrentMTD() - self.Received_RecurrentLMTD()) / (self.Received_RecurrentLMTD())) * 100);

        });


        self.GrowthFixedDepositAmount = ko.computed(function () {

            return (((self.FundReceived_FixedAmountMTD() - self.FundReceived_FixedAmountLMTD()) / (self.FundReceived_FixedAmountLMTD())) * 100);
        });

        self.GrowthRecurringAmount = ko.computed(function () {

            return (((self.FundReceived_RecurrentAmountMTD() - self.FundReceived_RecurrentAmountLMTD()) / (self.FundReceived_RecurrentAmountLMTD())) * 100);

        });


        self.GrowthFixedDepositWar = ko.computed(function () {
            return (((self.FixedDepositWarMTD() - self.FixedDepositWarLMTD()) / (self.FixedDepositWarLMTD())) * 100).toFixed(2);
        });

        self.GrowthRecurringDepositWar = ko.computed(function () {

            return (((self.ReccurentDepositWarMTD() - self.ReccurentDepositWarLMTD()) / (self.ReccurentDepositWarLMTD()) * 100)).toFixed(2);
        });


        self.OverallGrowthDepositFiles = ko.computed(function () {
            return (((self.OverallDepositCountMTD() - self.OverallDepositCountLMTD()) / self.OverallDepositCountLMTD()) * 100).toFixed(2);
        });

        self.OverallGrowthDepositAmount = ko.computed(function () {
            return (((self.OverallDepositAmountMTD() - self.OverallDepositAmountLMTD()) / self.OverallDepositAmountLMTD()) * 100).toFixed(2);
        });

        self.OverallGrowthDepositWar = ko.computed(function () {
            return (((self.OverallDepositWarMTd() - self.OverallDepositWarLMTd()) / self.OverallDepositWarLMTd()) * 100).toFixed(2);
        });


        var chart = {
            type: 'bar'
        };
        var title = {
            text: ''

        };
        var subtitle = {
            text: ''
        };
        var yAxis = {
            min: 0,
            title: {
                text: '',
                align: 'high'
            },
            labels: {
                overflow: 'justify'
            }
        };
        var tooltip = {
            valueSuffix: '',
            headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
            pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                '<td style="padding:0"><b>{point.y:.1f} </b></td></tr>',
            footerFormat: '</table>',
            shared: true,
            useHTML: true
        };
        var plotOptions = {
            bar: {
                dataLabels: {
                    enabled: true
                }
            }
        };
        var credits = {
            enabled: false
        };

        var assetcolors = [
            '#EC017F'
        ];

        var libcolors = [
            '#979797'
        ];

        brAsset.chart = chart;
        brAsset.title = title;
        brAsset.subtitle = subtitle;
        brAsset.tooltip = tooltip;
        brAsset.yAxis = yAxis;
        brAsset.colors = assetcolors;
        brAsset.plotOptions = plotOptions;
        brAsset.credits = credits;

        brLiability.chart = chart;
        brLiability.title = title;
        brLiability.subtitle = subtitle;
        brLiability.tooltip = tooltip;
        brLiability.yAxis = yAxis;
        brLiability.colors = libcolors;
        brLiability.plotOptions = plotOptions;
        brLiability.credits = credits;

    }

    dashboard = new nsmDashboardVM();
    dashboard.GetProducts();
    dashboard.GetProductTypes();
    dashboard.GetBranches();
    dashboard.GetCostCenters();
    dashboard.GetHighlightsNSMTL();
    dashboard.GetProdDisbByBranch();
    dashboard.GetProdReceivedByBranch();
    dashboard.CustomFunction();


    ko.applyBindings(dashboard, document.getElementById('nsmDashboard'));

    //Business Overview
    function drawChartBusiness() {
        // Define the chart to be drawn.
        var data = new google.visualization.DataTable();
        data.addColumn('string', 'Browser');
        data.addColumn('number', 'Percentage');

        if (typeof (dashboard) != 'undefined') {
            data.addRows([
                ['Asset', dashboard.AssetCount()],
                ['Liability', dashboard.LiabilityCount()]

            ]);
        } else {
            data.addRows([
                ['Liability', 38],
                ['Asset', 62]
            ]);
        }

        // Set chart options
        var options = {
            'title': '',
            pieHole: 0.4,
            'colors': ['#EC017F', '#979797'],
            responsive: {
                rules: [
                    {
                        condition: {
                            maxWidth: 800
                        },
                        chartOptions: {
                            legend: {
                                align: 'center',
                                verticalAlign: 'bottom',
                                layout: 'horizontal'
                            },
                            yAxis: {
                                labels: {
                                    align: 'left',
                                    x: 0,
                                    y: -5
                                },
                                title: {
                                    text: null
                                }
                            },
                            subtitle: {
                                text: null
                            },
                            credits: {
                                enabled: false
                            }
                        }
                    }
                ]
            }
        };

        // Instantiate and draw the chart.
        var chart = new google.visualization.PieChart(document.getElementById('containerp'));
        chart.draw(data, options);
    }
    google.charts.setOnLoadCallback(drawChartBusiness);
    //Business Overview


    function drawChartProductivity(){

        var data;

        if (dashboard.SelectedStage === 1) { }

        var arrayToData = [];

        if (typeof (dashboard) != 'undefined') {
            arrayToData.push(['Month', 'No Call', 'Success %', 'Cumulative Success %']);
            var divisor = 0;
            var cumulativeTotal = 0;
                
            $.each(dashboard.ChartData(), function (index, value) {
                divisor++;
                cumulativeTotal += parseFloat(value.CompareValue);
                arrayToData.push([value.Month + " - " + value.Year, value.Value, value.CompareValue, (cumulativeTotal / divisor)]);
            })
                
            data = google.visualization.arrayToDataTable(arrayToData);
        }

        // Set chart options
        var options = {
            title: '',
            vAxis: { title: 'No Call' },
            hAxis: { title: 'Month' },
            seriesType: 'bars',
            series: {
                1: {
                    type: 'line',
                    lineWidth: 2,
                    lineDashStyle: [3]
                },
                2: {
                    type: 'line'
                }
            },
            colors: ['#7C7C7C', '#EC017F', '#005098'],
            responsive: {
                rules: [
                    {
                        condition: {
                            maxWidth: 800
                        },
                        chartOptions: {
                            legend: {
                                align: 'center',
                                verticalAlign: 'bottom',
                                layout: 'horizontal'
                            },
                            yAxis: {
                                labels: {
                                    align: 'left',
                                    x: 0,
                                    y: -5
                                },
                                title: {
                                    text: null
                                }
                            },
                            subtitle: {
                                text: null
                            },
                            credits: {
                                enabled: false
                            }
                        }
                    }
                ]
            }

        };

        // Instantiate and draw the chart.
        var chart = new google.visualization.ComboChart(document.getElementById('ProductivityMatrix'));
        if (typeof (data) != 'undefined') {
            chart.draw(data, options);
        }
    }
    //Productivity Matrix

});
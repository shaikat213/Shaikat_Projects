    var dashboard;
    function numberFormatter(value) {
        if (value > 999999)
            return (value / 1000000).toFixed(1) + ' mln';
        else if (value > 999)
            return (value / 1000).toFixed(1) + ' k';
        else
            return value;

    }

    $(document).ready(function () {
        $('#branchSelection1,#branchSelection2').each(function () {
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
        
        function mdDashboardVM() {
            var self = this;

            var call = {};
            var lead = {};
            var fileSubmit = {};
            var fileApproved = {};
            var fileDisbRec = {};

            var brLiability = {};
            var brAsset = {};

            // Call
            self.Call_HomeLoan = ko.observable();
            self.Call_HomeLoanToDay = ko.observable();
            self.Call_HomeLoanLastDay = ko.observable();

            self.TodayHomeLoanAmount = ko.observable();
            self.LastdayHomeLoanAmount = ko.observable();

            self.Call_PersonalLoan = ko.observable();
            self.Call_PersonalLoanToDay = ko.observable();
            self.Call_PersonalLoanLastDay = ko.observable();

            self.TodayPersonalLoanAmount = ko.observable();
            self.LastdayPersonalLoanAmount = ko.observable();

            self.Call_AutoLoan = ko.observable();
            self.Call_AutoLoanToDay = ko.observable();
            self.Call_AutoLoanLastDay = ko.observable();

            self.TodayAutoLoanAmount = ko.observable();
            self.LastdayAutoLoanAmount = ko.observable();

            self.Call_FixedDeposit = ko.observable();
            self.Call_FixedDepositToDay = ko.observable();
            self.Call_FixedDepositLastDay = ko.observable();

            self.TodayFixedDepositAmount = ko.observable();
            self.LastdayFixedDepositAmount = ko.observable();

            self.Call_RecurrentDeposit = ko.observable();
            self.Call_RecurrentDepositToDay = ko.observable();
            self.Call_RecurrentDepositLastDay = ko.observable();

            self.TodayRecurrentDepositAmount = ko.observable();
            self.LastdayRecurrentDepositAmount = ko.observable();

            self.Call_Undefined = ko.observable();
            self.Call_UndefinedToDay = ko.observable();
            self.Call_UndefinedLastDay = ko.observable();

            self.TodayUndefinedAmount = ko.observable();
            self.LastdayUndefinedAmount = ko.observable();

            self.Call_Liability = ko.observable();

            // Call

            //Lead

            self.Lead_HomeLoan = ko.observable();
            self.Lead_HomeLoanToDay = ko.observable();
            self.Lead_HomeLoanLastDay = ko.observable();

            self.TodayLeadHomeLoanAmount = ko.observable();
            self.LastdayLeadHomeLoanAmount = ko.observable();

            self.Lead_PersonalLoan = ko.observable();
            self.Lead_PersonalLoanToDay = ko.observable();
            self.Lead_PersonalLoanLastDay = ko.observable();

            self.TodayLeadPersonalLoanAmount = ko.observable();
            self.LastdayLeadPersonalLoanAmount = ko.observable();

            self.Lead_AutoLoan = ko.observable();
            self.Lead_AutoLoanToDay = ko.observable();
            self.Lead_AutoLoanLastDay = ko.observable();

            self.TodayLeadAutoLoanAmount = ko.observable();
            self.LastdayLeadAutoLoanAmount = ko.observable();

            self.Lead_FixedDeposit = ko.observable();
            self.Lead_FixedDepositToDay = ko.observable();
            self.Lead_FixedDepositLastDay = ko.observable();

            self.TodayLeadFixedDepositAmount = ko.observable();
            self.LastdayLeadFixedDepositAmount = ko.observable();

            self.Lead_RecurrentDeposit = ko.observable();
            self.Lead_RecurrentDepositToDay = ko.observable();
            self.Lead_RecurrentDepositLastDay = ko.observable();

            self.TodayLeadRecurrentDepositAmount = ko.observable();
            self.LastdayLeadRecurrentDepositAmount = ko.observable();

            self.Lead_Liability = ko.observable();

            // Lead

            // File Submited Apps
            self.Application_HomeLoan = ko.observable();
            self.Application_HomeLoanToDay = ko.observable();
            self.Application_HomeLoanLastDay = ko.observable();

            self.TodayApplicationHomeLoanAmount = ko.observable();
            self.LastdayApplicationHomeLoanAmount = ko.observable();

            self.Application_PersonalLoan = ko.observable();
            self.Application_PersonalLoanToDay = ko.observable();
            self.Application_PersonalLoanLastDay = ko.observable();

            self.TodayApplicationPersonalLoanAmount = ko.observable();
            self.LastdayApplicationPersonalLoanAmount = ko.observable();

            self.Application_AutoLoan = ko.observable();
            self.Application_AutoLoanToDay = ko.observable();
            self.Application_AutoLoanLastDay = ko.observable();

            self.TodayApplicationAutoLoanAmount = ko.observable();
            self.LastdayApplicationAutoLoanAmount = ko.observable();

            self.Application_FixedDeposit = ko.observable();
            self.Application_FixedDepositToDay = ko.observable();
            self.Application_FixedDepositLastDay = ko.observable();

            self.TodayApplicationFixedDepositAmount = ko.observable();
            self.LastdayApplicationFixedDepositAmount = ko.observable();

            self.Application_RecurrentDeposit = ko.observable();
            self.Application_RecurrentDepositToDay = ko.observable();
            self.Application_RecurrentDepositLastDay = ko.observable();

            self.TodayApplicationRecurrentDepositAmount = ko.observable();
            self.LastdayApplicationRecurrentDepositAmount = ko.observable();

            self.Application_Liability = ko.observable();

            // File Submited Apps

            // File Approved Apps

            self.Approved_HomeLoan = ko.observable();
            self.Approved_HomeLoanToDay = ko.observable();
            self.Approved_HomeLoanLastDay = ko.observable();

            self.Approved_HomeLoanAmount = ko.observable();
            self.TodayApprovedApplicationHomeLoanAmount = ko.observable();
            self.LastdayApprovedApplicationHomeLoanAmount = ko.observable();

            self.Approved_AutoLoan = ko.observable();
            self.Approved_AutoLoanToDay = ko.observable();
            self.Approved_AutoLoanLastDay = ko.observable();

            self.Approved_AutoLoanAmount = ko.observable();
            self.TodayApprovedApplicationAutoLoanAmount = ko.observable();
            self.LastdayApprovedApplicationAutoLoanAmount = ko.observable();

            self.Approved_PersonalLoan = ko.observable();
            self.Approved_PersonalLoanToDay = ko.observable();
            self.Approved_PersonalLoanLastDay = ko.observable();

            self.Approved_PersonalLoanAmount = ko.observable();
            self.TodayApprovedApplicationPersonalLoanAmount = ko.observable();
            self.LastdayApprovedApplicationPersonalLoanAmount = ko.observable();

            // File Approved Apps

            // File DisApproved Apps

            self.Disapproved_HomeLoan = ko.observable();
            self.DisApproved_HomeLoanToDay = ko.observable();
            self.DisApproved_HomeLoanLastDay = ko.observable();

            self.Disapproved_HomeLoanAmount = ko.observable();
            self.TodayDisApprovedApplicationHomeLoanAmount = ko.observable();
            self.LastdayDisApprovedApplicationHomeLoanAmount = ko.observable();

            self.Disapproved_AutoLoan = ko.observable();
            self.DisApproved_AutoLoanToDay = ko.observable();
            self.DisApproved_AutoLoanLastDay = ko.observable();

            self.Disapproved_AutoLoanAmount = ko.observable();
            self.TodayDisApprovedApplicationAutoLoanAmount = ko.observable();
            self.LastdayDisApprovedApplicationAutoLoanAmount = ko.observable();

            self.Disapproved_PersonalLoan = ko.observable();
            self.DisApproved_PersonalLoanToDay = ko.observable();
            self.DisApproved_PersonalLoanLastDay = ko.observable();

            self.Disapproved_PersonalLoanAmount = ko.observable();
            self.TodayDisApprovedApplicationPersonalLoanAmount = ko.observable();
            self.LastdayDisApprovedApplicationPersonalLoanAmount = ko.observable();

            // File DisApproved Apps

            // Disbursed/Received Apps
            self.Disbursed_HomeLoanToDay = ko.observable();
            self.Disbursed_HomeLoanLastDay = ko.observable();

            self.Disbursed_AutoLoanToDay = ko.observable();
            self.Disbursed_AutoLoanLastDay = ko.observable();

            self.Disbursed_PersonalLoanToDay = ko.observable();
            self.Disbursed_PersonalLoanLastDay = ko.observable();

            self.Received_FixedToDay = ko.observable();
            self.Received_FixedLastDay = ko.observable();

            self.Received_RecurrentToDay = ko.observable();
            self.Received_RecurrentLastDay = ko.observable();

            self.Disbursed_HomeLoanAmount = ko.observable();
            self.Disbursed_HomeLoanAmountToday = ko.observable();
            self.Disbursed_HomeLoanAmountLastday = ko.observable();

            self.Disbursed_AutoLoanAmount = ko.observable();
            self.Disbursed_AutoLoanAmountToday = ko.observable();
            self.Disbursed_AutoLoanAmountLastday = ko.observable();

            self.Disbursed_PersonalLoanAmount = ko.observable();
            self.Disbursed_PersonalLoanAmountToday = ko.observable();
            self.Disbursed_PersonalLoanAmountLastday = ko.observable();

            self.FundReceived_FixedAmountToday = ko.observable();
            self.FundReceived_FixedAmountLastday = ko.observable();

            self.FundReceived_RecurrentAmountToday = ko.observable();
            self.FundReceived_RecurrentAmountLastday = ko.observable();

            self.ReceivedDepositAmount = ko.observable();
            // Disbursed/Received Apps

            ///
            

            self.AssetCount = ko.observable();
            self.LiabilityCount = ko.observable();
            ///

            self.BusinessConAsset = ko.observableArray([]);
            self.BusinessConLiability = ko.observableArray([]);

            self.BranchIds = ko.observableArray([]);
            self.RightBranchIds = ko.observableArray([]);
            self.BranchName = ko.observable();

            self.BranchList = ko.observableArray([]);

            self.BranchListRight = ko.observableArray([]);

            self.TimeLines = ko.observableArray([
                { "Name": "Today - Yesterday", "Id": 1 },
                //{ "Name": "Today - Yesterday", "Id": 2 },
                { "Name": "MTD - LMTD", "Id": 3 },
                //{ "Name": "LMTD", "Id": 4 },
                { "Name": "YTD - LYTD", "Id": 5 },
                //{ "Name": "LYTD", "Id": 6 },
                { "Name": "QTD - LYQTD", "Id": 7 }
                //{ "Name": "LYQTD", "Id": 8 }

            ]);
            self.Criteria = ko.observableArray([
                { "Name": "Number", "Id": 1 },
                { "Name": "Amount", "Id": 2 }
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

            self.SelectedTimeLine = ko.observable(1);
            self.SelectedTimeLineRight = ko.observable(1);
            self.SelectedCriteria = ko.observable(1);


            self.GetHighlights = function () {
                var submitData = {
                    timeLine: self.SelectedTimeLine(),
                    criteria: self.SelectedCriteria(),
                    branchIds:self.BranchIds()
                };

                //debugger;
                return $.ajax({
                    type: "POST",
                    url: '/IPDC/Dashboard/GetMDDashboardHightlidhts',
                    contentType: "application/json;",
                    data: ko.toJSON(submitData),
                    dataType: "json",
                    success: function (data) {
                        // Call
                        self.Call_HomeLoan(data.Call_HomeLoan);
                        self.Call_HomeLoanToDay(data.Call_HomeLoanToDay);
                        self.Call_HomeLoanLastDay(data.Call_HomeLoanLastDay);

                        self.TodayHomeLoanAmount(data.TodayHomeLoanAmount);
                        self.LastdayHomeLoanAmount(data.LastdayHomeLoanAmount);

                        self.Call_PersonalLoan(data.Call_PersonalLoan);
                        self.Call_PersonalLoanToDay(data.Call_PersonalLoanToDay);
                        self.Call_PersonalLoanLastDay(data.Call_PersonalLoanLastDay);

                        self.TodayPersonalLoanAmount(data.TodayPersonalLoanAmount);
                        self.LastdayPersonalLoanAmount(data.LastdayPersonalLoanAmount);

                        self.Call_AutoLoan(data.Call_AutoLoan);
                        self.Call_AutoLoanToDay(data.Call_AutoLoanToDay);
                        self.Call_AutoLoanLastDay(data.Call_AutoLoanLastDay);

                        self.TodayAutoLoanAmount(data.TodayAutoLoanAmount);
                        self.LastdayAutoLoanAmount(data.LastdayAutoLoanAmount);

                        self.Call_FixedDeposit(data.Call_FixedDeposit);
                        self.Call_FixedDepositToDay(data.Call_FixedDepositToDay);
                        self.Call_FixedDepositLastDay(data.Call_FixedDepositLastDay);

                        self.TodayFixedDepositAmount(data.TodayFixedDepositAmount);
                        self.LastdayFixedDepositAmount(data.LastdayFixedDepositAmount);

                        self.Call_RecurrentDeposit(data.Call_RecurrentDeposit);
                        self.Call_RecurrentDepositToDay(data.Call_RecurrentDepositToDay);
                        self.Call_RecurrentDepositLastDay(data.Call_RecurrentDepositLastDay);

                        self.TodayRecurrentDepositAmount(data.TodayRecurrentDepositAmount);
                        self.LastdayRecurrentDepositAmount(data.LastdayRecurrentDepositAmount);

                        self.Call_Undefined(data.Call_Undefined);
                        self.Call_UndefinedToDay(data.Call_UndefinedToDay);
                        self.Call_UndefinedLastDay(data.Call_UndefinedLastDay);

                        self.TodayUndefinedAmount(data.TodayUndefinedAmount);
                        self.LastdayUndefinedAmount(data.LastdayUndefinedAmount);

                        self.Call_Liability(data.Call_Liability);

                        // Call
                        // Lead
                        self.Lead_HomeLoan(data.Leadl_HomeLoan);
                        self.Lead_HomeLoanToDay(data.Lead_HomeLoanToDay);
                        self.Lead_HomeLoanLastDay(data.Lead_HomeLoanLastDay);

                        self.TodayLeadHomeLoanAmount(data.TodayLeadHomeLoanAmount);
                        self.LastdayLeadHomeLoanAmount(data.LastdayLeadHomeLoanAmount);

                        self.Lead_PersonalLoan(data.Lead_PersonalLoan);
                        self.Lead_PersonalLoanToDay(data.Lead_PersonalLoanToDay);
                        self.Lead_PersonalLoanLastDay(data.Lead_PersonalLoanLastDay);

                        self.TodayLeadPersonalLoanAmount(data.TodayLeadPersonalLoanAmount);
                        self.LastdayLeadPersonalLoanAmount(data.LastdayLeadPersonalLoanAmount);

                        self.Lead_AutoLoan(data.Lead_AutoLoan);
                        self.Lead_AutoLoanToDay(data.Lead_AutoLoanToDay);
                        self.Lead_AutoLoanLastDay(data.Lead_AutoLoanLastDay);

                        self.TodayLeadAutoLoanAmount(data.TodayLeadAutoLoanAmount);
                        self.LastdayLeadAutoLoanAmount(data.LastdayLeadAutoLoanAmount);

                        self.Lead_FixedDeposit(data.Lead_FixedDeposit);
                        self.Lead_FixedDepositToDay(data.Lead_FixedDepositToDay);
                        self.Lead_FixedDepositLastDay(data.Lead_FixedDepositLastDay);

                        self.TodayLeadFixedDepositAmount(data.TodayLeadFixedDepositAmount);
                        self.LastdayLeadFixedDepositAmount(data.LastdayLeadFixedDepositAmount);

                        self.Lead_RecurrentDeposit(data.Lead_RecurrentDeposit);
                        self.Lead_RecurrentDepositToDay(data.Lead_RecurrentDepositToDay);
                        self.Lead_RecurrentDepositLastDay(data.Lead_RecurrentDepositLastDay);

                        self.TodayLeadRecurrentDepositAmount(data.TodayLeadRecurrentDepositAmount);
                        self.LastdayLeadRecurrentDepositAmount(data.LastdayLeadRecurrentDepositAmount);

                        self.Lead_Liability(data.Lead_Liability);
                        // Lead

                        // File Submited Apps
                        self.Application_HomeLoan(data.Application_HomeLoan);
                        self.Application_HomeLoanToDay(data.Application_HomeLoanToDay);
                        self.Application_HomeLoanLastDay(data.Application_HomeLoanLastDay);

                        self.TodayApplicationHomeLoanAmount(data.TodayApplicationHomeLoanAmount);
                        self.LastdayApplicationHomeLoanAmount(data.LastdayApplicationHomeLoanAmount);

                        self.Application_PersonalLoan(data.Application_PersonalLoan);
                        self.Application_PersonalLoanToDay(data.Application_PersonalLoanToDay);
                        self.Application_PersonalLoanLastDay(data.Application_PersonalLoanLastDay);

                        self.TodayApplicationPersonalLoanAmount(data.TodayApplicationPersonalLoanAmount);
                        self.LastdayApplicationPersonalLoanAmount(data.LastdayApplicationPersonalLoanAmount);

                        self.Application_AutoLoan(data.Application_AutoLoan);
                        self.Application_AutoLoanToDay(data.Application_AutoLoanToDay);
                        self.Application_AutoLoanLastDay(data.Application_AutoLoanLastDay);

                        self.TodayApplicationAutoLoanAmount(data.TodayApplicationAutoLoanAmount);
                        self.LastdayApplicationAutoLoanAmount(data.LastdayApplicationAutoLoanAmount);

                        self.Application_FixedDeposit(data.Application_FixedDeposit);
                        self.Application_FixedDepositToDay(data.Application_FixedDepositToDay);
                        self.Application_FixedDepositLastDay(data.Application_FixedDepositLastDay);

                        self.TodayApplicationFixedDepositAmount(data.TodayApplicationFixedDepositAmount);
                        self.LastdayApplicationFixedDepositAmount(data.LastdayApplicationFixedDepositAmount);

                        self.Application_RecurrentDeposit(data.Application_RecurrentDeposit);
                        self.Application_RecurrentDepositToDay(data.Application_RecurrentDepositToDay);
                        self.Application_RecurrentDepositLastDay(data.Application_RecurrentDepositLastDay);

                        self.TodayApplicationRecurrentDepositAmount(data.TodayApplicationRecurrentDepositAmount);
                        self.LastdayApplicationRecurrentDepositAmount(data.LastdayApplicationRecurrentDepositAmount);

                        self.Application_Liability(data.Application_Liability);

                        // File Submited Apps

                        // File Approved Apps
                        self.Approved_HomeLoan(data.Approved_HomeLoan);
                        self.Approved_HomeLoanToDay(data.Approved_HomeLoanToDay);
                        self.Approved_HomeLoanLastDay(data.Approved_HomeLoanLastDay);

                        self.Approved_HomeLoanAmount(numberFormatter(data.Approved_HomeLoanAmount));
                        self.TodayApprovedApplicationHomeLoanAmount(data.TodayApprovedApplicationHomeLoanAmount);
                        self.LastdayApprovedApplicationHomeLoanAmount(data.LastdayApprovedApplicationHomeLoanAmount);

                        self.Approved_AutoLoan(data.Approved_AutoLoan);
                        self.Approved_AutoLoanToDay(data.Approved_AutoLoanToDay);
                        self.Approved_AutoLoanLastDay(data.Approved_AutoLoanLastDay);

                        self.Approved_AutoLoanAmount(numberFormatter(data.Approved_AutoLoanAmount));
                        self.TodayApprovedApplicationAutoLoanAmount(data.TodayApprovedApplicationAutoLoanAmount);
                        self.LastdayApprovedApplicationAutoLoanAmount(data.LastdayApprovedApplicationAutoLoanAmount);

                        self.Approved_PersonalLoan(data.Approved_PersonalLoan);
                        self.Approved_PersonalLoanToDay(data.Approved_PersonalLoanToDay);
                        self.Approved_PersonalLoanLastDay(data.Approved_PersonalLoanLastDay);

                        self.Approved_PersonalLoanAmount(numberFormatter(data.Approved_PersonalLoanAmount));
                        self.TodayApprovedApplicationPersonalLoanAmount(data.TodayApprovedApplicationPersonalLoanAmount);
                        self.LastdayApprovedApplicationPersonalLoanAmount(data.LastdayApprovedApplicationPersonalLoanAmount);
                        // File Approved Apps

                        // File DisApproved Apps
                        self.Disapproved_HomeLoan(data.Disapproved_HomeLoan);
                        self.DisApproved_HomeLoanToDay(data.DisApproved_HomeLoanToDay);
                        self.DisApproved_HomeLoanLastDay(data.DisApproved_HomeLoanLastDay);

                        self.Disapproved_HomeLoanAmount(numberFormatter(data.Disapproved_HomeLoanAmount));
                        self.TodayDisApprovedApplicationHomeLoanAmount(data.TodayDisApprovedApplicationHomeLoanAmount);
                        self.LastdayDisApprovedApplicationHomeLoanAmount(data.LastdayDisApprovedApplicationHomeLoanAmount);

                        self.Disapproved_AutoLoan(data.Disapproved_AutoLoan);
                        self.DisApproved_AutoLoanToDay(data.DisApproved_AutoLoanToDay);
                        self.DisApproved_AutoLoanLastDay(data.DisApproved_AutoLoanLastDay);

                        self.Disapproved_AutoLoanAmount(numberFormatter(data.Disapproved_AutoLoanAmount));
                        self.TodayDisApprovedApplicationAutoLoanAmount(data.TodayDisApprovedApplicationAutoLoanAmount);
                        self.LastdayDisApprovedApplicationAutoLoanAmount(data.LastdayDisApprovedApplicationAutoLoanAmount);

                        self.Disapproved_PersonalLoan(data.Disapproved_PersonalLoan);
                        self.DisApproved_PersonalLoanToDay(data.DisApproved_PersonalLoanToDay);
                        self.DisApproved_PersonalLoanLastDay(data.DisApproved_PersonalLoanLastDay);

                        self.Disapproved_PersonalLoanAmount(numberFormatter(data.Disapproved_PersonalLoanAmount));
                        self.TodayDisApprovedApplicationPersonalLoanAmount(data.TodayDisApprovedApplicationPersonalLoanAmount);
                        self.LastdayDisApprovedApplicationPersonalLoanAmount(data.LastdayDisApprovedApplicationPersonalLoanAmount);
                        // File DisApproved Apps

                        // Disbursed/Received Apps

                        self.Disbursed_HomeLoanToDay(data.Disbursed_HomeLoanToDay);
                        self.Disbursed_HomeLoanLastDay(data.Disbursed_HomeLoanLastDay);

                        self.Disbursed_AutoLoanToDay(data.Disbursed_AutoLoanToDay);
                        self.Disbursed_AutoLoanLastDay(data.Disbursed_AutoLoanLastDay);

                        self.Disbursed_PersonalLoanToDay(data.Disbursed_PersonalLoanToDay);
                        self.Disbursed_PersonalLoanLastDay(data.Disbursed_PersonalLoanLastDay);

                        self.Received_FixedToDay(data.Received_FixedToDay);
                        self.Received_FixedLastDay(data.Received_FixedLastDay);

                        self.Received_RecurrentToDay(data.Received_RecurrentToDay);
                        self.Received_RecurrentLastDay(data.Received_RecurrentLastDay);

                        self.Disbursed_HomeLoanAmount(data.Disbursed_HomeLoanAmount);
                        self.Disbursed_HomeLoanAmountToday(data.Disbursed_HomeLoanAmountToday);
                        self.Disbursed_HomeLoanAmountLastday(data.Disbursed_HomeLoanAmountLastday);

                        self.Disbursed_AutoLoanAmount(data.Disbursed_AutoLoanAmount);
                        self.Disbursed_AutoLoanAmountToday(data.Disbursed_AutoLoanAmountToday);
                        self.Disbursed_AutoLoanAmountLastday(data.Disbursed_AutoLoanAmountLastday);

                        self.Disbursed_PersonalLoanAmount(data.Disbursed_PersonalLoanAmount);
                        self.Disbursed_PersonalLoanAmountToday(data.Disbursed_PersonalLoanAmountToday);
                        self.Disbursed_PersonalLoanAmountLastday(data.Disbursed_PersonalLoanAmountLastday);

                        self.ReceivedDepositAmount(data.ReceivedDepositAmount);

                        self.FundReceived_FixedAmountToday(data.FundReceived_FixedAmountToday);
                        self.FundReceived_FixedAmountLastday(data.FundReceived_FixedAmountLastday);

                        self.FundReceived_RecurrentAmountToday(data.FundReceived_RecurrentAmountToday);
                        self.FundReceived_RecurrentAmountLastday(data.FundReceived_RecurrentAmountLastday);

                        // Disbursed/Received Apps
                        
                        var totalYesterdayCall = self.Call_HomeLoanToDay() + self.Call_AutoLoanToDay() + self.Call_PersonalLoanToDay() + self.Call_FixedDepositToDay() + self.Call_RecurrentDepositToDay() + self.Call_UndefinedToDay();
                        var totallastCall = self.Call_HomeLoanLastDay() + self.Call_AutoLoanLastDay() + self.Call_PersonalLoanLastDay() + self.Call_FixedDepositLastDay() + self.Call_RecurrentDepositLastDay() + self.Call_UndefinedLastDay();

                        var totalYesterdayLead = self.Lead_HomeLoanToDay() + self.Lead_AutoLoanToDay() + self.Lead_PersonalLoanToDay() + self.Lead_FixedDepositToDay() + self.Lead_RecurrentDepositToDay();
                        var totallastLead = self.Lead_HomeLoanLastDay() + self.Lead_AutoLoanLastDay() + self.Lead_PersonalLoanLastDay() + self.Lead_FixedDepositLastDay() + self.Lead_RecurrentDepositLastDay();

                        var totalYesterdaySubmit = self.Application_HomeLoanToDay() + self.Application_AutoLoanToDay() + self.Application_PersonalLoanToDay() + self.Application_FixedDepositToDay() + self.Application_RecurrentDepositToDay();
                        var totallastLeadSubmit = self.Application_HomeLoanLastDay() + self.Application_AutoLoanLastDay() + self.Application_PersonalLoanLastDay() + self.Application_FixedDepositLastDay() + self.Application_RecurrentDepositLastDay();

                        var totalYesterdayApproved = self.Approved_HomeLoanToDay() + self.Approved_AutoLoanToDay() + self.Approved_PersonalLoanToDay();
                        var totallastLeadApproved = self.Approved_HomeLoanLastDay() + self.Approved_AutoLoanLastDay() + self.Approved_PersonalLoanLastDay();

                        var totalYesterdaydisApproved = self.DisApproved_HomeLoanToDay() + self.DisApproved_AutoLoanToDay() + self.DisApproved_PersonalLoanToDay();
                        var totallastLeaddisApproved = self.DisApproved_HomeLoanLastDay() + self.DisApproved_AutoLoanLastDay() + self.DisApproved_PersonalLoanLastDay();

                        var totalYesterdaydisbursedApproved = self.Disbursed_HomeLoanToDay() + self.Disbursed_AutoLoanToDay() + self.Disbursed_PersonalLoanToDay() + self.Received_FixedToDay() + self.Received_RecurrentToDay();
                        var totallastLeaddisbursedApproved = self.Disbursed_HomeLoanLastDay() + self.Disbursed_AutoLoanLastDay() + self.Disbursed_PersonalLoanLastDay() + self.Received_FixedLastDay() + self.Received_RecurrentLastDay();

                        if (self.SelectedCriteria() === 1) {
                            call.series = [
                            {
                                name: 'Yesterday' + ' (' + totalYesterdayCall + ')',
                                data: [self.Call_HomeLoanToDay(), self.Call_AutoLoanToDay(), self.Call_PersonalLoanToDay(), self.Call_FixedDepositToDay(), self.Call_RecurrentDepositToDay(), self.Call_UndefinedToDay()]
                            }, {
                                name: 'Day Before Yesterday' + ' (' + totallastCall + ')',
                                data: [self.Call_HomeLoanLastDay(), self.Call_AutoLoanLastDay(), self.Call_PersonalLoanLastDay(), self.Call_FixedDepositLastDay(), self.Call_RecurrentDepositLastDay(), self.Call_UndefinedLastDay()]
                            }];

                            lead.series = [
                            {
                                name: 'Yesterday' + ' (' + totalYesterdayLead + ')',
                                data: [self.Lead_HomeLoanToDay(), self.Lead_AutoLoanToDay(), self.Lead_PersonalLoanToDay(), self.Lead_FixedDepositToDay(), self.Lead_RecurrentDepositToDay()]
                            }, {
                                name: 'Day Before Yesterday' + ' (' + totallastLead + ')',
                                data: [self.Lead_HomeLoanLastDay(), self.Lead_AutoLoanLastDay(), self.Lead_PersonalLoanLastDay(), self.Lead_FixedDepositLastDay(), self.Lead_RecurrentDepositLastDay()]
                            }];

                            fileSubmit.series = [
                            {
                                name: 'Yesterday (' + totalYesterdaySubmit +')',
                                data: [self.Application_HomeLoanToDay(), self.Application_AutoLoanToDay(), self.Application_PersonalLoanToDay(), self.Application_FixedDepositToDay(), self.Application_RecurrentDepositToDay()]
                            }, {
                                name: 'Day Before Yesterday(' + totallastLeadSubmit + ')',
                                data: [self.Application_HomeLoanLastDay(), self.Application_AutoLoanLastDay(), self.Application_PersonalLoanLastDay(), self.Application_FixedDepositLastDay(), self.Application_RecurrentDepositLastDay()]
                            }];

                            fileApproved.series = [
                            {
                                name: 'Yesterday (' + totalYesterdayApproved + '/' + totalYesterdaydisApproved + ')',
                                data: [self.Approved_HomeLoanToDay(), self.Approved_AutoLoanToDay(), self.Approved_PersonalLoanToDay()]
                            }, {
                                name: 'Day Before Yesterday (' + totallastLeadApproved + '/' + totallastLeaddisApproved + ')',
                                data: [self.Approved_HomeLoanLastDay(), self.Approved_AutoLoanLastDay(), self.Approved_PersonalLoanLastDay()]
                            }];

                            fileDisbRec.series = [
                            {
                                name: 'Yesterday (' +totalYesterdaydisbursedApproved+')',
                                data: [self.Disbursed_HomeLoanToDay(), self.Disbursed_AutoLoanToDay(), self.Disbursed_PersonalLoanToDay(), self.Received_FixedToDay(), self.Received_RecurrentToDay()]
                            }, {
                                name: 'Day Before Yesterday (' + totallastLeaddisbursedApproved + ')',
                                data: [self.Disbursed_HomeLoanLastDay(), self.Disbursed_AutoLoanLastDay(), self.Disbursed_PersonalLoanLastDay(), self.Received_FixedLastDay(), self.Received_RecurrentLastDay()]
                            }];
                        }
                        else if (self.SelectedCriteria() === 2) {
                            call.series = [
                            {
                                name: 'Yesterday',
                                data: [self.TodayHomeLoanAmount(), self.TodayAutoLoanAmount(), self.TodayPersonalLoanAmount(), self.TodayFixedDepositAmount(), self.TodayRecurrentDepositAmount(), self.TodayUndefinedAmount()]
                            }, {
                                name: 'Day Before Yesterday',
                                data: [self.LastdayHomeLoanAmount(), self.LastdayAutoLoanAmount(), self.LastdayPersonalLoanAmount(), self.LastdayFixedDepositAmount(), self.LastdayRecurrentDepositAmount(), self.LastdayUndefinedAmount()]
                            }];

                            lead.series = [
                            {
                                name: 'Yesterday',
                                data: [self.TodayLeadHomeLoanAmount(), self.TodayLeadAutoLoanAmount(), self.TodayLeadPersonalLoanAmount(), self.TodayLeadFixedDepositAmount(), self.TodayLeadRecurrentDepositAmount()]
                            }, {
                                name: 'Day Before Yesterday',
                                data: [self.LastdayLeadHomeLoanAmount(), self.LastdayLeadAutoLoanAmount(), self.LastdayLeadPersonalLoanAmount(), self.LastdayLeadFixedDepositAmount(), self.LastdayLeadRecurrentDepositAmount()]
                            }];

                            fileSubmit.series = [
                            {
                                name: 'Yesterday',
                                data: [self.TodayApplicationHomeLoanAmount(), self.TodayApplicationAutoLoanAmount(), self.TodayApplicationPersonalLoanAmount(), self.TodayApplicationFixedDepositAmount(), self.TodayApplicationRecurrentDepositAmount()]
                            }, {
                                name: 'Day Before Yesterday',
                                data: [self.LastdayApplicationHomeLoanAmount(), self.LastdayApplicationAutoLoanAmount(), self.LastdayApplicationPersonalLoanAmount(), self.LastdayApplicationFixedDepositAmount(), self.LastdayApplicationRecurrentDepositAmount()]
                            }];

                            fileApproved.series = [
                            {
                                name: 'Yesterday',
                                data: [self.TodayApprovedApplicationHomeLoanAmount(), self.TodayApprovedApplicationAutoLoanAmount(), self.TodayApprovedApplicationPersonalLoanAmount()]
                            }, {
                                name: 'Day Before Yesterday',
                                data: [self.LastdayApprovedApplicationHomeLoanAmount(), self.LastdayApprovedApplicationAutoLoanAmount(), self.LastdayApprovedApplicationPersonalLoanAmount()]
                            }];

                            fileDisbRec.series = [
                            {
                                name: 'Yesterday',
                                data: [self.Disbursed_HomeLoanAmountToday(), self.Disbursed_AutoLoanAmountToday(), self.Disbursed_PersonalLoanAmountToday(), self.FundReceived_FixedAmountToday(), self.FundReceived_RecurrentAmountToday()]
                            }, {
                                name: 'Day Before Yesterday',
                                data: [self.Disbursed_HomeLoanAmountLastday(), self.Disbursed_AutoLoanAmountLastday(), self.Disbursed_PersonalLoanAmountLastday(), self.FundReceived_FixedAmountLastday(), self.FundReceived_RecurrentAmountLastday()]
                            }];

                        }
                        $('#containera').highcharts(call);
                        $('#containerb').highcharts(lead);
                        $('#containerc').highcharts(fileSubmit);
                        $('#containerd').highcharts(fileApproved);
                        $('#containere').highcharts(fileDisbRec);
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
                //debugger;
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

                        google.charts.setOnLoadCallback(drawChart);

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

            var chart = {
                type: 'bar'
            };
            var title = {
                text: ''

            };
            var subtitle = {
                text: ''
            };

            var callxAxis = {
                categories: ['Home Loan', 'Auto Loan', 'Personal Loan', 'Fixed Deposit', 'Recurrent Deposit', 'Undefined'],
                title: {
                    text: null
                }
            };
            var leadxAxis = {
                categories: ['Home Loan', 'Auto Loan', 'Personal Loan', 'Fixed Deposit', 'Recurrent Deposit'],
                title: {
                    text: null
                }
            };
            var applicationxAxis = {
                categories: ['Home Loan', 'Auto Loan', 'Personal Loan', 'Fixed Deposit', 'Recurrent Deposit'],
                title: {
                    text: null
                }
            };

            var appApplicationxAxis = {
                categories: ['Home Loan', 'Auto Loan', 'Personal Loan'],
                title: {
                    text: null
                }
            };
            var disRecxAxis = {
                categories: ['Home Loan', 'Auto Loan', 'Personal Loan', 'Fixed Deposit', 'Recurrent Deposit'],
                title: {
                    text: null
                }
            };
            var brxAxis = {
                categories: ['Gulshan Br', 'Motijheel Br', 'Dhanmondi Br', 'Uttara Br', 'Chittagong Br', 'Sylhet Br', 'Gazipur Br'],
                title: {
                    text: null
                }
            };
            var series = [
                    {
                        name: 'ASSET',
                        data: [300, 130, 20, 300, 130, 20, 20]
                    }
            ];
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

            var recplotOptions = {
                bar: {
                    dataLabels: {
                        enabled: true,
                        formatter: function () {
                            return (10 + "/" + 20);
                        }
                    }
                }
            };

            var displotOptions = {
                bar: {
                    dataLabels: {
                        enabled: true,
                        formatter: function () {
                            return (10 + "/" + 20);
                        }
                    }
                }
            };

            var credits = {
                enabled: false
            };
            var colors = [
                '#EC017F', '#979797'
            ];

            var assetcolors = [
                '#EC017F'
            ];

            var libcolors = [
                '#979797'
            ];

            call.chart = chart;
            call.title = title;
            call.subtitle = subtitle;
            call.tooltip = tooltip;
            call.xAxis = callxAxis;
            call.yAxis = yAxis;
            call.colors = colors;
            call.plotOptions = plotOptions;
            call.credits = credits;

            lead.chart = chart;
            lead.title = title;
            lead.subtitle = subtitle;
            lead.tooltip = tooltip;
            lead.xAxis = leadxAxis;
            lead.yAxis = yAxis;
            lead.colors = colors;
            lead.plotOptions = plotOptions;
            lead.credits = credits;

            fileSubmit.chart = chart;
            fileSubmit.title = title;
            fileSubmit.subtitle = subtitle;
            fileSubmit.tooltip = tooltip;
            fileSubmit.xAxis = applicationxAxis;
            fileSubmit.yAxis = yAxis;
            fileSubmit.colors = colors;
            fileSubmit.plotOptions = plotOptions;
            fileSubmit.credits = credits;

            fileApproved.chart = chart;
            fileApproved.title = title;
            fileApproved.subtitle = subtitle;
            fileApproved.tooltip = tooltip;
            fileApproved.xAxis = appApplicationxAxis;
            fileApproved.yAxis = yAxis;
            fileApproved.colors = colors;
            fileApproved.plotOptions = plotOptions;
            fileApproved.credits = credits;

            fileDisbRec.chart = chart;
            fileDisbRec.title = title;
            fileDisbRec.subtitle = subtitle;
            fileDisbRec.tooltip = tooltip;
            fileDisbRec.xAxis = disRecxAxis;
            fileDisbRec.yAxis = yAxis;
            fileDisbRec.colors = colors;
            fileDisbRec.plotOptions = plotOptions;
            fileDisbRec.credits = credits;

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

        dashboard = new mdDashboardVM();
        dashboard.GetHighlights();
        dashboard.CustomFunction();
        dashboard.GetBranches();
        ko.applyBindings(dashboard, document.getElementById('mdDashboard'));

        function drawChart() {
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
        google.charts.setOnLoadCallback(drawChart);

        
    });
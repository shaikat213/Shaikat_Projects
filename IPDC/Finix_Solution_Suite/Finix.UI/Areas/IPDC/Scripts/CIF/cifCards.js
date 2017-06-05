

var creditCard = function () {
    var self = this;

    self.Id = ko.observable();
    self.CIFId = ko.observable();
    self.CreditCardNo = ko.observable().extend({ digit: true, minLength: 15 });;
    self.CreditCardIssuersName = ko.observable();
    self.CreditCardIssueDate = ko.observable();
    self.CreditCardIssueDate.subscribe(function () {
        self.CreditCardIssueDateText(moment(self.CreditCardIssueDate()).format("DD/MM/YYYY"));
    })
    self.CreditCardIssueDateText = ko.observable();
    self.CreditCardLimit = ko.observable();
    
    self.Load = function(data) {
        self.Id(data ? data.Id : '');
        self.CIFId(data ? data.CIFId : '');
        self.CreditCardNo(data ? data.CreditCardNo : '');
        self.CreditCardIssuersName(data ? data.CreditCardIssuersName : '');
        self.CreditCardIssueDate(data ? data.CreditCardIssueDate : '');
        self.CreditCardIssueDateText(data ? data.CreditCardIssueDateText : '');
        self.CreditCardLimit(data ? data.CreditCardLimit : '');
    }

};

var bankAccount = function () {
    var self = this;

    self.Id = ko.observable();
    self.CIFId = ko.observable();
    self.AccountNo = ko.observable();
    self.RoutingNo = ko.observable();
    self.BankName = ko.observable();
    self.BranchName = ko.observable();

    self.Load = function (data) {
        self.Id(data ? data.Id : '');
        self.CIFId(data ? data.CIFId : '');
        self.AccountNo(data ? data.AccountNo : '');
        self.RoutingNo(data ? data.RoutingNo : '');
        self.BankName(data ? data.BankName : '');
        self.BranchName(data ? data.BranchName : '');
    }
}

var CIFCardsViewModel = function () {
    var self = this;
    self.CIFId = ko.observable();
    self.CreditCards = ko.observableArray([]);
    self.BankAccounts = ko.observableArray([]);
    self.AddCIFReferenceLine = function () {
        self.CIFReference.push(new CIFReferenceLine());
    }

    self.AddCreditCardLine = function () {
        var newInstance = new creditCard();
        newInstance.CIFId(self.CIFId());
        self.CreditCards.push(newInstance);
    }
    self.RemovedCreditCards = ko.observableArray([]);
    self.RemoveCreditCardLine = function (line) {
        if (line.Id() > 0) {
            self.RemovedCreditCards.push(line.Id());
        }
        self.CreditCards.remove(line);
    }

    self.AddBankAccountLine = function () {
        var newInstance = new bankAccount();
        newInstance.CIFId(self.CIFId());
        self.BankAccounts.push(newInstance);
    }
    self.RemovedBankAccounts = ko.observableArray([]);
    self.RemoveBankAccountLine = function (line) {
        if (line.Id() > 0) {
            self.RemovedBankAccounts.push(line.Id());
        }
        self.BankAccounts.remove(line);
    }

    //$.ajax({
    //    type: "GET",
    //    url: '/IPDC/CIF/RemoveRef/?cid=' + line.Id(),
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "json",
    //    success: function (data) {

    //    },
    //    error: function (error) {
    //        alert(error.status + "<--and--> " + error.statusText);
    //    }
    //});

    self.LoadData = function () {
        if (self.CIFId() > 0) {
            $.getJSON("/IPDC/CIF/GetCIFCardsByCIFId/?cifId=" + self.CIFId(),
                null,
                function (data) {
                    self.BankAccounts([]);
                    self.CreditCards([]);
                    $.each(data.BankAccountDtos, function (index, value) {
                        var newInstance = new bankAccount();
                        newInstance.Load(value);
                        self.BankAccounts.push(newInstance);
                    });
                    $.each(data.CreditCardDtos, function (index, value) {
                        var newInstance = new creditCard();
                        newInstance.Load(value);
                        self.CreditCards.push(newInstance);
                    });
                });
        }

    }

    //Add New Data
    self.Submit = function () {
        var submitData = {
            BankAccountDtos: self.BankAccounts(),
            RemovedBankAccounts: self.RemovedBankAccounts(),
            CreditCardDtos: self.CreditCards(),
            RemovedCreditCards: self.RemovedCreditCards()
        };
            
        $.ajax({
            url: '/IPDC/CIF/SaveCIFCards',
            type: 'POST',
            contentType: 'application/json',
            data: ko.toJSON(submitData),
            success: function (data) {
                $('#successModalCards').modal('show');
                $('#successModalCardsText').text(data.Message);
                if (data.Success === true) {
                    self.LoadData();
                }
                //self.Reset();
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.Reset = function () {
        self.BankAccounts([]);
        self.CreditCards([]);
    }
    
};

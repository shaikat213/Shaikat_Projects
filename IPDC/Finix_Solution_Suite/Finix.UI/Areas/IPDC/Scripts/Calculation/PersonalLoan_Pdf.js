
$(document).ready(function () {
    var PersonalLoanPdfViewModel = function () {
        var self = this;
        self.EmbedPDFLink = ko.observable("/Areas/IPDC/Content/PDFDocuments/Personal-Loan.pdf");
    };

    var alVm = new PersonalLoanPdfViewModel();
    ko.applyBindings(alVm, document.getElementById("personalLoanPdf")[0]);
})

//http://acroeng.adobe.com/Test_Files/browser_tests/embedded/simple5.pdf
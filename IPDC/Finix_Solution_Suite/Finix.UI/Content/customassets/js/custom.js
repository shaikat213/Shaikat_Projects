$(document).ready(function () {
    $(".dropdown").click(
        function () {
            $('.dropdown-menu', this).stop().fadeIn("fast");
        },
        function () {
            $('.dropdown-menu', this).stop().fadeOut("fast");
        });
    //$('.datepicker').datepicker();
    //$(".datepicker").datepicker({
    //    changeMonth: true,
    //    changeYear: true
    //});
});


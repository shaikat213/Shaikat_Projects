
function gd(year, month, day) {
    return new Date(year, month, day).getTime();
}

var previousPoint = null, previousLabel = null;
var monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

$.fn.UseTooltip = function () {
    $(this).bind("plothover", function (event, pos, item) {
        if (item) {
            if ((previousLabel != item.series.label) || (previousPoint != item.dataIndex)) {
                previousPoint = item.dataIndex;
                previousLabel = item.series.label;
                $("#tooltip").remove();

                var x = item.datapoint[0];
                var y = item.datapoint[1];

                var color = item.series.color;
                var month = new Date(x).getMonth();

                //console.log(item);

                if (item.seriesIndex == 0) {
                    showTooltip(item.pageX,
                    item.pageY,
                    color,
                    "<strong>" + item.series.label + "</strong><br>" + monthNames[month] + " : <strong>" + y + "</strong>");
                } else {
                    showTooltip(item.pageX,
                    item.pageY,
                    color,
                    "<strong>" + item.series.label + "</strong><br>" + monthNames[month] + " : <strong>" + y + "</strong>");
                }
            }
        } else {
            $("#tooltip").remove();
            previousPoint = null;
        }
    });
};

function showTooltip(x, y, color, contents) {
    $('<div id="tooltip">' + contents + '</div>').css({
        position: 'absolute',
        display: 'none',
        top: y - 40,
        left: x - 120,
        border: '2px solid ' + color,
        padding: '3px',
        'font-size': '9px',
        'border-radius': '5px',
        'background-color': '#fff',
        'font-family': 'Verdana, Arial, Helvetica, Tahoma, sans-serif',
        opacity: 0.9
    }).appendTo("body").fadeIn(200);
}
$(document).ready(function () {

    $(function () {
        var data1 = [
            [gd(2012, 0, 1), 1452.21], [gd(2012, 1, 1), 1542.14], [gd(2012, 2, 1), 1673.77], [gd(2012, 3, 1), 1649.69],
            [gd(2012, 4, 1), 1691.19], [gd(2012, 5, 1), 1298.76], [gd(2012, 6, 1), 1559.90], [gd(2012, 7, 1), 1130.31],
            [gd(2012, 8, 1), 1844.81], [gd(2012, 9, 1), 1846.58], [gd(2012, 10, 1), 1781.64], [gd(2012, 11, 2), 1584.76]
        ];

        //var data2 = [
        //[gd(2012, 0, 1), 0.63], [gd(2012, 1, 1), 5.44], [gd(2012, 2, 1), -3.92], [gd(2012, 3, 1), -1.44],
        //[gd(2012, 4, 1), -3.55], [gd(2012, 5, 1), 0.48], [gd(2012, 6, 1), -0.55], [gd(2012, 7, 1), 2.54],
        //[gd(2012, 8, 1), 7.02], [gd(2012, 9, 1), 0.10], [gd(2012, 10, 1), -1.43], [gd(2012, 11, 2), -2.14]
        //];
        var dataset = [
        { label: "App", data: data1 }
        //,{ label: "Change", data: data2, yaxis: 2 }
        ];

        var options = {
            series: {
                lines: {
                    show: true
                },
                points: {
                    radius: 3,
                    fill: true,
                    show: true
                }
            },
            xaxis: {
                mode: "time",
                tickSize: [1, "month"],
                tickLength: 0,
                axisLabel: $('#amount2').val(),
                axisLabelUseCanvas: true,
                axisLabelFontSizePixels: 12,
                axisLabelFontFamily: 'Verdana, Arial',
                axisLabelPadding: 10
            },
            yaxes: [{
                axisLabel: "App",
                axisLabelUseCanvas: true,
                axisLabelFontSizePixels: 12,
                axisLabelFontFamily: 'Verdana, Arial',
                axisLabelPadding: 3,
                tickFormatter: function (v, axis) {
                    return v;//jQuery.formatNumber(v, { format: "#,###", locale: "us" });
                }
            }
            //, {
            //    position: "right",
            //    axisLabel: "Change(%)",
            //    axisLabelUseCanvas: true,
            //    axisLabelFontSizePixels: 12,
            //    axisLabelFontFamily: 'Verdana, Arial',
            //    axisLabelPadding: 3
            //}
            ],
            legend: {
                noColumns: 0,
                labelBoxBorderColor: "#000000",
                position: "nw"
            },
            grid: {
                hoverable: true,
                borderWidth: 2,
                borderColor: "#633200",
                backgroundColor: { colors: ["#EDF5FF", "#ffffff"] }
            },
            colors: ["#0022FF", "#FF0000"]
        };

        $.plot($("#app-chart"), dataset, options);
        $("#app-chart").UseTooltip();
    });
});

// Charts
	var dataVisitors = [ [0,0],[1,-0.5],[1.5,-1],[2,-0.75],[3,0],[3.75,0.5],[5,1],[5.75,0.5],[6.2,0],[6.5,-0.5],[7.2,-0.75],[8,-0.5],[9,-0.25],[9.5,0.5],[10.2,1],[11.2,0.5],[12,0],[12.5,-0.5],[13.5,-0.75],[14,0] ];
	var dataOrders = [ [0,0],[1,-0.75],[1.5,-0.5],[2,0],[3,0.5],[3.75,0.61],[5,0.5],[5.75,-0.25],[6.2,-0.5],[6.5,-0.7],[7.2,-0.5],[8,0],[9,0.5],[9.5,-0.3],[10.2,0.5],[11.2,0],[12,-0.75],[12.5,-1],[13.5,-0.5],[14,0] ];
	var dataUsers = [ [0,0],[1,-0.75],[1.5,-0.5],[2,0],[3,0.5],[3.75,0.61],[5,0.5],[5.75,-0.25],[6.2,-0.5],[6.5,-0.7],[7.2,-0.5],[8,0],[9,0.5],[9.5,0.5],[10.2,1],[11.2,0.5],[12,0],[12.5,-0.5],[13.5,-0.75],[14,0] ];


    var plotVisitors = $.plot($("#chart-visitors"),
           [ { data: dataVisitors } ], {
               series: {
                   lines: { show: true },
                   points: { show: true, fill:true, fillColor: '#8fd7d4' }
               },
               grid: { hoverable: true, clickable: true },
               yaxis: { min: -1.1, max: 1.1 },
               xaxis: { min: 0, max: 14 },
               legend: {
                    show: false,
                    // margin: number of pixels or [x margin, y margin]
                    //container: '.cLegend'
                    // sorted: null/false, true, "ascending", "descending", "reverse", or a comparator
                },
                colors: [ '#8fd7d4' ]
             });


    function showTooltip(x, y, contents) {
        $('<div id="tooltip" class="tooltip">' + contents + '</div>').css( {
            position: 'absolute',
            display: 'none',
            top: y - 43,
            'padding': '7px 10px',
            'background' : '#3e3e3e',
            'z-index': '9999',
            'color': '#fff',
            'font-size': '11px',
            opacity: 0.65
        }).appendTo("body");
        var tw = $('#tooltip').width();
        $('#tooltip').css({ left: x - tw/2 - 5});
        $('#tooltip').fadeIn(200);
    }

    var previousPoint = null;
    $(".chart").bind("plothover", function (event, pos, item) {
        $("#x").text(pos.x.toFixed(2));
        $("#y").text(pos.y.toFixed(2));

        if ($(".chart").length > 0) {
            if (item) {
                if (previousPoint != item.dataIndex) {
                    previousPoint = item.dataIndex;
                    
                    $("#tooltip").remove();
                    var x = item.datapoint[0].toFixed(2),
                        y = item.datapoint[1].toFixed(2);
                    var tVis = 'Total visitor: ';
                    var tOrd = 'Total orders: ';
                    var tUsr = 'Total users: ';

                    var totalString = '';
                    if($(this).is('#chart-visitors')){ totalString = tVis; }
                    else if($(this).is('#chart-orders')){ totalString = tOrd; }
                    else if($(this).is('#chart-users')){ totalString = tUsr; }
                    showTooltip(item.pageX, item.pageY,
                    			totalString + 
                                (x*y*1500).toFixed());
                }
            }
            else {
                $("#tooltip").remove();
                previousPoint = null;            
            }
        }
    });


    var plotOrders = $.plot($("#chart-orders"),
           [ { data: dataOrders } ], {
               series: {
                   lines: { show: true },
                   points: { show: true, fill:true, fillColor: '#8fd7d4' }
               },
               grid: { hoverable: true, clickable: true },
               yaxis: { min: -1.1, max: 1.1 },
               xaxis: { min: 0, max: 14 },
               legend: {
                    show: false,
                    // margin: number of pixels or [x margin, y margin]
                    //container: '.cLegend'
                    // sorted: null/false, true, "ascending", "descending", "reverse", or a comparator
                },
                colors: [ '#8fd7d4' ]
             });

    var plotUsers = $.plot($("#chart-users"),
           [ { data: dataUsers } ], {
               series: {
                   lines: { show: true },
                   points: { show: true, fill:true, fillColor: '#8fd7d4' }
               },
               grid: { hoverable: true, clickable: true },
               yaxis: { min: -1.1, max: 1.1 },
               xaxis: { min: 0, max: 14 },
               legend: {
                    show: false,
                    // margin: number of pixels or [x margin, y margin]
                    //container: '.cLegend'
                    // sorted: null/false, true, "ascending", "descending", "reverse", or a comparator
                },
                colors: [ '#8fd7d4' ]
             });

     var plotQuick = $.plot($("#chart-quick"),
           [ { data: dataOrders } ], {
               series: {
                   lines: { show: true },
                   points: { show: true, fill:true, fillColor: '#8fd7d4' }
               },
               grid: { hoverable: true, clickable: true },
               yaxis: { min: -1.1, max: 1.1 },
               xaxis: { min: 0, max: 14 },
               legend: {
                    show: false,
                    // margin: number of pixels or [x margin, y margin]
                    //container: '.cLegend'
                    // sorted: null/false, true, "ascending", "descending", "reverse", or a comparator
                },
                colors: [ '#8fd7d4' ]
             });

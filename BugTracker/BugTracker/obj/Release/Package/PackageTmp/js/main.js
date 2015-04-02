$(window).load(function(){
	$('#loading').fadeOut(1000);

	$('#stats_visits').sparkline('html', { 
		type: 'bar',
		chartRangeMin: 0,
		height: '40px',
		barWidth: '5px',
		barColor: '#3e3e3e',
		tooltipClassname:'tooltip-sp'
	});
	$('#stats_balance').sparkline('html', { 
		type: 'bar',
		chartRangeMin: 0,
		height: '40px',
		barWidth: '5px',
		barColor: '#ffffff',
		tooltipClassname:'tooltip-sp'
	});

  // checkbox slider change -> hide the widget
      $('.widget-controls .sl').change(function(){
        var thisRoot = $(this)
                .parent() // widget-header
                .parent() // widget-controls
                .parent(); // widget
        if(!this.checked){
          //thisRoot.find('.widget-content').fadeOut();
          //thisRoot.find('.widget-content, .widget-title, .xtra').animate({'opacity':0.25}).css({'cursor':'default', 'pointer-events':'none'});
          thisRoot.find('.widget-content, .widget-title, .xtra').addClass('w-off');
          thisRoot.find('.etabs li a').css({'cursor':'default', 'pointer-events':'none'})
        } else {
          //thisRoot.find('.widget-content').fadeIn();
          //thisRoot.find('.widget-content, .widget-title, .xtra').animate({'opacity':1}).css({'cursor':'pointer', 'pointer-events':'auto'});
          thisRoot.find('.widget-content, .widget-title, .xtra').removeClass('w-off');
          thisRoot.find('.etabs li a').css({'cursor':'pointer', 'pointer-events':'auto'});
        }
      })

  $('.icon-nav-mobile').click(function(){
    $('.mainNav').toggleClass('open');
  })

	$('#topTabs-container-docs, #statsTabs-container').easytabs({
		updateHash: false,
		tabs: "ul.etabs > li"
	});
	$('#topTabs-container-home').easytabs({
		updateHash: false,
		tabs: "ul.etabs > li",
		animate: true,
  		transitionIn: 'slideDown',
  		transitionOut: 'slideUp'
	});

	
	$('.small-calendar').datepicker({
		showOtherMonths: true,
			selectOtherMonths: true,
			altField: "#calendar-date",
			dateFormat:"dd/mm/yy"
	});

	$('.ttip').hover(
		function(){
			var ttip_c = $(this).data('ttip');
			var ttip_h = $(this).height();
			$(this).append('<div class="ttip_t">' + ttip_c + '</div>');
			$('.ttip_t').css({ 'top' : ttip_h + ttip_h/2 + 10 });
			$('.ttip_t').fadeIn();
		},
		function(){
			$('.ttip_t').fadeOut(function(){
				$(this).remove()
			});
		}
	);


}) //.Ready
$(function(){
	$('.accContainer').hide();
	$('.accContainer:first').next().slideDown();
	
	$('.accordionHeader').click(function(){
		if($(this).next().is(':hidden')){
			$('.accordionHeader').next().slideUp();
			$(this).next().slideDown();
			}
			return false;
			});
			
});
	
	
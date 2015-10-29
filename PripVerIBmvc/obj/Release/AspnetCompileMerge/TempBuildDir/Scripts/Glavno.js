$(document).ready(function(){
    
    $('#dp3').datepicker('setValue', new Date());
    $('#gotvi').click(function() {
        var $btn = $(this);
        $btn.button('loading');
        // simulating a timeout
        setTimeout(function () {
            $btn.button('reset');
        }, 60000);
    });
});
$(document).ready(function () {
    $("#btnSubmit").click(function () {
       
        $.ajax({
            url: "/_GridListInvoices",
            type: "POST",
            success:function(msg){
                gvInv.Refresh();
            },
                error: function(){
                    alert("Error while loading invoices!");
                }
        });
    })
    function showPaymentPopup(transId) {
        var data = {transId : transId}
        $.ajax({
            url: ajaxUrl,
            type: "POST",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (response) {
                pcModalMode.Show();
            }
        });
    }
    //var baseUrl = '';
    //var FromEndDate = new Date();
    //$('#invoice-start-date, #invoice-end-date, #payment-start-date, #payment-end-date, #payment-date, #payment-date-n').datepicker({
    //    endDate: FromEndDate,
    //    autoclose: true,
    //    orientation: "top auto",
    //    todayHighlight: true,
    //    calendarWeeks: true
    //});


});
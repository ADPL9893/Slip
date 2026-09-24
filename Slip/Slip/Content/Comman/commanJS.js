/* Form object Serialize */

$(function () {
    $.fn.serializeObject = function () {
        var o = {};
        var a = this.serializeArray();
        $.each(a, function () {
            if (o[this.name]) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || '');
            } else {
                o[this.name] = this.value || '';
            }
        });
        return o;
    };

    //$('.required-catalog-duplication,.required ').blur(function () {
    //    //if ($(this).val() == "") {
    //    //    $(this).addClass('show-error-red-border');
    //    //}
    //    //else {
    //    //    $(this).removeClass('show-error-red-border');
    //    //}
    //})

    //----------------------------CLear Form / Div Inside declare Input Tag Data------------------------------
    $.fn.clearForm = function () {
        return this.each(function () {
            var type = this.type, tag = this.tagName.toLowerCase();

            if (tag == 'form')
                return $(':input', this).clearForm();
            else if (tag == 'div')
                return $(':input', this).clearForm();

            if (type == 'text' || type == 'password' || tag == 'textarea')
                this.value = '';
            else if (type == 'checkbox' || type == 'radio')
                this.checked = false;
            else if (tag == 'select')
                this.selectedIndex = -1;
        });
    };
    //------------------------------------------END------------------------------------------------
    

    $('.date-picker-current').datepicker({
        format: "dd-M-yyyy",
        autoclose: true,
        daysOfWeekHighlighted: "0",
        daysOfWeekDisabled: "",
        // todayHighlight: true,
    }).datepicker("setDate", "0");



    $('.input-daterange').datepicker({
        format: "dd-M-yyyy",
        autoclose: true,
        daysOfWeekHighlighted: "0",
        daysOfWeekDisabled: ""

    });

});
/*-- end function -- */

/* -- toaster Message -- */

function toaster_message(priority, title, message) {
    toastr[priority](message, title)
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": true,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }

}
function RealTimemessages(priority, Message) {
    toastr[priority](Message + "<br /><br /><button type='button' class='btn clear'>Got It??</button>")
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": true,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": 0,
        "extendedTimeOut": 0,
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut",
        "tapToDismiss": false
    }

}



function loaderShow() {
    //setTimeout(function () {
    //    $.fancybox.close();
    //}, 100);

    $.blockUI({
        message: $('#displayBox'),
        css: {
            top: ($(window).height() - 200) / 2 + 'px',
            left: ($(window).width() - 400) / 2 + 'px',
            width: '400px'
        }
    });


}

function loaderShow1() {
    $.blockUI({
        message: $('#displayBox'),
        css: {
            top: ($(window).height() - 200) / 2 + 'px',
            left: ($(window).width() - 400) / 2 + 'px',
            width: '400px'
        }
    });


}


function loaderHide() {
    setTimeout($.unblockUI, 0);
}
function DateFormate(d) {
    return $.datepicker.formatDate('dd/mm/yy', new Date(d));
}
function jsonDateFormate(Jsondate) {
    return $.datepicker.formatDate('mm/dd/yy', new Date(Date(parseInt(Jsondate.substr(6)))));

}



function getDisplayRecord() {
    if ($(window).height() > 2100) {
        return 50;
    } else if ($(window).height() > 1300) {
        return 50;
    } else if ($(window).height() > 900) {
        return 50;
    } else if ($(window).height() > 800) {
        return 50;
    } else if ($(window).height() > 700) {
        return 25;
    }
    else {
        return 25;
    }
}



function SetCurrentDate() {
    var m_names = new Array("Jan", "Feb", "Mar",
                             "Apr", "May", "Jun", "Jul", "Aug", "Sep",
                              "Oct", "Nov", "Dec");
    var d = new Date();
    var curr_date = d.getDate();
    var curr_month = d.getMonth();
    var curr_year = d.getFullYear();
    var FinalDate = (curr_date + "-" + m_names[curr_month]
    + "-" + curr_year);

    return FinalDate;
}
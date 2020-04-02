$(function () {

    $(document).on('input', '.length-limit', function () {

        var text = $(this).val();
        var len = text.length;
        var maxlength = $(this).attr('maxlength');

        if (maxlength == null) return;

        if (len > maxlength) {
            $(this).val(text.substring(0, maxlength));
        }

    });

});
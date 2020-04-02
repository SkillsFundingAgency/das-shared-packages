// character limitation script for Add/Edit Apprentice page
$('#charCount').show(); // javascript enabled version only
$('#charCount-noJS').hide(); // javascript disabled version only

// variables
var totalChars = 20, countTextBox = $('#EmployerRef'), charsCountEl = $('#countchars');
charsCountEl.text(totalChars);
var thisChars = countTextBox.val().replace(/{.*}/g, '').length;

// function to trigger on page load
charLeft(countTextBox, thisChars, totalChars);

countTextBox.keyup(function (e) {
    var $this = $(this);
    var thisChars = $this.val().replace(/{.*}/g, '').length;

    if (thisChars > totalChars) {
        $this.val($this.val().slice(0, totalChars));
        $("#charCount").addClass('limit-reached');
    } else {
        charLeft($this, thisChars, totalChars);
    }
});

function charLeft($this, thisChars, totalChars) {
    charsCountEl.text(totalChars - thisChars);
    $("#charCount").removeClass('limit-reached');
}

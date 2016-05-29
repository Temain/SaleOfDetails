var showAlert = function (type, message) {
    var $element = $('.alert-' + type);
    $element.find('.content').text(message);
    $element.fadeTo(2000, 500).slideUp(2000);
};
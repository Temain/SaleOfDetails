$(function () {
    app.initialize();

    // Активировать Knockout
    ko.validation.init({
        decorateInputElement: true,
        errorClass: 'validation-error-message',
        grouping: { observable: false }
    });
    ko.applyBindings(app);
});

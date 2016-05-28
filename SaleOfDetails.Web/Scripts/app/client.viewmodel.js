var ClientViewModel = function (app, dataModel) {
    var self = this;

    Sammy(function () {
        this.get('#clients', function () {
            app.view(self);
        });
    });

    return self;
}
 
app.addViewModel({
    name: "Client",
    bindingMemberName: "clients",
    factory: ClientViewModel
});
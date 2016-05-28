var ReportViewModel = function (app, dataModel) {
    var self = this;

    Sammy(function () {
        this.get('#reports', function () {
            app.view(self);
        });
    });

    return self;
}
 
app.addViewModel({
    name: "Report",
    bindingMemberName: "reports",
    factory: ReportViewModel
});
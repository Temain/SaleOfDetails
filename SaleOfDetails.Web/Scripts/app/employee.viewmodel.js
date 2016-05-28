var EmployeeViewModel = function (app, dataModel) {
    var self = this;

    self.list = ko.observableArray([]);

    Sammy(function () {
        this.get('#employees', function () {
            $.ajax({
                method: 'get',
                url: '/api/Employee',
                contentType: "application/json; charset=utf-8",
                headers: {
                    'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
                },
                success: function (data) {
                    ko.mapping.fromJS(data, {}, self.list);
                    app.view(self);
                }
            });
        });
    });

    return self;
}

app.addViewModel({
    name: "Employee",
    bindingMemberName: "employees",
    factory: EmployeeViewModel
});
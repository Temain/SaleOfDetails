﻿var EmployeeViewModel = function (app, dataModel) {
    var self = this;

    self.list = ko.observableArray([]);

    Sammy(function () {
        this.get('#employee', function () {
            $.ajax({
                method: 'get',
                url: '/api/Employee',
                contentType: "application/json; charset=utf-8",
                headers: {
                    'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
                },
                success: function (response) {
                    ko.mapping.fromJS(response, {}, self.list);
                    app.view(self);
                }
            });
        });
    });

    self.removeEmployee = function (employee) {
        $.ajax({
            method: 'delete',
            url: '/api/Employee/' + employee.employeeId(),
            data: JSON.stringify(ko.toJS(self)),
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
            },
            success: function (response) {
                self.list.remove(employee);
                showAlert('success', 'Сотрудник успешно удалён.');
            }
        });
    }

    return self;
}

var EditEmployeeViewModel = function(app, dataModel) {
    var self = this;

    self.lastName = ko.observable().extend({
        required: {
            params: true,
            message: "Необходимо указать фамилию."
        }
    });
    self.firstName = ko.observable().extend({
        required: {
            params: true,
            message: "Необходимо указать имя."
        }
    });

    self.save = function () {
        var result = ko.validation.group(self, { deep: true });
        if (!self.isValid()) {
            result.showAllMessages(true);

            return false;
        }

        $.ajax({
            method: 'put',
            url: '/api/Employee/',
            data: JSON.stringify(ko.toJS(self)),
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
            },
            success: function (response) {
                app.navigateToEmployee();
                showAlert('success', 'Изменения успешно сохранены.');
            }
        });
    }

    Sammy(function () {
        this.get('#employee/:id', function () {
            var id = this.params['id'];
            if (id === 'create') {
                app.view(app.Views.CreateEmployee);
            } else {
                $.ajax({
                    method: 'get',
                    url: '/api/Employee/' + id,
                    contentType: "application/json; charset=utf-8",
                    headers: {
                        'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
                    },
                    success: function (response) {
                        ko.mapping.fromJS(response, {}, self);
                        app.view(self);
                    }
                });
            }
        });
    });
}

var CreateEmployeeViewModel = function (app, dataModel) {
    var self = this;

    self.lastName = ko.observable().extend({
        required: {
            params: true,
            message: "Необходимо указать фамилию."
        }
    });
    self.firstName = ko.observable().extend({
        required: {
            params: true,
            message: "Необходимо указать имя."
        }
    });
    self.middleName = ko.observable();
    self.employeeDateStart = ko.observable(moment());

    //self.validationObject = ko.validatedObservable({
    //    lastName: self.lastName.extend({
    //        required: {
    //            params: true,
    //            message: "Необходимо указать фамилию."
    //        }
    //    }),
    //    firstName: self.firstName.extend({
    //        required: {
    //            params: true,
    //            message: "Необходимо указать имя."
    //        }
    //    })
    //});

    self.save = function() {
        var result = ko.validation.group(self, { deep: true });
        if (!self.isValid()) {
            result.showAllMessages(true);

            return false;
        }

        $.ajax({
            method: 'post',
            url: '/api/Employee/',
            data: JSON.stringify(ko.toJS(self)),
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
            },
            error: function(response) {
                // showAlert('danger', 'Произошла ошибка при добавлении сотрудника. Обратитесь в службу технической поддержки.');
            },
            success: function (response) {
                self.lastName('');
                self.firstName('');
                self.middleName('');
                self.employeeDateStart('');

                result.showAllMessages(false);

                app.navigateToEmployee();
                showAlert('success', 'Сотрудник успешно добавлен.');
            }
        });
    }
}

app.addViewModel({
    name: "Employee",
    bindingMemberName: "employee",
    factory: EmployeeViewModel
});

app.addViewModel({
    name: "EditEmployee",
    bindingMemberName: "editEmployee",
    factory: EditEmployeeViewModel
});

app.addViewModel({
    name: "CreateEmployee",
    bindingMemberName: "createEmployee",
    factory: CreateEmployeeViewModel
});
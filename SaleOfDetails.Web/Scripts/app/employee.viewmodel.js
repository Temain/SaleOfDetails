var EmployeeViewModel = function (app, dataModel) {
    var self = this;

    self.list = ko.observableArray([]);
    self.selectedPage = ko.observable(1);
    self.pageSizes = ko.observableArray([10, 25, 50, 100, 200]);
    self.selectedPageSize = ko.observable(10);
    self.employeesCount = ko.observable();
    self.pagesCount = ko.observable();

    self.selectedPageChanged = function (page) {
        if (page > 0 && page <= self.pagesCount()) {
            self.selectedPage(page);
            self.loadEmployees();

            window.scrollTo(0, 0);
        }
    }

    self.pageSizeChanged = function () {
        self.selectedPage(1);
        self.loadEmployees();

        window.scrollTo(0, 0);
    };

    Sammy(function () {
        this.get('#employee', function () {
            app.markLinkAsActive('employee');

            self.loadEmployees();
        });
    });

    self.loadEmployees = function () {
        $.ajax({
            method: 'get',
            url: '/api/Employee',
            data: { page: self.selectedPage(), pageSize: self.selectedPageSize() },
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
            },
            success: function (response) {
                ko.mapping.fromJS(response.items, {}, self.list);
                self.pagesCount(response.pagesCount);
                self.employeesCount(response.itemsCount);
                app.view(self);
            }
        });
    }

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
            app.markLinkAsActive('employee');

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
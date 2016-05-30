var ClientViewModel = function (app, dataModel) {
    var self = this;

    self.list = ko.observableArray([]);
    self.selectedPage = ko.observable(1);
    self.pageSizes = ko.observableArray([10, 25, 50, 100, 200]);
    self.selectedPageSize = ko.observable(10);
    self.clientsCount = ko.observable();
    self.pagesCount = ko.observable();

    self.selectedPageChanged = function (page) {
        if (page > 0 && page <= self.pagesCount()) {
            self.selectedPage(page);
            self.loadClients();

            window.scrollTo(0, 0); 
        }
    }

    self.pageSizeChanged = function () {
        self.selectedPage(1);
        self.loadClients();

        window.scrollTo(0, 0);
    };

    Sammy(function () {
        this.get('#client', function () {
            app.markLinkAsActive('client');

            self.loadClients();
        });
    });

    self.loadClients = function () {
        $.ajax({
            method: 'get',
            url: '/api/Client',
            data: { page: self.selectedPage(), pageSize: self.selectedPageSize() },
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
            },
            success: function (response) {
                ko.mapping.fromJS(response.items, {}, self.list);
                self.pagesCount(response.pagesCount);
                self.clientsCount(response.itemsCount);
                app.view(self);
            }
        });
    }

    self.removeClient = function (client) {
        $.ajax({
            method: 'delete',
            url: '/api/Client/' + client.clientId(),
            data: JSON.stringify(ko.toJS(self)),
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
            },
            success: function (response) {
                self.list.remove(client);
                showAlert('success', 'Клиент успешно удалён.');
            }
        });
    }

    return self;
}

var EditClientViewModel = function (app, dataModel) {
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
            url: '/api/Client/',
            data: JSON.stringify(ko.toJS(self)),
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
            },
            success: function (response) {
                app.navigateToClient();
                showAlert('success', 'Изменения успешно сохранены.');
            }
        });
    }

    Sammy(function () {
        this.get('#client/:id', function () {
            app.markLinkAsActive('client');

            var id = this.params['id'];
            if (id === 'create') {
                app.view(app.Views.CreateClient);
            } else {
                $.ajax({
                    method: 'get',
                    url: '/api/Client/' + id,
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

var CreateClientViewModel = function (app, dataModel) {
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
    self.phone = ko.observable();

    self.save = function () {
        var result = ko.validation.group(self, { deep: true });
        if (!self.isValid()) {
            result.showAllMessages(true);

            return false;
        }

        $.ajax({
            method: 'post',
            url: '/api/Client/',
            data: JSON.stringify(ko.toJS(self)),
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
            },
            error: function (response) {
                // showAlert('danger', 'Произошла ошибка при добавлении сотрудника. Обратитесь в службу технической поддержки.');
            },
            success: function (response) {
                self.lastName('');
                self.firstName('');
                self.middleName('');
                self.phone('');

                result.showAllMessages(false);

                app.navigateToClient();
                showAlert('success', 'Клиент успешно добавлен.');
            }
        });
    }
}
 
app.addViewModel({
    name: "Client",
    bindingMemberName: "client",
    factory: ClientViewModel
});

app.addViewModel({
    name: "EditClient",
    bindingMemberName: "editClient",
    factory: EditClientViewModel
});

app.addViewModel({
    name: "CreateClient",
    bindingMemberName: "createClient",
    factory: CreateClientViewModel
});
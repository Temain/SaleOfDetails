var SaleViewModel = function (app, dataModel) {
    var self = this;

    self.list = ko.observableArray([]);
    self.selectedPage = ko.observable(1);
    self.pageSizes = ko.observableArray([10, 25, 50, 100, 200]);
    self.selectedPageSize = ko.observable(10);
    self.salesCount = ko.observable();
    self.pagesCount = ko.observable();
    self.searchQuery = ko.observable('');

    self.selectedPageChanged = function (page) {
        if (page > 0 && page <= self.pagesCount()) {
            self.selectedPage(page);
            self.loadSales();

            window.scrollTo(0, 0);
        }
    }

    self.pageSizeChanged = function () {
        self.selectedPage(1);
        self.loadSales();

        window.scrollTo(0, 0);
    };

    Sammy(function () {
        this.get('#sale', function () {
            app.markLinkAsActive('sale');

            self.loadSales();
        });

        this.get('/', function () { this.app.runRoute('get', '#sale') });
    });

    self.loadSales = function() {
        $.ajax({
            method: 'get',
            url: '/api/Sale',
            data: { query: self.searchQuery(), page: self.selectedPage(), pageSize: self.selectedPageSize() },
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
            },
            success: function (response) {
                ko.mapping.fromJS(response.items, {}, self.list);
                self.pagesCount(response.pagesCount);
                self.salesCount(response.itemsCount);
                app.view(self);
            }
        });
    }

    self.search = _.debounce(function () {
        self.selectedPage(1);
        self.loadSales();
    }, 300);

    self.removeSale = function (sale) {
        $.ajax({
            method: 'delete',
            url: '/api/Sale/' + sale.saleId(),
            data: JSON.stringify(ko.toJS(self)),
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
            },
            success: function (response) {
                self.list.remove(sale);
                showAlert('success', 'Запись успешно удалёна.');
            }
        });
    }

    return self;
}

var EditSaleViewModel = function(app, dataModel) {
    var self = this;

    self.productId = ko.observable();
    self.productName = ko.observable().extend({
        required: {
            params: true,
            message: "Необходимо указать наименование детали / спецтехники."
        }
    });
    self.productCost = ko.observable(0);
    self.clientId = ko.observable().extend({
        required: {
            params: true,
            message: "Необходимо выбрать покупателя."
        }
    });
    self.clients = ko.observable([]);
    self.employeeId = ko.observable().extend({
        required: {
            params: true,
            message: "Необходимо выбрать продавца."
        }
    });
    self.employees = ko.observable([]);
    self.numberOfProducts = ko.observable();

    self.totalCost = ko.computed(function () {
        return self.numberOfProducts() * self.productCost();
    }, this);


    self.save = function () {
        var result = ko.validation.group(self, { deep: true });
        if (!self.isValid()) {
            result.showAllMessages(true);

            return false;
        }

        var postData = {
            saleId : self.saleId(),
            productId: self.productId(),
            clientId: self.clientId(),
            employeeId: self.employeeId(),
            numberOfProducts: self.numberOfProducts(),
            saleDate: self.saleDate()
        };

        $.ajax({
            method: 'put',
            url: '/api/Sale/',
            data: JSON.stringify(postData),
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
            },
            success: function (response) {
                app.navigateToSale();
                showAlert('success', 'Изменения успешно сохранены.');
            }
        });
    }

    Sammy(function () {
        this.get('#sale/:id', function () {
            app.markLinkAsActive('sale');

            var id = this.params['id'];
            if (id === 'create') {
                $.ajax({
                    method: 'get',
                    url: '/api/Sale/0',
                    contentType: "application/json; charset=utf-8",
                    headers: {
                        'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
                    },
                    success: function (response) {
                        ko.mapping.fromJS(response, {}, app.Views.CreateSale);
                        app.view(app.Views.CreateSale);
                        app.Views.CreateSale.isValidationEnabled(false);
                    }
                });
            } else {
                $.ajax({
                    method: 'get',
                    url: '/api/Sale/' + id,
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

var CreateSaleViewModel = function (app, dataModel) {
    var self = this;
    self.isValidationEnabled = ko.observable(false);

    self.productId = ko.observable();
    self.productName = ko.observable().extend({
        required: {
            params: true,
            message: "Необходимо указать наименование детали / спецтехники.",
            onlyIf: function () {
                return self.isValidationEnabled();
            }
        }
    });
    self.productCost = ko.observable(0);
    self.clientId = ko.observable().extend({
        required: {
            params: true,
            message: "Необходимо выбрать покупателя.",
            onlyIf: function () {
                return self.isValidationEnabled();
            }
        }
    });
    self.clients = ko.observable([]);
    self.employeeId = ko.observable().extend({
        required: {
            params: true,
            message: "Необходимо выбрать продавца.",
            onlyIf: function () {
                return self.isValidationEnabled();
            }
        }
    });
    self.employees = ko.observable([]);
    self.numberOfProducts = ko.observable();
    self.saleDate = ko.observable(moment());

    self.totalCost = ko.computed(function () {
        return self.numberOfProducts() * self.productCost();
    }, this);

    self.save = function () {
        self.isValidationEnabled(true);
        var result = ko.validation.group(self, { deep: true });
        if (!self.isValid()) {
            result.showAllMessages(true);

            return false;
        }

        var postData = {
            productId: self.productId(),
            clientId: self.clientId(),
            employeeId: self.employeeId(),
            numberOfProducts: self.numberOfProducts(),
            saleDate: self.saleDate()
        };

        $.ajax({
            method: 'post',
            url: '/api/Sale/',
            data: JSON.stringify(postData),
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
            },
            error: function(response) {
                // showAlert('danger', 'Произошла ошибка при добавлении сотрудника. Обратитесь в службу технической поддержки.');
            },
            success: function (response) {
                self.productId('');
                self.clientId('');
                self.employeeId('');
                self.numberOfProducts('');
                self.saleDate('');

                result.showAllMessages(false);

                app.navigateToSale();
                showAlert('success', 'Запись успешно добавлена.');
            }
        });
    }
}

app.addViewModel({
    name: "Sale",
    bindingMemberName: "sale",
    factory: SaleViewModel
});

app.addViewModel({
    name: "EditSale",
    bindingMemberName: "editSale",
    factory: EditSaleViewModel
});

app.addViewModel({
    name: "CreateSale",
    bindingMemberName: "createSale",
    factory: CreateSaleViewModel
});
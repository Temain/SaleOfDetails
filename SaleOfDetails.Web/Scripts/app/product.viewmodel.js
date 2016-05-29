var ProductViewModel = function (app, dataModel) {
    var self = this;

    self.list = ko.observableArray([]);

    Sammy(function () {
        this.get('#product', function () {
            $.ajax({
                method: 'get',
                url: '/api/Product',
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

    self.removeProduct = function (product) {
        $.ajax({
            method: 'delete',
            url: '/api/Product/' + product.productId(),
            data: JSON.stringify(ko.toJS(self)),
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
            },
            success: function (response) {
                self.list.remove(product);
                showAlert('success', 'Товар успешно удалён.');
            }
        });
    }

    return self;
}

var EditProductViewModel = function(app, dataModel) {
    var self = this;

    self.productName = ko.observable().extend({
        required: {
            params: true,
            message: "Необходимо указать название."
        }
    });
    self.cost = ko.observable().extend({
        required: {
            params: true,
            message: "Необходимо указать цену."
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
            url: '/api/Product/',
            data: JSON.stringify(ko.toJS(self)),
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
            },
            success: function (response) {
                app.navigateToProduct();
                showAlert('success', 'Изменения успешно сохранены.');
            }
        });
    }

    Sammy(function () {
        this.get('#product/:id', function () {
            var id = this.params['id'];
            if (id === 'create') {
                app.view(app.Views.CreateProduct);
            } else {
                $.ajax({
                    method: 'get',
                    url: '/api/Product/' + id,
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

var CreateProductViewModel = function (app, dataModel) {
    var self = this;

    self.productName = ko.observable().extend({
        required: {
            params: true,
            message: "Необходимо указать название."
        }
    });
    self.cost = ko.observable().extend({
        required: {
            params: true,
            message: "Необходимо указать цену."
        }
    });
    self.inStock = ko.observable();

    self.save = function() {
        var result = ko.validation.group(self, { deep: true });
        if (!self.isValid()) {
            result.showAllMessages(true);

            return false;
        }

        $.ajax({
            method: 'post',
            url: '/api/Product/',
            data: JSON.stringify(ko.toJS(self)),
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
            },
            error: function(response) {
                // showAlert('danger', 'Произошла ошибка при добавлении сотрудника. Обратитесь в службу технической поддержки.');
            },
            success: function (response) {
                self.productName('');
                self.cost('');
                self.inStock('');

                result.showAllMessages(false);

                app.navigateToProduct();
                showAlert('success', 'Товар успешно добавлен.');
            }
        });
    }
}

app.addViewModel({
    name: "Product",
    bindingMemberName: "product",
    factory: ProductViewModel
});

app.addViewModel({
    name: "EditProduct",
    bindingMemberName: "editProduct",
    factory: EditProductViewModel
});

app.addViewModel({
    name: "CreateProduct",
    bindingMemberName: "createProduct",
    factory: CreateProductViewModel
});
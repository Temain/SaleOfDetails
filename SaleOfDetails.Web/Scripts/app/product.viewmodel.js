var ProductViewModel = function (app, dataModel) {
    var self = this;

    self.list = ko.observableArray([]);
    self.selectedPage = ko.observable(1);
    self.pageSizes = ko.observableArray([10, 25, 50, 100, 200]);
    self.selectedPageSize = ko.observable(10);
    self.productsCount = ko.observable();
    self.pagesCount = ko.observable();

    self.selectedPageChanged = function (page) {
        if (page > 0 && page <= self.pagesCount()) {
            self.selectedPage(page);
            self.loadProducts();

            window.scrollTo(0, 0);
        }
    }

    self.pageSizeChanged = function () {
        self.selectedPage(1);
        self.loadProducts();

        window.scrollTo(0, 0);
    };

    Sammy(function () {
        this.get('#product', function () {
            app.markLinkAsActive('product');

            self.loadProducts();
        });
    });

    self.loadProducts = function () {
        $.ajax({
            method: 'get',
            url: '/api/Product',
            data: { page: self.selectedPage(), pageSize: self.selectedPageSize() },
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
            },
            success: function (response) {
                ko.mapping.fromJS(response.items, {}, self.list);
                self.pagesCount(response.pagesCount);
                self.productsCount(response.itemsCount);
                app.view(self);
            }
        });
    }

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
            app.markLinkAsActive('product');

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
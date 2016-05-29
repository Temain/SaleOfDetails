/* Биндинги
------------------------------------------------------------*/
ko.bindingHandlers.selectPicker = {
    after: ['options'],
    init: function (element, valueAccessor, allBindingsAccessor) {
        if ($(element).is('select')) {
            if (ko.isObservable(valueAccessor())) {
                if ($(element).prop('multiple') && $.isArray(ko.utils.unwrapObservable(valueAccessor()))) {
                    // in the case of a multiple select where the valueAccessor() is an observableArray, call the default Knockout selectedOptions binding
                    ko.bindingHandlers.selectedOptions.init(element, valueAccessor, allBindingsAccessor);
                } else {
                    // regular select and observable so call the default value binding
                    ko.bindingHandlers.value.init(element, valueAccessor, allBindingsAccessor);
                }
            }

            $(element).addClass('selectpicker').selectpicker();
        }
    },
    update: function (element, valueAccessor, allBindingsAccessor) {
        if ($(element).is('select')) {
            var selectPickerOptions = allBindingsAccessor().selectPickerOptions;
            if (typeof selectPickerOptions !== 'undefined' && selectPickerOptions !== null) {
                var options = selectPickerOptions.optionsArray,
                    optionsText = selectPickerOptions.optionsText,
                    optionsValue = selectPickerOptions.optionsValue,
                    optionsCaption = selectPickerOptions.optionsCaption,
                    isDisabled = selectPickerOptions.disabledCondition || false,
                    resetOnDisabled = selectPickerOptions.resetOnDisabled || false;
                if (ko.utils.unwrapObservable(options).length > 0) {
                    // call the default Knockout options binding
                    ko.bindingHandlers.options.update(element, options, allBindingsAccessor);
                }
                if (isDisabled && resetOnDisabled) {
                    // the dropdown is disabled and we need to reset it to its first option
                    $(element).selectpicker('val', $(element).children('option:first').val());
                }
                $(element).prop('disabled', isDisabled);
            }
            if (ko.isObservable(valueAccessor())) {
                if ($(element).prop('multiple') && $.isArray(ko.utils.unwrapObservable(valueAccessor()))) {
                    // in the case of a multiple select where the valueAccessor() is an observableArray, call the default Knockout selectedOptions binding
                    ko.bindingHandlers.selectedOptions.update(element, valueAccessor);
                } else {
                    // call the default Knockout value binding
                    ko.bindingHandlers.value.update(element, valueAccessor);
                }
            }

            $(element).selectpicker('refresh');
        }
    }
};

ko.bindingHandlers.tooltip = {
    init: function (element, valueAccessor) {
        var local = ko.utils.unwrapObservable(valueAccessor()),
            options = {};

        ko.utils.extend(options, ko.bindingHandlers.tooltip.options);
        ko.utils.extend(options, local);

        $(element).tooltip(options);

        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            $(element).tooltip("destroy");
        });
    },
    options: {
        placement: "bottom",
        trigger: "hover"
    }
};

ko.bindingHandlers.datepicker = {
    init: function(element, valueAccessor, allBindingsAccessor) {
        //initialize datepicker with some optional options
        var options = allBindingsAccessor().datepickerOptions || { format: 'DD/MM/YYYY HH:mm' };
        $(element).datetimepicker(options);

        //when a user changes the date, update the view model
        ko.utils.registerEventHandler(element, "dp.change", function(event) {
            var value = valueAccessor();
            if (ko.isObservable(value)) {
                value(event.date);
            }
        });
    },
    update: function(element, valueAccessor) {
        var widget = $(element).data("DateTimePicker");
        //when the view model is updated, update the widget
        if (widget) {
            var date = ko.utils.unwrapObservable(valueAccessor());
            widget.date(moment(date));
        }
    }
};

ko.bindingHandlers.numeric = {
    init: function (element, valueAccessor) {
        $(element).on("keydown", function (event) {
            // Allow: backspace, delete, tab, escape, and enter
            if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 27 || event.keyCode == 13 ||
                // Allow: Ctrl+A
                (event.keyCode == 65 && event.ctrlKey === true) ||
                // Allow: . ,
                (event.keyCode == 188 || event.keyCode == 190 || event.keyCode == 110) ||
                // Allow: home, end, left, right
                (event.keyCode >= 35 && event.keyCode <= 39)) {
                // let it happen, don't do anything
                return;
            }
            else {
                // Ensure that it is a number and stop the keypress
                if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                    event.preventDefault();
                }
            }
        });
    }
};

ko.bindingHandlers.typeahead = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var $element = $(element);
        // var allBindings = allBindingsAccessor();      
        var unwrappedValue = ko.utils.unwrapObservable(valueAccessor());
        var apiUrl = unwrappedValue.api;
        var elementData = unwrappedValue.data;
        var elementDataId = elementData[unwrappedValue.key];

        var items = {};
        var itemLabels = [];

        var search = _.debounce(function (query, process) {
            $.get(apiUrl, { query: query }, function (data) {

                items = {};
                itemLabels = [];

                // if (query.length > 2) {

                _.each(data, function (item, ix, list) {
                    if (_.contains(items, item.name)) {
                        item.name = item.name + ' #' + item.id;
                    }
                    itemLabels.push(item.name);
                    items[item.name] = {
                        id: item.id,
                        name: item.name,
                        cost: item.cost
                    };
                });

                var labelsCount = Object.keys(itemLabels).length;
                if (labelsCount === 0) {
                    $element.siblings('.msg-text').slideDown(250);
                    elementDataId("");
                    elementData.productCost(0);
                } else {
                    $element.siblings('.msg-text').slideUp(250);
                }

                process(itemLabels);
                // }

                if (query.length === 0) $element.siblings('.msg-text').slideUp(250);
            });
        }, 300);

        var options = {
            source: function (query, process) {
                search(query, process);
            },
            updater: function (item) {
                elementDataId(items[item].id);
                elementData.productCost(items[item].cost);

                return item;
            },
            matcher: function (item) {
                if (item.toLowerCase().indexOf(this.query.trim().toLowerCase()) !== -1) {
                    elementDataId(items[item].id);
                    elementData.productCost(items[item].cost);

                    return item;
                }

                elementDataId("");
                return this.query;
            }
            //highlighter: function (item) {
            //    var discipline = items[item];
            //    var template = ''
            //        + "<div class='typeahead_wrapper'>"
            //        + "<div class='typeahead_labels'>"
            //        + "<div class='typeahead_primary'>" + discipline.DisciplineName + "</div>"
            //        + "<div class='typeahead_secondary'>" + discipline.ChairName + "</div>"
            //        + "</div>"
            //        + "</div>";
            //    return template;
            //}
        };

        $element
            .attr('autocomplete', 'off')
            .typeahead(options);
    }
};
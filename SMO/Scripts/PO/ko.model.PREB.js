ko.bindingHandlers.inputmask =
{
    init: function (element, valueAccessor, allBindingsAccessor) {

        var mask = valueAccessor();

        var observable = mask.value;

        if (ko.isObservable(observable)) {

            $(element).on('focusout change', function () {

                if ($(element).inputmask('isComplete')) {
                    observable($(element).val());
                } else {
                    observable(null);
                }

            });
        }

        $(element).inputmask(mask);


    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var mask = valueAccessor();

        var observable = mask.value;

        if (ko.isObservable(observable)) {

            var valuetoWrite = observable();

            $(element).val(valuetoWrite);
        }
    }

};

var IsChange = false;
function CheckChange(obj) {
    var p = ko.computed(function () {
        return ko.toJS(obj);
    });
    p.subscribe(function (value) {
        IsChange = true;
    });
}

function AddDetail(materialCode, materialText, unitCode, price, currencyCode) {
    model.AddDetail(number, materialCode, materialText, unitCode, price, currencyCode);
    number = number + 1;
    Forms.CompleteUI();
}

function RemoveDetail(obj) {
    model.RemoveDetail($(obj).attr("order"));
}

var Detail = function (order, marterialCode, marterialText, quantity, unitCode, price, currencyCode) {
    var self = this;
    self.ORDER = ko.observable(order);
    self.QUANTITY = ko.observable(quantity);
    self.UNIT_CODE = ko.observable(unitCode);
    self.MATERIAL_CODE = ko.observable(marterialCode);
    self.MATERIAL_TEXT = ko.observable(marterialText);
    self.PRICE = ko.observable(price);
    self.SUM_VALUE = ko.computed(function () {
        return self.PRICE() * self.QUANTITY();
    }, this);
    self.CURRENCY_CODE = ko.observable(currencyCode);
};

var Model = function () {
    var self = this;
    this.ObjHeader = new Object();
    this.ObjListDetail = ko.observableArray([]);
    this.ObjListDetail.removeAll();
    /*
    ---------------------------------------------------------------------------------
    - Thêm mới detail
    ---------------------------------------------------------------------------------
    */
    this.AddDetail = function (order, materialCode, materialText, unitCode, price, currencyCode) {
        if (self.ObjListDetail().filter(function (data) {
            return data.MATERIAL_CODE() === materialCode;
        }).length !== 0) {
            //Message.func.AlertDanger({ Message: { Code: "", Message: "Mặt hàng này đã được chọn!" } });
            return false;
        }

        self.ObjListDetail.push(new Detail(order, materialCode, materialText, 1, unitCode, price, currencyCode));
        Forms.CompleteUI();
    };

    /*
    ---------------------------------------------------------------------------------
    - Xóa detail
    ---------------------------------------------------------------------------------
    */
    this.RemoveDetail = function (order) {
        self.ObjListDetail.remove(function (item) {
            return item.ORDER() == order;
        });
    };
}
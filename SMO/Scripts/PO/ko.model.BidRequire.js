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

function AddDetail(materialCode, materialText, quantity, unitCode, sumValue, currencyCode) {
    model.AddDetail(number, materialCode, materialText, quantity, unitCode, sumValue, currencyCode);
    number = number + 1;
    Forms.CompleteUI();
}

function RemoveDetail(obj) {
    model.RemoveDetail($(obj).attr("order"));
}

function AddPR(code, name, type, listDetail) {
    model.AddPR(code, name, type, listDetail);
}

function RemovePR(obj) {
    model.RemovePR($(obj).attr("code"));
}

var Detail = function (order, marterialCode, marterialText, quantity, unitCode, sumValue, currencyCode) {
    var self = this;
    self.ORDER = ko.observable(order);
    self.QUANTITY = ko.observable(quantity);
    self.UNIT_CODE = ko.observable(unitCode);
    self.MATERIAL_CODE = ko.observable(marterialCode);
    self.MATERIAL_TEXT = ko.observable(marterialText);
    self.SUM_VALUE = ko.observable(sumValue);
    self.CURRENCY_CODE = ko.observable(currencyCode);
};

var PR = function (code, name, type) {
    var self = this;
    self.PR_CODE = ko.observable(code);
    self.TYPE = ko.observable(type);
    self.PR_NAME = ko.observable(name);
    self.ONCLICK = ko.computed(function () {
        return "ShowDetailPR('" + self.TYPE() + "','" + self.PR_CODE() + "')";
    }, this);
    self.ObjListDetail = ko.observableArray([]);
}

var Model = function () {
    var self = this;
    this.ObjHeader = new Object();
    this.ObjListDetail = ko.observableArray([]);
    this.ObjListPR = ko.observableArray([]);
    this.ObjListDetail.removeAll();
    this.ObjListPR.removeAll();

    /*
    ---------------------------------------------------------------------------------
    - Thêm mới PR
    ---------------------------------------------------------------------------------
    */

    this.AddPR = function (code, name, type, listDetail) {
        var find = self.ObjListPR().filter(function (data) {
            return data.PR_CODE() === code;
        });

        if (find.length == 0) {
            var pr = new PR(code, name, type);
            $.each(listDetail, function () {
                var detail = this;
                number = number + 1;
                pr.ObjListDetail.push(new Detail(number, detail.MATERIAL_CODE, detail.MATERIAL_NAME, detail.QUANTITY, detail.UNIT_CODE, detail.SUM_VALUE, detail.CURRENCY_CODE));
                self.AddDetail(number, detail.MATERIAL_CODE, detail.MATERIAL_NAME, detail.QUANTITY, detail.UNIT_CODE, detail.SUM_VALUE, detail.CURRENCY_CODE);
            });
            self.ObjListPR.push(pr);
        }
    }

    /*
    ---------------------------------------------------------------------------------
    - Xóa PR
    ---------------------------------------------------------------------------------
    */
    this.RemovePR = function (code) {
        var find = self.ObjListPR().filter(function (data) {
            return data.PR_CODE() === code;
        });

        if (find.length !== 0) {
            $.each(find[0].ObjListDetail(), function () {
                var detailPR = this;
                var findDetail = self.ObjListDetail().filter(function (data) {
                    return data.MATERIAL_CODE() === detailPR.MATERIAL_CODE();
                });
                if (findDetail.length !== 0) {
                    if (findDetail[0].QUANTITY() - detailPR.QUANTITY() > 0) {
                        findDetail[0].QUANTITY(findDetail[0].QUANTITY() - detailPR.QUANTITY());
                        findDetail[0].SUM_VALUE(findDetail[0].SUM_VALUE() - detailPR.SUM_VALUE());
                    } else {
                        self.ObjListDetail.remove(findDetail[0]);
                    }
                }
            });
            self.ObjListPR.remove(find[0]);
        }
    };

    /*
    ---------------------------------------------------------------------------------
    - Thêm mới detail
    ---------------------------------------------------------------------------------
    */
    this.AddDetail = function (order, materialCode, materialText, quantity, unitCode, sumValue, currencyCode) {
        var find = self.ObjListDetail().filter(function (data) {
            return data.MATERIAL_CODE() === materialCode;
        });

        if (find.length !== 0) {
            find[0].QUANTITY(find[0].QUANTITY() + quantity);
            find[0].SUM_VALUE(find[0].SUM_VALUE() + sumValue);
        }
        else {
            self.ObjListDetail.push(new Detail(order, materialCode, materialText, quantity, unitCode, sumValue, currencyCode));
            Forms.CompleteUI();
        }
    };

    /*
    ---------------------------------------------------------------------------------
    - Xóa detail
    ---------------------------------------------------------------------------------
    */
    this.RemoveDetail = function (order) {
        debugger;
        self.ObjListDetail.remove(function (item) {
            return item.ORDER() == order;
        });
    };
}
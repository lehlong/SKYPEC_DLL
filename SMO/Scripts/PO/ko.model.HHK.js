var IsChange = false;
function CheckChange(obj) {
    var p = ko.computed(function () {
        return ko.toJS(obj);
    });
    p.subscribe(function (value) {
        IsChange = true;
    });
}

function AddDetail(materialCode, materialText, unitCode) {
    model.AddDetail(number, materialCode, materialText, unitCode);
    number = number + 1;
    Forms.CompleteUI();
}

function RemoveDetail(obj) {
    model.RemoveDetail($(obj).attr("order"));
}

var Detail = function (order, marterialCode, marterialText, quantity, approveQuantity, unitCode) {
    var self = this;
    self.ORDER = ko.observable(order);
    self.QUANTITY = ko.observable(quantity);
    self.APPROVE_QUANTITY = ko.observable(approveQuantity);
    self.UNIT_CODE = ko.observable(unitCode);
    self.MATERIAL_CODE = ko.observable(marterialCode);
    self.MATERIAL_TEXT = ko.observable(marterialText);
    self.ClickAppove = function () {
        self.APPROVE_QUANTITY(self.QUANTITY());
    }
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
    this.AddDetail = function (order, materialCode, materialText, unitCode) {
        if (self.ObjListDetail().filter(function (data) {
            return data.MATERIAL_CODE() === materialCode;
        }).length !== 0) {
            //Message.func.AlertDanger({ Message: { Code: "", Message: "Mặt hàng này đã được chọn!" } });
            return false;
        }

        self.ObjListDetail.push(new Detail(order, materialCode, materialText, 1, 0, unitCode));
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
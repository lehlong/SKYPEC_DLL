var IsChange = false;
function CheckChange(obj) {
    var p = ko.computed(function () {
        return ko.toJS(obj);
    });
    p.subscribe(function (value) {
        IsChange = true;
    });
}

function OnChangeSpecType(obj) {
    $("#divDetailSpec").hide();
    var type = $(obj).val();
    if (type == "NUMBER" || type == "SELECT") {
        $("#divDetailSpec").show();
    }
    model.RemoveAllDetail();
}

function AddDetail() {
    model.AddDetail(number, 0, 0, "","", 0,"");
    number = number + 1;
    Forms.CompleteUI();
}

function RemoveDetail(obj) {
    model.RemoveDetail($(obj).attr("order"));
}

var Detail = function (order, fromValue, toValue, value, value_en, score, pkId) {
    var self = this;
    self.C_ORDER = ko.observable(order);
    self.FROM_VALUE = ko.observable(fromValue);
    self.TO_VALUE = ko.observable(toValue);
    self.VALUE = ko.observable(value);
    self.VALUE_EN = ko.observable(value_en);
    self.SCORE = ko.observable(score);
    self.PKID = ko.observable(pkId);
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
    this.AddDetail = function (order, fromValue, toValue, value, value_en, score, pkId) {
        self.ObjListDetail.push(new Detail(order, fromValue, toValue, value, value_en, score, pkId));
        Forms.CompleteUI();
    };

    /*
    ---------------------------------------------------------------------------------
    - Xóa detail
    ---------------------------------------------------------------------------------
    */
    this.RemoveDetail = function (order) {
        self.ObjListDetail.remove(function (item) {
            return item.C_ORDER() == order;
        });
    };

    /*
    ---------------------------------------------------------------------------------
    - Xóa tất cả detail
    ---------------------------------------------------------------------------------
    */
    this.RemoveAllDetail = function () {
        self.ObjListDetail.removeAll();
    };
}
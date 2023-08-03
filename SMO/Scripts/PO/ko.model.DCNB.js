﻿var IsChange = false;
function CheckChange(obj) {
    var p = ko.computed(function () {
        return ko.toJS(obj);
    });
    p.subscribe(function (value) {
        IsChange = true;
    });
}

function OnChangeVehicle(obj) {
    if ($(obj).find(':selected').data('oic-pbatch') != "" && $(obj).find(':selected').data('oic-pbatch') != null && $(obj).find(':selected').data('oic-pbatch') != undefined) {
        model.ObjHeader.OIC_PBATCH($(obj).find(':selected').data('oic-pbatch'));
    }

    if ($(obj).find(':selected').data('oic-ptrip') != "" && $(obj).find(':selected').data('oic-ptrip') != null && $(obj).find(':selected').data('oic-ptrip') != undefined) {
        model.ObjHeader.OIC_PTRIP($(obj).find(':selected').data('oic-ptrip'));
    }

    if ($(obj).find(':selected').data('transmode') != "" && $(obj).find(':selected').data('transmode') != null && $(obj).find(':selected').data('transmode') != undefined) {
        model.ObjHeader.TRANSMODE_CODE($(obj).find(':selected').data('transmode'));
    }
    Forms.CompleteUI();
    ValidateCapacity();
}

function ValidateCapacity() {
    $("#lblNoti").html("");
    $("#tbodyNoti").hide();

    var capacity = $("#dllVehicle").find(':selected').data('capacity');
    if (capacity == undefined || capacity == "" || capacity == "0") {
        return;
    }

    //Tính tổng lượng đặt
    var sum = model.ObjListDetail().reduce(function (prev, cur) {
        return prev + Number(cur.QUANTITY());
    }, 0);

    if (sum > Number(capacity)) {
        var noti = "CHÚ Ý: TỔNG LƯỢNG ĐẶT (" + sum + ") ĐÃ VƯỢT QUÁ DUNG TÍCH XE (" + capacity + ")";
        $("#lblNoti").html(noti);
        $("#tbodyNoti").show();
    }
}

function AddDetail(material) {
    model.AddDetail(number, material);
    number = number + 1;
    ValidateCapacity();
}

function RemoveDetail(obj) {
    model.RemoveDetail($(obj).attr("order"));
    ValidateCapacity();
}

var Detail = function (order, marterialCode, quantity, approveQuantity, unitCode) {
    var self = this;
    self.ORDER = ko.observable(order);
    self.QUANTITY = ko.observable(quantity);
    self.QUANTITY.subscribe(function (newValue) {
        ValidateCapacity();
    });
    self.APPROVE_QUANTITY = ko.observable(approveQuantity);
    self.UNIT_CODE = ko.observable(unitCode);
    self.MATERIAL_CODE = ko.observable(marterialCode);
    self.MATERIAL_TEXT = ko.computed(function () {
        var find = lstMaterial.filter(function (data) {
            return data.CODE == self.MATERIAL_CODE();
        });
        if (find.length != 0) {
            return find[0].TEXT;
        }
    }, this);
};

var Model = function () {
    var self = this;
    this.ObjHeader = new Object();
    this.ObjListDetail = ko.observableArray([]);
    this.ObjListStoSyn = ko.observableArray([]);
    this.ObjListDetail.removeAll();

    /*
    ---------------------------------------------------------------------------------
    - Thêm mới detail
    ---------------------------------------------------------------------------------
    */
    this.AddDetail = function (order, material) {
        if (self.ObjListDetail().filter(function (data) {
            return data.MATERIAL_CODE() === material;
        }).length !== 0) {
            Message.func.AlertDanger({ Message: { Code: "", Message: "Mặt hàng này đã được chọn!" } });
            return false;
        }
        self.ObjListDetail.push(new Detail(order, material, 1000, 0, 'L'));
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
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

    if ($(obj).val() == undefined || $(obj).val() == null || $(obj).val() == "") {
        return;
    }

    var ajaxParams = {
        url: "/PO/PO/GetInfoVehicle",
        type: 'POST',
        dataType: 'json',
        data: { vehicleCode: $(obj).val() },
        success: function (response) {
            var html = "";
            if (response != null) {
                $.each(response, function () {
                    html += "<span class='badge bg-teal m-r-10 m-b-10'>Ngăn " + this.SEQ_NUMBER + " - " + this.CAPACITY + "</span>";
                });
            }
            $("#divCompartment").html(html);
        }
    };
    Forms.Ajax(ajaxParams);
}

function ValidateMaterialStore(material) {
    if (!lstStoreMaterial.some(e => e.MATERIAL_CODE == material)) {
        Message.func.AlertDanger({ Message: { Code: "", Message: "Mặt hàng bạn chọn không có trong kho" } });
        return false;
    }

    if (model.ObjListDetail().filter(function (data) {
            return data.MATERIAL_CODE() === material;
    }).length !== 0) {
        Message.func.AlertDanger({ Message: { Code: "", Message: "Mặt hàng này đã được chọn!" } });
        return false;
    }

    if (model.ObjListDetail().length > 0) {
        var lstMaterialCheck = model.ObjListDetail().map(function (x) { return x.MATERIAL_CODE() });
        lstMaterialCheck.push(material);

        var check = false;
        $.each(lstStore, function () {
            var store = this;
            var lstMaterialOfStore = (lstStoreMaterial.filter(function (data) {
                return (data.STORE_CODE == store.CODE);
            })).map(function (x) { return x.MATERIAL_CODE });

            if (
                lstMaterialCheck.every(function (value) {
                    return (lstMaterialOfStore.indexOf(value) >= 0);
            })) {
                check = true;
                return false;
            }
        });

        if (!check) {
            Message.func.AlertDanger({ Message: { Code: "", Message: "Mặt hàng vừa chọn không cùng kho với các mặt hàng đã chọn!" } });
            return false;
        }
    }
    return true;
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

function AddAllDetail() {
    $.each(lstMaterial, function (key, value) {
        AddDetail(value.CODE);
    });
}

function AddDetail(material) {
    if (!ValidateMaterialStore(material)) {
        return;
    }
    model.AddDetail(number, material);
    number = number + 1;
    ValidateCapacity();
}

function RemoveDetail(obj) {
    model.RemoveDetail($(obj).attr("order"));
    ValidateCapacity();
}

var Detail = function (order, marterialCode, quantity, approveQuantity, unitCode, price, currency) {
    var self = this;
    self.ORDER = ko.observable(order);
    self.QUANTITY = ko.observable(quantity);
    self.QUANTITY.subscribe(function (newValue) {
        ValidateCapacity();
    });
    self.APPROVE_QUANTITY = ko.observable(approveQuantity);
    self.UNIT_CODE = ko.observable(unitCode);
    self.PRICE = ko.observable(price);
    self.CURRENCY = ko.observable(currency);
    self.MATERIAL_CODE = ko.observable(marterialCode);
    self.MATERIAL_TEXT = ko.computed(function () {
        var find = lstMaterial.filter(function (data) {
            return data.CODE == self.MATERIAL_CODE();
        });
        if (find.length != 0) {
            if (find[0].TEXT.indexOf("FO") != -1)
                self.UNIT_CODE("KG");
            return find[0].TEXT;
        }
    }, this);
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
    this.AddDetail = function (order, material) {
        if (self.ObjListDetail().filter(function (data) {
            return data.MATERIAL_CODE() === material;
        }).length !== 0) {
            //Message.func.AlertDanger({ Message: { Code: "", Message: "Mặt hàng này đã được chọn!" } });
            return false;
        }
        self.ObjListDetail.push(new Detail(order, material, 1000, 0, 'KG','',''));
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
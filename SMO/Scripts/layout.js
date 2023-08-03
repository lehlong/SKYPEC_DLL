$.validator.methods.date = function (value, element) {
    return this.optional(element) || moment(value, "DD/MM/YYYY", true).isValid();
}

var modelNotify = new ModelNotify();
modelNotify.IntCountNew(notifyService.IntCountNew);
if (notifyService.IntCountNew == 0) {
    modelNotify.IntCountNew("");
}
$.each(notifyService.ObjList, function () {
    modelNotify.ObjListNotify.push(new Notify(this.PKID, this.CONTENTS, this.RAW_CONTENTS, this.IS_REAED, this.IS_COUNTED));
});

function NotifyBrowser(pkid, title, desc) {
    try {
        if (!Notification) {
            console.log('Desktop notifications not available in your browser..');
            return;
        }
        if (Notification.permission !== "granted") {
            Notification.requestPermission();
        }
        else {
            var notification = new Notification(title, {
                icon: '/Content/Images/icon-budget.png',
                body: desc,
            });

            // Remove the notification from Notification Center when clicked.
            notification.onclick = function () {
                $('#a' + pkid).click();
                if (modelNotify.IntCountNew() != "" || modelNotify.IntCountNew() != 0) {
                    var total = parseInt(modelNotify.IntCountNew()) - 1;
                    modelNotify.IntCountNew("");
                    if (total > 0) {
                        modelNotify.IntCountNew(total);
                    }
                }
            };
        }
    } catch (e) {
        console.log(e);
    }
}

var notification = $.connection.notificationHub;
notification.client.RefreshNotify = function (data) {
    var service = jQuery.parseJSON(data);
    modelNotify.IntCountNew(service.IntCountNew);
    if (service.IntCountNew == 0) {
        modelNotify.IntCountNew("");
    }
    modelNotify.ObjListNotify.removeAll();
    $.each(service.ObjList, function () {
        modelNotify.ObjListNotify.push(new Notify(this.PKID, this.CONTENTS, this.RAW_CONTENTS, this.IS_REAED, this.IS_COUNTED));
    });
    NotifyBrowser(service.ObjList[0].PKID, "[THM BPS] NOTIFICATION", service.ObjList[0].RAW_CONTENTS)

    $("#frmViewAllNotify").submit();
};

notification.client.NotifyIsViewed = function () {
    modelNotify.IntCountNew("");
};

notification.client.NotifyIsReaded = function (pkId) {
    var find = modelNotify.ObjListNotify().filter(function (data) {
        return data.PKID() === pkId;
    });
    if (find.length !== 0) {
        find[0].IS_REAED(true);
    }
    $("#li" + pkId).attr("class", "");
};

$.connection.hub.start().done(function () {
});



function GetTopBidContractor() {
    try {
        var ajaxParams = {
            url: '/EB/EB/TopBidContractor',
            type: 'POST',
            success: function (response) {
                response = response.trim();
                if (response != undefined && response != null && response != "") {
                    $("#divTopBidContractor .body").html(response);
                }
            },
            error: function () {

            }
        };
        Forms.Ajax(ajaxParams);
        Forms.HideLoading();
    } catch (e) {
        console.log(e);
    }
    
}

function GetTopBidContractorWait() {
    try {
        var ajaxParams = {
            url: '/EB/EB/TopBidContractorWait',
            type: 'POST',
            success: function (response) {
                response = response.trim();
                if (response != undefined && response != null && response != "") {
                    $("#divTopBidContractorWait .body").html(response);
                }
            },
            error: function () {

            }
        };
        Forms.Ajax(ajaxParams);
        Forms.HideLoading();
    } catch (e) {
        console.log(e);
    }
    
}

function GetTopBidContractorOpen() {
    try {
        var ajaxParams = {
            url: '/EB/EB/TopBidContractorOpen',
            type: 'POST',
            success: function (response) {
                response = response.trim();
                if (response != undefined && response != null && response != "") {
                    $("#divTopBidContractorOpen .body").html(response);
                }
            },
            error: function () {

            }
        };
        Forms.Ajax(ajaxParams);
        Forms.HideLoading();
    } catch (e) {
        console.log(e);
    }

}

function GetTopBid() {
    try {
        var ajaxParams = {
            url: '/EB/EB/TopBid',
            type: 'POST',
            success: function (response) {
                response = response.trim();
                if (response != undefined && response != null && response != "") {
                    $("#divTopBid .body").html(response);
                }
            },
            error: function () {

            }
        };
        Forms.Ajax(ajaxParams);
        Forms.HideLoading();
    } catch (e) {
        console.log(e);
    }

}

function GetTopRequest() {
    try {
        var ajaxParams = {
            url: '/EB/EB/TopRequest',
            type: 'POST',
            success: function (response) {
                response = response.trim();
                if (response != undefined && response != null && response != "") {
                    $("#divTopRequest .body").html(response);
                }
            },
            error: function () {

            }
        };
        Forms.Ajax(ajaxParams);
        Forms.HideLoading();
    } catch (e) {
        console.log(e);
    }
}

function GetTopQuestionContractor() {
    try {
        var ajaxParams = {
            url: '/EB/EB/TopQuestionContractor',
            type: 'POST',
            success: function (response) {
                response = response.trim();
                if (response != undefined && response != null && response != "") {
                    $("#divTopQuestionContractor .body").html(response);
                }
            },
            error: function () {
            }
        };
        Forms.Ajax(ajaxParams);
        Forms.HideLoading();
    } catch (e) {
        console.log(e);
    }
}

function GetTopQuestion() {
    try {
        var ajaxParams = {
            url: '/EB/EB/TopQuestion',
            type: 'POST',
            success: function (response) {
                response = response.trim();
                if (response != undefined && response != null && response != "") {
                    $("#divTopQuestion .body").html(response);
                }
            },
            error: function () {
            }
        };
        Forms.Ajax(ajaxParams);
        Forms.HideLoading();
    } catch (e) {
        console.log(e);
    }
}


$(function () {
    try {
        if (Notification.permission !== "granted") {
            Notification.requestPermission();
        }
    } catch (e) {
        console.log("Không hỗ trợ notification");
    }
    //if (Notification.permission !== "granted") {
    //    Notification.requestPermission();
    //}

    $('.alert').click(function (event) {
        event.stopPropagation();
    });

    $('body').click(function (event) {
        if (!($(event.target).is('.alert')) && !($(event.target).is(".btn")) && !($(event.target).is("span"))) {
            $(".alert").remove();
        }
    });

    ko.applyBindings(modelNotify, document.getElementById('divNotify'));
    //ko.applyBindings(modelNotify, document.getElementById('divTopNotifyBody'));

    $("#divNotify .body").slimScroll({
        height: '400px'
    });

    $("#divTopNotify .body").slimScroll({
        height: '400px'
    });
});
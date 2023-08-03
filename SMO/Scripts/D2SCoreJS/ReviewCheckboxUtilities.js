//checkbox
function check_uncheck_all(isChecked, checkOneValue, ignoreAttribute, type, wrapChildContainer) {
    if (!checkOneValue) {
        checkOneValue = "checkOne";
    }
    if (!type) {
        type = "name";
    }
    if (isChecked.toString() === 'True' || isChecked.toString() === 'true') {
        $(`${wrapChildContainer} input[${type}="${checkOneValue}"]:not([${ignoreAttribute}])`).each(function () {
            this.checked = true;
            $(this).change();
        });
    } else {
        $.each($(`${wrapChildContainer} input[${type}="${checkOneValue}"]:not([${ignoreAttribute}])`), function () {
            this.checked = false;
            $(this).change();
        });
    }
}

function check_uncheck_one(checkOneValue, checkAllId, ignoreAttribute, type, wrapChildContainer) {
    if (!checkAllId) {
        checkAllId = ".checkAll";
    }
    if (!checkOneValue) {
        checkOneValue = "checkOne";
    }
    if (!type) {
        type = "name";
    }

    if ($(`${wrapChildContainer} input[${type}="${checkOneValue}"]:not([${ignoreAttribute}])`).length === $(`${wrapChildContainer} input[${type}="${checkOneValue}"]:not([${ignoreAttribute}]):checked`).length) {
        $(checkAllId)[0].checked = true;
        $('#checkAllHidden').val('True');
    } else {
        $(checkAllId)[0].checked = false;
        $('#checkAllHidden').val('False');
    }
}

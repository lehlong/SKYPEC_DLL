// tableId: Id xác định table. Có thể bao gồm cả Id của parent
// ignoreColumns: Array chứa index của các columns sẽ bỏ qua khi xuất excel (bắt đầu từ 0)
function ExportExcelFromTable(tableId, ignoreColumns) {
    if (tableId === undefined || tableId === '') {
        return;
    }
    var html = $('<div>').append($(tableId).clone()).html().trim();
    if (html === '') {
        return;
    }

    var form = $('<form>', { method: 'post', action: 'Admin/ExportExcel', target: '_blank' });
    form.append($('<input>', { value: html, name: 'html' }));
    if (ignoreColumns && Array.isArray(ignoreColumns) && ignoreColumns.length > 0) {
        $.each(ignoreColumns, function (index, value) {
            form.append($('<input>', { value, name: 'ignoreColumns' }));
        });
    }
    $(document.body).append(form);
    form.submit();
    form.remove();
}
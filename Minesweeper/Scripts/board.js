$(document).ready(function () {
    $.get('/Home/GetBoard', function (data) {
        console.log(data);
        writeEntireBoard(data);
    });
});

function writeEntireBoard(data) {
    var appendString = '<div class="cell-row">';
    for (var i = 0; i < data.Values.length; i++) {
        var cellValue = data.Values[i].Value;
        var className = getClassName(cellValue);
        appendString += '<div class="cell ' + className + '" data-cell="' + i + '"></div>';
        if (i % data.Width === data.Width - 1) {
            $('#board').append(appendString + '</div>');
            appendString = '<div class="cell-row">';
        }
    }
}

function getClassName(cellValue) {
    switch (cellValue) {
        case 'Unselected':
            return 'unselected';
        case 'Flagged':
            return 'flagged';
        case 'Blank':
            return 'blank';
        case '1':
            return 'selected one';
        case '2':
            return 'selected two';
        case '3':
            return 'selected three';
        case '4':
            return 'selected four';
        case '5':
            return 'selected five';
        case '6':
            return 'selected six';
        case '7':
            return 'selected seven';
        case '8':
            return 'selected eight';
        default:
            return '';
    }
}
$(document).ready(function () {
    $.get('/Home/GetBoard', function (data) {
        console.log(data);
        writeEntireBoard(data);
    });
});

function writeEntireBoard(data) {
    var appendString = '<div class="row">';
    for (var i = 0; i < data.Values.length; i++) {
        var cellValue = data.Values[i];
        var className;
        switch (cellValue) {
            case 'U':
                className = 'unselected';
                break;
            case 'F':
                className = 'flagged';
                break;
            case 0:
                className = 'selected blank';
            default:
                className = 'selected'
        }

        appendString += '<div class="cell ' + className + '" data-cell="' + i + '">' + data.Values[i] + '</div>';
        if (i % data.Width === data.Width - 1) {
            $('#board').append(appendString + '</div>');
            appendString = '<div class="row">';
        }
    };
};
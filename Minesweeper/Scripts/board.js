$(document).ready(function () {
    $.get('/Home/GetBoard', function (data) {
        console.log(data);
        writeEntireBoard(data);
        bindMouseButtons();
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

function bindMouseButtons() {
    $('.cell').on('contextmenu', function (e) { // bind right click and prevent context menu from popping up
        e.preventDefault();
        if ($(this).hasClass('unselected') || $(this).hasClass('flag'))
            $(this).toggleClass('unselected flag');
    }).click(function () {
        if ($(this).hasClass('unselected'))
            selectCell($(this).data('cell'));
    });
}

function selectCell(id) {
    $.get('/Home/SelectCell?id=' + id, function (data) {
        console.log(data);
        updateCellClasses(data);
        if (data[0].Value === "Mine")
            mineSelected();
    });
}

function mineSelected() {
    $('.cell').off();
    $('#board-container').click(function () {
        $(this).off();
        $('.cell').attr('class', 'cell unselected');
        bindMouseButtons();
    });
}

function updateCellClasses(data) {
    $.each(data, function () {
        var id = this.Id;
        var className = getClassName(this.Value);
        $('div.cell[data-cell="' + id + '"]').attr('class', 'cell ' + className);
    });
}

function getClassName(cellValue) {
    switch (cellValue) {
        case 'Unselected':
            return 'unselected';
        case 'Flagged':
            return 'flag';
        case 'Blank':
            return 'blank';
        case 'Mine':
            return 'mine';
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
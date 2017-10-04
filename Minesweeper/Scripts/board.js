$(document).ready(function () {
    preventContextMenu();
    $.get('/Home/GetBoard', function (data) {
        console.log(data);
        writeEntireBoard(data);
        bindMouseButtons();
    });
});

function preventContextMenu() {
    $('#board-container').on('contextmenu', function (e) { //Prevent context menu from popping up on right click
        e.preventDefault();
    });
}

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
    $('.cell').mouseup(function (e) {
        if (e.which === 1) { //Left click
            selectCell($(this));
        }
        else if (e.which === 3) { //Right click
            flagCell($(this));
        }
    });
}

function selectCell($this) {
    if (!$this.hasClass('unselected')) {
        return;
    }
    var id = $this.data('cell');
    $.get('/Home/SelectCell?id=' + id, function (data) {
        console.log(data);
        updateCellClasses(data.Values);
        if (data.State !== 'GameInProgress')
            endGame();
    });
}

function endGame() {
    $('.cell').off();
    $('#board-container').mouseup(function (e) {
        if (e.which === 1) { //Left click
            $(this).off();
            preventContextMenu(); 
            $('.cell').attr('class', 'cell unselected');
            bindMouseButtons();
        }
    });
}

function flagCell($this) {
    if ($this.hasClass('unselected') || $this.hasClass('flag')) {
        $this.toggleClass('unselected flag');
        $.get('/Home/FlagCell?id=' + $this.data('cell'), function (data) {
            if (data === false)
                console.log("Error flagging on server");
        });
    }
}

function updateCellClasses(data) {
    $.each(data, function () {
        var id = this.Id;
        var className = getClassName(this.Value);
        var $cell = $('div.cell[data-cell="' + id + '"]');
        $cell.attr('class', 'cell ' + className);
    });
}

function getClassName(cellValue) {
    switch (cellValue) {
        case 'Unselected':
            return 'unselected';
        case 'Flag':
            return 'flag';
        case 'MineFlagged':
            return 'mineflagged';
        case 'MineMisFlagged':
            return 'minemisflagged';
        case 'Blank':
            return 'blank';
        case 'Mine':
            return 'mine';
        case 'MineDeath':
            return 'minedeath';
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
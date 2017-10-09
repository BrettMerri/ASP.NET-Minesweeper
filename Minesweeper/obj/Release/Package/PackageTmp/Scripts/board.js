$(document).ready(function () {
    preventContextMenu();

    var mouseIsDown = false;
    var difficulty;

    $(document).mousedown(function (e) {
        if (e.which === 1) { //Left click
            mouseIsDown = true; // When mouse goes down, set mouseIsDown to true
        }
    }).mouseup(function (e) {
        if (e.which === 1) { //Left click
            mouseIsDown = false; // When mouse goes up, set mouseIsDown to false
        }    
    });

    $.get('/Home/GetBoard', function (data) {
        console.log(data);
        difficulty = data.Difficulty.toLowerCase();
        disableDifficultyButton(difficulty);
        writeEntireBoard(data);
        bindMouseButtons();
    });

    $('.difficultyButton').click(function () {
        var oldDifficulty = difficulty;
        difficulty = $(this).data('difficulty');
        if (oldDifficulty === difficulty)
            return
        $.get('/Home/ChangeDifficulty?difficulty=' + difficulty, function (data) {
            console.log(data);
            difficulty = data.Difficulty.toLowerCase();
            disableDifficultyButton(difficulty);
            writeEntireBoard(data);
            bindMouseButtons();
        });
    });

    function disableDifficultyButton(difficulty) {
        $('.difficultyButton[data-difficulty="' + difficulty + '"]').prop('disabled', true);
        $('.difficultyButton').not('[data-difficulty="' + difficulty + '"]').prop('disabled', false);
    }

    function preventContextMenu() {
        $('#board-container').on('contextmenu', function (e) { //Prevent context menu from popping up on right click
            e.preventDefault();
        });
    }

    function writeEntireBoard(data) {
        $('#board').empty();
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
        $('#face').mousedown(function (e) {
            if (e.which === 1) { //Left click
                $(this).addClass('facepressed');
            }
        }).mouseup(function (e) {
            if (e.which === 1) { //Left click
                $(this).removeClass('facepressed');
            }
        }).hover(function () {
            if (mouseIsDown) {
                $(this).addClass('facepressed');
            }
        }, function () {
            if (mouseIsDown) {
                $(this).removeClass('facepressed');
            }
        });
        $('.cell').mousedown(function (e) {
            if (e.which === 1 && $(this).hasClass('unselected')) { //Left click
                $(this).addClass('blank');
                $('#face').attr('class', 'faceooh');
            }
            else if (e.which === 3) { //Right click
                flagCell($(this));
            }
        }).mouseup(function (e) {
            if (e.which === 1) { //Left click
                selectCell($(this));
            }
        }).hover(function () {
            if ($(this).hasClass('unselected') && mouseIsDown)
                $(this).addClass('blank');
        }, function () {
            if ($(this).hasClass('unselected') && mouseIsDown)
                $(this).removeClass('blank');
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
            if (data.State !== 'GameInProgress') {
                endGame(data.State);
            }
            else {
                $('#face').attr('class', 'facesmile');
            }
        });
    }

    function endGame(state) {
        $('.cell').off();
        if (state === 'MineSelected')
            $('#face').attr('class', 'facedead');
        else
            $('#face').attr('class', 'facewin');
        $('#board-container').mouseup(function (e) {
            if (e.which === 1) { //Left click
                $('.cell').attr('class', 'cell unselected');
                $('#face').attr('class', 'facesmile');
                $(this).off();
                preventContextMenu(); //Because $(this).off() unbinds contextmenu so we gotta rebind it
                bindMouseButtons(); //Because $('.cell').off() unbinds cells
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
});
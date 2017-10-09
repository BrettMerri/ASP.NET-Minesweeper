$(document).ready(function () {
    preventContextMenu();

    var mouseIsDown = false;
    var timerRunning = false;
    var difficulty;
    var seconds = 0;
    var intervalHandle;
    startGame();

    $(document).mousedown(function (e) {
        if (e.which === 1) { //Left click
            mouseIsDown = true; // When mouse goes down, set mouseIsDown to true
        }
    }).mouseup(function (e) {
        if (e.which === 1) { //Left click
            mouseIsDown = false; // When mouse goes up, set mouseIsDown to false
        }    
    });

    $('.difficultyButton').click(changeDifficulty);

    function changeDifficulty() {
        var oldDifficulty = difficulty;
        difficulty = $(this).data('difficulty');
        if (oldDifficulty === difficulty)
            return
        $.get('/Home/ChangeDifficulty?difficulty=' + difficulty, function (data) {
            if (data === false)
                console.log("Error changing difficulties");
            endTimer()
            setTime(); //Resets time to 000
            startGame();
        });
    }

    function startGame() {

        $.get('/Home/GetBoard', function (data) {
            difficulty = data.Difficulty.toLowerCase();
            disableDifficultyButton(difficulty);
            writeEntireBoard(data);
            bindMouseButtons();
        });

    }

    function startTimer() {
        intervalHandle = setInterval(incrementSeconds, 1000);
        timerRunning = true;
    }

    function endTimer() {
        clearInterval(intervalHandle);
        timerRunning = false;
        seconds = 0;
    }

    function incrementSeconds() {
        if (seconds >= 999)
            return;
        seconds++;
        setTime();
    }

    function setTime() {
        var seperateDigits = seconds.toString().split("").reverse();
        for (i = 0; i < seperateDigits.length; i++) {
            var $element;
            if (i === 0)
                $element = $('#ones');
            else if (i === 1)
                $element = $('#tens');
            else
                $element = $('#hundreds');
            $element.attr('class', 'time time' + seperateDigits[i]);
        }
    }

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
        var boardHTML = [],
            appendString = '<div class="cell-row">';
        for (var i = 0; i < data.Values.length; i++) {
            var cellValue = data.Values[i].Value;
            var className = getClassName(cellValue);
            appendString += '<div class="cell ' + className + '" data-cell="' + i + '"></div>';
            if (i % data.Width === data.Width - 1) {
                boardHTML.push(appendString + '</div>');
                appendString = '<div class="cell-row">';
            }
        }
        $('#board').html(boardHTML.join(''));
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
            updateCellClasses(data.Values);
            if (data.State !== 'GameInProgress') {
                endTimer();
                endGame(data.State);
            }
            else {
                if (timerRunning === false)
                    startTimer();
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
        $('#face').mouseup(function (e) {
            if (e.which === 1) { //Left click
                $(this).off();
                setTime(); //Resets time to 000
                $('#face').attr('class', 'facesmile');
                startGame();
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
                return cellValue.toLowerCase();
        }
    }
});
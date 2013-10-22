var controllers = (function () {
    'use strict';
    //var baseUrl = "http://localhost:55181/api/";
    var baseUrl = "http://pigletcheckers.apphb.com/api/";

    var UIController = Class.create({
        init: function () {
            this.persister = persisters.create(baseUrl);
        },
        loadUI: function (selector) {
            this.attachUIEvents(selector);
            if (this.persister.isLoggedIn()) {
                this.loadGame(selector);
            } else {
                this.loadLogin(selector);
            }
        },
        loadLogin: function (selector) {
            var self = this;
            $(selector).load("partial-views/login-form.html", function () {

            });
        },
        loadGame: function (selector) {
            var self = this;

            function getOpenGames() {
                self.persister.game.open(function (data) {
                    var list = gameLists.getOpenGames(data);
                    $('#open-games').html(list);
                });
                setTimeout(getOpenGames, 3000);
            }

            function getActiveGames() {
                self.persister.game.myactive(function (data) {
                    var list = gameLists.getActiveGames(data);
                    $('#active-games').html(list);
                });
                setTimeout(getActiveGames, 3000);
            }

            function getAllMessages() {
                self.persister.messages.all(function (data) {
                    var messagesCountainer = $('#messages-feed > pre');
                    messagesCountainer.append(data.map(function (message) {
                        return $('<p></p>').text(message.gameId + ": " + message.text);
                    }));
                    messagesCountainer.animate({ scrollTop: $(document).height() }, "fast");
                });
            }

            $(selector).load("partial-views/game-layout.html", function () {
                $('#nickname').text(self.persister.user.getNickname());
                getOpenGames();
                getActiveGames();
                getAllMessages();
            });
        },
        attachUIEvents: function (selector) {
            var self = this;
            var container = $(selector);

            function handleError(err) {
                var message = JSON.parse(err.responseText).Message;
                var errorMessage = $('<p class="text-error"></p>').text(message);
                $('#messages-feed > pre').append(errorMessage);
                $('#messages-feed > pre').animate({ scrollTop: $(document).height() }, "fast");
            }

            //------login/register logic-------------------------
            container.on('submit', '#login-form', function (ev) {
                ev.preventDefault();

                $('input', this).attr("disabled", "disabled");

                var user = {
                    username: $(this).find('#username').val(),
                    password: $(this).find('#password').val()
                };

                setTimeout(function () {
                    self.persister.user.login(user, function (data) {
                        self.loadGame(selector);

                    }, function (err) {
                        var message = JSON.parse(err.responseText).Message;
                        var errorMessage = $('<p class="text-error"></p>').text(message);
                        errorMessage.delay(2000).fadeOut();
                        $('#login-form').append(errorMessage);
                        $('input').removeAttr('disabled');
                    });
                }, 500);
            });

            container.on('click', '#register-link', function (ev) {
                ev.preventDefault();
                container.load('partial-views/register-form.html');
            });

            container.on('click', '#back-to-login', function (ev) {
                ev.preventDefault();
                container.load('partial-views/login-form.html');
            });

            container.on('submit', '#register-form', function (ev) {
                ev.preventDefault();

                var user = {
                    username: $(this).find('#username').val(),
                    nickname: $(this).find('#nickname').val(),
                    password: $(this).find('#password').val()
                };

                self.persister.user.register(user, function (data) {
                    self.loadGame(selector);
                }, function (err) {
                    var message = JSON.parse(err.responseText).Message;
                    var errorMessage = $('<p class="text-error"></p>').text(message);
                    errorMessage.delay(2000).fadeOut();
                    $('#register-form').append(errorMessage);
                });
            });

            container.on('click', '#logout-button', function (ev) {
                ev.preventDefault();

                self.persister.user.logout(function () {
                    self.loadLogin(selector);
                }, function (data) {
                    console.log(data);
                });
            });
            //-------------------------------------------------------------

            //---------main-game logic-------------------------------------
            container.on('click', '#scores-button', function () {
                var scoresModal = $('#scores');
                self.persister.game.scores(function (data) {
                    //debugger 
                    var body = scoresModal.find('tbody');

                    body.html("");

                    body.append(data.map(function (score) {
                        return $('<tr></tr>')
                            .append($('<td></td>').text(score.nickname))
                            .append($('<td></td>').text(score.score));
                    }));

                    scoresModal.modal("show");
                }, handleError);
            });

            //-----------left-panel (create game + open games) region-----------------------------------
            container.on('submit', '#create-game-wrapper > form', function (ev) {
                ev.preventDefault();

                var game = {
                    title: $('#create-game-title').val(),
                };
                var password = $('#create-game-password').val();
                if (password) {
                    game.password = password;
                }
                self.persister.game.create(game, function () {
                    alert("Game " + game.title + " created successfully!");
                }, handleError);
            });

            //-------join game logic---------------------------
            container.on('click', '#open-games a', function (ev) {
                ev.preventDefault();

                var joinGameModal = $('#join-game-input');
                joinGameModal.data('game-id', $(this).parents('li').first().data('game-id'));
                joinGameModal.modal("show");
            });

            container.on('click', '#join-game-button', function () {
                var modal = $('#join-game-input');
                var game = {
                    gameId: modal.data('game-id')
                };

                var password = $('#game-password').val();
                if (password) {
                    game.password = password;
                }

                self.persister.game.join(game, function () {
                    modal.modal("hide");
                }, handleError);
            });

            //------------middle-panel(game layout) region--------------------------------------
            function movePieceTo(selector, pieceId) {
                var currentPosition = {
                    x: selector.index(),
                    y: selector.closest('tr').index()
                };

                var data = {
                    Id: pieceId,
                    position: currentPosition
                };

                var currentGameId = $('#game-container').data('gameid');

                self.persister.battle.move(currentGameId, data, function (data) {
                    renderState(currentGameId);
                }, function (err) {
                    renderState(currentGameId);
                    handleError(err);
                });
            }

            function clearLastPosition(selector) {
                selector.removeClass('selected');
                if (selector.hasClass('white-unit')) {
                    selector.removeClass('white-unit');
                    selector.text("");
                }
                if (selector.hasClass('black-unit')) {
                    selector.removeClass('black-unit');
                    selector.text("");
                }
            }

            container.on('click', '#board td', function () {
                var ifBoardHasSelected = $('#board td.selected');

                if (ifBoardHasSelected.length != 0) {
                    var selectedUnitId = ifBoardHasSelected.first().data('id');
                    movePieceTo($(this), selectedUnitId);
                } else {
                    $(this).addClass('selected');
                }
            });

            //------------right-panel(active games + messages) region---------------------------

            function clearTable() {
                var cells = $('#board-table td');
                cells.removeClass('white-unit');
                cells.removeClass('black-unit');
                cells.removeClass('selected');
                cells.text("");
            }

            function renderState(currentId) {
                clearTable();

                $('#game-container').data('gameid', currentId);

                self.persister.game.field(currentId, function (data) {
                    //console.log(data);
                    function putPieceOnBoard(id, type, coordX, coordY) {
                        var board = $('#board > table');
                        var position = board.find('tr').eq(coordY).find('td').eq(coordX);
                        position.data('id', id);
                        position.data('type', type);
                        position.data('x', coordX);
                        position.data('y', coordY);

                        if (type === "white") {
                            position.addClass('white-unit');
                        } else {
                            position.addClass('black-unit');
                        }
                    }

                    //Gameplay header
                    $('#black-user-info span')[0].innerText = data.blackPieces[0].owner;
                    $('#white-user-info > span')[0].innerText = data.whitePieces[0].owner;
                    $('#turns-count')[0].innerText = data.turn;
                    $('#player-in-turn').text("It's " + data.inTurn + "'s turn");

                    //Init field
                    data.whitePieces.forEach(function (unit) {
                        putPieceOnBoard(unit.id, 'white', unit.position.x, unit.position.y);
                    });
                    data.blackPieces.forEach(function (unit) {
                        putPieceOnBoard(unit.id, 'black', unit.position.x, unit.position.y);
                    });

                }, handleError);
            }

            container.on('click', '#active-games li', function () {
                var currentId = $(this).data('game-id');
                $('#game-container').data('gameid', currentId);
                self.persister.game.field(currentId, function () {
                    //debugger;
                    renderState(currentId);
                }, handleError);
            });

            //handle user messages feed
            function appendUnreadMessages() {
                var messagesContainer = $('#messages-feed > pre');
                if (self.persister.isLoggedIn()) {
                    self.persister.messages.unread(function (data) {

                        messagesContainer.append(data.map(function (message) {
                            return $('<p></p>').text(message.gameId + ": " + message.text);
                        }));
                        $('#messages-feed > pre').animate({ scrollTop: $(document).height() }, "fast");
                        data.forEach(function (message) {
                            //game-joined
                            if (message.type === 1) {
                                self.persister.game.start(message.gameId, function () {
                                    renderState(message.gameId);
                                }, handleError);
                            }
                            //game-moved
                            if (message.type === 2) {
                                self.persister.game.field(message.gameId, function () {
                                    renderState(message.gameId);
                                });
                            }
                            //game-finished
                            if (message.type === 3) {
                                alert(message.text);
                            }
                        });

                        setTimeout(appendUnreadMessages, 1000);
                    }, handleError);
                } else {
                    setTimeout(appendUnreadMessages, 1000);
                }
            }
            appendUnreadMessages();
        }
    });

    return {
        create: function () {
            return new UIController();
        }
    };
}());
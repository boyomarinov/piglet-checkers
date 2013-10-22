var gameLists = (function () {
    
    function buildOpenGamesList(games) {
        var list = '<ul class="game-list open-games nav nav-tabs nav-stacked">';
        for (var i = 0; i < games.length; i++) {
            var game = games[i];
            list +=
                '<li data-game-id="' + game.id + '">' +
                    '<a href="#" >' +
                        $("<div />").html(game.title).text() +
                    '</a>' +
                    '<span> by ' +
                        game.creator +
                    '</span>' +
                '</li>';
        }
        list += "</ul>";
        return list;
    }

    function buildActiveGamesList(games) {
        var gamesList = Array.prototype.slice.call(games, 0);
        gamesList.sort(function (g1, g2) {
            if (g1.status == g2.status) {
                return g1.title > g2.title;
            }
            else {
                if (g1.status == "in-progress") {
                    return -1;
                }
            }
            return 1;
        });

        var list = '<ul class="game-list active-games nav nav-tabs nav-stacked">';
        for (var i = 0; i < gamesList.length; i++) {
            var game = gamesList[i];
            list +=
				'<li data-game-id="' + game.id + '">' +
					'<a href="#" class="' + game.status + '">' +
						$("<div />").html(game.title).text() +
				        " (" + game.status + ")" +
					'</a>' +
					'<span> by ' +
						game.creator +
					'</span>' +
				'</li>';
        }
        list += "</ul>";
        return list;
    }

    return {
        getOpenGames: buildOpenGamesList,
        getActiveGames: buildActiveGamesList
    };
}());


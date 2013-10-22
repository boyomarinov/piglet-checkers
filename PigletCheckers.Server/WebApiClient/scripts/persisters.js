/// <reference path="http-requester.js" />
/// <reference path="http://crypto-js.googlecode.com/svn/tags/3.1.2/build/rollups/sha1.js" />

var persisters = (function () {
    var nickname = localStorage.getItem("nickname");
    var sessionKey = localStorage.getItem("sessionKey");

    function storeData(data) {
        localStorage.setItem("nickname", data.nickname);
        localStorage.setItem("sessionKey", data.sessionKey);
        nickname = data.nickname;
        sessionKey = data.sessionKey;
    }

    function clearData() {
        localStorage.removeItem("nickname");
        localStorage.removeItem("sessionKey");
        nickname = "";
        sessionKey = "";
    }

    var BasePersister = Class.create({
        init: function (baseUrl) {
            this.baseUrl = baseUrl;
            this.user = new UserPersister(this.baseUrl);
            this.game = new GamePersister(this.baseUrl);
            this.battle = new BattlePersister(this.baseUrl);
            this.messages = new MessagesPersister(this.baseUrl);
        },
        isLoggedIn: function () {
            return (nickname && sessionKey);
        }
    });

    var GamePersister = Class.create({
        init: function (baseUrl) {
            this.baseUrl = baseUrl + "game/";
        },
        create: function (game, success, error) {
            var url = this.baseUrl + "create/" + sessionKey;

            var data = {
                title: game.title,
            };
            if (game.password) {
                data.password = CryptoJS.SHA1(game.password).toString();
            }

            httpRequest.postJSON(url, data, success, error);
        },
        join: function (game, success, error) {
            var url = this.baseUrl + "join/" + sessionKey;

            var data = {
                id: game.gameId,
            };
            if (game.password) {
                data.password = CryptoJS.SHA1(game.password).toString();
            }

            httpRequest.postJSON(url, data, success, error);
        },
        start: function (gameId, success, error) {
            var url = this.baseUrl + gameId + "/start/" + sessionKey;

            httpRequest.getJSON(url, success, error);
        },
        open: function (success, error) {
            var url = this.baseUrl + "open/" + sessionKey;

            httpRequest.getJSON(url, success, error);
        },
        myactive: function (success, error) {
            var url = this.baseUrl + "my-active/" + sessionKey;

            httpRequest.getJSON(url, success, error);
        },
        field: function (gameId, success, error) {
            var url = this.baseUrl + gameId + "/field/" + sessionKey;

            httpRequest.getJSON(url, success, error);
        },
        scores: function (success, error) {
            var url = this.baseUrl + "scores/" + sessionKey;

            httpRequest.getJSON(url, success, error);
        }
    });

    var BattlePersister = Class.create({
        init: function (baseUrl) {
            this.baseUrl = baseUrl + "checkers/";
        },
        move: function (gameId, piece, success, error) {
            var url = this.baseUrl + gameId + "/move/" + sessionKey;

            var data = {
                pieceId: piece.Id,
                position: piece.position
            };

            httpRequest.postJSON(url, data, success, error);
        }
    });

    var MessagesPersister = Class.create({
        init: function (baseUrl) {
            this.baseUrl = baseUrl + "usermessages/";
        },
        unread: function (success, error) {
            var url = this.baseUrl + "unread/" + sessionKey;

            httpRequest.getJSON(url, success, error);
        },
        all: function (success, error) {
            var url = this.baseUrl + "all/" + sessionKey;

            httpRequest.getJSON(url, success, error);
        }
    });

    var UserPersister = Class.create({
        init: function (baseUrl) {
            this.baseUrl = baseUrl + "user/";
        },
        register: function (user, success, error) {
            var url = this.baseUrl + "register/";
            var data = {
                username: user.username,
                nickname: user.nickname,
                authCode: CryptoJS.SHA1(user.password).toString()
            };

            httpRequest.postJSON(url, data, function (data) {
                storeData(data);
                success(data);
            }, error);
        },
        login: function (user, success, error) {
            var url = this.baseUrl + "login/";
            var data = {
                username: user.username,
                authCode: CryptoJS.SHA1(user.password).toString()
            };
             
            httpRequest.postJSON(url, data, function (data) {
                storeData(data);
                success(data);
            }, error);
        },
        logout: function (success, error) {
            var url = this.baseUrl + "logout/" + sessionKey;

            clearData();
            httpRequest.getJSON(url, success, error);
        },
        getNickname: function () {
            return nickname;
        }
    });

    return {
        create: function (url) {
            return new BasePersister(url);
        }
    };
}());

////login test
//var user = {
//    username: "boyo",
//    password: "1"
//};

//var testPersister = persisters.create("http://localhost:40643/api/");
//testPersister.user.login(user, function(data) {
//    console.log(JSON.stringify(data));
//}, function(data) {
//    console.log(JSON.stringify(data));
//});

////register test
//var userToRegister = {
//    username: "username1",
//    nickname: "nickname1",
//    password: "1"
//};

//testPersister.user.register(userToRegister, function (data) {
//    console.log(JSON.stringify(data));
//}, function (data) {
//    console.log(JSON.stringify(data));
//});
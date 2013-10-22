var httpRequest = (function () {
    function getJSON(url, success, error) {
        $.ajax({
            url: url,
            type: "GET",
            contentType: "application/json",
            success: success,
            error: error
        });
    }

    function postJSON(url, data, success, error) {
        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(data),
            success: success,
            error: error
        });
    }

    return {
        getJSON: getJSON,
        postJSON: postJSON
    };
}());

var testUser = {
    "username": "Dodo",
    "authCode": "6fa9133efe05348e430bd5a4585b595f0cb6cba3"
};


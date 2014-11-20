$(function () {

    var userDataTag = "userData";

    var authSection = $("#authSection");
    var dataSection = $("#dataSection");

    var registrationForm = $("#registrationForm");
    var loginForm = $("#loginForm");

    var lastToken;
    var tableContent = $("#tableDisplay tbody");

    authSection.hide();
    dataSection.hide();

    function start() {
        var savedData = sessionStorage.getItem(userDataTag);
        if (savedData != null) {
            var data = JSON.parse(savedData);
            lastToken = data.token;
            displayName(data.user);

            dataSection.show();
            refreshTable();
        } else
            authSection.show();
    }

    //auth script

    $("#btnRegister").click(function () {

        var dataRegister = {
            Email: registrationForm.find("#Email").val(),
            Password: registrationForm.find("#Password").val(),
            ConfirmPassword: registrationForm.find("#ConfirmPassword").val()
        };

        register(dataRegister);
    });

    $("#btnLogin").click(function () {

        var dataLogin = {
            Email: loginForm.find("#Email").val(),
            Password: loginForm.find("#Password").val()
        };

        login(dataLogin);
    });

    function blockAuth() {
        $("#btnRegister").prop("disabled", true);
        $("#btnLogin").prop("disabled", true);
    }

    function unlockAuth() {
        $("#btnRegister").prop("disabled", false);
        $("#btnLogin").prop("disabled", false);
    }

    function register(dataRegister) {

        blockAuth();
        $.ajax({
            url: '/api/Account/Register',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dataRegister),
            success: function (data) {
                login(dataRegister);
                unlockAuth();
            },
            error: function (xhr) {
                console.log("Errore nella registrazione: " + xhr.responseText);
                unlockAuth();
            }
        });
    }

    function login(dataLogin) {
        var tokenRequest = "grant_type=password&userName=Alice&password=password123";
        tokenRequest = tokenRequest.replace("Alice", dataLogin.Email);
        tokenRequest = tokenRequest.replace("password123", dataLogin.Password);

        blockAuth();
        $.ajax({
            url: '/Token',
            type: 'POST',
            data: tokenRequest,
            success: function (data) {
                saveToken(data.userName, data.access_token);
                displayName(data.userName);
                isLoggedIn();
                unlockAuth();
            },
            error: function (xhr) {
                console.log("Errore nel login: " + xhr.responseText);
                unlockAuth();
            }
        });
    }

    function saveToken(user, token) {
        var data = {
            user: user,
            token: token
        };
        sessionStorage.setItem(userDataTag, JSON.stringify(data));
        lastToken = token;
    }

    function isLoggedIn() {
        authSection.hide(1000, function () {
            dataSection.show();
            refreshTable();
        });
    }

    //data script

    $("#btnGetAllActivities").on("click", function () {
        $.ajax({
            url: 'http://localhost:11331/api/ActivityApi',
            type: 'GET',
            headers: {
                'Authorization': 'Bearer ' + lastToken
            },
            success: function (data) {
                clearTable();
                data.forEach(function (activity) {
                    insertActivityInTable(activity);
                });
            },
            error: function (xhr) {
                console.log("Errore durante il recupero dei dati: " + xhr.responseText);
            }
        });
    });

    $("#btnAddActivity").click(function () {
        
        var dataActivity = {
            Title: dataSection.find("#Title").val(),
            Desc: dataSection.find("#Desc").val(),
            Duration: dataSection.find("#Duration").val(),
            CategoryId: dataSection.find("#CategoryId").val()
        };

        $.ajax({
            url: 'http://localhost:11331/api/ActivityApi',
            type: 'POST',
            data: dataActivity,
            headers: {
                'Authorization': 'Bearer ' + lastToken
            },
            success: function (data) {
                refreshTable();
                dataSection.find("input:not([type='button'])").val("");
            },
            error: function (xhr) {
                console.log("Errore nel recupero dei dati: " + xhr.responseText);
            }
        });
    });

    function clearTable() {
        tableContent.html("");
    }

    function insertActivityInTable(activity) {

        var deleteIcon = $("<td><span class='glyphicon glyphicon-remove'></span></td>");
        deleteIcon.click(function () {
            askForDeleteActivity(activity.Id, activity.Title);
        });

        var row = $("<tr>" +
                    "<td>" + activity.Title + "</td>" +
                    "<td>" + activity.Desc + "</td>" +
                    "<td>" + activity.Duration + "</td>" +
                    "<td>" + activity.CreationDate + "</td>" +
                    "<td>" + activity.CategoryDescription + "</td>" +
                  "</tr>");

        row.append(deleteIcon);
        row.hide();
        tableContent.append(row);
        row.show(700);
    }

    function askForDeleteActivity(id, title) {
        if (confirm("Eliminare l'attività \"" + title + "\"?"))
            deleteActivity(id);
    }

    function deleteActivity(id) {
        $.ajax({
            url: 'http://localhost:11331/api/ActivityApi/' + id,
            type: 'DELETE',
            headers: {
                'Authorization': 'Bearer ' + lastToken
            },
            success: function (data) {
                refreshTable();
            },
            error: function (xhr) {
                console.log("Errore nel recupero dei dati: " + xhr.responseText);
            }
        });
    }

    function refreshTable() {
        $("#btnGetAllActivities").trigger("click");
        dataSection.find("#Title").focus();
    }

    function displayName(user) {
        $("#userNameDisplay").text("user: " + user);
    }

    start();
});
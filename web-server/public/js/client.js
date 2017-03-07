var pomelo = window.pomelo;
var username;
var users;
var rid;
var base = 1000;
var increase = 25;
var reg = /^[a-zA-Z0-9_\u4e00-\u9fa5]+$/;
var LOGIN_ERROR = "There is no server to log in, please wait.";
var LENGTH_ERROR = "Name/Channel is too long or too short. 20 character max.";
var NAME_ERROR = "Bad character in Name/Channel. Can only have letters, numbers, Chinese characters, and '_'";
var DUPLICATE_ERROR = "Please change your name to login.";

var gameDict = [];//游戏字典
gameDict.jobs = [];//职位
gameDict.jobs["redCaptain"] = null;
gameDict.jobs["redFirst Mate"] = null;
gameDict.jobs["redEngineer"] = null;
gameDict.jobs["redRadio Operator"] = null;
gameDict.jobs["blueCaptain"] = null;
gameDict.jobs["blueFirst Mate"] = null;
gameDict.jobs["blueEngineer"] = null;
gameDict.jobs["blueRadio Operator"] = null;




util = {
    urlRE: /https?:\/\/([-\w\.]+)+(:\d+)?(\/([^\s]*(\?\S+)?)?)?/g,
    //  html sanitizer
    toStaticHTML: function (inputHtml) {
        inputHtml = inputHtml.toString();
        return inputHtml.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    },
    //pads n with zeros on the left,
    //digits is minimum length of output
    //zeroPad(3, 5); returns "005"
    //zeroPad(2, 500); returns "500"
    zeroPad: function (digits, n) {
        n = n.toString();
        while (n.length < digits)
            n = '0' + n;
        return n;
    },
    //it is almost 8 o'clock PM here
    //timeString(new Date); returns "19:49"
    timeString: function (date) {
        var minutes = date.getMinutes().toString();
        var hours = date.getHours().toString();
        return this.zeroPad(2, hours) + ":" + this.zeroPad(2, minutes);
    },

    //does the argument only contain whitespace?
    isBlank: function (text) {
        var blank = /^\s*$/;
        return (text.match(blank) !== null);
    }
};

//always view the most recent message when it is added
function scrollDown(base) {
    window.scrollTo(0, base);
    $("#entry").focus();
};

// add message on board
function addMessage(from, target, text, time) {
    var name = (target == '*' ? 'all' : target);
    if (text === null) return;
    if (time == null) {
        // if the time is null or undefined, use the current time.
        time = new Date();
    } else if ((time instanceof Date) === false) {
        // if it's a timestamp, interpret it
        time = new Date(time);
    }
    //every message you see is actually a table with 3 cols:
    //  the time,
    //  the person who caused the event,
    //  and the content
    var messageElement = $(document.createElement("table"));
    messageElement.addClass("message");
    // sanitize
    text = util.toStaticHTML(text);
    var content = '<tr>' + '  <td class="date">' + util.timeString(time) + '</td>' + '  <td class="nick">' + util.toStaticHTML(from) + ' says to ' + name + ': ' + '</td>' + '  <td class="msg-text">' + text + '</td>' + '</tr>';
    messageElement.html(content);
    //the log is the stream that we view
    $("#GameInfo").append(messageElement);
    base += increase;
    scrollDown(base);
};

// show tip
function tip(type, name) {
    var tip, title;
    switch (type) {
        case 'online':
            tip = name + ' is online now.';
            title = 'Online Notify';
            break;
        case 'offline':
            tip = name + ' is offline now.';
            title = 'Offline Notify';
            break;
        case 'message':
            tip = name + ' is saying now.'
            title = 'Message Notify';
            break;
    }
    var pop = new Pop(title, tip);
};

// init user list
function initUserList(data) {
    users = data.users;
    for (var i = 0; i < users.length; i++) {
        var slElement = $(document.createElement("option"));
        slElement.attr("value", users[i]);
        slElement.text(users[i]);
        $("#usersList").append(slElement);
    }
};

// add user in user list
function addUser(user) {
    var slElement = $(document.createElement("option"));
    slElement.attr("value", user);
    slElement.text(user);
    $("#usersList").append(slElement);
};

// remove user from user list
function removeUser(user) {
    $("#usersList option").each(
        function () {
            if ($(this).val() === user) $(this).remove();
        });
};

// set your name
function setName() {
    $("#name").text(username);
};

// set your room
function setRoom() {
    $("#room").text(rid);
};

// show error
function showError(content) {
    $("#loginError").text(content);
    $("#loginError").show();
};

// show login panel
function showLogin() {
    $("#loginView").show();
    $("#GameInfo").hide();
    $("#toolbar").hide();
    $("#loginError").hide();
    $("#TeamSelect").hide();
    $("#loginUser").focus();
};

// show chat panel
function showChat() {
    $("#loginView").hide();
    $("#loginError").hide();
    $("#toolbar").show();
    $("entry").focus();
    $("#TeamSelect").hide();
    scrollDown(base);
};


// show team select
function showTeamSelect() {
    $("#loginView").hide();
    $("#loginError").hide();
    $("#toolbar").hide();
    $("#TeamSelect").show();
    scrollDown(base);
};

// query connector
function queryEntry(uid, callback) {
    var route = 'gate.gateHandler.queryEntry';
    pomelo.init({
        host: window.location.hostname,
        port: 3014,
        log: true
    }, function () {
        pomelo.request(route, {
            uid: uid
        }, function (data) {
            pomelo.disconnect();
            if (data.code === 500) {
                showError(LOGIN_ERROR);
                return;
            }
            callback(data.host, data.port);
        });
    });
};

$(document).ready(function () {
    //when first time into chat room.
    showLogin();

    //wait message from the server.
    pomelo.on('onChat', function (data) {
        // addMessage(data.from, data.target, data.msg);
        $("#GameInfo").show();
        if (data.from !== username)
            tip('message', data.from);
    });

    //update user list
    pomelo.on('onAdd', function (data) {
        var user = data.user;
        tip('online', user);
        addUser(user);
    });

    //update user list
    pomelo.on('onLeave', function (data) {
        var user = data.user;
        tip('offline', user);
        removeUser(user);
    });


    //handle disconect message, occours when the client is disconnect with servers
    pomelo.on('disconnect', function (reason) {
        showLogin();
    });

    //deal with login button click.
    $("#login").click(function () {
        username = $("#loginUser").attr("value");
        rid = $('#channelList').val();

        if (username.length > 20 || username.length == 0 || rid.length > 20 || rid.length == 0) {
            showError(LENGTH_ERROR);
            return false;
        }

        if (!reg.test(username) || !reg.test(rid)) {
            showError(NAME_ERROR);
            return false;
        }

        //query entry of connection
        queryEntry(username, function (host, port) {
            pomelo.init({
                host: host,
                port: port,
                log: true
            }, function () {
                var route = "connector.entryHandler.enter";
                pomelo.request(route, {
                    username: username,
                    rid: rid
                }, function (data) {
                    if (data.error) {
                        showError(DUPLICATE_ERROR);
                        return;
                    }
                    setName();
                    setRoom();
                    showTeamSelect();
                    initUserList(data);
                });
            });
        });
    });

    //deal with team select button click.
    $("#teamOK").click(function () {
        var team = _getRadio("team");

        var jobs = _getCheckBox("jobs");

        var route = "chat.chatHandler.send";
        pomelo.request(route, {//告诉所有人，我的职业
            rid: rid,
            content: _arrToStr(jobs),
            from: username,
            target: "*"
        }, function (data) {
            alert("告诉所有人，我的职业 finished!");
        });
    });

    //deal with chat mode.
    $("#entry").keypress(function (e) {
        var route = "chat.chatHandler.send";
        if (e.keyCode != 13 /* Return */) return;
        var msg = $("#entry").attr("value").replace("\n", "");
        if (!util.isBlank(msg)) {
            pomelo.request(route, {
                rid: rid,
                content: msg,
                from: username,
                target: "*"
            }, function (data) {
                $("#entry").attr("value", ""); // clear the entry field.
                if (target != '*') {
                    addMessage(username, target, msg);
                    $("#GameInfo").show();
                }
            });
        }
    });
});


////////////// private //////////////////////////
function _getRadio(radioName) {
    var obj = document.getElementsByName(radioName);
    for (var i = 0; i < obj.length; i++) {
        if (obj[i].checked) {
            return obj[i].value;
        }
    }
};

function _getCheckBox(checkboxName) {
    var res = new Array();
    var obj = document.getElementsByName(checkboxName);
    for (var i = 0; i < obj.length; i++) {
        if (obj[i].checked) {
            // res=res+"|"+obj[i].value;
            res[i] = obj[i].value;
        }
    }
    return res;

};


function _arrToStr(arr) {
    var res = "";
    for (var i in arr) {
        res = res + i + ": " + arr[i] + "\n";
    }
    return res;
};
var chatRemote = require('../remote/chatRemote');

module.exports = function (app) {
    return new Handler(app);
};

var Handler = function (app) {
    this.app = app;
};

var handler = Handler.prototype;

/**
 * Send messages to users & record user's jobs and teams
 *
 *
 * @param {Object} msg message from client
 * @param {Object} session
 * @param  {Function} next next stemp callback
 *
 */
handler.send = function (msg, session, next) {
    var self = this;
    var rid = session.get('rid');
    var username = session.uid.split('*')[0];
    var channelService = this.app.get('channelService');
    var param = {
        msg: msg.content,
        from: username,
        target: msg.target
    };
    channel = channelService.getChannel(rid, false);


    if (msg.target == '*') {
        //the target is all users
        // channel.pushMessage('onChat', param);

        if (msg.content == "aaa") {

            var param = {
                msg: "aaabbb",
                from: username,
                target: "*",
                opt: "oopptt"
            };
            channel.pushMessage('onChat', param);
        }else{
            channel.pushMessage('onChat', param);
        }
    } else {
        //the target is specific user
        var tuid = msg.target + '*' + rid;
        var tsid = channel.getMember(tuid)['sid'];
        channelService.pushMessageByUids('onChat', param, [{
            uid: tuid,
            sid: tsid
        }]);
    }

    next(null, {
        route: msg.route
    });
}
;
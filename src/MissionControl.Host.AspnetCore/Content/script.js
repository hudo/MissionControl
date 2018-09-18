var CliResponse = /** @class */ (function () {
    function CliResponse() {
    }
    return CliResponse;
}());
var Arg = /** @class */ (function () {
    function Arg(key, val) {
        this.key = key;
        this.val = val;
    }
    return Arg;
}());
var HostService = /** @class */ (function () {
    function HostService() {
    }
    HostService.prototype.send = function (cmd, args) {
    };
    return HostService;
}());
var Parser = /** @class */ (function () {
    function Parser() {
    }
    Parser.prototype.getArgs = function (text) {
        var args = new Array();
        var parts = text.split(" ");
        parts.forEach(function (part) {
            var arg = part.split("=");
            args.push(new Arg(arg[0], arg.length > 1 ? arg[1] : ""));
        });
        return args;
    };
    return Parser;
}());
window.onload = function () {
};

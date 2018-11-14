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
    Parser.prototype.getCommand = function (input) {
        return input.split(" ")[0];
    };
    return Parser;
}());
var ViewModel = /** @class */ (function () {
    function ViewModel(input) {
        var _this = this;
        this.input = input;
        this.input.addEventListener("keypress", function (e) {
            if (e.which == 13)
                _this.onExecute(e);
        });
    }
    ViewModel.prototype.onExecute = function (e) {
        console.log("enter!");
        this.input.value = "";
        e.preventDefault();
    };
    return ViewModel;
}());
window.onload = function () {
    new ViewModel(document.getElementById("cli-input"));
};
//# sourceMappingURL=script.js.map
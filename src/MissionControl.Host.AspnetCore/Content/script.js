var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
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
        return __awaiter(this, void 0, void 0, function () {
            var headerArgs, _i, args_1, item, data, response;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        headerArgs = "";
                        for (_i = 0, args_1 = args; _i < args_1.length; _i++) {
                            item = args_1[_i];
                            headerArgs += item.key + "=" + item.val + ";";
                        }
                        return [4 /*yield*/, fetch("mc/cmd/" + cmd, {
                                method: "POST",
                                headers: new Headers({
                                    "mc.id": "123",
                                    "mc.args": headerArgs
                                })
                            })];
                    case 1:
                        data = _a.sent();
                        return [4 /*yield*/, data.json()];
                    case 2:
                        response = _a.sent();
                        return [2 /*return*/, response];
                }
            });
        });
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
        return args.splice(1); // exclude command name
    };
    Parser.prototype.getCommand = function (input) {
        return input.split(" ")[0];
    };
    return Parser;
}());
var ViewModel = /** @class */ (function () {
    function ViewModel(input, view) {
        this.view = view;
        this.input = input;
        this.parser = new Parser();
        this.hostService = new HostService();
    }
    ViewModel.prototype.init = function () {
        var _this = this;
        this.input.addEventListener("keypress", function (e) {
            if (e.which === 13) {
                _this.onExecute(e);
                _this.input.value = "";
                e.preventDefault();
            }
        });
    };
    ViewModel.prototype.onExecute = function (e) {
        var _this = this;
        var input = this.input.value;
        var command = this.parser.getCommand(input);
        var args = this.parser.getArgs(input);
        this.hostService
            .send(command, args)
            .then(function (resp) {
            console.log(resp);
            _this.view.innerHTML += "<div class='row'><div class='inner output " + resp.type + "'>\n                    <p class='cmd'>" + input + "</p><div class='content'>" + resp.content.replace(/\r?\n/g, '<br/>') + "</div></div>\n                </div>";
        });
    };
    return ViewModel;
}());
window.onload = function () {
    new ViewModel(document.getElementById("cli-input"), document.getElementById("cli-view"))
        .init();
};
//# sourceMappingURL=script.js.map
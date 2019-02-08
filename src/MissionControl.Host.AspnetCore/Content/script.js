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
    function HostService(termId) {
        this.termId = termId;
    }
    HostService.prototype.send = function (cmd, args, print, finish) {
        return __awaiter(this, void 0, void 0, function () {
            var headerArgs, _i, args_1, item, fetchResponse, reader, response, cursor, stream;
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
                                    "mc.id": this.termId,
                                    "mc.args": headerArgs
                                })
                            })];
                    case 1:
                        fetchResponse = _a.sent();
                        reader = fetchResponse.body.getReader();
                        response = "";
                        cursor = 0;
                        stream = new ReadableStream({
                            start: function () {
                                function push() {
                                    reader.read().then(function (_a) {
                                        var done = _a.done, value = _a.value;
                                        if (done) {
                                            finish();
                                            return;
                                        }
                                        var chunk = new TextDecoder("utf-8").decode(value);
                                        //console.log("Received chunk: " + chunk);
                                        response += chunk;
                                        var begin = response.indexOf("BEGIN>>", cursor);
                                        var end = response.indexOf("<<END", cursor);
                                        if (begin > -1 && end > -1) {
                                            try {
                                                var json = response.substring(begin + 7, end);
                                                var item = JSON.parse(json);
                                                if (item)
                                                    print(item);
                                                cursor = end + 1;
                                            }
                                            catch (e) {
                                                //console.log("Error parsing json, waiting for the next chunk")
                                            }
                                        }
                                        push();
                                    });
                                }
                                push();
                            }
                        });
                        return [2 /*return*/];
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
        var args = [];
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
    function ViewModel(inputEl, viewEl) {
        this.history = [];
        this.historyCursor = -1;
        this.inputEl = inputEl;
        this.parser = new Parser();
        this.hostService = new HostService(Utils.newGuid());
        this.view = new ViewRenderer(viewEl);
    }
    ViewModel.prototype.init = function () {
        var _this = this;
        this.view.printPlain(Resources.help);
        this.inputEl.addEventListener("keypress", function (e) {
            if (e.which === 13) {
                _this.onExecute(e);
                _this.inputEl.value = "";
                e.preventDefault();
            }
        });
        this.inputEl.addEventListener("keyup", function (e) { return _this.onKeyUpDown(e); });
    };
    ViewModel.prototype.onKeyUpDown = function (e) {
        if ((e.code != 'ArrowUp' && e.code != 'ArrowDown') || this.history.length == 0) {
            return;
        }
        ;
        var isUp = e.code == "ArrowUp";
        var isDown = !isUp;
        this.inputEl.value = this.history[this.historyCursor];
        if (isUp && this.historyCursor > 0) {
            this.historyCursor -= 1;
        }
        else if (isDown && this.historyCursor < this.history.length - 1) {
            this.historyCursor += 1;
        }
    };
    ViewModel.prototype.onExecute = function (e) {
        return __awaiter(this, void 0, void 0, function () {
            var input, command, args;
            var _this = this;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        input = this.inputEl.value;
                        command = this.parser.getCommand(input);
                        args = this.parser.getArgs(input);
                        this.history.push(input);
                        this.historyCursor = this.history.length - 1;
                        if (command === "help") {
                            this.view.printRow(Resources.help);
                            return [2 /*return*/];
                        }
                        if (command === "cls") {
                            this.view.clear();
                            return [2 /*return*/];
                        }
                        this.inputEl.disabled = true;
                        return [4 /*yield*/, this.hostService.send(command, args, function (resp) {
                                _this.view.getLastRow().classList.add(resp.type);
                                _this.view.getLastRowContent().innerHTML += resp.content.replace(/\r?\n/g, "<br/>");
                            }, function () {
                                _this.inputEl.disabled = false;
                                _this.inputEl.focus();
                            })];
                    case 1:
                        _a.sent();
                        return [2 /*return*/];
                }
            });
        });
    };
    return ViewModel;
}());
var ViewRenderer = /** @class */ (function () {
    function ViewRenderer(view) {
        this.view = view;
    }
    ViewRenderer.prototype.getLastRow = function () {
        var inners = document.getElementsByClassName("inner");
        return inners[inners.length - 1];
    };
    ViewRenderer.prototype.getLastRowContent = function () {
        return this.getLastRow().querySelector(".content");
    };
    ViewRenderer.prototype.printPlain = function (text) {
        this.view.innerHTML += "<div class='row'><div class='inner'>" + text + "<br/></div></div>";
    };
    ViewRenderer.prototype.printRow = function (command) {
        this.view.innerHTML += "<div class='row'><div class='inner output'><p class='cmd'><span class=\"icon\"></span>" + command + "</p>\n        <div class='content'></div></div></div>";
    };
    ViewRenderer.prototype.clear = function () {
        this.view.innerHTML = "";
    };
    return ViewRenderer;
}());
var ResponseRenderer = /** @class */ (function () {
    function ResponseRenderer(item) {
        this.item = item;
    }
    ResponseRenderer.prototype.Print = function () {
        return "";
    };
    return ResponseRenderer;
}());
var Resources = /** @class */ (function () {
    function Resources() {
    }
    Resources.help = "Some help to get you started:<br>\n" +
        "<b>list-commands</b> will show a list for discovered commands in your app.<br>\n" +
        "Add <b>--help</b> argument to the command to see description and available parameters. ";
    return Resources;
}());
var Utils = /** @class */ (function () {
    function Utils() {
    }
    Utils.newGuid = function () {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    };
    return Utils;
}());
window.onload = function () {
    new ViewModel(document.getElementById("cli-input"), document.getElementById("cli-view"))
        .init();
};
//# sourceMappingURL=script.js.map
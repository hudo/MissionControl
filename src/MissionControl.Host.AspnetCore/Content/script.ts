interface ICliResponse {
    type: string;
    content: string;
    statusCode: number;
}

interface ITableResponse extends ICliResponse {
    description: string, 
    hasHeader: boolean,
    rows: Array<string[]>,
    numberColumns: number[],
    maxNumberOfColumns: number
}

class Arg {
    key: string;
    val: string;
    constructor(key: string, val: string) {
        this.key = key;
        this.val = val;
    }
}

class HostService {
    termId: string;
    constructor(termId: string) {
        this.termId = termId;
    }

    async send(cmd: string, args: Array<Arg>, print: (x: ICliResponse) => void, finish: () => void) {
        let headerArgs = "";
        for (let item of args) headerArgs += item.key + "=" + item.val + ";";

        const fetchResponse = await fetch("mc/cmd/" + cmd, {
            method: "POST",
            headers: new Headers({
                "mc.id": this.termId,
                "mc.args": headerArgs
            })
        });

        const reader = fetchResponse.body.getReader();
        let response = "";
        let cursor = 0;

        // @ts-ignore
        const stream = new ReadableStream({
            start() {
                function push() {
                    reader.read().then(({ done, value }) => {
                        if (done) {
                            finish();
                            return;
                        }

                        let chunk = new TextDecoder("utf-8").decode(value);
                        //console.log("Received chunk: " + chunk);
                        response += chunk;

                        let begin = response.indexOf("BEGIN>>", cursor);
                        let end = response.indexOf("<<END", cursor);

                        if (begin > -1 && end > -1) {
                            try {

                                let json = response.substring(begin + 7, end);

                                //console.log("Trying to parse: " + json);

                                let item = <ICliResponse>JSON.parse(json);
                                if (item.content !== "")
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
    }
}

class Parser {
    getArgs(text: string): Array<Arg> {
        const args = [];
        const parts = text.split(" ");
        parts.forEach(part => {
            const arg = part.split("=");
            args.push(new Arg(arg[0], arg.length > 1 ? arg[1] : ""));
        });
        return args.splice(1); // exclude command name
    }
    getCommand(input: string): string {
        return input.split(" ")[0];
    }
}

class ViewModel {
    input: HTMLTextAreaElement;
    view: HTMLDivElement;

    history: Array<string> = [];
    historyCursor: number = -1;

    parser: Parser;
    hostService: HostService;

    constructor(input: HTMLTextAreaElement, view: HTMLDivElement) {
        this.view = view;
        this.input = input;
        this.parser = new Parser();
        this.hostService = new HostService(Utils.newGuid());
    }

    init(): void {
        this.print(Resources.help);
        this.input.addEventListener("keypress", (e: KeyboardEvent) => {
            if (e.which === 13) {
                this.onExecute(e);
                this.input.value = "";
                e.preventDefault();
            }
        });
        this.input.addEventListener("keyup", (e: KeyboardEvent) => this.onKeyUpDown(e));

    }

    private onKeyUpDown(e: KeyboardEvent) {
        if ((e.code != 'ArrowUp' && e.code != 'ArrowDown') || this.history.length == 0) {
            return
        };

        let isUp = e.code == "ArrowUp";
        let isDown = !isUp;

        this.input.value = this.history[this.historyCursor];

        if (isUp && this.historyCursor > 0) {
            this.historyCursor -= 1;
        }
        else if (isDown && this.historyCursor < this.history.length - 1) {
            this.historyCursor += 1;
        }
    }

    print(text: string): void {
        this.view.innerHTML += "<div class='row'><div class='inner'>" + text + "<br/></div></div>";
    }

    printOutput(command: string) {
        this.view.innerHTML += `<div class='row'><div class='inner output'><p class='cmd'><span class="icon"></span>${command}</p>
        <div class='content'></div></div></div>`;
    }

    private async onExecute(e: KeyboardEvent) {
        const input = this.input.value;
        const command = this.parser.getCommand(input);
        const args = this.parser.getArgs(input);

        this.history.push(input);
        this.historyCursor = this.history.length - 1;

        if (command === "help") {
            this.printOutput(Resources.help);
            return;
        }

        if (command === "cls") {
            this.view.innerHTML = "";
            return;
        }

        this.printOutput(input);
        let inners = document.getElementsByClassName("inner");
        let lastInner = inners[inners.length - 1];
        let lastInnerContent = lastInner.querySelector(".content");

        this.input.disabled = true;
        await this.hostService.send(command, args,
            resp => {
                lastInner.classList.add(resp.type);
                lastInnerContent.innerHTML += resp.content.replace(/\r?\n/g, "<br/>");
            },
            () => {
                this.input.disabled = false;
                this.input.focus();
            });
    }
}

class Resources {
    static readonly help: string = "Some help to get you started:<br>\n" +
        "<b>list-commands</b> will show a list for discovered commands in your app.<br>\n" +
        "Add <b>--help</b> argument to the command to see description and available parameters. ";
}

class Utils {
    static newGuid(): string {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }
}

window.onload = () => {
    new ViewModel(
        <HTMLTextAreaElement>document.getElementById("cli-input"),
        <HTMLDivElement>document.getElementById("cli-view"))
        .init();
};


interface ICliResponse {
    type: string;
    content: string;
    statusCode: number;
}

interface ITableResponse extends ICliResponse {
    description: string; 
    rows: Array<object>
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

        // when receiving multi-part responses (chunks), each part will have BEGIN>> and <<END
        // and will contain full JSON inside. So this function is reading chunk by chunk, and trying
        // to find that begin and end, and print whatever is received inide:

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

                                let item = <ICliResponse>JSON.parse(json);

                                if (item) print(item);

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

// very simple parser that expects input command format like: "some-command -arg1=a -arg2=b"
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
    inputEl: HTMLTextAreaElement;

    history: Array<string> = [];
    historyCursor: number = -1;

    view: ViewRenderer;
    parser: Parser;
    hostService: HostService;

    constructor(inputEl: HTMLTextAreaElement, viewEl: HTMLDivElement) {
        this.inputEl = inputEl;
        this.parser = new Parser();
        this.hostService = new HostService(Utils.newGuid());
        this.view = new ViewRenderer(viewEl);
    }

    init(): void {
        this.view.printPlain(Resources.help);
        this.inputEl.addEventListener("keypress", (e: KeyboardEvent) => {
            if (e.which === 13) { // handle key enter
                this.onExecute(e);
                this.inputEl.value = "";
                e.preventDefault();
            }
        });
        this.inputEl.addEventListener("keyup", (e: KeyboardEvent) => this.onKeyUpDown(e));

    }

    // scrolls through command history when user presses keys up or down
    private onKeyUpDown(e: KeyboardEvent) {
        if ((e.code != 'ArrowUp' && e.code != 'ArrowDown') || this.history.length == 0) {
            return
        };

        let isUp = e.code == "ArrowUp";
        let isDown = !isUp;

        this.inputEl.value = this.history[this.historyCursor];

        if (isUp && this.historyCursor > 0) {
            this.historyCursor -= 1;
        }
        else if (isDown && this.historyCursor < this.history.length - 1) {
            this.historyCursor += 1;
        }
    }

   
    private async onExecute(e: KeyboardEvent) {
        const input = this.inputEl.value;
        const command = this.parser.getCommand(input);
        const args = this.parser.getArgs(input);

        this.history.push(input);
        this.historyCursor = this.history.length - 1;

        if (command === "help") {
            this.view.printCommand(Resources.help);
            return;
        }

        if (command === "cls") {
            this.view.clear();
            return;
        }
        
        // print actual command. Creates new response "block" where response content is injected
        this.view.printCommand(input);

        // we will not allow multiple commands while one is still executing
        this.inputEl.disabled = true;
        await this.hostService.send(command, args,
            resp => {
                // on chunk received
                this.view.printResponse(resp);
            },
            () => {
                // on receiving finished, re-enable input box
                this.inputEl.disabled = false;
                this.inputEl.focus();
            });
    }
}

// this nasty thing needs to be refactored
class ViewRenderer {
    view:HTMLDivElement; 
    constructor(view:HTMLDivElement){
        this.view = view;
    }

    getLastRow() : Element {
        let inners = document.getElementsByClassName("inner");
        return inners[inners.length - 1];
    }

    getLastRowContent() : Element { 
        return this.getLastRow().querySelector(".content");
    }

    printResponse(response:ICliResponse) : void {
        this.getLastRow().classList.add(response.type);

        // refactor to strategy
        if (response.type == "table") {
            this.renderTable(<ITableResponse>response);
        }
        else {
            this.getLastRowContent().innerHTML +=  response.content.replace(/\r?\n/g, "<br/>");
        }
    }

    renderTable(resp : ITableResponse) {
        // move to separate function, maybe also refactor
        const headers = this.setHeaders(resp.rows);
        let html = "<table>";
        html += "<thead><tr>";
        headers.forEach(header => {
            html += `<td>${header}</td>`;
        })
        html += "</tr></thead>"
        for (var i = 0; i < resp.rows.length; i++) {
            const row = resp.rows[i];
            for (var cell in row) {
                html += "<td>" + row[cell] + "</td>";
            }
            
            html += "</tr>";
        }
        html += "</table>";

        this.getLastRowContent().innerHTML += html;
    }

    setHeaders(rows: Array<object>) {
        let headers = [];

        rows.forEach((row, idx) => {
            if(idx === 0) {
                for (var cell in row) {
                    let value = cell.charAt(0).toUpperCase() + cell.slice(1);
                    headers.push(value);
                }
            }
        });

        return headers;
    }

    printPlain(text: string): void {
        this.view.innerHTML += "<div class='row'><div class='inner'>" + text + "<br/></div></div>";
    }

    printCommand(command: string)  {
        this.view.innerHTML += `<div class='row'><div class='inner output'><p class='cmd'><span class="icon"></span>${command}</p>
        <div class='content'></div></div></div>`;
    }

    clear() {
        this.view.innerHTML = "";
    }
}

// WIP, each response type should have its own renderer. Should be func and not a class maybe?
class ResponseRenderer {
    item : ICliResponse;
    constructor(item : ICliResponse) {
        this.item = item;
    }

    Print() : string {
        return  "";
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


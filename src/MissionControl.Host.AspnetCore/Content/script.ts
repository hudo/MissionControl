interface ICliResponse {
    type: string;
    content: string;
    statusCode: number;
}

class Arg {
    key: string;
    val: string;
    constructor(key:string, val: string) {
        this.key = key;
        this.val = val;
    }
}

class HostService {
    async send(cmd: string, args: Array<Arg>, print: (x:string) => void, finish: () => void) {    
        let headerArgs = "";
        for (let item of args) headerArgs += item.key + "=" + item.val + ";";

        const fetchResponse = await fetch("mc/cmd/" + cmd, {
            method : "POST",
            headers : new Headers({
                "mc.id" : "123",
                "mc.args" : headerArgs
            })
        });

        const reader = fetchResponse.body.getReader();
        let response = "";
        let cursor = 0;
        
        // @ts-ignore
        const stream = new ReadableStream({ start() {
            function push() {
                reader.read().then(({done, value}) => {
                    if (done) {
                        finish();
                        return;
                    }
                    
                    let chunk = new TextDecoder("utf-8").decode(value);
                    console.log("Received chunk: " + chunk);
                    response += chunk;
                    
                    let begin = response.indexOf("BEGIN>>", cursor);
                    let end = response.indexOf("<<END", cursor);
                    
                    if (begin > -1 && end > -1) {
                        try {
                            
                            let json = response.substring(begin + 7, end);
                            
                            console.log("Trying to parse: " + json);
                            
                            let item = <ICliResponse>JSON.parse(json);
                            if (item.content !== "")
                                print(item.content + "<br/>");

                            cursor = end + 1;
                        }
                        catch (e) {
                            console.log("Error parsing json, waiting for the next chunk")
                        }
                    }

                    push();
                });
            }
            push();
        }});
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
    input : HTMLTextAreaElement;
    view : HTMLDivElement;
    
    parser : Parser;
    hostService : HostService;
    
    constructor(input: HTMLTextAreaElement, view : HTMLDivElement) {
        this.view = view;
        this.input = input;        
        this.parser = new Parser();
        this.hostService = new HostService();
    }

    init():void {
        this.print(Resources.help);
        this.input.addEventListener("keypress", (e : KeyboardEvent) => {
            if (e.which === 13) {
                this.onExecute(e);
                this.input.value = "";
                e.preventDefault();
            }
        });
    }
    
    print(text: string) : void {
        this.view.innerHTML += "<div class='row'><div class='inner'>" + text + "<br/></div></div>";
    }

    private async onExecute(e: KeyboardEvent) {
        const input = this.input.value;
        const command = this.parser.getCommand(input);
        const args = this.parser.getArgs(input);
        
        if (command === "help") {
            this.print(Resources.help);
            return;
        }
        
        // add: cls, history
        
        this.print(input);
        let inners = document.getElementsByClassName("inner");
        let last = inners[inners.length - 1];

        this.input.disabled = true;
        await this.hostService.send(command, args,  
            txt => last.innerHTML += txt.replace(/\r?\n/g, "<br/>"),
            () => {
                this.input.disabled = false;
                this.input.focus();    
            });
    }
}

class Resources {
    static readonly help : string = "Some help to get you started:<br>\n" +
        "<b>list-commands</b> will show a list for discovered commands in your app.<br>\n" +
        "Add <b>--help</b> argument to see description and available parameters. ";
}

window.onload = () => {
    new ViewModel(
        <HTMLTextAreaElement>document.getElementById("cli-input"),
        <HTMLDivElement>document.getElementById("cli-view"))
        .init();
};


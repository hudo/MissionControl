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
    async send(cmd: string, args: Array<Arg>) : Promise<ICliResponse> {    
        let headerArgs = "";
        for (let item of args) headerArgs += item.key + "=" + item.val + ";";

        const data = await fetch("mc/cmd/" + cmd, {
            method : "POST",
            headers : new Headers({
                "mc.id" : "123",
                "mc.args" : headerArgs
            })
        });


        const response: ICliResponse = await data.json();
        return response;
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
        this.input.addEventListener("keypress", (e : KeyboardEvent) => {
            if (e.which === 13) {
                this.onExecute(e);
                this.input.value = "";
                e.preventDefault();
            }
        });
    }

    private onExecute(e:KeyboardEvent) {
        const input = this.input.value;
        const command = this.parser.getCommand(input);
        const args = this.parser.getArgs(input);

        this.hostService
            .send(command, args)
            .then((resp) => {
                console.log(resp);
                this.view.innerHTML += "<div class='row'><div class='inner'>" 
                    + input + "<br/>" 
                    + resp.content.replace(/\r?\n/g, "<br/>") 
                    + "<div></div>";
            });
    }
}


window.onload = () => {
    new ViewModel(
        <HTMLTextAreaElement>document.getElementById("cli-input"),
        <HTMLDivElement>document.getElementById("cli-view"))
        .init();
};


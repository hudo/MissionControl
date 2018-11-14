class CliResponse {
    type: string;
    content: string;
    commandId: string;
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
    send(cmd: string, args: Array<Arg>) {

    }
}

class Parser {
    getArgs(text: string): Array<Arg> {
        const args = new Array<Arg>();
        const parts = text.split(" ");    
        parts.forEach(part => {
            const arg = part.split("=");
            args.push(new Arg(arg[0], arg.length > 1 ? arg[1] : ""));
        });
        return args;
    }
    getCommand(input: string): string {
        return input.split(" ")[0];
    }
}

class ViewModel {
    private readonly input : HTMLTextAreaElement;
    
    constructor(input: HTMLTextAreaElement) {
        this.input = input;
        this.input.addEventListener("keypress", (e : KeyboardEvent) => {
            if (e.which == 13) 
                this.onExecute(e);
        });
    }

    private onExecute(e:KeyboardEvent) {
        console.log("enter!");
        this.input.value = "";
        e.preventDefault();
    }
}


window.onload = () => {
    new ViewModel(<HTMLTextAreaElement>document.getElementById("cli-input"));
};

/// <reference path="../jquery-1.8.d.ts" />
/// <reference path="../codemirror-3.0.d.ts" />

module type.run {
    export class CodeEditor {
        editor: CodeMirrorEditor;
        errorHandle: CodeMirrorLineHandle;
        compileTimeoutHandle: number;

        constructor(public editorElement: string, public resultElement: string) {
            var that = this;
            this.editor = CodeMirror.fromTextArea(<HTMLTextAreaElement>$(editorElement)[0], {
                lineNumbers: true,
                onChange: this.compile.bind(that),
                autofocus: true,
                mode: "text/x-csharp",
                theme: "neat",
                extraKeys: {
                    "Ctrl-S": this.save.bind(that),
                    "F6": this.compile.bind(that),
                    "F2": () => $('#shortcut').css('visibility', 'visible'),
                    "F1": () => $('#help').css('visibility', 'visible'),
                }
            });
            //this.editor.setSize(960, 500);
        }

        private compile(editor: CodeMirrorEditor, change: CodeMirrorChange) {
            var skip = false;
            editor = editor || this.editor;
            if (change)
                for (var i in change.text)
                    if (!change.text[i] || /^\s+$/g.test(change.text[i])) {
                        skip = true;
                        break;
                    }
            if (skip) return;
            clearTimeout(this.compileTimeoutHandle);
            this.compileTimeoutHandle = setTimeout(() => {
                if (this.errorHandle)
                    editor.clearMarker(this.errorHandle);
                $.ajax(Endpoints.Compile, {
                    type: "POST",
                    data: {
                        __RequestVerificationToken: this.antiForgeryToken(),
                        code: this.editor.getValue()
                    },
                    success: (compiled: CompileResult) => compiled.error ? this.error(compiled.error) : this.success(compiled.result),
                    //timeout: 5000,
                    error: (jqXhr, textStatus) => {
                        //if (textStatus === "timeout")
                        //    this.error("Request timeout. This can occur when the code contains infinite loops. Please review it, and try again.");
                        //else
                        this.error("Error while sending code for compilation. Please, try again.");
                    }
                })
            }, 1000);
        }

        private error(error) {
            var lineNumber = error.substr(1, error.indexOf(",") - 1);
            this.errorHandle = this.editor.setMarker((parseInt(lineNumber) || 1) - 1, "●");

            $(this.resultElement).fadeOut('fast', () => {
                $(this.resultElement).addClass("alert-error")
                                     .removeClass("alert-success")
                                     .text(error)
                                     .slideDown('fast');
            });
        }

        private success(result) {
            $(this.resultElement).fadeOut('fast', () => {
                $(this.resultElement).addClass("alert-success")
                                     .removeClass("alert-error")
                                     .text(result !== null ? result : "")
                                     .slideDown('fast');
            });
        }

        private save(editor: CodeMirrorEditor) {
            editor = editor || this.editor;
            var code = this.editor.getValue();

            if (!code || /^\s+$/g.test(code))
                return;

            $.post(Endpoints.Save, {
                __RequestVerificationToken: this.antiForgeryToken(),
                code: editor.getValue(),
                result: $(this.resultElement).text(),
            }, (result: SaveResult) => {
                history.replaceState(null, "saved code", result.url);
            });
        }

        private antiForgeryToken() {
            return $('input[name=__RequestVerificationToken]').val();
        }
    }

    class Endpoints {
        static get Compile() { return "Home/Compile"; }
        static get Save() { return "Home/Save"; }
    }

    interface CompileResult {
        error: string;
        result: string;
    }

    interface SaveResult {
        url: string;
    }
}
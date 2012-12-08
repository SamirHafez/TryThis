﻿/// <reference path="../jquery-1.8.d.ts" />
/// <reference path="../codemirror-3.0.d.ts" />

module type.run {
    export class CodeEditor {
        editor: CodeMirrorEditor;
        errorHandle: CodeMirrorLineHandle;
        compileTimeoutHandle: number;

        constructor(public editorElement: string, public resultElement: string, public compileTimeout? = 1000) {
            var that = this;
            this.editor = CodeMirror.fromTextArea(<HTMLTextAreaElement>$(editorElement)[0], {
                lineNumbers: true,
                onChange: this.compile.bind(that),
                autofocus: true,
                mode: "text/x-csharp",
                theme: "neat"
            });
        }

        private compile(editor: CodeMirrorEditor, change: CodeMirrorChange) {
            var skip = false;
            for (var i in change.text)
                if (!change.text[i] || /^\s+$/g.test(change.text[i])) {
                    skip = true;
                    break;
                }
            if (skip) return;
            clearTimeout(this.compileTimeoutHandle);
            this.compileTimeoutHandle = setTimeout(() => {
                if (this.errorHandle)
                    this.editor.clearMarker(this.errorHandle);
                $.ajax(Endpoints.Compile, {
                    type: "GET",
                    data: { code: editor.getValue() },
                    success: (compiled: CompileResult) => compiled.error ? this.error(compiled.error) : this.success(compiled.result),
                    //timeout: 5000,
                    error: (jqXhr, textStatus) => {
                        if (textStatus === "timeout")
                            this.error("Request timeout. This can occur when the code contains infinite loops. Please review it, and try again.");
                        else
                            this.error("Error while sending code for compilation. Please, try again.");
                    },
                    cache: false,
                    contentType: "application/json",
                    dataType: "json"
                })
            }, this.compileTimeout);
        }

        private error(error) {
            var lineNumber = error.substr(1, error.indexOf(",") - 1);
            this.errorHandle = this.editor.setMarker((parseInt(lineNumber) || 1) - 1, "●");

            $(this.resultElement).addClass("alert-error")
                                 .removeClass("alert-success")
                                 .text(error);
        }

        private success(result) {
            $(this.resultElement).addClass("alert-success")
                                 .removeClass("alert-error")
                                 .text(result || "");
        }
    }

    class Endpoints {
        static get Compile() { return "Home/Compile"; }
    }

    interface CompileResult {
        error: string;
        result: string;
    }
}
﻿var type;
(function (type) {
    (function (run) {
        var CodeEditor = (function () {
            function CodeEditor(editorElement, resultElement, compileTimeout) {
                if (typeof compileTimeout === "undefined") { compileTimeout = 1000; }
                this.editorElement = editorElement;
                this.resultElement = resultElement;
                this.compileTimeout = compileTimeout;
                var that = this;
                this.editor = CodeMirror.fromTextArea($(editorElement)[0], {
                    lineNumbers: true,
                    onChange: this.compile.bind(that),
                    autofocus: true,
                    mode: "text/x-csharp",
                    theme: "neat",
                    extraKeys: {
                        "Ctrl-S": this.save.bind(that)
                    }
                });
                this.editor.setSize(960, 500);
            }
            CodeEditor.prototype.compile = function (editor, change) {
                var _this = this;
                var skip = false;
                for(var i in change.text) {
                    if(!change.text[i] || /^\s+$/g.test(change.text[i])) {
                        skip = true;
                        break;
                    }
                }
                if(skip) {
                    return;
                }
                clearTimeout(this.compileTimeoutHandle);
                this.compileTimeoutHandle = setTimeout(function () {
                    if(_this.errorHandle) {
                        _this.editor.clearMarker(_this.errorHandle);
                    }
                    $.ajax(Endpoints.Compile, {
                        type: "GET",
                        data: {
                            code: editor.getValue()
                        },
                        success: function (compiled) {
                            return compiled.error ? _this.error(compiled.error) : _this.success(compiled.result);
                        },
                        error: function (jqXhr, textStatus) {
                            _this.error("Error while sending code for compilation. Please, try again.");
                        },
                        cache: false,
                        contentType: "application/json",
                        dataType: "json"
                    });
                }, this.compileTimeout);
            };
            CodeEditor.prototype.error = function (error) {
                var _this = this;
                var lineNumber = error.substr(1, error.indexOf(",") - 1);
                this.errorHandle = this.editor.setMarker((parseInt(lineNumber) || 1) - 1, "●");
                $(this.resultElement).fadeOut('fast', function () {
                    $(_this.resultElement).addClass("alert-error").removeClass("alert-success").text(error).slideDown('fast');
                });
            };
            CodeEditor.prototype.success = function (result) {
                var _this = this;
                $(this.resultElement).fadeOut('fast', function () {
                    $(_this.resultElement).addClass("alert-success").removeClass("alert-error").text(result || "").slideDown('fast');
                });
            };
            CodeEditor.prototype.save = function (editor) {
                var code = editor.getValue();
                if(!code || /^\s+$/g.test(code)) {
                    return;
                }
                $.post(Endpoints.Save, {
                    code: editor.getValue(),
                    result: $(this.resultElement).text()
                }, function (result) {
                    history.replaceState(null, "saved code", result.url);
                });
            };
            return CodeEditor;
        })();
        run.CodeEditor = CodeEditor;        
        var Endpoints = (function () {
            function Endpoints() { }
            Object.defineProperty(Endpoints, "Compile", {
                get: function () {
                    return "Home/Compile";
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(Endpoints, "Save", {
                get: function () {
                    return "Home/Save";
                },
                enumerable: true,
                configurable: true
            });
            return Endpoints;
        })();        
    })(type.run || (type.run = {}));
    var run = type.run;
})(type || (type = {}));
//@ sourceMappingURL=CodeEditor.js.map
## Web Compiler

A Visual Studio extension that compiles LESS, Sass Stylus, JSX, ES6 and CoffeeScript
files.

[![Build status](https://ci.appveyor.com/api/projects/status/kyk8vpst641r2n0r?svg=true)](https://ci.appveyor.com/project/madskristensen/webcompiler)

Download the extension at the
[VS Gallery](https://visualstudiogallery.msdn.microsoft.com/3b329021-cd7a-4a01-86fc-714c2d05bb6c)
or get the
[nightly build](http://vsixgallery.com/extension/148ffa77-d70a-407f-892b-9ee542346862/)

See the
[changelog](https://github.com/madskristensen/WebCompiler/blob/master/CHANGELOG.md)
for changes and roadmap.

### Features

- Compilation of LESS, Scss, Stylus, JSX, ES6 and (Iced)CoffeeScript files
- Saving a source file triggers re-compilation automatically
- Specify compiler options for each individual file
- Error List integration
- MSBuild support for CI scenarios
- Minify the compiled output
- Minification options for each language is customizable
- Shows a watermark when opening a generated file
- Shortcut to compile all specified files in solution
- Task Runner Explorer integration
- Command line interface
- Integrates with [Web Analyzer](https://visualstudiogallery.msdn.microsoft.com/6edc26d4-47d8-4987-82ee-7c820d79be1d)

### Getting started

Right-click any `.less`, `.scss`, `.styl`, `.jsx`, `.es6` or `.coffee` file in Solution Explorer to
setup compilation.

![Compile file](art/contextmenu-compile.png)

A file called `compilerconfig.json` is created in the root of the
project. This file lets you modify the behavior of the compiler.

Right-clicking the `compilerconfig.json` file lets you easily
run all the configured compilers.

![Recompile](art/contextmenu-recompile.png)

### Compile on save

Any time a `.less`, `.scss`, `.styl`, `.jsx`, `.es6` or `.coffee` file is modified within
Visual Studio, the compiler runs automatically to produce the compiled output file.

The same is true when saving the `compilerconfig.json` file where
all configured files will be compiled.

### Compile on build / CI support

In ASP.NET MVC and WebForms projects you can enable compilation as part
of the build step. Simply right-click the `compilerconfig.json` file to
enable it.

![Compile on build](art/contextmenu-compileonbuild.png)

Clicking the menu item will prompt you with information about what will
happen if you click the OK button.

![Compile on build prompt](art/prompt-compileonsave.png)

A NuGet package will be installed into the `packages` folder without adding
any files to the project itself. The NuGet package contains an MSBuild
task that will run the exact same compilers on the `compilerconfig.json`
file in the root of the project.

### Compile all

You can run the compiler on all `compilerconfig.json` files
in the solution by using the keyboard shortcut `Shift+Alt+Y`
or by using the button on the top level Build menu.

![Compile all](art/build-menu.png)

### Task Runner Explorer

Get a quick overview of the files you've specified or execute a
compilation directly in Task Runner Explorer.

![Task Runner Explorer](art/task-runner-explorer.png)

You can even set bindings so that compilation happens automatically
during certain Visual Studio events, such as *BeforeBuild* and
*Project Open*.

![Task Runner bindings](art/task-runner-bindings.png)

### Error list

When a compiler error occurs, the error list in Visual Studio
will show the error and its exact location in the source file.

![Error List](art/errorlist.png)

### Source maps

Source maps are supported for `.scss` files only for now, but the
plan is to have source map support for all languages. Web Compiler differs from it's predecesor, Web Essentials, in that it inlines a base64 encoded version of the map in the generated .css file rather than producing a separate .map file. 

### compilerconfig.json

The extension adds a `compilerconfig.json` file at the root of the
project which is used to configure all compilation.

Here's an example of what that file looks like:

```json
[
  {
    "outputFile": "output/site.css",
    "inputFile": "input/site.less",
    "minify": {
        "enabled": true
    },
    "includeInProject": true,
    "options":{
        "sourceMap": false
    }
  },
  {
    "outputFile": "output/scss.css",
    "inputFile": "input/scss.scss",
    "minify": {
        "enabled": true
    },
    "includeInProject": true,
    "options":{
        "sourceMap": true
    }
  }
]
```
Default values for `compilerconfig.json` can be found in the `compilerconfig.json.defaults` file in the same location. 

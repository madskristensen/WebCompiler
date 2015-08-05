## Web Compiler

A Visual Studio extension that compiles LESS, Sass and CoffeeScript
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

- Compilation of LESS, Scss and (Iced)CoffeeScript files
- Saving a source file triggers re-compilation automatically
- Specify compiler options for each individual file
- Error List integration
- MSBuild support for CI scenarios
- Minify the compiled output
- Minification options for each language is customizable
- Shows a watermark when opening a generated file
- Shortcut to compile all specified files in solution

### Getting started

Right-click and `.less`, `.scss` or `.coffee` file in Solution Explorer to
setup compilation.

![Compile file](art/contextmenu-compile.png)

A file called `compilerconfig.json` is created in the root of the
project. This file let's you modify the behavior of the compiler.

Right-clicking the `compilerconfig.json` file let's you easily
run all the configured compilers.

![Recompile](art/contextmenu-recompile.png)

### Compile on save

Any time a `.less`, `.scss` or `.coffee` file is modified within Visual Studio,
the compiler runs automatically to produces the compiled output file.

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
any files to the project itself. thead NuGet package contains an MSBuild
task that will run the exact same compilers on the `compilerconfig.json`
file in the root of the project.

### Compile all

You can run the compiler on all `compilerconfig.json` files
in the solution by using the keyboard shortcut `Shift+Alt+Y`
or by using the button on the top level Build menu.

![Compile all](art/build-menu.png)

### Error list

When a compiler error occurs, the error list in Visual Studio
will show the error and its exact location in the source file.

![Error List](art/errorlist.png)

### Source maps

Source maps are supported for `.scss` files only for now, but the
plan is to have source map support for all languages.

### compilerconfig.json

The extension adds a `compilerconfig.json` file at the root of the
project which is used to configure all conmpilation.

Here's an example of what that file looks like:

```js
[
  {
    "outputFile": "output/site.css",
    "inputFile": "input/site.less",
    "minify": {
        "enabled": true
    },
    "includeInProject": true,
    "sourceMap": false,
    "options":{}
  },
  {
    "outputFile": "output/scss.css",
    "inputFile": "input/scss.scss",
    "minify": {
        "enabled": true
    },
    "includeInProject": true,
    "sourceMap": true,
    "options":{}
  }
]
```
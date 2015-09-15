# Roadmap

- [ ] Generate gulpfile.js from compilerconfig.json (#34)
- [ ] Preview window (#6)
- [ ] File globbing pattern support (#49)
- [x] Show compiler warnings as warnings in Error List (#80)
- [x] Universal Windows Apps support (#46, #84)
- [x] Better logic for (re-)building the node.js modules (#98)
- [x] ES6 and JSX compiler using Babel
- [x] Better minification default options
- [x] Don't check out files that didn't change (#101)
- [x] SaveAll no longer produces an error (#97)

Features that have a checkmark are complete and available for
download in the
[nightly build](http://vsixgallery.com/extension/148ffa77-d70a-407f-892b-9ee542346862/).

# Changelog

These are the changes to each version that has been released
on the official Visual Studio extension gallery.

## 1.3

**2015-08-24**

- [x] MSBuild targets `Compile` instead of `Build` (#73)
- [x] Task Runner Explorer integration
- [x] Command line support
- [x] Optimize compilation of newly added configs (#72)
- [x] Use `node-sass` to compile Sass files (#74)
- [x] Source Maps support for Sass compiler
- [x] LESS: Adjust relative paths in @import files by default
- [x] Project wide global compiler options (#69)

## 1.2

**2015-08-07**

- [x] Shortcut to easily setup compilation of single files
- [x] LESS: Added StrictUnits and RootPath compiler support
- [x] LESS: Added relative URL compiler support (#63)
- [x] Use the official CoffeeScript compiler (#68)
- [x] Enable source maps for Iced CoffeeScript
- [x] Compile up the LESS @import chain (#67)

## 1.1

**2015-08-03**

- [x] Support paths with spaces (#63)
- [x] Added "Compile All" button (#61)
- [x] Support for Iced CoffeeScript

## 1.0

**2015-07-20**

- [x] Compilation of LESS, Scss and (Iced)CoffeeScript files
- [x] Saving a source file triggers re-compilation automatically
- [x] Specify compiler options for each individual file
- [x] Error List integration
- [x] MSBuild support for CI scenarios
- [x] Minify the compiled output
- [x] Minification options for each language is customizable
- [x] Shows a watermark when opening a generated file
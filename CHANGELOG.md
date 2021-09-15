# Roadmap

- [ ] Generate gulpfile.js from compilerconfig.json (#34)
- [ ] Preview window (#6)
- [ ] File globbing pattern support (#49)
- [ ] Run compilers from a node server

Features that have a checkmark are complete and available for
download in the
[nightly build](http://vsixgallery.com/extension/148ffa77-d70a-407f-892b-9ee542346862/).

# Changelog

These are the changes to each version that has been released
on the official Visual Studio extension gallery.

## 1.13

**2021-09-06**

- [x] Update .NET version to 4.8
- [x] Update nuget packages
- [x] Migrate `node-sass` to `sass` (dart-sass) to compile Sass files

## 1.11

**2016-05-10**

- [x] Fix for Sass/LESS imports with comments
- [x] Fix for path containing spaces

## 1.10

**2016-01-19**

- [x] Sass/LESS dependent file support

## 1.9

**2015-12-18**

- [x] Set build action to "None" on default file (#159)
- [x] Minify even when empty (#157)
- [x] Updated node.exe to version 0.12.9
- [x] Fixed issue in the JSON Schema
- [x] Fixed typo in config file (#148)

## 1.8

**2015-10-26**

- [x] Default settings for source maps (#120)
- [x] Support for Stylus compilation (.styl files)
- [x] Sass: Added _sourceMapRoot_ option (#141)
- [x] LESS: Added _sourceMapRoot_ and _sourceMapBasePath_ options
- [x] Fixed: Support for Sass' _include-path_ (#137)
- [x] Fixed: CSS minification stripped selector spaces (#138)

## 1.7

**2015-10-16**

- [x] Clean Error List on _Clean_
- [x] Use new VS2015 Error List API
- [x] MSBuild: Changed build ordering (#124)
- [x] Compile only when files have changed (#121)
- [x] Fixed loss of compile on save (#125)
- [x] LESS: Support for _less-plugin-csscomb_ (#126)
- [x] LESS: Warning no longer stops compilation (#127)
- [x] Initializing automatically in the background

## 1.6

**2015-10-06**

- [x] Add minification default settings
- [x] Support for the old Sass syntax and `.sass` files
- [x] New icon for _Delete all output files_
- [x] Re-compile when defaults file is saved
- [x] Creates output directory if it doesn't exist

## 1.5

**2015-09-29**

- [x] Delete all output files context menu command
- [x] Adjust relative CSS URLs (#104)
- [x] Re-compile support for multiple config files (#99)
- [x] MSBuild task now supports conditions
- [x] MSBuild task now uses BuildDependsOn (#73, #89)
- [x] AutoPrefixer support in the LESS compiler (#35)
- [x] LESS: Added `--no-ie-compat` compiler switch (#111)
- [x] Sass: Adjusted SourceMap relative paths (#108)
- [x] Moved context menu into its own sub menu

## 1.4

**2015-09-22**

- [x] Show compiler warnings as warnings in Error List (#80)
- [x] Universal Windows Apps support (#46, #84)
- [x] Better logic for (re-)building the node.js modules (#98)
- [x] ES6 and JSX compiler using Babel
- [x] Better minification default options
- [x] Don't check out files that didn't change (#101)
- [x] SaveAll no longer produces an error (#97)

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
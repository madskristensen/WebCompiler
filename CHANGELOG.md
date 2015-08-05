# Roadmap

- [x] Shortcut to easily setup compilation of single files
- [x] LESS: Added StrictUnits and RootPath compiler support
- [x] LESS: Added relative URL compiler support (#63)
- [x] Use the official CoffeeScript compiler (#68)
- [ ] Generate gulpfile.js from compilerconfig.json (#34)
- [ ] Preview window (#6)
- [ ] File globbing pattern support (#49)
- [ ] Sweet.js compiler support
- [ ] LiveScript compiler support

Features that have a checkmark are complete and available for
download in the
[nightly build](http://vsixgallery.com/extension/148ffa77-d70a-407f-892b-9ee542346862/).

# Changelog

These are the changes to each version that has been released
on the official Visual Studio extension gallery.

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
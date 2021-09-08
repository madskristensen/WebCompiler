@echo off

:: IMPORTANT!! npm 3.x is required to avoid long path exceptions

if exist "%~dp0..\src\WebCompiler\node\node_modules.7z" goto:EOF

if not exist "%~dp0..\src\WebCompiler\node" md "%~dp0..\src\WebCompiler\node"

pushd "%~dp0..\src\WebCompiler\node"

echo Installing packages...
call npm install --quiet ^
        babel-cli ^
        iced-coffee-script ^
        less ^
        less-plugin-autoprefix ^
        less-plugin-csscomb ^
        sass ^
        postcss-cli ^
        autoprefixer ^
        stylus ^
        handlebars ^
        > nul
call npm install --quiet > nul

::if not exist "node_modules\node-sass\vendor\win32-ia32-48" (
::    echo Copying node binding...
::    md "node_modules\node-sass\vendor\win32-ia32-48"
::    copy binding.node "node_modules\node-sass\vendor\win32-ia32-48"
::)

echo Deleting unneeded files and folders...
del /s /q *.html > nul
del /s /q *.markdown > nul
del /s /q *.md > nul
del /s /q *.npmignore > nul
del /s /q *.patch > nul
del /s /q *.txt > nul
del /s /q *.yml > nul
del /s /q .editorconfig > nul
del /s /q .eslintrc > nul
del /s /q .gitattributes > nul
del /s /q .jscsrc > nul
del /s /q .jshintrc > nul
del /s /q CHANGELOG > nul
del /s /q CNAME > nul
del /s /q example.js > nul
del /s /q generate-* > nul
del /s /q gruntfile.js > nul
del /s /q gulpfile.* > nul
del /s /q makefile.* > nul
del /s /q README > nul

for /d /r . %%d in (benchmark)  do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (bench)      do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (doc)        do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (docs)       do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (example)    do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (examples)   do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (images)     do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (man)        do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (media)      do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (scripts)    do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (test)       do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (tests)      do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (testing)    do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (tst)        do @if exist "%%d" rd /s /q "%%d" > nul

echo Compressing artifacts and cleans up...
"%~dp07z.exe" a -r -mx9 node_modules.7z node_modules > nul
rmdir /S /Q node_modules > nul


:done
echo Done
pushd "%~dp0"

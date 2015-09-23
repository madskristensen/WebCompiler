@echo off

if exist %~dp0..\src\WebCompiler\node\node_modules.7z goto:EOF

if not exist %~dp0..\src\WebCompiler\node md %~dp0..\src\WebCompiler\node

pushd %~dp0..\src\WebCompiler\node

echo Installing packages...
call npm install flatten-packages -g --no-optional --quiet > nul
call npm install less --no-optional --quiet > nul
call npm install less-plugin-autoprefix --no-optional --quiet > nul
call npm install iced-coffee-script --no-optional --quiet > nul
call npm install node-sass --no-optional --quiet > nul
call npm install babel --no-optional --quiet > nul

if not exist "node_modules\node-sass\vendor\win32-ia32-14" (
    md "node_modules\node-sass\vendor\win32-ia32-14"
    copy binding.node "node_modules\node-sass\vendor\win32-ia32-14"
)

echo Flattening node_modules...
call flatten-packages > nul


echo Deleting unneeded files and folders
del /s /q *.md > nul
del /s /q *.markdown > nul
del /s /q *.html > nul
del /s /q *.txt > nul
del /s /q *.old > nul
del /s /q *.patch > nul
del /s /q *.yml > nul
del /s /q *.npmignore > nul
del /s /q makefile.* > nul
del /s /q generate-* > nul
del /s /q .jshintrc > nul
del /s /q .jscsrc > nul
del /s /q LICENSE > nul
del /s /q README > nul
del /s /q CHANGELOG > nul
del /s /q CNAME > nul

for /d /r . %%d in (benchmark)  do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (doc)        do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (example)    do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (examples)   do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (images)     do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (man)        do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (media)      do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (scripts)    do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (test)       do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (testing)    do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (tst)        do @if exist "%%d" rd /s /q "%%d" > nul

echo Compressing artifacts and cleans up

:: Zips and deletes the node_modules folder
%~dp07z.exe a -r -mx9 node_modules.7z node_modules > nul
rmdir /S /Q node_modules > nul

:: Zips and deletes node.exe
::%~dp07z.exe a -r -mx9 node.7z node.exe > nul
::del node.exe > nul

:done
pushd "%~dp0"
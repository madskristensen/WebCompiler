@echo off
:: Use ConsoleAppLauncher NuGet package https://github.com/slavagu/ConsoleAppLauncher

:: create httpget.js for use with cscript.exe
set HTTPGET_JS="%TEMP%\httpget.js"
echo (function(b,d){var a=b.Arguments(0);var c=b.Arguments(1);var f=new d("MSXML2.XMLHTTP");f.open("GET",a,false);f.send();if(f.Status==200){var e=new d("ADODB.Stream");e.Open();e.Type=1;e.Write(f.ResponseBody);e.Position=0;e.SaveToFile(c);e.Close()}else{b.Echo("Error: HTTP "+f.status+" "+f.statusText)}})(WScript,ActiveXObject); > %HTTPGET_JS%

if exist %~dp0..\src\WebCompiler\node\node.7z goto:EOF

if not exist %~dp0..\src\WebCompiler\node md %~dp0..\src\WebCompiler\node

pushd %~dp0..\src\WebCompiler\node

if not exist node.7z (
    echo Downloading node...
    cscript //nologo %HTTPGET_JS% http://nodejs.org/dist/latest/node.exe node.exe
)


echo Installing packages...
call npm install flatten-packages -g --quiet > nul
call npm install less --quiet > nul
call npm install iced-coffee-script --quiet > nul


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

echo Compresses artifacts and cleans up

:: Zips and deletes the node_modules folder
%~dp07z.exe a -r -mx9 node_modules.7z node_modules > nul
rmdir /S /Q node_modules > nul

:: Zips and deletes node.exe
%~dp07z.exe a -r -mx9 node.7z node.exe > nul
del node.exe > nul

:done
pushd "%~dp0"
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
    %~dp07z.exe a -r -mx9 node.7z node.exe
    del node.exe
)


echo Installing packages...
call npm install flatten-packages -g --quiet
call npm install less --quiet
call npm install iced-coffee-script --quiet


echo Flatten the node_modules
call flatten-packages


echo Deleting unneeded files and folders
del /s /q *.md
del /s /q *.markdown
del /s /q *.html
del /s /q *.txt
del /s /q *.old
del /s /q *.patch
del /s /q *.ico
del /s /q *.yml
del /s /q *.tscache
del /s /q *.npmignore
del /s /q makefile.*
del /s /q rakefile.*
del /s /q generate-*
del /s /q .jshintrc
del /s /q .jscsrc
del /s /q .bowerrc
del /s /q LICENSE
del /s /q README
del /s /q CHANGELOG
del /s /q CNAME

for /d /r . %%d in (benchmark)  do @if exist "%%d" rd /s /q "%%d"
for /d /r . %%d in (doc)        do @if exist "%%d" rd /s /q "%%d"
for /d /r . %%d in (example)    do @if exist "%%d" rd /s /q "%%d"
for /d /r . %%d in (examples)   do @if exist "%%d" rd /s /q "%%d"
for /d /r . %%d in (images)     do @if exist "%%d" rd /s /q "%%d"
for /d /r . %%d in (man)        do @if exist "%%d" rd /s /q "%%d"
for /d /r . %%d in (media)      do @if exist "%%d" rd /s /q "%%d"
for /d /r . %%d in (scripts)    do @if exist "%%d" rd /s /q "%%d"
for /d /r . %%d in (test)       do @if exist "%%d" rd /s /q "%%d"
for /d /r . %%d in (testing)    do @if exist "%%d" rd /s /q "%%d"
for /d /r . %%d in (tst)        do @if exist "%%d" rd /s /q "%%d"


:: Zips the node_modules folder
%~dp07z.exe a -r -mx9 node_modules.7z node_modules


:: Deletes the node_modules folder after it has been zipped
rmdir /S /Q node_modules


:done
pushd "%~dp0"
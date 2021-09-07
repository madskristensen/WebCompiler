7za.exe x -y node.7z node.exe
del /q node.7z

7za.exe x -y node_modules.7z
del /q node_modules.7z

del /q 7za.exe
del /q prepare.cmd

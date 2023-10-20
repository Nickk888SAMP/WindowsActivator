Dim WinScriptHost
Set WinScriptHost = CreateObject("WScript.Shell")

' Get the full path of the currently executing script
Dim scriptPath
scriptPath = WScript.ScriptFullName

' Extract the directory from the script path
Dim scriptDirectory
scriptDirectory = Left(scriptPath, InStrRev(scriptPath, "\"))

' Construct the path to Windows Activator.exe
Dim activatorPath
activatorPath = scriptDirectory & "*FILENAME*"

Dim arguments
arguments = "-iea -ac -opt:2"

WinScriptHost.Run Chr(34) & activatorPath & Chr(34) & " " & arguments, 0
Set WinScriptHost = Nothing
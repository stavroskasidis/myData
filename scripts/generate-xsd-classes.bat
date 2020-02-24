@echo off
cd /d "%~dp0"
pushd ..\temp\

rem Clean the generated/copied files
del /q "*.xsd"
del /q "*.cs"

rem copy files to temp folder to generate there
xcopy "..\src\myData.Client\Schema\*.xsd" ".\" /s /y

rem Generate the .cs files based on the xsds
setlocal EnableDelayedExpansion
for /f "usebackq delims=|" %%f in (`dir /b *.xsd`) do (
 	set "files=!files! %%f"
)
setlocal DisableDelayedExpansion

"%~dp0"xsd.exe %files% /classes /namespace:myData.Client.Schema

setlocal EnableDelayedExpansion
for /f "usebackq delims=|" %%f in (`dir /b *.cs`) do (
 	rename %%f GeneratedClasses.cs
	if %errorlevel% neq 0 exit /b %errorlevel%
)
setlocal DisableDelayedExpansion

rem copy generated file back into project
xcopy "GeneratedClasses.cs" "..\src\myData.Client\Schema\" /s /y
if %errorlevel% neq 0 exit /b %errorlevel%
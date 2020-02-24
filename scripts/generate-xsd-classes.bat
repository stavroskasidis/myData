rem @echo off
cd /d "%~dp0"
pushd ..\src\myData.Client\

rem Clean the generated/copied files
rmdir /s /q "obj\Schema\"

rem copy files to obj folder to generate there
xcopy "Schema\*.xsd" "obj\Schema\" /s /y

rem Generate the .cs files based on the xsds
pushd "obj\Schema"
setlocal EnableDelayedExpansion
for /f "usebackq delims=|" %%f in (`dir /b *.xsd`) do (
	set "files=!files! %%f"
)
setlocal DisableDelayedExpansion
"%~dp0"xsd.exe %files% /classes /namespace:myData.Client.Schema
if %errorlevel% neq 0 exit /b %errorlevel%

setlocal EnableDelayedExpansion
for /f "usebackq delims=|" %%f in (`dir /b *.cs`) do (
	rename %%f Generated.cs
	if %errorlevel% neq 0 exit /b %errorlevel%
)
setlocal DisableDelayedExpansion

rem copy generated file back into project
xcopy "Generated.cs" "..\..\Schema\" /s /y
if %errorlevel% neq 0 exit /b %errorlevel%
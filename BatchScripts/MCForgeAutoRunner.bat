@echo off
setlocal enabledelayedexpansion

REM Find the Forge JAR file dynamically
set "FORGE_FOLDER=libraries\net\minecraftforge\forge\"
for /d %%i in (%FORGE_FOLDER%*) do (
    set "FORGE_VERSION=%%~nxi"
)

REM Forge requires a configured set of both JVM and program arguments.
REM Add custom JVM arguments to the user_jvm_args.txt
REM Add custom program arguments {such as nogui} to this file in the next line before the %* or
REM pass them to this script directly

REM Use the dynamically found Forge version
java @user_jvm_args.txt @%FORGE_FOLDER%%FORGE_VERSION%\win_args.txt %*

pause
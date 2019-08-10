@ECHO OFF
SETLOCAL
setlocal enabledelayedexpansion
CALL %~dp0\InitializeEnvironment.bat || EXIT /b 10

IF "%1"=="" (SET "Configuration=Debug") ELSE (SET "Configuration=%1")
IF "%2"=="" (SET "ScalarVersion=0.2.173.2") ELSE (SET "ScalarVersion=%2")

SET SolutionConfiguration=%Configuration%.Windows

SET nuget="%Scalar_TOOLSDIR%\nuget.exe"
IF NOT EXIST %nuget% (
  mkdir %nuget%\..
  powershell -ExecutionPolicy Bypass -Command "Invoke-WebRequest 'https://dist.nuget.org/win-x86-commandline/latest/nuget.exe' -OutFile %nuget%"
)

:: Acquire vswhere to find dev15 installations reliably.
SET vswherever=2.6.7
%nuget% install vswhere -Version %vswherever% || exit /b 1
SET vswhere=%Scalar_PACKAGESDIR%\vswhere.%vswherever%\tools\vswhere.exe

:: Assumes default installation location for Windows 10 SDKs
IF NOT EXIST "c:\Program Files (x86)\Windows Kits\10\Include\10.0.10240.0" (
  echo ERROR: Could not find Windows 10 SDK Version 10240
  exit /b 1
)

:: Use vswhere to find the latest VS installation with the msbuild component.
:: See https://github.com/Microsoft/vswhere/wiki/Find-MSBuild
for /f "usebackq tokens=*" %%i in (`%vswhere% -all -prerelease -latest -products * -requires Microsoft.Component.MSBuild Microsoft.VisualStudio.Workload.ManagedDesktop Microsoft.VisualStudio.Workload.NativeDesktop Microsoft.VisualStudio.Workload.NetCoreTools Microsoft.Net.Core.Component.SDK.2.1 -find MSBuild\**\Bin\amd64\MSBuild.exe`) do (
 set msbuild="%%i"
)

IF NOT DEFINED msbuild (
  echo ERROR: Could not locate a Visual Studio installation with required components.
  echo Refer to Readme.md for a list of the required Visual Studio components.
  exit /b 10
)

%msbuild% %Scalar_SRCDIR%\Scalar.sln /p:ScalarVersion=%ScalarVersion% /p:Configuration=%SolutionConfiguration% /p:Platform=x64 || exit /b 1

ENDLOCAL

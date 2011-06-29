@ECHO OFF
%SYSTEMROOT%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe iSynaptic.Core.msbuild /t:ValidateCodeCoverage %*
PAUSE
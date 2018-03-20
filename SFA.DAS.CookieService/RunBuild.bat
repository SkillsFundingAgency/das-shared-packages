"tools\nuget\nuget.exe" "install" "FAKE" "-OutputDirectory" "tools" "-ExcludeVersion"
"tools\nuget\nuget.exe" "install" "NUnit.Console" "-OutputDirectory" "tools" "-ExcludeVersion"
"tools\nuget\nuget.exe" "install" "xunit.runner.console" "-OutputDirectory" "tools" "-ExcludeVersion"
"tools\nuget\nuget.exe" "install" "PhantomJS" "-OutputDirectory" "tools" "-ExcludeVersion"

"tools\FAKE\tools\FAKE.exe" "Build.fsx" %2 "publishDirectory=Publish" "buildMode=%1" "playList=%3"

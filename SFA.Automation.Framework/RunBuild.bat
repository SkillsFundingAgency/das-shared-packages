"tools\nuget\nuget.exe" "install" "FAKE" "-OutputDirectory" "tools" "-ExcludeVersion"
"tools\nuget\nuget.exe" "install" "NUnit.Console" "-OutputDirectory" "tools" "-ExcludeVersion"

tools\FAKE\tools\FAKE.exe Build.fsx publishDirectory="Publish" buildMode=%1 target=%2

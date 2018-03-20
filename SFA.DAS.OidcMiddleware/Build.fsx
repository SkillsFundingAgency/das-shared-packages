#r @"tools/FAKE/tools/FakeLib.dll"
#r @"tools/FAKE/tools/Fake.IIS.dll"
#r @"tools/FAKE/tools/Microsoft.Web.Administration.dll"

open Fake

#load "DefaultTargets.fsx"
#load "CustomTargets.fsx"

"Set version number"
   ==>"Set Solution Name"
   ==>"Clean Directories"
   ==>"Build Projects"
   ==>"Building Acceptance Tests"
    //==>"Run Acceptance Tests"

"Set version number"
   ==>"Set Solution Name"
   ==>"Update Assembly Info Version Numbers"
   ==>"Clean Directories" 
   ==>"Build Projects"
   ==>"Run NUnit Tests"
   ==>"Run XUnit Tests"
   ==>"Run Jasmine Tests"
   ==>"Build Cloud Projects"
   ==>"Build Database project"
   ==>"Build WebJob Project" 
   ==>"Publish Solution"  
   ==>"Compile Views"
   ==>"Create Nuget Package"

"Set Solution Name"
    ==> "Build Database project"
    ==> "Publish Database project"

"Set version number"
    ==>"Set Solution Name"
    ==> "Zip Compiled Source"
   
RunTargetOrDefault  "Create Nuget Package"
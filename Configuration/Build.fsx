#r @"tools/FAKE/tools/FakeLib.dll"
#r @"tools/FAKE/tools/Fake.IIS.dll"
#r @"tools/FAKE/tools/Microsoft.Web.Administration.dll"

open Fake

open Fake.IISHelper

let findNuget = @"tools/nuget"

RestorePackages()

let nUnitToolPath = @"tools\NUnit.ConsoleRunner\tools\nunit3-console.exe"
let rootPublishDirectory = getBuildParamOrDefault "publishDirectory"  @"C:\CompiledSource"
let testDirectory = getBuildParamOrDefault "buildMode" "Debug"
let myBuildConfig = if testDirectory = "Release" then MSBuildRelease else MSBuildDebug
let userPath = getBuildParamOrDefault "userDirectory" @"C:\Users\buildguest\"

let publishNuget = getBuildParamOrDefault "publishNuget" "false"
let nugetOutputDirectory = getBuildParamOrDefault "nugetOutputDirectory" "bin/Release"
let nugetAccessKey = getBuildParamOrDefault "nugetAccessKey" ""

let isAutomationProject = getBuildParamOrDefault "AcceptanceTests" "false"

let devWebsitePort = getBuildParamOrDefault "devport" "7071"
let accWebsitePort = getBuildParamOrDefault "accport" "5051"

let mutable projectName = ""
let mutable folderPrecompiled = @"\"+ projectName + ".Release_precompiled "
let mutable publishDirectory = rootPublishDirectory @@ projectName
let mutable publishingProfile = projectName + "PublishProfile"
let mutable shouldPublishSite = false

let mutable versionNumber = getBuildParamOrDefault "versionNumber" "1.0.0.0"

let mutable solutionFilePresent = true

Target "Set Solution Name" (fun _ ->
    
    let directoryHelper = FileSystemHelper.directoryInfo(currentDirectory).Name

    

    let mutable solutionNameToMatch = ""
    if isAutomationProject.ToLower() = "false" then 
        solutionNameToMatch <- "*.sln" 
    else 
        solutionNameToMatch <- "*Automation.sln"

    let findSolutionFile = TryFindFirstMatchingFile "*.sln" currentDirectory
    
    if findSolutionFile.IsSome then
        
        let solutionFileHelper = FileSystemHelper.fileInfo(findSolutionFile.Value)
            
        projectName <- solutionFileHelper.Name.Replace(solutionFileHelper.Extension, "")
        folderPrecompiled <- sprintf @"\%s.%s_precompiled " projectName testDirectory
        publishDirectory <- rootPublishDirectory @@  projectName
        publishingProfile <- projectName + "PublishProfile"

        let subDirectories = directoryInfo(currentDirectory).GetDirectories()
    
        if subDirectories.Length > 0 then 
            for directory in subDirectories do
                if shouldPublishSite = false then 
                    shouldPublishSite <- fileExists((directory.FullName @@ @"Properties\PublishProfiles\" @@ publishingProfile + ".pubxml"))
                
        else
            shouldPublishSite <- false
    
        versionNumber <- "1.0.0.0"    
        let assemblyMajorNumber = environVarOrDefault "BUILD_MAJORNUMBER" "1" 
        let assemblyMinorNumber = environVarOrDefault "BUILD_MINORNUMBER" "0" 

        if testDirectory.ToLower() = "release" then
            versionNumber <- buildVersion
            if versionNumber.ToLower() <> "localbuild" then
                versionNumber <- sprintf  @"%s.%s.0.%s" assemblyMajorNumber assemblyMinorNumber buildVersion
            else
                versionNumber <- "1.0.0.0"

        trace ("Will publish: " + (shouldPublishSite.ToString()))
        trace ("PublishingProfile: " + publishingProfile)
        trace ("PublishDirectory: " + publishDirectory)
        trace ("PrecompiledFolder: " + folderPrecompiled)
        trace ("VersionNumber:" + versionNumber)
        trace ("Project Name has been set to: " + projectName)
    else
        solutionFilePresent <- false

)

Target "Update Assembly Info Version Numbers"(fun _ ->

    if testDirectory.ToLower() = "release" then
        trace "Update Assembly Info Version Numbers"
        BulkReplaceAssemblyInfoVersions(currentDirectory) (fun p ->
                {p with
                    AssemblyFileVersion = versionNumber 
                    AssemblyVersion = versionNumber 
                    })
)

Target "Clean Publish Directory" (fun _ ->
    trace "Clean Publish Directory"

    if FileHelper.TestDir(rootPublishDirectory) then
        FileHelper.CleanDir(rootPublishDirectory)
    else
        FileHelper.CreateDir(rootPublishDirectory)


    if FileHelper.TestDir(publishDirectory) then
        FileHelper.CleanDir(publishDirectory)
    else
        FileHelper.CreateDir(publishDirectory)
    

    if FileHelper.TestDir(publishDirectory) then
        FileHelper.CleanDir(publishDirectory)
    else
        FileHelper.CreateDir(publishDirectory)

    let directoryinfo = FileSystemHelper.directoryInfo(EnvironmentHelper.combinePaths publishDirectory @"\..\" + folderPrecompiled)
    let directory = directoryinfo.FullName
    trace directory
    if FileHelper.TestDir(directory) then
        FileHelper.CleanDir(directory)
    else
        FileHelper.CreateDir(directory)

)

Target "Build DNX Project"(fun _ ->

    trace "Build DNX PRoject"
    let dnuDir = userPath @@ @"\.dnx\runtimes\dnx-clr-win-x86.1.0.0-rc1-update1\bin\dnu.cmd"
    trace dnuDir
        
    let result =
        ExecProcess (fun info ->
            info.FileName <- dnuDir
            info.Arguments <- @"publish .\src\MvcPOC --out .\WR\Publish --configuration Release --runtime dnx-clr-win-x86.1.0.0-rc1-update1"
            info.WorkingDirectory <- rootPublishDirectory @@ @"..\..\"
        ) (System.TimeSpan.FromMinutes 10.)
        
    if result <> 0 then failwith "Failed to build DNX project"

)

let buildSolution() = 

    if solutionFilePresent then
        let buildMode = getBuildParamOrDefault "buildMode" "Debug"

        let properties = 
                        [
                            ("TargetProfile","cloud")
                        ]

        !! (@"./" + projectName + ".sln")
            |> MSBuildReleaseExt null properties "Publish"
            |> Log "Build-Output: "


Target "Build Acceptance Solution"(fun _ ->
    buildSolution()
)



Target "Build Solution"(fun _ ->
    buildSolution()
)




Target "Publish Solution"(fun _ ->
    

    if shouldPublishSite then

        let buildMode = getBuildParamOrDefault "buildMode" "Debug"


        let directoryinfo = FileSystemHelper.directoryInfo(@".\" @@ publishDirectory)
        let directory = directoryinfo.FullName
        trace directory

        let properties = 
                        [
                            ("DebugSymbols", "False");
                            ("Configuration", buildMode);
                            ("PublishProfile", @".\" + publishingProfile + ".pubxml");
                            ("PublishUrl", directory);
                            ("DeployOnBuild","True");
                            ("ToolsVersion","14");
                        ]

        !! (@"./" + projectName + ".sln")
            |> MSBuildReleaseExt null properties "Build"
            |> Log "Build-Output: "
    else
        trace "Skipping publish"
)

Target "Cleaning Unit Tests" (fun _ ->

    trace "Cleaning Unit Tests"
    !! (".\**\*.UnitTests.csproj")
      |> myBuildConfig "" "Clean"
      |> Log "AppBuild-Output: "

)

Target "Building Unit Tests" (fun _ ->

    trace "Building Unit Tests"
    !! (".\**\*.UnitTests.csproj")
      |> myBuildConfig "" "Rebuild"
      |> Log "AppBuild-Output: "

)

Target "Run NUnit Tests" (fun _ ->


    trace "Run NUnit Tests"
    let testDlls = !! ("./*.UnitTests/bin/" + testDirectory + "/*.UnitTests.dll") 

    
    for testDll in testDlls do
        [testDll] |> Fake.Testing.NUnit3.NUnit3 (fun p ->
            {p with
                ToolPath = nUnitToolPath;
                ShadowCopy = false;
                Framework = Testing.NUnit3.NUnit3Runtime.Net45;
                ResultSpecs = ["TestResult.xml;format=nunit2"];
                })
)

Target "Cleaning Integration Tests" (fun _ ->

    trace "Cleaning Acceptance Tests"
    !! (".\**\*.AcceptanceTests.csproj")
      |> myBuildConfig "" "Clean"
      |> Log "AppBuild-Output: "

)

Target "Building Integration Tests" (fun _ ->

    trace "Building Integration Tests"
    !! (".\**\*.AcceptanceTests.csproj")
      |> myBuildConfig "" "Rebuild"
      |> Log "AppBuild-Output: "

)

Target "Run Integration Tests" (fun _ ->

    trace "Run Integration Tests"
    let testDlls = !! ("./**/bin/" + testDirectory + "/*.AcceptanceTests.dll") 
        
    for testDll in testDlls do
        [testDll] |> Fake.Testing.NUnit3.NUnit3 (fun p ->
            {p with
                ToolPath = nUnitToolPath;
                StopOnError = false;
                ResultSpecs = ["TestResult.xml;format=nunit2"];
                })
)

Target "Compile Views" (fun _ ->
    if shouldPublishSite then
        trace "Compiling views"
    
        let directoryinfo = FileSystemHelper.directoryInfo(@".\" @@ publishDirectory)
        let directory = directoryinfo.FullName
        trace directory


        let directoryinfo = FileSystemHelper.directoryInfo(publishDirectory @@ @"\..\"+ folderPrecompiled)
        let directoryOutput = directoryinfo.FullName
        trace directoryOutput


        let result =
            ExecProcess (fun info ->
                info.FileName <- ("C:/Windows/Microsoft.NET/Framework/v4.0.30319/aspnet_compiler.exe")
                info.Arguments <- @"-v \" + folderPrecompiled + " -p . " + directoryOutput
                info.WorkingDirectory <- publishDirectory
            ) (System.TimeSpan.FromMinutes 10.)
        


        if result <> 0 then failwith "Failed to compile views"
    else
        trace "skipping compiling views"
)


Target "Clean Project" (fun _ ->

    trace "Clean Project"
    trace (@".\" + projectName + "\*.csproj")
    !! (@".\" + projectName + "\*.csproj")
      |> myBuildConfig "" "Clean"
      |> Log "AppBuild-Output: "

)

Target "Build Project" (fun _ ->

    trace "Building Project"
    trace (@".\" + projectName + "\*.csproj")
    !! (@".\" + projectName + "\*.csproj")
      |> myBuildConfig "" "Rebuild"
      |> Log "AppBuild-Output: "

)

Target "Zip Compiled Source" (fun _ ->

    if shouldPublishSite then
        trace "Zip Compiled Source"

        let directoryinfo = FileSystemHelper.directoryInfo(EnvironmentHelper.combinePaths publishDirectory @"\..\" @@ folderPrecompiled)
        let directory = directoryinfo.FullName


        !! (directory + "/**/*.*") 
            -- "*.zip"
            |> Zip directory (publishDirectory @@  (sprintf  @"\..\%s.%s.zip" projectName versionNumber))
    else
        trace "Skipping zipping source"
)


let CreateSite (environment, directory, port) =
    if shouldPublishSite then
        let siteName = (sprintf "%s.%s.WebSite" projectName environment)
        let appPoolName = (sprintf "%s.%s.appPool" projectName environment)
        let port = port
        let path =  directory

        let siteConfig = SiteConfig(siteName, port, path, appPoolName)
        let appPoolConfig = ApplicationPoolConfig(appPoolName)

        (IIS
          (Site siteConfig)
          (ApplicationPool appPoolConfig)
          (None))

Target "Create Accceptance Test Site in IIS" (fun _ ->
    trace "Create Acceptance site"
    let directoryinfo = FileSystemHelper.directoryInfo(publishDirectory)
    let directory = directoryinfo.FullName
    CreateSite("Acc",directory,":" + accWebsitePort + ":")

)

Target "Create Development Site in IIS" (fun _ ->
    trace "Create Development Site"
    let directory = FileSystemHelper.directoryInfo(currentDirectory).FullName @@ projectName
    CreateSite("Dev",directory,":"+ devWebsitePort + ":")
)



Target "Create Nuget Package" (fun _ ->
    

    if testDirectory.ToLower() = "release" then
        let nupkgFiles = !! (currentDirectory + "/**/*.nuspec") 

    
        for nupkgFile in nupkgFiles do
            let fileInfo = fileSystemInfo(nupkgFile)
            let name = fileInfo.Name.Replace(fileInfo.Extension,"")
            
            (fileInfo.FullName)
            |> NuGet (fun p -> 
                {p with               
                    Authors = [name]
                    Project = name
                    Summary = name
                    Description = name
                    Version = versionNumber
                    NoPackageAnalysis = true
                    OutputPath = FileSystemHelper.DirectoryName(fileInfo.FullName) @@ nugetOutputDirectory
                    WorkingDir = FileSystemHelper.DirectoryName(fileInfo.FullName)
                    AccessKey = nugetAccessKey
                    Publish = System.Convert.ToBoolean(publishNuget)
                    })
)

"Set Solution Name"
    ==>"Build Acceptance Solution"
    ==>"Cleaning Integration Tests"
    ==>"Building Integration Tests"
    //==>"Run Integration Tests"

"Set Solution Name"
   ==>"Update Assembly Info Version Numbers"
   ==>"Clean Publish Directory"
   ==>"Build Solution"
   ==>"Publish Solution"
   ==>"Cleaning Unit Tests"
   ==>"Building Unit Tests"
   ==>"Run NUnit Tests"
   ==>"Compile Views"
   ==>"Create Nuget Package"
   ==>"Zip Compiled Source"
   ==>"Create Development Site in IIS"
   ==>"Create Accceptance Test Site in IIS"

   
RunTargetOrDefault  "Create Accceptance Test Site in IIS"
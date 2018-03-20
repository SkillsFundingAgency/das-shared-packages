#r @"tools/FAKE/tools/FakeLib.dll"
#r @"tools/FAKE/tools/Fake.IIS.dll"
#r @"tools/FAKE/tools/Microsoft.Web.Administration.dll"

open Fake

open Fake.IISHelper

let findNuget = @"tools/nuget"

RestorePackages()

let nUnitRunner = "nunit3-console.exe"
let xUnitRunner = "xunit.console.exe"
let mutable nUnitToolPath = @"tools\NUnit.ConsoleRunner\"
let mutable xUnitToolPath = @"tools\xunit.runner.console\"
let mutable nUnitRunnerLocation = "";
let mutable xUnitRunnerLocation = "";
let phantomJsPath = currentDirectory @@ @"tools\PhantomJS\tools\phantomjs\phantomjs.exe"
let jasmineRunnerPath = currentDirectory @@ @"tools\jasmine\jasminerunner.js"
//let jasmineRunnerPath = @"tools\jasmine\phantomjs-testrunner.js"
let rootPublishDirectory = getBuildParamOrDefault "publishDirectory"  @"C:\CompiledSource"
let testDirectory = getBuildParamOrDefault "buildMode" "Debug"
let myBuildConfig = if testDirectory = "Release" then MSBuildRelease else MSBuildDebug
let userPath = getBuildParamOrDefault "userDirectory" @"C:\Users\buildguest\"

let nunitTestFormat = getBuildParamOrDefault "nunitTestFormat" "nunit2"
let publishNuget = getBuildParamOrDefault "publishNuget" "false"
let nugetOutputDirectory = getBuildParamOrDefault "nugetOutputDirectory" "bin/Release"
let nugetAccessKey = getBuildParamOrDefault "nugetAccessKey" ""

let isAutomationProject = getBuildParamOrDefault "AcceptanceTests" "false"

let devWebsitePort = getBuildParamOrDefault "devport" "7071"
let accWebsitePort = getBuildParamOrDefault "accport" "5051"

let acceptanceTestPlayList = getBuildParamOrDefault "playList" ""

let mutable projectName = ""
let mutable folderPrecompiled = @"\"+ projectName + ".Release_precompiled "
let mutable publishDirectory = rootPublishDirectory @@ projectName
let mutable publishingProfile = projectName + "PublishProfile"
let mutable shouldPublishSite = false
let mutable shouldCreateDbProject = false

let mutable versionNumber = getBuildParamOrDefault "versionNumber" "1.0.0.0"

let mutable solutionFilePresent = true

let mutable directoryInfoWebPublishLocation = null

Target "Set version number" (fun _ ->
    if publishNuget.ToLower() = "false" then
        let assemblyMajorNumber = environVarOrDefault "BUILD_MAJORNUMBER" "1"
        let assemblyMinorNumber = environVarOrDefault "BUILD_MINORNUMBER" "0"

        if testDirectory.ToLower() = "release" then
            if buildVersion.Contains(".") then
                versionNumber <- buildVersion
            else if versionNumber.ToLower() <> "localbuild" then
                versionNumber <- sprintf  @"%s.%s.0.%s" assemblyMajorNumber assemblyMinorNumber buildVersion
            else
                versionNumber <- "1.0.0.0"

    else
        trace "Skipping version number set"

    trace versionNumber
)

let GetToolPath(toolPath , toolName) =
    let mainDirectory = directoryInfo(currentDirectory @@ toolPath).GetDirectories()
    let mutable toolLocation = ""
    if mainDirectory.Length > 0 then
        for directory in mainDirectory do
            if toolLocation.Equals("") then
                if fileExists(directory.FullName @@ toolName) then
                    toolLocation <-(directory.FullName @@ toolName)
                if toolLocation.Equals("") then
                    for subDirectory in directory.GetDirectories() do
                        if fileExists(subDirectory.FullName @@ toolName) then
                            toolLocation <-(subDirectory.FullName @@ toolName)
    toolLocation

Target "Set Solution Name" (fun _ ->

    let mutable solutionNameToMatch = ""
    if isAutomationProject.ToLower() = "false" then
        solutionNameToMatch <- "*.sln"
    else
        solutionNameToMatch <- "*Automation.sln"

    let findSolutionFile = TryFindFirstMatchingFile "*.sln" currentDirectory

    // check test runner paths exist

    nUnitRunnerLocation <- GetToolPath(nUnitToolPath, nUnitRunner)
    xUnitRunnerLocation <- GetToolPath(xUnitToolPath, xUnitRunner)
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
                    if shouldPublishSite = true then
                        directoryInfoWebPublishLocation <- directory

        else
            shouldPublishSite <- false

        let subdirs = FileSystemHelper.directoryInfo(currentDirectory).EnumerateDirectories("*.Database")

        let mutable databaseDir = ""

        for directs in subdirs do
            trace directs.Name
            let dirExists = directs.Name.Contains("Database")
            if(dirExists) then
                databaseDir <- directs.Name

        let sqlPublishFile = (@"./"+ databaseDir + "/Database.Publish.xml")
        shouldCreateDbProject <- fileExists(sqlPublishFile)

        trace ("Will publish: " + (shouldPublishSite.ToString()))
        trace ("Will build db project: " + (shouldCreateDbProject.ToString()))
        trace ("PublishingProfile: " + publishingProfile)
        trace ("PublishDirectory: " + publishDirectory)
        trace ("PrecompiledFolder: " + folderPrecompiled)
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

Target "Clean Directories" (fun _ ->

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

    let mutable files = !! ("./**/bin/*.*")
    files <- files.And("./**/**/debug/*.*")
    files <- files.And("./**/bin/**/*.*")
    files <- files.And("./**/**/release/*.*")
    files <- files.And("./**/obj/*.*")
    files <- files.ButNot("./**/node_modules/*.*")
    files <- files.ButNot("./**/node_modules/**/*.*")
    files <- files.ButNot("./**/release/*.cshtml")
    FileHelper.DeleteFile("./TestResult.xml")
    FileHelper.DeleteFile("./TestResultXUnit.xml")
    FileHelper.DeleteFile("./TestResultJasmine.xml")

    let directoryNames = [| for file in files -> fileInfo(file).Directory.FullName |]

    FileHelper.DeleteDirs(Seq.distinct(directoryNames)) |> ignore
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


Target "Build Cloud Projects"(fun _ ->
    let buildMode = getBuildParamOrDefault "buildMode" "Debug"

    if solutionFilePresent && buildMode.ToLower().Equals("release") then

        let fileIncludes = !! ("./**/Configuration/ServiceDefinition.*.csdef")

        let seqFileIncludes : seq<string> = Seq.cast fileIncludes

        if (Seq.length seqFileIncludes <> 0) then
            for file in fileIncludes do
                trace file
                let configurationName = FileSystemHelper.fileInfo(file).Name.Replace("ServiceDefinition.","").Replace(".csdef","")

                let directory = FileSystemHelper.fileInfo(file).DirectoryName

                let configFile = FileSystemHelper.fileInfo(file)

                let targetFile = FileSystemHelper.fileInfo(directory @@ (@"./../ServiceDefinition.csdef"))

                let fileExists = FileSystemHelper.fileExists configFile.FullName

                if fileExists && testDirectory.ToLower() = "release" then
                    let fileRename = FileSystemHelper.fileInfo((configFile.DirectoryName @@ "./ServiceDefinition.csdef")).FullName
                    FileHelper.CopyFile fileRename configFile.FullName
                    FileHelper.MoveFile targetFile.DirectoryName fileRename

                if configurationName.ToUpper() <> "LOCAL" then
                    let properties =
                            [
                                ("TargetProfile",configurationName);
                                ("OutputPath",@"bin/" @@ configurationName);
                            ]

                    !! (@"./" + directory + "/../*.ccproj")
                        |> MSBuildReleaseExt null properties "Publish"
                        |> Log "Build-Output: "
        else
            let properties =
                            [
                                ("TargetProfile","cloud");
                                ("OutputPath",@"bin/");
                            ]
            !! (@".\**\*.ccproj")
                        |> MSBuildReleaseExt null properties "Publish"
                        |> Log "Build-Output: "
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
        !! (@"./" + directoryInfoWebPublishLocation.Name + "/*.csproj")
            |> MSBuildReleaseExt null properties "Build"
            |> Log "Build-Output: "
    else
        trace "Skipping publish"
)

Target "Build Database project"(fun _ ->


    if shouldCreateDbProject then
        trace "Publish Database project"

        let properties =
                        [
                            ("DebugSymbols", "False");
                            ("SqlPublishProfilePath", @".\Database.Publish.xml");
                            ("ToolsVersion","14");
                        ]

        !! (@".\**\*.sqlproj")
            |> MSBuildReleaseExt null properties "Build"
            |> Log "Build-Output: "
    else
        trace "Skipping Build Database project"

)

Target "Publish Database project"(fun _ ->

    if shouldCreateDbProject then
        trace "Publish Database project"

        trace (@".\" + projectName + ".Database.Publish.xml")

        let properties =
                        [
                            ("DebugSymbols", "False");
                            ("TargetDatabaseName", "Database");
                            ("SqlPublishProfilePath", @".\Database.Publish.xml");
                            ("TargetConnectionString", "Data Source=.;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=60;Encrypt=False;TrustServerCertificate=True");
                            ("PublishScriptFileName","Database.sql");
                            ("ToolsVersion","14");
                            ("PublishToDatabase","true");
                        ]

        !! (@".\**\*.sqlproj")
            |> MSBuildReleaseExt null properties "Publish"
            |> Log "Build-Output: "
    else
        trace "Skipping Publish Database project"
)

Target "Build WebJob Project" ( fun _ ->
    let buildMode = getBuildParamOrDefault "buildMode" "Debug"

    if buildMode.ToLower().Equals("release") then
        let directoryinfo = FileSystemHelper.directoryInfo(@".\" @@ publishDirectory @@ "\..\WebJob")
        let directory = directoryinfo.FullName
        traceImportant directory
        let properties =
                        [
                            ("DeployOnBuild", "True");
                            ("WebPublishMethod", "Package");
                            ("SkipInvalidConfigurations", "true");
                            ("PackageLocation", directory);
                            ("ToolsVersion","14");
                        ]

        !! (@".\**\*.WebJob.csproj")
            |> MSBuildReleaseExt null properties "Build"
            |> Log "Build-Output: "

)

Target "Run NUnit Tests" (fun _ ->

    trace "Run NUnit Tests"

    let mutable shouldRunTests = false

    let testDlls = !! ("./*.UnitTests/bin/" + testDirectory + "/*.UnitTests.dll")

    for testDll in testDlls do
        shouldRunTests <- true

    if shouldRunTests then
        testDlls  |>
            Fake.Testing.NUnit3.NUnit3 (fun p ->
            {p with
                ToolPath = nUnitRunnerLocation;
                ShadowCopy = false;
                Framework = Testing.NUnit3.NUnit3Runtime.Net45;
                ResultSpecs = [("TestResult.xml;format=" + nunitTestFormat)];
                })
)

Target "Run XUnit Tests" (fun _ ->

    trace "Run XUnit Tests"

    let mutable shouldRunTests = false

    let testDlls = !! ("./*.XUnitTests/bin/" + testDirectory + "/*.XUnitTests.dll")

    for testDll in testDlls do
        shouldRunTests <- true

    if shouldRunTests then
        testDlls  |>
            Fake.Testing.XUnit2.xUnit2(fun p ->
            {p with
                ToolPath = xUnitRunnerLocation;
                ShadowCopy = false;
                XmlOutputPath = Some ("TestResultXUnit.xml");
                })

)

Target "Run Jasmine Tests" (fun _ ->
    trace "Run Jasmine Tests"

    let subdirs = FileSystemHelper.directoryInfo(currentDirectory).EnumerateDirectories("*.JasmineTests")

    let mutable jamsineTestDir = ""

    for directs in subdirs do
        if (directs.Exists) then
            trace directs.Name
            let dirExists = directs.Name.Contains("JasmineTests")
            if(dirExists) then
                jamsineTestDir <- directs.FullName


    if jamsineTestDir = "" then
        trace "Skipping Jasmine Tests"
    else
        let specRunnerFile = TryFindFirstMatchingFile "SpecRunner.html" (jamsineTestDir)

        if specRunnerFile.IsNone then
            trace "Skipping Jasmine Tests"
        else
            let result =
                    ExecProcess (fun info ->
                        info.FileName <- phantomJsPath
                        info.Arguments <- jasmineRunnerPath + @" file:///" + specRunnerFile.Value.Replace(@"\",@"/")
                        info.RedirectStandardError <- true
                    ) (System.TimeSpan.FromMinutes 10.)

            if result <> 0 then failwith "Failed to run Jasmine Tests"
)


Target "Run Acceptance Tests" (fun _ ->

    trace "Run Acceptance Tests"

    let mutable shouldRunTests = false

    let testDlls = !! ("./**/bin/" + testDirectory + "/*.AcceptanceTests.dll")

    for testDll in testDlls do
        shouldRunTests <- true

    if shouldRunTests then
        testDlls |> Fake.Testing.NUnit3.NUnit3 (fun p ->
            {p with
                ToolPath = nUnitToolPath;
                StopOnError = false;
                Agents = Some 1;
                Testlist = acceptanceTestPlayList;
                ResultSpecs = [("TestResult.xml;format=" + nunitTestFormat)];
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
            ) (System.TimeSpan.FromMinutes 30.)



        if result <> 0 then failwith "Failed to compile views"
    else
        trace "skipping compiling views"
)

Target "Build Projects" (fun _ ->

    trace "Building Projects"

    !! (@".\**\*.csproj")
      |> myBuildConfig "" "Rebuild"
      |> Log "AppBuild-Output: "

)

Target "Building Acceptance Tests" (fun _ ->

    trace "Building Acceptance Tests"
    !! (".\**\*.AcceptanceTests.csproj")
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

            let mutable outputPath = FileSystemHelper.DirectoryName(fileInfo.FullName)

            if FileSystemHelper.directoryExists(FileSystemHelper.DirectoryName(fileInfo.FullName) @@ nugetOutputDirectory) then
                outputPath <- FileSystemHelper.DirectoryName(fileInfo.FullName) @@ nugetOutputDirectory

            (fileInfo.FullName)
            |> NuGet (fun p ->
                {p with
                    Authors = [name]
                    Project = name
                    Summary = name
                    Description = name
                    Version = versionNumber
                    NoPackageAnalysis = true
                    OutputPath = outputPath
                    WorkingDir = FileSystemHelper.DirectoryName(fileInfo.FullName)
                    AccessKey = nugetAccessKey
                    Publish = System.Convert.ToBoolean(publishNuget)
                    })
)

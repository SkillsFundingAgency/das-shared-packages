using System.Collections.Generic;

namespace SFA.DAS.Testing.PackageScanning;

public class PackageScanResult
{
    public List<string> VulnerablePackageResults { get; set; } = [];
    public List<string> DeprecatedPackageResults { get; set; } = [];
}
function Invoke-CITest
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true, Position = 0)]
        $BuildFolder,

        [Parameter(Position = 1)]
        $AdditionalArgs,

        [Parameter(Mandatory = $false)]
        $Configuration = $env:CONFIGURATION,

        [Parameter(Mandatory = $true)]
        [switch]$IsCore
    )

    Invoke-CIPowerShellTest $BuildFolder $AdditionalArgs -IsCore:$IsCore
    Invoke-CICSharpTest $BuildFolder $AdditionalArgs $Configuration -IsCore:$IsCore
}

function Invoke-CICSharpTest
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true, Position = 0)]
        $BuildFolder,

        [Parameter(Position = 1)]
        $AdditionalArgs,

        [Parameter(Mandatory = $false, Position = 2)]
        $Configuration = $env:CONFIGURATION,

        [Parameter(Mandatory = $true)]
        [switch]$IsCore,

        [Parameter(Mandatory = $false)]
        [switch]$Integration
    )

    Write-LogInfo "`tExecuting C# tests"

    $relativePath = (Get-TestProject $IsCore $Integration).CSProj

    $csproj = Join-Path $BuildFolder $relativePath

    Write-Verbose "Using csproj '$csproj'"

    if($IsCore)
    {
        Invoke-CICSharpTestCore $csproj $Configuration $AdditionalArgs
    }
    else
    {
        Invoke-CICSharpTestFull $BuildFolder $Configuration $AdditionalArgs
    }
}

function Invoke-CICSharpTestCore($csproj, $Configuration, $AdditionalArgs)
{
    $dotnetTestArgs = @(
        "test"
        $csproj
        "-nologo"
        "--no-restore"
        "--no-build"
        "--verbosity:n"
        "-c"
        $Configuration
    )

    if($AdditionalArgs)
    {
        $dotnetTestArgs += $AdditionalArgs
    }

    Install-CIDependency dotnet

    Write-Verbose "Executing command 'dotnet $dotnetTestArgs'"

    Invoke-Process { & "dotnet" @dotnetTestArgs } -WriteHost
}

function Invoke-CICSharpTestFull($BuildFolder, $Configuration, $AdditionalArgs)
{
    $vsTestArgs = @(
        Join-Path $BuildFolder "PrtgAPI.Tests.UnitTests\bin\$Configuration\PrtgAPI.Tests.UnitTests.dll"
    )

    if($AdditionalArgs)
    {
        $vsTestArgs += $AdditionalArgs
    }

    $vstest = Get-VSTest

    Write-Verbose "Executing command $vstest $vsTestArgs"

    Invoke-Process {
        & $vstest $vsTestArgs
    } -WriteHost
}

function Invoke-CIPowerShellTest
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true, Position = 0)]
        $BuildFolder,

        [Parameter(Position = 1)]
        $AdditionalArgs,

        [Parameter(Mandatory = $true)]
        [switch]$IsCore,

        [Parameter(Mandatory = $false)]
        [switch]$Integration
    )

    Write-LogInfo "`tExecuting PowerShell tests"

    $relativePath = (Get-TestProject $IsCore $Integration).PowerShell

    $directory = Join-Path $BuildFolder $relativePath

    Install-CIDependency Pester

    Invoke-Pester $directory -PassThru @AdditionalArgs -ExcludeTag Build
}
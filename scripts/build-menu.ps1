#requires -Version 5.1
<#
  MGModSeries-CSharp 交互式构建菜单
  ==================================
  用法（交互式）：
      .\build-menu.ps1             ← ↑↓ 选择 + Enter 确认
  用法（非交互，供脚本/测试）：
      .\build-menu.ps1 -Run all          # 一键编译（先清空 Build）
      .\build-menu.ps1 -Run mgmodserver  # 单独编译 MGModServer
      .\build-menu.ps1 -Run mggtmod      # 单独编译 MGGTMod
      .\build-menu.ps1 -Run mgeditor     # 单独编译 MGModEditor
      .\build-menu.ps1 -Run mgclient     # 单独编译 MGModClient

  构建逻辑：
    - 一键编译：先删除 Build\ 下全部内容，再构建四个项目（slnx，-c Release）
    - 单独编译 MGModServer：删除 Build\SPT_Runtime\user\mods\MGMod\ 整个目录，再编译
    - 单独编译 MGGTMod    ：删除 Build\SPT_Runtime\user\mods\MGGTMod\ 整个目录，再编译
    - 单独编译 MGModEditor：不删除目录（与 MGModServer 共用输出目录，避免误删），
                            构建后自动 publish 单文件覆盖到 mods\MGMod\
    - 单独编译 MGModClient ：删除 Build\BepInEx\plugins\MGModClient\ 整个目录，再编译

  路径统一由仓库根 Directory.Build.props 定义；本脚本按仓库根定位。
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [ValidateSet("all", "mgmodserver", "mggtmod", "mgeditor", "mgclient")]
    [string]$Run = ""
)

$ErrorActionPreference = "Stop"

# 控制台 UTF-8 输出（脚本为 UTF-8 with BOM；入口 .cmd 已 chcp 65001）
try { [Console]::OutputEncoding = [System.Text.Encoding]::UTF8 } catch { }

# ---------- 路径与配置 ----------
$RepoRoot    = Split-Path -Parent $PSScriptRoot          # 仓库根
$BuildRoot   = Join-Path $RepoRoot "Build"
$ModsRoot    = Join-Path $BuildRoot "SPT_Runtime\user\mods"
$MGModOut    = Join-Path $ModsRoot "MGMod"
$MGGTModOut  = Join-Path $ModsRoot "MGGTMod"
$ClientOut   = Join-Path $BuildRoot "BepInEx\plugins\MGModClient"
$Slnx        = Join-Path $RepoRoot "MGModSeries-CSharp.slnx"
$MGModCsproj = Join-Path $RepoRoot "MGModServer\MGMod.csproj"
$MGGTCsproj  = Join-Path $RepoRoot "MGGTMod\MGGTMod.csproj"
$MGCsproj    = Join-Path $RepoRoot "MGModEditor\MGModEditor.csproj"
$MGCClientCsproj = Join-Path $RepoRoot "MGModClient\MGModClient.csproj"
$Config      = "Release"   # 可改为 Debug

# ---------- 工具函数 ----------
function Write-Step($msg) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "  $msg" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
}

function Invoke-DotNet([string[]]$DotNetArgs) {
    Write-Host ""
    Write-Host ">>> dotnet $($DotNetArgs -join ' ')" -ForegroundColor Yellow
    & dotnet @DotNetArgs
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet 命令失败 (exit=$LASTEXITCODE): dotnet $($DotNetArgs -join ' ')"
    }
}

function Remove-IfExists([string]$Path, [string]$Label) {
    if (Test-Path $Path) {
        Write-Host "删除 $Label ..." -ForegroundColor DarkGray
        Remove-Item -Path $Path -Recurse -Force
    }
    else {
        Write-Host "$Label 不存在，跳过删除" -ForegroundColor DarkGray
    }
}

# ---------- 构建任务 ----------
function Build-All {
    Write-Step "一键编译：清空 Build\ 后构建四个项目"
    Remove-IfExists $BuildRoot "Build\"
    Invoke-DotNet @("build", $Slnx, "-c", $Config)
    Write-Host "一键编译完成，产物位于 $BuildRoot" -ForegroundColor Green
}

function Build-MGModServer {
    Write-Step "单独编译 MGModServer"
    Remove-IfExists $MGModOut "mods\MGMod\（含 MGModEditor.exe，后续如需请再单独编译 MGModEditor）"
    Invoke-DotNet @("build", $MGModCsproj, "-c", $Config)
    Write-Host "MGModServer 完成：$MGModOut" -ForegroundColor Green
}

function Build-MGGTMod {
    Write-Step "单独编译 MGGTMod"
    Remove-IfExists $MGGTModOut "mods\MGGTMod\"
    Invoke-DotNet @("build", $MGGTCsproj, "-c", $Config)
    Write-Host "MGGTMod 完成：$MGGTModOut" -ForegroundColor Green
}

function Build-MGEditor {
    Write-Step "单独编译 MGModEditor（不删目录，自动 publish 单文件）"
    Invoke-DotNet @("build", $MGCsproj, "-c", $Config)
    Write-Host "MGModEditor 完成：$MGModOut\MGModEditor.exe（单文件）" -ForegroundColor Green
}

function Build-MGClient {
    Write-Step "单独编译 MGModClient"
    Remove-IfExists $ClientOut "BepInEx\plugins\MGModClient\"
    Invoke-DotNet @("build", $MGCClientCsproj, "-c", $Config)
    Write-Host "MGModClient 完成：$ClientOut" -ForegroundColor Green
}

# ---------- 菜单 ----------
$menuItems = @(
    @{ Id = "all";         Label = "一键编译全部（先清空 Build\ 再构建四项目）" },
    @{ Id = "mgmodserver"; Label = "单独编译 MGModServer（删 mods\MGMod 后编译）" },
    @{ Id = "mggtmod";     Label = "单独编译 MGGTMod（删 mods\MGGTMod 后编译）" },
    @{ Id = "mgeditor";    Label = "单独编译 MGModEditor（不删目录，publish 单文件）" },
    @{ Id = "mgclient";    Label = "单独编译 MGModClient（删 BepInEx 插件目录后编译）" },
    @{ Id = "exit";        Label = "退出" }
)

function Show-Menu([int]$selected) {
    try {
        [Console]::CursorVisible = $false
        [Console]::SetCursorPosition(0, 0)
    } catch { }
    $width = [Console]::WindowWidth
    $pad = New-Object string ('-', [Math]::Max(10, $width - 2))
    Write-Host $pad
    Write-Host "  MGModSeries-CSharp 构建菜单  ($Config)" -ForegroundColor Cyan
    Write-Host "  ↑/↓ 选择 · Enter 确认 · Esc 退出" -ForegroundColor DarkGray
    Write-Host $pad
    Write-Host ""
    for ($i = 0; $i -lt $menuItems.Count; $i++) {
        if ($i -eq $selected) {
            Write-Host ("  > {0}" -f $menuItems[$i].Label) -ForegroundColor Green
        }
        else {
            Write-Host ("    {0}" -f $menuItems[$i].Label) -ForegroundColor Gray
        }
    }
    Write-Host ""
}

function Run-Interactive {
    $selected = 0
    # 检测是否有真实控制台（重定向/无窗口时 ReadKey 会抛 IOException，回退数字选择）
    $hasConsole = $false
    try { $hasConsole = [Console]::IsInputRedirected -eq $false -and $null -ne [Console]::KeyAvailable } catch { $hasConsole = $false }
    if (-not $hasConsole) {
        Write-Host ""
        Write-Host "（未检测到交互式控制台，改用数字选择）" -ForegroundColor DarkYellow
        for ($i = 0; $i -lt $menuItems.Count; $i++) {
            Write-Host ("  [{0}] {1}" -f ($i + 1), $menuItems[$i].Label)
        }
        while ($true) {
            $choice = Read-Host "  请选择 (1-$($menuItems.Count))"
            if ($choice -match '^\d+$' -and [int]$choice -ge 1 -and [int]$choice -le $menuItems.Count) {
                return $menuItems[[int]$choice - 1].Id
            }
        }
    }
    while ($true) {
        Show-Menu $selected
        $key = [Console]::ReadKey($true)
        switch ($key.Key) {
            ([ConsoleKey]::UpArrow)   { $selected = ($selected - 1 + $menuItems.Count) % $menuItems.Count; break }
            ([ConsoleKey]::DownArrow) { $selected = ($selected + 1) % $menuItems.Count; break }
            ([ConsoleKey]::Enter)     { break }
            ([ConsoleKey]::Escape)    { $selected = $menuItems.Count - 1; break }
        }
        if ($key.Key -eq [ConsoleKey]::Enter -or $key.Key -eq [ConsoleKey]::Escape) { break }
    }
    try { [Console]::CursorVisible = $true } catch { }
    return $menuItems[$selected].Id
}

function Invoke-Task([string]$id) {
    switch ($id) {
        "all"         { Build-All }
        "mgmodserver" { Build-MGModServer }
        "mggtmod"     { Build-MGGTMod }
        "mgeditor"    { Build-MGEditor }
        "mgclient"    { Build-MGClient }
        "exit"        { Write-Host "退出。" ; exit 0 }
        default       { throw "未知任务: $id" }
    }
}

# ---------- 入口 ----------
if (Get-Command dotnet -ErrorAction SilentlyContinue) {
    if ($Run) {
        Invoke-Task $Run
    }
    else {
        $taskId = Run-Interactive
        if ($taskId -eq "exit") { exit 0 }
        Invoke-Task $taskId
    }
}
else {
    Write-Host "未找到 dotnet CLI。请先安装 .NET SDK 并加入 PATH。" -ForegroundColor Red
    exit 1
}

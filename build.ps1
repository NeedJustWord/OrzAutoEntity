# 设置错误处理：遇到非终止错误时停止执行
$ErrorActionPreference = "Stop"

# 清空日志文件
$logFile = ".\Build.log"
if (Test-Path $logFile) {
    Clear-Content -Path $logFile
}

# 定义输入日志函数
function Write-Log {
    param(
        [string]$Message,
        [string]$Color = "White"
    )

    if ($Message -match '\[信息\]') {
        $Color = "Cyan"
    }
    elseif ($Message -match '\[警告\]') {
        $Color = "Yellow"
    }
    elseif ($Message -match '\[(成功|完成)\]') {
        $Color = "Green"
    }
    elseif ($Message -match '\[(错误|失败)\]') {
        $Color = "Red"
    }

    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $log = "$timestamp - $Message"
    Write-Host $log -ForegroundColor $Color
    $log | Out-File -FilePath $logFile -Encoding utf8 -Append
}


Write-Log "[信息] 开始执行脚本..."


# 读取版本号
Write-Log "[信息] 正在读取版本号..."
$versionFile = ".\Src\OrzAutoEntity\source.extension.vsixmanifest"
foreach ($line in Get-Content $versionFile) {
    if ($line -match '<Identity.*Version="([^"]+)"') {
        $version = $matches[1]
        break
    }
}

if (-not $version) {
    Write-Log "[警告] 未能读取到版本号，改成手动输入版本号"
    $version = Read-Host -Prompt "请输入版本号"
    Write-Log "[成功] 输入的版本号: $version"
}
else {
    Write-Log "[成功] 读取的版本号: $version"
}


# 查找 MSBuild.exe 路径
Write-Log "[信息] 正在查找 Visual Studio MSBuild..."
$vsWherePath = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
if (-not (Test-Path $vsWherePath)) {
    Write-Log "[错误] 未找到 vswhere.exe，请确认已安装 Visual Studio。"
    exit 1
}

# 执行 vswhere 获取最新 MSBuild 路径
$msbuildExe = & $vsWherePath -latest -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe | Select-Object -First 1
if ([string]::IsNullOrEmpty($msbuildExe)) {
    Write-Log "[错误] 未找到 Visual Studio 或 MSBuild 组件。"
    exit 1
}
Write-Log "[信息] 找到 MSBuild: $msbuildExe"


# 执行编译
$projectPath = ".\Src\OrzAutoEntity\OrzAutoEntity.csproj"
$configuration = "Release"
Write-Log "[信息] 开始编译项目..."
try {
    # 注意：MSBuild 返回 0 表示成功，非 0 表示失败
    # 使用 & 调用操作符执行外部命令
    $msbuildLogFile = ".\MSBuild.log"
    & $msbuildExe $projectPath /p:Configuration=$configuration /t:Rebuild /nologo /v:minimal /fl /flp:"logfile=$msbuildLogFile;verbosity=minimal"

    if ($LASTEXITCODE -ne 0) {
        throw "MSBuild 退出代码: $LASTEXITCODE"
    }

    Write-Log "[成功] 编译完成。"
}
catch {
    Write-Log "[失败] 编译出错: $_"
    exit 1
}


# 压缩 VSIX 文件
$vsixSource = ".\Src\OrzAutoEntity\bin\$configuration\OrzAutoEntity.vsix"
$zipDestination = ".\Vsixs\OrzAutoEntity v$version.zip"

# 确保目标目录存在
$zipDir = Split-Path $zipDestination -Parent
if (-not (Test-Path $zipDir)) {
    New-Item -ItemType Directory -Force -Path $zipDir | Out-Null
}

if (Test-Path $vsixSource) {
    $vsixCopy = ".\Vsixs\OrzAutoEntity v$version.vsix"
    Write-Log "[信息] 正在复制并重命名 VSIX 文件..."
    Copy-Item -Path $vsixSource -Destination $vsixCopy -Force
    Write-Log "[成功] 复制并重命名完成: $vsixCopy"

    Write-Log "[信息] 正在压缩 VSIX 文件..."
    Compress-Archive -Path $vsixCopy -DestinationPath $zipDestination -Force
    Write-Log "[成功] 使用 PowerShell 压缩完成: $zipDestination"

    Write-Log "[信息] 正在删除复制文件..."
    Remove-Item -Path $vsixCopy
    Write-Log "[成功] 删除复制文件完成"
}
else {
    Write-Log "[警告] 未找到生成的 VSIX 文件: $vsixSource"
}

Write-Log "[完成] 所有任务结束。"

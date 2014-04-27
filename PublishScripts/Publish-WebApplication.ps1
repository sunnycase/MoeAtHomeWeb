#Requires -Version 3.0

<#
.SYNOPSIS
为 Visual Studio Web 项目创建并部署 Windows Azure 网站、虚拟机、SQL 数据库和存储帐户。

.DESCRIPTION
Publish-WebApplication.ps1 脚本在 Visual Studio Web 项目中创建您指定的 Windows Azure 资源，并(可选)为您部署它们。它可以创建 Windows Azure 网站、虚拟机、SQL 数据库和存储帐户。

To manage the entire application lifecycle of your web application in this script, implement the placeholder functions New-WebDeployPackage and Test-WebApplication.

如果您通过有效的 Web Deploy 包 ZIP 文件指定 WebDeployPackage 参数，Publish-WebApplication.ps1 还会部署您的网页或它创建的虚拟机。

此脚本要求装有 Windows PowerShell 3.0 或更高版本以及 Windows Azure PowerShell 0.7.4 或更高版本。有关如何安装 Windows Azure PowerShell 及其 Azure 模块的信息，请参见 http://go.microsoft.com/fwlink/?LinkID=350552。要查找您的 Azure 模块的版本，请键入: (Get-Module -Name Azure -ListAvailable).version 要查找 Windows PowerShell 的版本，请键入: $PSVersionTable.PSVersion

在运行此脚本前，请运行 Add-AzureAccount cmdlet 以向 Windows PowerShell 提供您的 Windows Azure 帐户的凭据。此外，如果您创建 SQL 数据库，则需要已经有 Windows Azure SQL 数据库服务器。若要创建 SQL 数据库，请使用 Azure 模块中的 New-AzureSqlDatabaseServer cmdlet。

Also, if you have never run a script, use the Set-ExecutionPolicy cmdlet to an execution policy that allows you to run scripts. To run this cmdlet, start Windows PowerShell with the 'Run as administrator' option.

此 Publish-WebApplication.ps1 脚本使用 Visual Studio 在您创建自己的 Web 项目时生成的 JSON 配置文件。您可以在 Visual Studio 解决方案中的 PublishScripts 文件夹中找到该 JSON 文件。

您可以删除或编辑 JSON 配置文件中的“databases”对象。不要删除“website”或“cloudservice”对象或者删除其特性。但是，您可以删除整个“databases”对象或删除表示数据库的特性。若要创建 SQL 数据库但不部署它，请删除“connectionStringName”特性或其值。

它还使用 AzureWebAppPublishModule.psm1 Windows PowerShell 脚本模块中的函数在您的 Windows Azure 订阅中创建资源。您可以在您的 Visual Studio 解决方案中的 PublishScripts 文件夹中找到此脚本模块的副本。

您可以按原样使用 Publish-WebApplication.ps1 脚本，也可对它进行编辑以满足您的需求。您还可以独立于此脚本使用 AzureWebAppPublishModule.psm1 模块中的函数，并对这些函数进行编辑。例如，您可以使用 Invoke-AzureWebRequest 函数在 Windows Azure Web 服务中调用任意 REST API。

获得了可创建所需 Windows Azure 资源的脚本后，您可以在 Windows Azure 中重复使用该脚本来创建环境和资源。

有关此脚本的更新，请访问 http://go.microsoft.com/fwlink/?LinkId=391217。
若要添加支持以生成您的 Web 应用程序项目，请参考 MSBuild 文档，网址如下: http://go.microsoft.com/fwlink/?LinkId=391339 
若要添加支持以在您的 Web 应用程序项目中运行单元测试，请参考 VSTest.Console 文档，网址如下: http://go.microsoft.com/fwlink/?LinkId=391340 

查看 WebDeploy 许可证条款: http://go.microsoft.com/fwlink/?LinkID=389744 

.PARAMETER Configuration
指定 Visual Studio 生成的 JSON 配置文件的路径及文件名。此参数是必需的。您可以在您的 Visual Studio 解决方案的 PublishScripts 文件夹中找到该文件。用户可以通过修改特性值和删除可选的 SQL 数据库对象来自定义 JSON 配置文件。为使脚本正确运行，可以删除网站和虚拟机配置文件中的 SQL 数据库对象。网站和云服务对象及特性则不能删除。如果用户不希望在发布期间创建 SQL 数据库或将 SQL 数据库应用于连接字符串，则必须确保 SQL 数据库对象中的“connectionStringName”特性为空，或者删除整个 SQL 数据库对象。

注意: 此脚本仅支持虚拟机的 Windows 虚拟硬盘(VHD)文件。若要使用 Linux VHD，请更改此脚本，让它使用 Linux 参数(如 New-AzureQuickVM 或 New-WAPackVM)调用 cmdlet。

.PARAMETER SubscriptionName
指定您的 Windows Azure 帐户中包含的订阅的名称。此参数是可选项。默认值为当前订阅 (Get-AzureSubscription -Current)。如果您指定非当前订阅，则该脚本会临时将指定的订阅更改为当前订阅，不过会在该脚本运行完毕前还原当前订阅状态。如果该脚本在运行完毕前因出错而退出，则指定的订阅可能仍设置为当前订阅。

.PARAMETER WebDeployPackage
指定 Visual Studio 生成的 Web 部署包 ZIP 文件的路径及文件名。此参数是可选的。

如果您指定有效的 Web 部署包，则此脚本会使用 MsDeploy.exe 和该 Web 部署包来部署网站。

若要创建 Web 部署包 ZIP 文件，请参见 http://go.microsoft.com/fwlink/?LinkId=391353 上的“如何：在 Visual Studio 中创建 Web 部署包”。

有关 MSDeploy.exe 的信息，请参见 http://go.microsoft.com/fwlink/?LinkId=391354 上的“Web Deploy 命令行参考” 

.PARAMETER AllowUntrusted
允许与虚拟机上的 Web Deploy 终结点建立不受信任的 SSL 连接。此参数在调用 MSDeploy.exe 的过程中使用。此参数是可选的。默认值为 False。此参数仅当您在有效的 ZIP 文件值中包含 WebDeployPackage 参数时才有效。有关 MSDeploy.exe 的信息，请参见 http://go.microsoft.com/fwlink/?LinkId=391354 上的“Web Deploy 命令行参考” 

.PARAMETER VMPassword
为此脚本创建的 Windows Azure 虚拟机的管理员指定用户名和密码。此参数接受包含 Name 和 Password 键的哈希表作为值，例如:
@{Name = "admin"; Password = "pa$$word"}

此参数是可选的。如果您省略此参数，默认值将为 JSON 配置文件中的虚拟机用户名和密码。

此参数仅在 JSON 配置文件用于包含虚拟机的云服务时才有效。

.PARAMETER DatabaseServerPassword
Sets the password for a Windows Azure SQL database server. This parameter takes an array of hash tables with Name (SQL database server name) and Password keys. Enter one hash table for each database server that your SQL databases use.

此参数是可选的。默认值是 Visual Studio 生成的 JSON 配置文件中的 SQL 数据库服务器密码。

此值在 JSON 配置文件包含 databases 和 serverName 特性且哈希表中的 Name 键与 serverName 值匹配时有效。

.INPUTS
无。您不能通过管道向此脚本传送参数值。

.OUTPUTS
无。此脚本不返回任何对象。若要了解脚本状态，请使用 Verbose 参数。

.EXAMPLE
PS C:\> C:\Scripts\Publish-WebApplication.ps1 -Configuration C:\Documents\Azure\WebProject-WAWS-dev.json

.EXAMPLE
PS C:\> C:\Scripts\Publish-WebApplication.ps1 `
-Configuration C:\Documents\Azure\ADWebApp-VM-prod.json `
-Subscription Contoso '
-WebDeployPackage C:\Documents\Azure\ADWebApp.zip `
-AllowUntrusted `
-DatabaseServerPassword @{Name='dbServerName';Password='adminPassword'} `
-Verbose

.EXAMPLE
PS C:\> $admin = @{name="admin";password="Test123"}
PS C:\> C:\Scripts\Publish-WebApplication.ps1 `
-Configuration C:\Documents\Azure\ADVM-VM-test.json `
-SubscriptionName Contoso `
-WebDeployPackage C:\Documents\Azure\ADVM.zip `
-VMPaassword = @{name = "vmAdmin"; password = "pa$$word"} `
-DatabaseServerPassword = @{Name='server1';Password='adminPassword1'}, @{Name='server2';Password='adminPassword2'} `
-Verbose

.LINK
New-AzureVM

.LINK
New-AzureStorageAccount

.LINK
New-AzureWebsite

.LINK
Add-AzureEndpoint
#>
[CmdletBinding(DefaultParameterSetName = 'None', HelpUri = 'http://go.microsoft.com/fwlink/?LinkID=391696')]
param
(
    [Parameter(Mandatory = $true)]
    [ValidateScript({Test-Path $_ -PathType Leaf})]
    [String]
    $Configuration,

    [Parameter(Mandatory = $false)]
    [String]
    $SubscriptionName,

    [Parameter(Mandatory = $false)]
    [ValidateScript({Test-Path $_ -PathType Leaf})]
    [String]
    $WebDeployPackage,

    [Parameter(Mandatory = $false)]
    [Switch]
    $AllowUntrusted,

    [Parameter(Mandatory = $false, ParameterSetName = 'VM')]
    [ValidateScript( { $_.Contains('Name') -and $_.Contains('Password') } )]
    [Hashtable]
    $VMPassword,

    [Parameter(Mandatory = $false, ParameterSetName = 'WebSite')]
    [ValidateScript({ !($_ | Where-Object { !$_.Contains('Name') -or !$_.Contains('Password')}) })]
    [Hashtable[]]
    $DatabaseServerPassword,

    [Parameter(Mandatory = $false)]
    [Switch]
    $SendHostMessagesToOutput = $false
)


function New-WebDeployPackage
{
    #编写一个函数以生成和打包 Web 应用程序

    #若要生成 Web 应用程序，请使用 MsBuild.exe。若要寻求帮助，请参见 MSBuild 命令行参考，网址如下: http://go.microsoft.com/fwlink/?LinkId=391339
}

function Test-WebApplication
{
    #编辑此函数以对 Web 应用程序运行单元测试

    #若要编写一个函数以对 Web 应用程序运行单元测试，请使用 VSTest.Console.exe。若要寻求帮助，请参见 http://go.microsoft.com/fwlink/?LinkId=391340 上的 VSTest.Console 命令行参考
}

function New-AzureWebApplicationEnvironment
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [Object]
        $Config,

        [Parameter (Mandatory = $false)]
        [AllowNull()]
        [Hashtable]
        $VMPassword,

        [Parameter (Mandatory = $false)]
        [AllowNull()]
        [Hashtable[]]
        $DatabaseServerPassword
    )
   
    $VMInfo = $null

    # 如果 JSON 文件包含“webSite”元素
    if ($Config.IsAzureWebSite)
    {
        Add-AzureWebsite -Name $Config.name -Location $Config.location | Out-String | Write-HostWithTime
        # 创建 SQL 数据库。此连接字符串用于部署。
    }
    else
    {
        $VMInfo = New-AzureVMEnvironment `
            -CloudServiceConfiguration $Config.cloudService `
            -VMPassword $VMPassword
    } 

    $connectionString = New-Object -TypeName Hashtable
    
    if ($Config.Contains('databases'))
    {
        @($Config.databases) |
            Where-Object {$_.connectionStringName -ne ''} |
            Add-AzureSQLDatabases -DatabaseServerPassword $DatabaseServerPassword -CreateDatabase:$Config.IsAzureWebSite |
            ForEach-Object { $connectionString.Add($_.Name, $_.ConnectionString) }           
    }
    
    return @{ConnectionString = $connectionString; VMInfo = $VMInfo}   
}

function Publish-AzureWebApplication
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [Object]
        $Config,

        [Parameter(Mandatory = $false)]
        [AllowNull()]
        [Hashtable]
        $ConnectionString,

        [Parameter(Mandatory = $true)]
        [ValidateScript({Test-Path $_ -PathType Leaf})]
        [String]
        $WebDeployPackage,
        
        [Parameter(Mandatory = $false)]
        [AllowNull()]
        [Hashtable]
        $VMInfo           
    )

    if ($Config.IsAzureWebSite)
    {
        if ($ConnectionString -and $ConnectionString.Count -gt 0)
        {
            Publish-AzureWebsiteProject `
                -Name $Config.name `
                -Package $WebDeployPackage `
                -ConnectionString $ConnectionString
        }
        else
        {
            Publish-AzureWebsiteProject `
                -Name $Config.name `
                -Package $WebDeployPackage
        }
    }
    else
    {
        $waitingTime = $VMWebDeployWaitTime

        $result = $null
        $attempts = 0
        $allAttempts = 60
        do 
        {
            $result = Publish-WebPackageToVM `
                -VMDnsName $VMInfo.VMUrl `
                -IisWebApplicationName $Config.webDeployParameters.IisWebApplicationName `
                -WebDeployPackage $WebDeployPackage `
                -UserName $VMInfo.UserName `
                -UserPassword $VMInfo.Password `
                -AllowUntrusted:$AllowUntrusted `
                -ConnectionString $ConnectionString
             
            if ($result)
            {
                Write-VerboseWithTime ($scriptName + ' 发布到虚拟机成功。')
            }
            elseif ($VMInfo.IsNewCreatedVM -and !$Config.cloudService.virtualMachine.enableWebDeployExtension)
            {
                Write-VerboseWithTime ($scriptName + ' 您需要将“enableWebDeployExtension”设置为 $true。')
            }
            elseif (!$VMInfo.IsNewCreatedVM)
            {
                Write-VerboseWithTime ($scriptName + ' 现有虚拟机不支持 Web Deploy。')
            }
            else
            {
                Write-VerboseWithTime ($scriptName + "发布到虚拟机失败。请进行第 $($attempts + 1) 次尝试(共能尝试 $allAttempts 次)。")
                Write-VerboseWithTime ($scriptName + " 发布到虚拟机的操作将在 $waitingTime 秒后开始。")
                
                Start-Sleep -Seconds $waitingTime
            }
             
             $attempts++
        
             #尝试再次仅为已安装 Web Deploy 的新建虚拟机进行发布。 
        } While( !$result -and $VMInfo.IsNewCreatedVM -and $attempts -lt $allAttempts -and $Config.cloudService.virtualMachine.enableWebDeployExtension)
        
        if (!$result)
        {                    
            Write-Warning 'Publishing to the virtual machine failed. This can be caused by an untrusted or invalid certificate.  You can specify �AllowUntrusted to accept untrusted or invalid certificates.'
            throw ($scriptName + ' 发布到虚拟机失败。')
        }
    }
}


# 脚本主例程
Set-StrictMode -Version 3

# 导入 AzureWebAppPublishModule.psm1 模块的当前版本
Remove-Module AzureWebAppPublishModule -ErrorAction SilentlyContinue
$scriptDirectory = Split-Path -Parent $PSCmdlet.MyInvocation.MyCommand.Definition
Import-Module ($scriptDirectory + '\AzureWebAppPublishModule.psm1') -Scope Local -Verbose:$false

New-Variable -Name VMWebDeployWaitTime -Value 30 -Option Constant -Scope Script 
New-Variable -Name AzureWebAppPublishOutput -Value @() -Scope Global -Force
New-Variable -Name SendHostMessagesToOutput -Value $SendHostMessagesToOutput -Scope Global -Force

try
{
    $originalErrorActionPreference = $Global:ErrorActionPreference
    $originalVerbosePreference = $Global:VerbosePreference
    
    if ($PSBoundParameters['Verbose'])
    {
        $Global:VerbosePreference = 'Continue'
    }
    
    $scriptName = $MyInvocation.MyCommand.Name + ':'
    
    Write-VerboseWithTime ($scriptName + ' 开始')
    
    $Global:ErrorActionPreference = 'Stop'
    Write-VerboseWithTime ('{0} $ErrorActionPreference 设置为 {1}' -f $scriptName, $ErrorActionPreference)
    
    Write-Debug ('{0}: $PSCmdlet.ParameterSetName = {1}' -f $scriptName, $PSCmdlet.ParameterSetName)

    # 保存当前订阅。它稍后将在脚本中还原为“当前”状态
    Backup-Subscription -UserSpecifiedSubscription $SubscriptionName
    
    # 验证您是否装有 Azure 模块 0.7.4 或更高版本。
    if (-not (Test-AzureModule))
    {
         throw '您安装的 Windows Azure PowerShell 版本已过时。若要安装最新版本，请访问 http://go.microsoft.com/fwlink/?LinkID=320552。'
    }
    
    if ($SubscriptionName)
    {

        # 如果您提供了订阅名称，请验证您的帐户中是否存在相应的订阅。
        if (!(Get-AzureSubscription -SubscriptionName $SubscriptionName))
        {
            throw ("{0}: 找不到订阅名称 $SubscriptionName" -f $scriptName)

        }

        # 将指定的订阅设为当前订阅。
        Select-AzureSubscription -SubscriptionName $SubscriptionName | Out-Null

        Write-VerboseWithTime ('{0}: 订阅设置为 {1}' -f $scriptName, $SubscriptionName)
    }

    $Config = Read-ConfigFile $Configuration -HasWebDeployPackage:([Bool]$WebDeployPackage)

    #生成并打包 Web 应用程序
    #New-WebDeployPackage

    #对 Web 应用程序运行单元测试
    #Test-WebApplication

    #创建 JSON 配置文件中描述的 Azure 环境
    $newEnvironmentResult = New-AzureWebApplicationEnvironment -Config $Config -DatabaseServerPassword $DatabaseServerPassword -VMPassword $VMPassword

    #如果用户指定了 $WebDeployPackage，则部署 Web 应用程序包 
    if($WebDeployPackage)
    {
        Publish-AzureWebApplication `
            -Config $Config `
            -ConnectionString $newEnvironmentResult.ConnectionString `
            -WebDeployPackage $WebDeployPackage `
            -VMInfo $newEnvironmentResult.VMInfo
    }
}
finally
{
    $Global:ErrorActionPreference = $originalErrorActionPreference
    $Global:VerbosePreference = $originalVerbosePreference

    # 将原来处于当前状态的订阅还原为“当前”状态
    Restore-Subscription

    Write-Output $Global:AzureWebAppPublishOutput    
    $Global:AzureWebAppPublishOutput = @()
}

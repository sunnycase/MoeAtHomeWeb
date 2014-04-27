# AzureWebAppPublishModule.psm1 是 Windows PowerShell 脚本模块。此模块将导出为 Web 应用程序自动执行生命周期管理的 Windows PowerShell 函数。您可以按原样使用这些函数，或根据您的应用程序和发布环境自定义这些函数。





Set-StrictMode -Version 3

# 用来保存原始订阅的变量。
$Script:originalCurrentSubscription = $null

# 用来保存存储帐户的变量。
$Script:originalCurrentStorageAccount = $null

# 用来保存用户指定订阅的存储帐户的变量。
$Script:originalStorageAccountOfUserSpecifiedSubscription = $null

# 用来保存订阅名称的变量。
$Script:userSpecifiedSubscription = $null

# Web Deploy 端口号
New-Variable -Name WebDeployPort -Value 8172 -Option Constant

<#
.SYNOPSIS
在消息前添加日期和时间作为前缀。

.DESCRIPTION
在消息前添加日期和时间作为前缀。此函数是针对写入到 Error 和 Verbose 流的消息而设计的。

.PARAMETER  Message
指定不带日期的消息。

.INPUTS
System.String

.OUTPUTS
System.String

.EXAMPLE
PS C:\> Format-DevTestMessageWithTime -Message "将 $filename 文件添加到目录"
2/5/2014 1:03:08 PM - 将 $filename 文件添加到目录

.LINK
Write-VerboseWithTime

.LINK
Write-ErrorWithTime
#>
function Format-DevTestMessageWithTime
{
    [CmdletBinding()]
    param
    (
        [Parameter(Position=0, Mandatory = $true, ValueFromPipeline = $true)]
        [String]
        $Message
    )

    return ((Get-Date -Format G)  + ' - ' + $Message)
}


<#

.SYNOPSIS
写入一条添加了当前时间作为前缀的错误消息。

.DESCRIPTION
写入一条添加了当前时间作为前缀的错误消息。此函数会先调用 Format-DevTestMessageWithTime 函数来添加时间作为前缀，然后再将消息写入到 Error 流中。

.PARAMETER  Message
指定在错误消息调用过程中使用的消息。您可以通过管道将消息字符串传送给此函数。

.INPUTS
System.String

.OUTPUTS
无。此函数向 Error 流中写入数据。

.EXAMPLE
PS C:> Write-ErrorWithTime -Message "Failed. Cannot find the file."

Write-Error: 2/6/2014 8:37:29 AM - Failed. Cannot find the file.
 + CategoryInfo     : NotSpecified: (:) [Write-Error], WriteErrorException
 + FullyQualifiedErrorId : Microsoft.PowerShell.Commands.WriteErrorException

.LINK
Write-Error

#>
function Write-ErrorWithTime
{
    [CmdletBinding()]
    param
    (
        [Parameter(Position=0, Mandatory = $true, ValueFromPipeline = $true)]
        [String]
        $Message
    )

    $Message | Format-DevTestMessageWithTime | Write-Error
}


<#
.SYNOPSIS
写入一条添加了当前时间作为前缀的详细消息。

.DESCRIPTION
写入一条添加了当前时间作为前缀的详细消息。由于它会调用 Write-Verbose，因此该消息仅在脚本使用 Verbose 参数运行时或者在 VerbosePreference 首选项设置为 Continue 时才会显示。

.PARAMETER  Message
指定在详细消息调用过程中使用的消息。您可以通过管道将消息字符串传送给此函数。

.INPUTS
System.String

.OUTPUTS
无。此函数向 Verbose 流中写入数据。

.EXAMPLE
PS C:> Write-VerboseWithTime -Message "The operation succeeded."
PS C:>
PS C:\> Write-VerboseWithTime -Message "The operation succeeded." -Verbose
VERBOSE: 1/27/2014 11:02:37 AM - The operation succeeded.

.EXAMPLE
PS C:\ps-test> "The operation succeeded." | Write-VerboseWithTime -Verbose
VERBOSE: 1/27/2014 11:01:38 AM - The operation succeeded.

.LINK
Write-Verbose
#>
function Write-VerboseWithTime
{
    [CmdletBinding()]
    param
    (
        [Parameter(Position=0, Mandatory = $true, ValueFromPipeline = $true)]
        [String]
        $Message
    )

    $Message | Format-DevTestMessageWithTime | Write-Verbose
}


<#
.SYNOPSIS
写入一条添加了当前时间作为前缀的宿主消息。

.DESCRIPTION
此函数向宿主程序(Write-Host)写入一条添加了当前时间作为前缀的消息。写入到宿主程序所产生的影响不尽相同。大多数用作 Windows PowerShell 宿主的程序都会将这些消息写入到标准输出。

.PARAMETER  Message
指定不带日期的基础消息。您可以通过管道将消息字符串传送给此函数。

.INPUTS
System.String

.OUTPUTS
无。此函数将消息写入到宿主程序。

.EXAMPLE
PS C:> Write-HostWithTime -Message "操作已成功。"
1/27/2014 11:02:37 AM - 操作已成功。

.LINK
Write-Host
#>
function Write-HostWithTime
{
    [CmdletBinding()]
    param
    (
        [Parameter(Position=0, Mandatory = $true, ValueFromPipeline = $true)]
        [String]
        $Message
    )
    
    if ((Get-Variable SendHostMessagesToOutput -Scope Global -ErrorAction SilentlyContinue) -and $Global:SendHostMessagesToOutput)
    {
        if (!(Get-Variable -Scope Global AzureWebAppPublishOutput -ErrorAction SilentlyContinue) -or !$Global:AzureWebAppPublishOutput)
        {
            New-Variable -Name AzureWebAppPublishOutput -Value @() -Scope Global -Force
        }

        $Global:AzureWebAppPublishOutput += $Message | Format-DevTestMessageWithTime
    }
    else 
    {
        $Message | Format-DevTestMessageWithTime | Write-Host
    }
}


<#
.SYNOPSIS
如果属性或方法为对象成员，则返回 $true。否则返回 $false。

.DESCRIPTION
如果属性或方法为对象成员，则返回 $true。对于类的静态方法以及视图(如 PSBase 和 PSObject)，此函数返回 $false。

.PARAMETER  Object
指定在测试过程中使用的对象。请输入一个包含对象或者包含可返回对象的表达式的变量。您不能指定类型(例如 [DateTime])，也不能通过管道向此函数传送对象。

.PARAMETER  Member
指定在测试过程中使用的属性或方法的名称。指定方法时，请省略方法名后面的圆括号。

.INPUTS
无。此函数不从管道中获取任何输入。

.OUTPUTS
System.Boolean

.EXAMPLE
PS C:\> Test-Member -Object (Get-Date) -Member DayOfWeek
True

.EXAMPLE
PS C:\> $date = Get-Date
PS C:\> Test-Member -Object $date -Member AddDays
True

.EXAMPLE
PS C:\> [DateTime]::IsLeapYear((Get-Date).Year)
True
PS C:\> Test-Member -Object (Get-Date) -Member IsLeapYear
False

.LINK
Get-Member
#>
function Test-Member
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [Object]
        $Object,

        [Parameter(Mandatory = $true)]
        [String]
        $Member
    )

    return $null -ne ($Object | Get-Member -Name $Member)
}


<#
.SYNOPSIS
如果 Azure 模块的版本为 0.7.4 或更高版本，则返回 $true。否则返回 $false。

.DESCRIPTION
如果 Azure 模块的版本为 0.7.4 或更高版本，则 Test-AzureModuleVersion 会返回 $true。如果该模块未安装或为更低的版本，则此函数返回 $false。此函数无参数。

.INPUTS
无

.OUTPUTS
System.Boolean

.EXAMPLE
PS C:\> Get-Module Azure -ListAvailable
PS C:\> #No module
PS C:\> Test-AzureModuleVersion
False

.EXAMPLE
PS C:\> (Get-Module Azure -ListAvailable).Version

Major  Minor  Build  Revision
-----  -----  -----  --------
0      7      4      -1

PS C:\> Test-AzureModuleVersion
True

.LINK
Get-Module

.LINK
PSModuleInfo object (http://msdn.microsoft.com/en-us/library/system.management.automation.psmoduleinfo(v=vs.85).aspx)
#>
function Test-AzureModuleVersion
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [ValidateNotNull()]
        [System.Version]
        $Version
    )

    return ($Version.Major -gt 0) -or ($Version.Minor -gt 7) -or ($Version.Minor -eq 7 -and $Version.Build -ge 4)
}


<#
.SYNOPSIS
如果所安装的 Azure 模块版本为 0.7.4 或更高版本，则返回 $true。

.DESCRIPTION
如果所安装的 Azure 模块版本为 0.7.4 或更高版本，则 Test-AzureModule 返回 $true。如果该模块未安装或为更低的版本，则返回 $false。此函数无参数。

.INPUTS
无

.OUTPUTS
System.Boolean

.EXAMPLE
PS C:\> Get-Module Azure -ListAvailable
PS C:\> #No module
PS C:\> Test-AzureModule
False

.EXAMPLE
PS C:\> (Get-Module Azure -ListAvailable).Version

Major  Minor  Build  Revision
-----  -----  -----  --------
    0      7      4      -1

PS C:\> Test-AzureModule
True

.LINK
Get-Module

.LINK
PSModuleInfo object (http://msdn.microsoft.com/en-us/library/system.management.automation.psmoduleinfo(v=vs.85).aspx)
#>
function Test-AzureModule
{
    [CmdletBinding()]

    $module = Get-Module -Name Azure

    if (!$module)
    {
        $module = Get-Module -Name Azure -ListAvailable

        if (!$module -or !(Test-AzureModuleVersion $module.Version))
        {
            return $false;
        }
        else
        {
            $ErrorActionPreference = 'Continue'
            Import-Module -Name Azure -Global -Verbose:$false
            $ErrorActionPreference = 'Stop'

            return $true
        }
    }
    else
    {
        return (Test-AzureModuleVersion $module.Version)
    }
}


<#
.SYNOPSIS
将当前 Windows Azure 订阅保存在脚本作用域内的 $Script:originalSubscription 变量中。

.DESCRIPTION
Backup-Subscription 函数将当前 Windows Azure 订阅(Get-AzureSubscription -Current)及其存储帐户以及由脚本($UserSpecifiedSubscription)更改的订阅及其存储帐户保存在脚本作用域内。通过保存这些值，在当前状态发生更改的情况下，可以使用函数(如 Restore-Subscription)来将原来的当前状态的订阅和存储帐户还原为当前状态。

.PARAMETER UserSpecifiedSubscription
指定将在其中创建和发布新资源的订阅的名称。该函数会将该订阅及其存储帐户的名称保存在脚本作用域中。此参数是必需的。

.INPUTS
无

.OUTPUTS
无

.EXAMPLE
PS C:\> Backup-Subscription -UserSpecifiedSubscription Contoso
PS C:\>

.EXAMPLE
PS C:\> Backup-Subscription -UserSpecifiedSubscription Contoso -Verbose
VERBOSE: Backup-Subscription: Start
VERBOSE: Backup-Subscription: Original subscription is Windows Azure MSDN - Visual Studio Ultimate
VERBOSE: Backup-Subscription: End
#>
function Backup-Subscription
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]
        $UserSpecifiedSubscription
    )

    Write-VerboseWithTime 'Backup-Subscription: 开始'

    $Script:originalCurrentSubscription = Get-AzureSubscription -Current -ErrorAction SilentlyContinue
    if ($Script:originalCurrentSubscription)
    {
        Write-VerboseWithTime ('Backup-Subscription: 原始订阅为 ' + $Script:originalCurrentSubscription.SubscriptionName)
        $Script:originalCurrentStorageAccount = $Script:originalCurrentSubscription.CurrentStorageAccountName
    }
    
    $Script:userSpecifiedSubscription = $UserSpecifiedSubscription
    if ($Script:userSpecifiedSubscription)
    {        
        $userSubscription = Get-AzureSubscription -SubscriptionName $Script:userSpecifiedSubscription -ErrorAction SilentlyContinue
        if ($userSubscription)
        {
            $Script:originalStorageAccountOfUserSpecifiedSubscription = $userSubscription.CurrentStorageAccountName
        }        
    }

    Write-VerboseWithTime 'Backup-Subscription: 结束'
}


<#
.SYNOPSIS
将保存在脚本作用域内的 $Script:originalSubscription 变量中的 Windows Azure 订阅还原为“当前”状态。

.DESCRIPTION
Restore-Subscription 函数将 $Script:originalSubscription 变量中保存的订阅设为当前订阅(再次)。如果原始订阅有对应的存储帐户，此函数会将该存储帐户设为当前订阅的当前帐户。此函数仅在环境中存在非 Null $SubscriptionName 变量时才还原订阅。否则，此函数将退出。如果 $SubscriptionName 内已填入值，但 $Script:originalSubscription 为 $null，Restore-Subscription 将使用 Select-AzureSubscription cmdlet 清除 Windows Azure PowerShell 内订阅的当前设置和默认设置。此函数无参数，不接受任何输入，也不返回任何值(void)。您可以使用 -Verbose 来向 Verbose 流写入消息。

.INPUTS
无

.OUTPUTS
无

.EXAMPLE
PS C:\> Restore-Subscription
PS C:\>

.EXAMPLE
PS C:\> Restore-Subscription -Verbose
VERBOSE: Restore-Subscription: Start
VERBOSE: Restore-Subscription: End
#>
function Restore-Subscription
{
    [CmdletBinding()]
    param()

    Write-VerboseWithTime 'Restore-Subscription: 开始'

    if ($Script:originalCurrentSubscription)
    {
        if ($Script:originalCurrentStorageAccount)
        {
            Set-AzureSubscription `
                -SubscriptionName $Script:originalCurrentSubscription.SubscriptionName `
                -CurrentStorageAccountName $Script:originalCurrentStorageAccount
        }

        Select-AzureSubscription -SubscriptionName $Script:originalCurrentSubscription.SubscriptionName
    }
    else 
    {
        Select-AzureSubscription -NoCurrent
        Select-AzureSubscription -NoDefault
    }
    
    if ($Script:userSpecifiedSubscription -and $Script:originalStorageAccountOfUserSpecifiedSubscription)
    {
        Set-AzureSubscription `
            -SubscriptionName $Script:userSpecifiedSubscription `
            -CurrentStorageAccountName $Script:originalStorageAccountOfUserSpecifiedSubscription
    }

    Write-VerboseWithTime 'Restore-Subscription: 结束'
}

<#
.SYNOPSIS
在当前订阅中查找名为 "devtest*" 的 Windows Azure 存储帐户。

.DESCRIPTION
Get-AzureVMStorage 函数返回指定位置或关联组内第一个采用 "devtest*" (不区分大小写)名称模式的存储帐户的名称。如果 "devtest*" 存储帐户与位置或关联组不匹配，此函数将会忽略该存储帐户。您必须指定位置或关联组。

.PARAMETER  Location
指定存储帐户的位置。有效的值为 Windows Azure 位置，例如“West US”。您可以输入位置，也可输入关联组，但不能同时输入这两者。

.PARAMETER  AffinityGroup
指定存储帐户的关联组。您可以输入位置，也可输入关联组，但不能同时输入这两者。

.INPUTS
无。您不能通过管道向此函数传送输入。

.OUTPUTS
System.String

.EXAMPLE
PS C:\> Get-AzureVMStorage -Location "East US"
devtest3-fabricam

.EXAMPLE
PS C:\> Get-AzureVMStorage -AffinityGroup Finance
PS C:\>

.EXAMPLE\
PS C:\> Get-AzureVMStorage -AffinityGroup Finance -Verbose
VERBOSE: Get-AzureVMStorage: Start
VERBOSE: Get-AzureVMStorage: End

.LINK
Get-AzureStorageAccount
#>
function Get-AzureVMStorage
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true, ParameterSetName = 'Location')]
        [String]
        $Location,

        [Parameter(Mandatory = $true, ParameterSetName = 'AffinityGroup')]
        [String]
        $AffinityGroup
    )

    Write-VerboseWithTime 'Get-AzureVMStorage: 开始'

    $storages = @(Get-AzureStorageAccount -ErrorAction SilentlyContinue)
    $storageName = $null

    foreach ($storage in $storages)
    {
        # 获取名称以 "devtest" 开头的第一个存储帐户
        if ($storage.Label -like 'devtest*')
        {
            if ($storage.AffinityGroup -eq $AffinityGroup -or $storage.Location -eq $Location)
            {
                $storageName = $storage.Label

                    Write-HostWithTime ('Get-AzureVMStorage: 已找到 devtest 存储帐户 ' + $storageName)
                    $storage | Out-String | Write-VerboseWithTime
                break
            }
        }
    }

    Write-VerboseWithTime 'Get-AzureVMStorage: 结束'
    return $storageName
}


<#
.SYNOPSIS
使用以 "devtest" 开头的唯一名称创建一个新的 Windows Azure 存储帐户。

.DESCRIPTION
Add-AzureVMStorage 函数在当前订阅中创建一个新的 Windows Azure 存储帐户。该帐户的名称以 "devtest" 开头，后跟一个唯一的字母数字字符串。此函数返回新存储帐户的名称。您必须为新存储帐户指定位置或关联组。

.PARAMETER  Location
指定存储帐户的位置。有效的值为 Windows Azure 位置，例如“West US”。您可以输入位置，也可输入关联组，但不能同时输入这两者。

.PARAMETER  AffinityGroup
指定存储帐户的关联组。您可以输入位置，也可输入关联组，但不能同时输入这两者。

.INPUTS
无。您不能通过管道向此函数传送输入。

.OUTPUTS
System.String. 此字符串是新存储帐户的名称

.EXAMPLE
PS C:\> Add-AzureVMStorage -Location "East Asia"
devtestd6b45e23a6dd4bdab

.EXAMPLE
PS C:\> Add-AzureVMStorage -AffinityGroup Finance
devtestd6b45e23a6dd4bdab

.EXAMPLE
PS C:\> Add-AzureVMStorage -AffinityGroup Finance -Verbose
VERBOSE: Add-AzureVMStorage: Start
VERBOSE: Add-AzureVMStorage: Created new storage acccount devtestd6b45e23a6dd4bdab"
VERBOSE: Add-AzureVMStorage: End
devtestd6b45e23a6dd4bdab

.LINK
New-AzureStorageAccount
#>
function Add-AzureVMStorage
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true, ParameterSetName = 'Location')]
        [String]
        $Location,

        [Parameter(Mandatory = $true, ParameterSetName = 'AffinityGroup')]
        [String]
        $AffinityGroup
    )

    Write-VerboseWithTime 'Add-AzureVMStorage: 开始'

    # 通过将 GUID 的一部分追加到“devtest”来创建一个唯一名称
    $name = 'devtest'
    $suffix = [guid]::NewGuid().ToString('N').Substring(0,24 - $name.Length)
    $name = $name + $suffix

    # 使用位置/关联组创建一个新的 Windows Azure 存储帐户
    if ($PSCmdlet.ParameterSetName -eq 'Location')
    {
        New-AzureStorageAccount -StorageAccountName $name -Location $Location | Out-Null
    }
    else
    {
        New-AzureStorageAccount -StorageAccountName $name -AffinityGroup $AffinityGroup | Out-Null
    }

    Write-HostWithTime ("Add-AzureVMStorage: 已创建新存储帐户 $name")
    Write-VerboseWithTime 'Add-AzureVMStorage: 结束'
    return $name
}


<#
.SYNOPSIS
验证配置文件并返回一个由配置文件值组成的哈希表。

.DESCRIPTION
Read-ConfigFile 函数验证 JSON 配置文件并返回由所选值组成的哈希表。
-- 它首先将 JSON 文件转换成 PSCustomObject。
-- 此函数会验证 environmentSettings 属性是否包含网站属性或云服务属性之一而不是同时包含这两个属性。
-- 创建并返回两种哈希表之一；一种哈希表用于网站，另一种哈希表用于云服务。网站哈希表具有以下键:
-- IsAzureWebSite: $True. 此配置文件用于网站。
-- Name: 网站名称
-- Location: 网站位置
-- Databases: 网站 SQL 数据库
云服务哈希表具有以下键:
-- IsAzureWebSite: $False. 此配置文件不用于网站。
-- webdeployparameters : 可选。可能为 $null 或为空。
-- Databases: SQL 数据库

.PARAMETER  ConfigurationFile
指定您 Web 项目的 JSON 配置文件的路径及名称。Visual Studio 会在您创建 Web 项目时自动生成 JSON 文件，并将该文件存储在您解决方案中的 PublishScripts 文件夹中。

.PARAMETER HasWebDeployPackage
Indicates that there is a web deploy package ZIP file for the web application. To specify a value of $true, use -HasWebDeployPackage or HasWebDeployPackage:$true. To specify a value of false, use HasWebDeployPackage:$false.This parameter is required.

.INPUTS
无。您不能通过管道向此函数传送输入。

.OUTPUTS
System.Collections.Hashtable

.EXAMPLE
PS C:\> Read-ConfigFile -ConfigurationFile <path> -HasWebDeployPackage


Name                           Value                                                                                                                                                                     
----                           -----                                                                                                                                                                     
databases                      {@{connectionStringName=; databaseName=; serverName=; user=; password=}}                                                                                                  
cloudService                   @{name=asdfhl; affinityGroup=stephwe1ag1cus; location=; virtualNetwork=; subnet=; availabilitySet=; virtualMachine=}                                                      
IsWAWS                         False                                                                                                                                                                     
webDeployParameters            @{iisWebApplicationName=Default Web Site} 
#>
function Read-ConfigFile
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [ValidateScript({Test-Path $_ -PathType Leaf})]
        [String]
        $ConfigurationFile,

        [Parameter(Mandatory = $true)]
        [Switch]
        $HasWebDeployPackage	    
    )

    Write-VerboseWithTime 'Read-ConfigFile: 开始'

    # 获取 JSON 文件的内容(使用 -raw 时会忽略换行符)并将其转换成 PSCustomObject
    $config = Get-Content $ConfigurationFile -Raw | ConvertFrom-Json

    if (!$config)
    {
        throw ('Read-ConfigFile: ConvertFrom-Json 失败: ' + $error[0])
    }

    # 确定 environmentSettings 对象是否有“webSite”或“cloudService”属性(不论属性值如何)
    $hasWebsiteProperty =  Test-Member -Object $config.environmentSettings -Member 'webSite'
    $hasCloudServiceProperty = Test-Member -Object $config.environmentSettings -Member 'cloudService'

    if (!$hasWebsiteProperty -and !$hasCloudServiceProperty)
    {
        throw 'Read-ConfigFile: 配置文件格式不正确。既没有 webSite，也没有 cloudService'
    }
    elseif ($hasWebsiteProperty -and $hasCloudServiceProperty)
    {
        throw 'Read-ConfigFile: 配置文件格式不正确。既有 webSite 又有 cloudService'
    }

    # 使用 PSCustomObject 中的值生成一个哈希表
    $returnObject = New-Object -TypeName Hashtable
    $returnObject.Add('IsAzureWebSite', $hasWebsiteProperty)

    if ($hasWebsiteProperty)
    {
        $returnObject.Add('name', $config.environmentSettings.webSite.name)
        $returnObject.Add('location', $config.environmentSettings.webSite.location)
    }
    else
    {
        $returnObject.Add('cloudService', $config.environmentSettings.cloudService)
        if ($HasWebDeployPackage)
        {
            $returnObject.Add('webDeployParameters', $config.environmentSettings.webdeployParameters)
        }
    }

    if (Test-Member -Object $config.environmentSettings -Member 'databases')
    {
        $returnObject.Add('databases', $config.environmentSettings.databases)
    }

    Write-VerboseWithTime 'Read-ConfigFile: 结束'

    return $returnObject
}

<#
.SYNOPSIS
向虚拟机添加新的输入终结点，并返回包含新终结点的虚拟机。

.DESCRIPTION
Add-AzureVMEndpoints 函数向虚拟机添加新的输入终结点，并返回包含新终结点的虚拟机。此函数会调用 Add-AzureEndpoint cmdlet (Azure 模块)。

.PARAMETER  VM
指定虚拟机对象。请输入一个虚拟机对象，例如 New-AzureVM 或 Get-AzureVM cmdlet 返回的类型。您可以通过管道将 Get-AzureVM 返回的对象传送给 Add-AzureVMEndpoints。

.PARAMETER  Endpoints
指定由要向虚拟机添加的终结点组成的阵列。通常，这些终结点来自于 Visual Studio 为 Web 项目生成的 JSON 配置文件。请使用此模块中的 Read-ConfigFile 函数将该文件转换成哈希表。这些终结点是该哈希表的 cloudservice 键的一个属性($<hashtable>.cloudservice.virtualmachine.endpoints)。例如
PS C:\> $config.cloudservice.virtualmachine.endpoints
name      protocol publicport privateport
----      -------- ---------- -----------
http      tcp      80         80
https     tcp      443        443
WebDeploy tcp      8172       8172

.INPUTS
Microsoft.WindowsAzure.Commands.ServiceManagement.Model.IPersistentVM

.OUTPUTS
Microsoft.WindowsAzure.Commands.ServiceManagement.Model.IPersistentVM

.EXAMPLE
Get-AzureVM

.EXAMPLE

.LINK
Get-AzureVM

.LINK
Add-AzureEndpoint
#>
function Add-AzureVMEndpoints
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [Microsoft.WindowsAzure.Commands.ServiceManagement.Model.PersistentVM]
        $VM,

        [Parameter(Mandatory = $true)]
        [PSCustomObject[]]
        $Endpoints
    )

    Write-VerboseWithTime 'Add-AzureVMEndpoints: 开始'

    # 将 JSON 文件中的每个终结点添加到虚拟机
    $Endpoints | ForEach-Object `
    {
        $_ | Out-String | Write-VerboseWithTime
        Add-AzureEndpoint -VM $VM -Name $_.name -Protocol $_.protocol -LocalPort $_.privateport -PublicPort $_.publicport | Out-Null
    }

    Write-VerboseWithTime 'Add-AzureVMEndpoints: 结束'
    return $VM
}

<#
.SYNOPSIS
在 Windows Azure 订阅中为新虚拟机创建所有元素。

.DESCRIPTION
此函数会创建一个 Windows Azure 虚拟机(VM)，并返回部署后的虚拟机的 URL。此函数会设置先决条件，然后调用 New-AzureVM cmdlet (Azure 模块)来创建一个新虚拟机。
-- 此函数通过调用 New-AzureVMConfig cmdlet (Azure 模块)来获取虚拟机配置对象。
-- 如果您包含 Subnet 参数以便将虚拟机添加到某个 Azure 子网，则此函数会调用 Set-AzureSubnet 来为虚拟机设置子网列表。
-- 此函数通过调用 Add-AzureProvisioningConfig (Azure 模块)来向虚拟机配置添加元素。它使用管理员帐户和密码创建一个独立的 Windows 设置配置(-Windows)。
-- 此函数调用此模块中的 Add-AzureVMEndpoints 函数来添加由 Endpoints 参数指定的终结点。此函数接受虚拟机对象作为参数并返回包含所添加终结点的虚拟机对象。
-- 此函数调用 Add-AzureVM cmdlet 来创建一个新的 Windows Azure 虚拟环境，并返回该新虚拟机。函数参数的值通常从 Visual Studio 为 Windows Azure 集成的 Web 项目生成的 JSON 配置文件中获取。此模块中的 Read-ConfigFile 函数可将 JSON 文件转换成哈希表。请将该哈希表的 cloudservice 键保存在一个变量中(作为 PSCustomObject)，然后使用该自定义对象的属性作为参数值。

.PARAMETER  UserName
指定管理员用户名。此用户名以 Add-AzureProvisioningConfig 的 AdminUserName 参数值形式加以提交。此参数是必需的。

.PARAMETER  UserPassword
指定管理员用户帐户的密码。此密码以 Add-AzureProvisioningConfig 的 Password 参数值形式加以提交。此参数是必需的。

.PARAMETER  VMName
为新虚拟机指定一个名称。该虚拟机名称在云服务中必须是唯一的。此参数是必需的。

.PARAMETER  VMSize
指定虚拟机的大小。有效的值有:“ExtraSmall”、“Small”、“Medium”、“Large”、“ExtraLarge”、“A5”、“A6”和“A7”。此值以 New-AzureVMConfig 的 InstanceSize 参数值形式加以提交。此参数是必需的。

.PARAMETER  ServiceName
指定现有的 Windows Azure 服务或指定新 Windows Azure 服务的名称。此值提交给 New-AzureVM cmdlet 的 ServiceName 参数，由该 cmdlet 将新虚拟机添加到现有的 Windows Azure 服务；或者，如果指定了 Location 或 AffinityGroup，该 cmdlet 会在当前订阅中创建一个新的虚拟机及服务。此参数是必需的。

.PARAMETER  ImageName
为操作系统磁盘指定要使用的虚拟机映像的名称。此参数以 New-AzureVMConfig cmdlet 的 ImageName 参数值形式加以提交。此参数是必需的。

.PARAMETER  Endpoints
指定一组要添加到虚拟机的终结点。该值提交给此模块导出的 Add-AzureVMEndpoints 函数。此参数是可选的。通常，这些终结点来自于 Visual Studio 为 Web 项目生成的 JSON 配置文件。请使用此模块中的 Read-ConfigFile 函数将该文件转换成哈希表。这些终结点是该哈希表的 cloudService 键的一个属性($<hashtable>.cloudservice.virtualmachine.endpoints)。

.PARAMETER  AvailabilitySetName
为新虚拟机指定可用性集的名称。当您将多个虚拟机放入一个可用性集内时，Windows Azure 会设法将这些虚拟机分别放在不同的主机上，以便提高在其中一个虚拟机发生故障时服务的连续性。此参数是可选的。

.PARAMETER  VNetName
指定将新虚拟机部署到的虚拟网络的名称。此值提交给 Add-AzureVM cmdlet 的 VNetName 参数。此参数是可选的。

.PARAMETER  Location
为新虚拟机指定一个位置。有效的值为 Windows Azure 位置，例如“West US”。默认值为订阅位置。此参数是可选的。

.PARAMETER  AffinityGroup
为新虚拟机指定一个关联组。关联组是一组相关的资源。当您指定了关联组时，Windows Azure 会设法将该组中的资源放在一起，以提升效率。

.PARAMETER EnableWebDeployExtension
为虚拟机做好部署的准备。为虚拟机做好部署的准备。此参数是可选的。如果未指定此参数，则会创建虚拟机，但不会部署它。此参数的值包含在 Visual Studio 为云服务生成的 JSON 配置文件中。此参数的值包含在 Visual Studio 为云服务生成的 JSON 配置文件中。

.PARAMETER  Subnet
指定新虚拟机配置的子网。此值将提交给 Set-AzureSubnet cmdlet (Azure 模块)，该 cmdlet 接受一个虚拟机和一个子网名称数组作为参数，并返回一个在配置中采用这些子网的虚拟机。

.INPUTS
无。此函数不从管道中获取任何输入。

.OUTPUTS
System.Url

.EXAMPLE
 此命令调用 Add-AzureVM 函数。其很多参数值都来自于 $CloudServiceConfiguration 对象。此 PSCustomObject 是 Read-ConfigFile 函数返回的哈希表的 cloudservice 键和值。数据来源是 Visual Studio 为 Web 项目生成的 JSON 配置文件。

PS C:\> $config = Read-Configfile <name>.json
PS C:\> $CloudServiceConfiguration = config.cloudservice

PS C:\> Add-AzureVM `
-UserName $userName `
-UserPassword  $userPassword `
-ImageName $CloudServiceConfiguration.virtualmachine.vhdImage `
-VMName $CloudServiceConfiguration.virtualmachine.name `
-VMSize $CloudServiceConfiguration.virtualmachine.size`
-Endpoints $CloudServiceConfiguration.virtualmachine.endpoints `
-ServiceName $serviceName `
-Location $CloudServiceConfiguration.location `
-AvailabilitySetName $CloudServiceConfiguration.availabilitySet `
-VNetName $CloudServiceConfiguration.virtualNetwork `
-Subnet $CloudServiceConfiguration.subnet `
-AffinityGroup $CloudServiceConfiguration.affinityGroup `
-EnableWebDeployExtension

http://contoso.cloudapp.net

.EXAMPLE
PS C:\> $endpoints = [PSCustomObject]@{name="http";protocol="tcp";publicport=80;privateport=80}, `
                        [PSCustomObject]@{name="https";protocol="tcp";publicport=443;privateport=443},`
                        [PSCustomObject]@{name="WebDeploy";protocol="tcp";publicport=8172;privateport=8172}
PS C:\> Add-AzureVM `
-UserName admin01 `
-UserPassword "pa$$word" `
-ImageName bd507d3a70934695bc2128e3e5a255ba__RightImage-Windows-2012-x64-v13.4.12.2 `
-VMName DevTestVM123 `
-VMSize Small `
-Endpoints $endpoints `
-ServiceName DevTestVM1234 `
-Location "West US"

.LINK
New-AzureVMConfig

.LINK
Set-AzureSubnet

.LINK
Add-AzureProvisioningConfig

.LINK
Get-AzureDeployment
#>
function Add-AzureVM
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [String]
        $UserName,

        [Parameter(Mandatory = $true)]
        [String]
        $UserPassword,

        [Parameter(Mandatory = $true)]
        [String]
        $VMName,

        [Parameter(Mandatory = $true)]
        [String]
        $VMSize,

        [Parameter(Mandatory = $true)]
        [String]
        $ServiceName,

        [Parameter(Mandatory = $true)]
        [String]
        $ImageName,

        [Parameter(Mandatory = $false)]
        [AllowNull()]
        [Object[]]
        $Endpoints,

        [Parameter(Mandatory = $false)]
        [AllowEmptyString()]
        [String]
        $AvailabilitySetName,

        [Parameter(Mandatory = $false)]
        [AllowEmptyString()]
        [String]
        $VNetName,

        [Parameter(Mandatory = $false)]
        [AllowEmptyString()]
        [String]
        $Location,

        [Parameter(Mandatory = $false)]
        [AllowEmptyString()]
        [String]
        $AffinityGroup,

        [Parameter(Mandatory = $false)]
        [AllowEmptyString()]
        [String]
        $Subnet,

        [Parameter(Mandatory = $false)]
        [Switch]
        $EnableWebDeployExtension
    )

    Write-VerboseWithTime 'Add-AzureVM: 开始'

    # 创建一个新的 Windows Azure 虚拟机配置对象。
    if ($AvailabilitySetName)
    {
        $vm = New-AzureVMConfig -Name $VMName -InstanceSize $VMSize -ImageName $ImageName -AvailabilitySetName $AvailabilitySetName
    }
    else
    {
        $vm = New-AzureVMConfig -Name $VMName -InstanceSize $VMSize -ImageName $ImageName
    }

    if (!$vm)
    {
        throw 'Add-AzureVM: 未能创建 Azure 虚拟机配置。'
    }

    if ($Subnet)
    {
        # 为虚拟机配置设置子网列表。
        $subnetResult = Set-AzureSubnet -VM $vm -SubnetNames $Subnet

        if (!$subnetResult)
        {
            throw ('Add-AzureVM: 未能设置子网' + $Subnet)
        }
    }

    # 向虚拟机配置添加配置数据
    $VMWithConfig = Add-AzureProvisioningConfig -VM $vm -Windows -Password $UserPassword -AdminUserName $UserName

    if (!$VMWithConfig)
    {
        throw ('Add-AzureVM: 未能创建设置配置。')
    }

    # 向虚拟机添加输入终结点
    if ($Endpoints -and $Endpoints.Count -gt 0)
    {
        $VMWithConfig = Add-AzureVMEndpoints -Endpoints $Endpoints -VM $VMWithConfig
    }

    if (!$VMWithConfig)
    {
        throw ('Add-AzureVM: 未能创建终结点。')
    }

    if ($EnableWebDeployExtension)
    {
        Write-VerboseWithTime 'Add-AzureVM: 添加 webdeploy 扩展插件'

        Write-VerboseWithTime '若要查看 WebDeploy 许可证，请参见 http://go.microsoft.com/fwlink/?LinkID=389744 '

        $VMWithConfig = Set-AzureVMExtension `
            -VM $VMWithConfig `
            -ExtensionName WebDeployForVSDevTest `
            -Publisher 'Microsoft.VisualStudio.WindowsAzure.DevTest' `
            -Version '1.*' 

        if (!$VMWithConfig)
        {
            throw ('Add-AzureVM: 未能添加 webdeploy 扩展。')
        }
    }

    # 创建一个由要展开的参数组成的哈希表
    $param = New-Object -TypeName Hashtable
    if ($VNetName)
    {
        $param.Add('VNetName', $VNetName)
    }

    if ($Location)
    {
        $param.Add('Location', $Location)
    }

    if ($AffinityGroup)
    {
        $param.Add('AffinityGroup', $AffinityGroup)
    }

    $param.Add('ServiceName', $ServiceName)
    $param.Add('VMs', $VMWithConfig)
    $param.Add('WaitForBoot', $true)

    $param | Out-String | Write-VerboseWithTime

    New-AzureVM @param | Out-Null

    Write-HostWithTime ('Add-AzureVM: 已创建虚拟机 ' + $VMName)

    $url = [System.Uri](Get-AzureDeployment -ServiceName $ServiceName).Url

    if (!$url)
    {
        throw 'Add-AzureVM: 找不到虚拟机 URL。'
    }

    Write-HostWithTime ('Add-AzureVM: 发布 URL https://' + $url.Host + ':' + $WebDeployPort + '/msdeploy.axd')

    Write-VerboseWithTime 'Add-AzureVM: 结束'

    return $url.AbsoluteUri
}


<#
.SYNOPSIS
获取指定的 Windows Azure 虚拟机。

.DESCRIPTION
Find-AzureVM 函数根据服务名称和虚拟机名称获取 Windows Azure 虚拟机(VM)。此函数通过调用 Test-AzureName cmdlet (Azure 模块)来验证 Windows Azure 中是否存在该服务名称。如果存在，此函数会调用 Get-AzureVM cmdlet 来获取相应虚拟机。此函数返回一个包含 vm 和 foundService 键的哈希表。
-- FoundService: 如果 Test-AzureName 找到了相应服务，则为 $True； 否则为 $False
-- VM: 当 FoundService 为 True 并且 Get-AzureVM 返回虚拟机对象时，包含相应的虚拟机对象。

.PARAMETER  ServiceName
现有 Windows Azure 服务的名称。此参数是必需的。

.PARAMETER  VMName
服务中虚拟机的名称。此参数是必需的。

.INPUTS
无。您不能通过管道向此函数传送输入。

.OUTPUTS
System.Collections.Hashtable

.EXAMPLE
PS C:\> Find-AzureVM -Service Contoso -Name ContosoVM2

Name                           Value
----                           -----
foundService                   True

DeploymentName        : Contoso
Name                  : ContosoVM2
Label                 :
VM                    : Microsoft.WindowsAzure.Commands.ServiceManagement.Model.PersistentVM
InstanceStatus        : ReadyRole
IpAddress             : 100.71.114.118
InstanceStateDetails  :
PowerState            : Started
InstanceErrorCode     :
InstanceFaultDomain   : 0
InstanceName          : ContosoVM2
InstanceUpgradeDomain : 0
InstanceSize          : Small
AvailabilitySetName   :
DNSName               : http://contoso.cloudapp.net/
ServiceName           : Contoso
OperationDescription  : Get-AzureVM
OperationId           : 3c38e933-9464-6876-aaaa-734990a882d6
OperationStatus       : Succeeded

.LINK
Get-AzureVM
#>
function Find-AzureVM
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [String]
        $ServiceName,

        [Parameter(Mandatory = $true)]
        [String]
        $VMName
    )

    Write-VerboseWithTime 'Find-AzureVM: 开始'
    $foundService = $false
    $vm = $null

    if (Test-AzureName -Service -Name $ServiceName)
    {
        $foundService = $true
        $vm = Get-AzureVM -ServiceName $ServiceName -Name $VMName
        if ($vm)
        {
            Write-HostWithTime ('Find-AzureVM: 已找到现有虚拟机 ' + $vm.Name )
            $vm | Out-String | Write-VerboseWithTime
        }
    }

    Write-VerboseWithTime 'Find-AzureVM: 结束'
    return @{ VM = $vm; FoundService = $foundService }
}


<#
.SYNOPSIS
在订阅中查找或创建与 JSON 配置文件中的值匹配的虚拟机。

.DESCRIPTION
New-AzureVMEnvironment 函数会在订阅中查找或创建一个与 Visual Studio 为 Web 项目生成的 JSON 配置文件中的值匹配的虚拟机。它接受作为 Read-ConfigFile 返回的哈希表的 cloudservice 键的 PSCustomObject。此数据来自于 Visual Studio 生成的 JSON 配置文件。此函数会在订阅中查找服务名称和虚拟机名称与 CloudServiceConfiguration 自定义对象中的值匹配的虚拟机(VM)。如果此函数找不到匹配的虚拟机，它会调用此模块中的 Add-AzureVM 函数，并使用 CloudServiceConfiguration 对象中的值来创建虚拟机。虚拟机环境包含一个名称以 "devtest" 开头的存储帐户。如果此函数在订阅中找不到采用该名称模式的存储帐户，它会创建一个这样的存储帐户。此函数返回一个包含 VMUrl、userName 和 Password 键及字符串值的哈希表。

.PARAMETER  CloudServiceConfiguration
接受 PSCustomObject 作为参数，后者包含 Read-ConfigFile 函数返回的哈希表的 cloudservice 属性。所有值均来自于 Visual Studio 为 Web 项目生成的 JSON 配置文件。您可以在您解决方案中的 PublishScripts 文件夹中找到该文件。此参数是必需的。
$config = Read-ConfigFile -ConfigurationFile <file>.json $cloudServiceConfiguration = $config.cloudService

.PARAMETER  VMPassword
接受包含 name 和 password 键的哈希表作为参数，例如: @{Name = "admin"; Password = "pa$$word"} 此参数是可选的。如果您省略此参数，默认值将为 JSON 配置文件中的虚拟机用户名和密码。

.INPUTS
PSCustomObject  System.Collections.Hashtable

.OUTPUTS
System.Collections.Hashtable

.EXAMPLE
$config = Read-ConfigFile -ConfigurationFile $<file>.json
$cloudSvcConfig = $config.cloudService
$namehash = @{name = "admin"; password = "pa$$word"}

New-AzureVMEnvironment `
    -CloudServiceConfiguration $cloudSvcConfig `
    -VMPassword $namehash

Name                           Value
----                           -----
UserName                       admin
VMUrl                          contoso.cloudnet.net
Password                       pa$$word

.LINK
Add-AzureVM

.LINK
New-AzureStorageAccount
#>
function New-AzureVMEnvironment
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [Object]
        $CloudServiceConfiguration,

        [Parameter(Mandatory = $false)]
        [AllowNull()]
        [Hashtable]
        $VMPassword
    )

    Write-VerboseWithTime ('New-AzureVMEnvironment: 开始')

    if ($CloudServiceConfiguration.location -and $CloudServiceConfiguration.affinityGroup)
    {
        throw 'New-AzureVMEnvironment: 配置文件格式不正确。既有 location 又有 affinityGroup'
    }

    if (!$CloudServiceConfiguration.location -and !$CloudServiceConfiguration.affinityGroup)
    {
        throw 'New-AzureVMEnvironment: 配置文件格式不正确。既没有 location，也没有 affinityGroup'
    }

    # 如果 CloudServiceConfiguration 对象有“name”属性(表示服务名称)，并且此“name”属性具有值，请使用该值。否则，请使用 CloudServiceConfiguration 对象中的虚拟机名称，该对象内始终都填有值。
    if ((Test-Member $CloudServiceConfiguration 'name') -and $CloudServiceConfiguration.name)
    {
        $serviceName = $CloudServiceConfiguration.name
    }
    else
    {
        $serviceName = $CloudServiceConfiguration.virtualMachine.name
    }

    if (!$VMPassword)
    {
        $userName = $CloudServiceConfiguration.virtualMachine.user
        $userPassword = $CloudServiceConfiguration.virtualMachine.password
    }
    else
    {
        $userName = $VMPassword.Name
        $userPassword = $VMPassword.Password
    }

    # 获取 JSON 文件中的虚拟机名称
    $findAzureVMResult = Find-AzureVM -ServiceName $serviceName -VMName $CloudServiceConfiguration.virtualMachine.name

    # 如果我们在此云服务中找不到具有该名称的虚拟机，请创建一个这样的虚拟机。
    if (!$findAzureVMResult.VM)
    {
        $storageAccountName = $null
        $imageInfo = Get-AzureVMImage -ImageName $CloudServiceConfiguration.virtualmachine.vhdimage 
        if ($imageInfo -and $imageInfo.Category -eq 'User')
        {
            $storageAccountName = ($imageInfo.MediaLink.Host -split '\.')[0]
        }

        if (!$storageAccountName)
        {
            if ($CloudServiceConfiguration.location)
            {
                $storageAccountName = Get-AzureVMStorage -Location $CloudServiceConfiguration.location
            }
            else
            {
                $storageAccountName = Get-AzureVMStorage -AffinityGroup $CloudServiceConfiguration.affinityGroup
            }
        }

        #If there's no devtest* storage account, create one.
        if (!$storageAccountName)
        {
            if ($CloudServiceConfiguration.location)
            {
                $storageAccountName = Add-AzureVMStorage -Location $CloudServiceConfiguration.location
            }
            else
            {
                $storageAccountName = Add-AzureVMStorage -AffinityGroup $CloudServiceConfiguration.affinityGroup
            }
        }

        $currentSubscription = Get-AzureSubscription -Current

        if (!$currentSubscription)
        {
            throw 'New-AzureVMEnvironment: 未能获取当前 Azure 订阅。'
        }

        # 将 devtest* 存储帐户设为当前帐户
        Set-AzureSubscription `
            -SubscriptionName $currentSubscription.SubscriptionName `
            -CurrentStorageAccountName $storageAccountName

        Write-VerboseWithTime ('New-AzureVMEnvironment: 存储帐户设置为 ' + $storageAccountName)

        $location = ''            
        if (!$findAzureVMResult.FoundService)
        {
            $location = $CloudServiceConfiguration.location
        }

        $endpoints = $null
        if (Test-Member -Object $CloudServiceConfiguration.virtualmachine -Member 'Endpoints')
        {
            $endpoints = $CloudServiceConfiguration.virtualmachine.endpoints
        }

        # 使用 JSON 文件中的值外加参数值创建一个虚拟机
        $VMUrl = Add-AzureVM `
            -UserName $userName `
            -UserPassword $userPassword `
            -ImageName $CloudServiceConfiguration.virtualMachine.vhdImage `
            -VMName $CloudServiceConfiguration.virtualMachine.name `
            -VMSize $CloudServiceConfiguration.virtualMachine.size`
            -Endpoints $endpoints `
            -ServiceName $serviceName `
            -Location $location `
            -AvailabilitySetName $CloudServiceConfiguration.availabilitySet `
            -VNetName $CloudServiceConfiguration.virtualNetwork `
            -Subnet $CloudServiceConfiguration.subnet `
            -AffinityGroup $CloudServiceConfiguration.affinityGroup `
            -EnableWebDeployExtension:$CloudServiceConfiguration.virtualMachine.enableWebDeployExtension

        Write-VerboseWithTime ('New-AzureVMEnvironment: 结束')

        return @{ 
            VMUrl = $VMUrl; 
            UserName = $userName; 
            Password = $userPassword; 
            IsNewCreatedVM = $true; }
    }
    else
    {
        Write-VerboseWithTime ('New-AzureVMEnvironment: 已找到现有虚拟机 ' + $findAzureVMResult.VM.Name)
    }

    Write-VerboseWithTime ('New-AzureVMEnvironment: 结束')

    return @{ 
        VMUrl = $findAzureVMResult.VM.DNSName; 
        UserName = $userName; 
        Password = $userPassword; 
        IsNewCreatedVM = $false; }
}


<#
.SYNOPSIS
返回一个命令以运行 MsDeploy.exe 工具

.DESCRIPTION
Get-MSDeployCmd 函数汇编并返回一个有效的命令来运行 Web Deploy 工具 MSDeploy.exe。它在注册表项中查找该工具在本地计算机上的正确路径。此函数无参数。

.INPUTS
无

.OUTPUTS
System.String

.EXAMPLE
PS C:\> Get-MSDeployCmd
C:\Program Files\IIS\Microsoft Web Deploy V3\MsDeploy.exe

.LINK
Get-MSDeployCmd

.LINK
Web Deploy Tool
http://technet.microsoft.com/en-us/library/dd568996(v=ws.10).aspx
#>
function Get-MSDeployCmd
{
    Write-VerboseWithTime 'Get-MSDeployCmd: 开始'
    $regKey = 'HKLM:\SOFTWARE\Microsoft\IIS Extensions\MSDeploy'

    if (!(Test-Path $regKey))
    {
        throw ('Get-MSDeployCmd: 找不到 ' + $regKey)
    }

    $versions = @(Get-ChildItem $regKey -ErrorAction SilentlyContinue)
    $lastestVersion =  $versions | Sort-Object -Property Name -Descending | Select-Object -First 1

    if ($lastestVersion)
    {
        $installPathKeys = 'InstallPath','InstallPath_x86'

        foreach ($installPathKey in $installPathKeys)
        {		    	
            $installPath = $lastestVersion.GetValue($installPathKey)

            if ($installPath)
            {
                $installPath = Join-Path $installPath -ChildPath 'MsDeploy.exe'

                if (Test-Path $installPath -PathType Leaf)
                {
                    $msdeployPath = $installPath
                    break
                }
            }
        }
    }

    Write-VerboseWithTime 'Get-MSDeployCmd: 结束'
    return $msdeployPath
}


<#
.SYNOPSIS
创建一个 Windows Azure 网站。

.DESCRIPTION
使用特定的名称和位置创建一个 Windows Azure 网站。此函数会调用 Azure 模块中的 New-AzureWebsite cmdlet。如果订阅尚无具有指定名称的网站，则此函数会创建这样的网站并返回一个网站对象。否则，此函数将返回 $null。

.PARAMETER  Name
为新网站指定一个名称。此名称在 Windows Azure 中必须是唯一的。此参数是必需的。

.PARAMETER  Location
指定网站的位置。有效的值为 Windows Azure 位置，例如“West US”。此参数是必需的。

.INPUTS
无。

.OUTPUTS
Microsoft.WindowsAzure.Commands.Utilities.Websites.Services.WebEntities.Site

.EXAMPLE
Add-AzureWebsite -Name TestSite -Location "West US"

Name       : contoso
State      : Running
Host Names : contoso.azurewebsites.net

.LINK
New-AzureWebsite
#>
function Add-AzureWebsite
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [String]
        $Name,

        [Parameter(Mandatory = $true)]
        [String]
        $Location
    )

    Write-VerboseWithTime 'Add-AzureWebsite: 开始'
    $website = Get-AzureWebsite -Name $Name -ErrorAction SilentlyContinue

    if ($website)
    {
        Write-HostWithTime ('Add-AzureWebsite: 现有网站 ' +
        $website.Name + ' 已找到')
    }
    else
    {
        if (Test-AzureName -Website -Name $Name)
        {
            Write-ErrorWithTime ('网站 {0} 已存在' -f $Name)
        }
        else
        {
            $website = New-AzureWebsite -Name $Name -Location $Location
        }
    }

    $website | Out-String | Write-VerboseWithTime
    Write-VerboseWithTime 'Add-AzureWebsite: 结束'

    return $website
}

<#
.SYNOPSIS
当该 URL 为绝对 URL 且其方案为 https 时，返回 $True。

.DESCRIPTION
Test-HttpsUrl 函数将输入 URL 转换成 System.Uri 对象。当该 URL 为绝对(而非相对) URL 且其方案为 https 时，此函数返回 $True。如果上述两项条件中有任意一项条件不满足，或者输入字符串无法转换为 URL，则此函数将返回 $false。

.PARAMETER Url
指定要测试的 URL。请输入一个 URL 字符串。

.INPUTS
无。

.OUTPUTS
System.Boolean

.EXAMPLE
PS C:\>$profile.publishUrl
waws-prod-bay-001.publish.azurewebsites.windows.net:443

PS C:\>Test-HttpsUrl -Url 'waws-prod-bay-001.publish.azurewebsites.windows.net:443'
False
#>
function Test-HttpsUrl
{

    param
    (
        [Parameter(Mandatory = $true)]
        [String]
        $Url
    )

    # 如果无法将 $uri 转换成 System.Uri 对象，则 Test-HttpsUrl 将返回 $false
    $uri = $Url -as [System.Uri]

    return $uri.IsAbsoluteUri -and $uri.Scheme -eq 'https'
}


<#
.SYNOPSIS
向 Windows Azure 部署 Web 包。

.DESCRIPTION
Publish-WebPackage 函数使用 MsDeploy.exe 和一个 Web 部署包 ZIP 文件将资源部署到 Windows Azure 网站。此函数不生成任何输出。如果调用 MSDeploy.exe 失败，此函数将引发异常。若要获取更详细的输出，请使用 Verbose 通用参数。

.PARAMETER  WebDeployPackage
指定 Visual Studio 生成的 Web 部署包 ZIP 文件的路径及文件名。此参数是必需的。若要创建 Web 部署包 ZIP 文件，请参见 http://go.microsoft.com/fwlink/?LinkId=391353 上的“如何: 在 Visual Studio 中创建 Web 部署包”。

.PARAMETER PublishUrl
指定将资源部署到的 URL。该 URL 必须采用 HTTPS 协议并包含端口。此参数是必需的。

.PARAMETER SiteName
为网站指定一个名称。此参数是必需的。

.PARAMETER Username
指定网站管理员的用户名。此参数是必需的。

.PARAMETER Password
指定网站管理员的密码。请以纯文本格式输入密码。不允许使用安全字符串。此参数是必需的。

.PARAMETER AllowUntrusted
允许与站点建立不受信任的 SSL 连接。此参数在调用 MSDeploy.exe 的过程中使用。此参数是必需的。

.PARAMETER ConnectionString
指定 SQL 数据库的连接字符串。此参数接受包含 Name 和 ConnectionString 键的哈希表作为值。Name 的值是数据库的名称。ConnectionString 的值是 JSON 配置文件中的 connectionStringName。

.INPUTS
无。此函数不从管道中获取任何输入。

.OUTPUTS
无

.EXAMPLE
Publish-WebPackage -WebDeployPackage C:\Documents\Azure\ADWebApp.zip `
    -PublishUrl $publishUrl "https://contoso.cloudnet.net:8172/msdeploy.axd" `
    -SiteName 'Contoso 测试站点' `
    -UserName $UserName admin01 `
    -Password $UserPassword pa$$word `
    -AllowUntrusted:$False `
    -ConnectionString @{Name='TestDB';ConnectionString='DefaultConnection'}

.LINK
Publish-WebPackageToVM

.LINK
Web Deploy Command Line Reference (MSDeploy.exe)
http://go.microsoft.com/fwlink/?LinkId=391354
#>
function Publish-WebPackage
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [ValidateScript({Test-Path $_ -PathType Leaf})]
        [String]
        $WebDeployPackage,

        [Parameter(Mandatory = $true)]
        [ValidateScript({Test-HttpsUrl $_ })]
        [String]
        $PublishUrl,

        [Parameter(Mandatory = $true)]
        [String]
        $SiteName,

        [Parameter(Mandatory = $true)]
        [String]
        $UserName,

        [Parameter(Mandatory = $true)]
        [String]
        $Password,

        [Parameter(Mandatory = $false)]
        [Switch]
        $AllowUntrusted = $false,

        [Parameter(Mandatory = $true)]
        [Hashtable]
        $ConnectionString
    )

    Write-VerboseWithTime 'Publish-WebPackage: 开始'

    $msdeployCmd = Get-MSDeployCmd

    if (!$msdeployCmd)
    {
        throw 'Publish-WebPackage: 找不到 MsDeploy.exe。'
    }

    $WebDeployPackage = (Get-Item $WebDeployPackage).FullName

    $msdeployCmd =  '"' + $msdeployCmd + '"'
    $msdeployCmd += ' -verb:sync'
    $msdeployCmd += ' -Source:Package="{0}"'
    $msdeployCmd += ' -dest:auto,computername="{1}?site={2}",userName={3},password={4},authType=Basic'
    if ($AllowUntrusted)
    {
        $msdeployCmd += ' -allowUntrusted'
    }
    $msdeployCmd += ' -setParam:name="IIS Web Application Name",value="{2}"'

    foreach ($DBConnection in $ConnectionString.GetEnumerator())
    {
        $msdeployCmd += (' -setParam:name="{0}",value="{1}"' -f $DBConnection.Key, $DBConnection.Value)
    }

    $msdeployCmd = $msdeployCmd -f $WebDeployPackage, $PublishUrl, $SiteName, $UserName, $Password

    Write-VerboseWithTime ('Publish-WebPackage: MsDeploy: ' + $msdeployCmd)

    $msdeployExecution = Start-Process cmd.exe -ArgumentList ('/C "' + $msdeployCmd + '" ') -WindowStyle Normal -Wait -PassThru

    if ($msdeployExecution.ExitCode -ne 0)
    {
         Write-VerboseWithTime ('Msdeploy.exe 因出错而退出。ExitCode:' + $msdeployExecution.ExitCode)
    }

    Write-VerboseWithTime 'Publish-WebPackage: 结束'
    return ($msdeployExecution.ExitCode -eq 0)
}


<#
.SYNOPSIS
向 Windows Azure 部署虚拟机。

.DESCRIPTION
Publish-WebPackageToVM 函数是一个帮助程序函数，因为该函数会验证参数值，然后调用 Publish-WebPackage 函数。

.PARAMETER  VMDnsName
指定 Windows Azure 虚拟机的 DNS 名称。此参数是必需的。

.PARAMETER IisWebApplicationName
为虚拟机指定 IIS Web 应用程序的名称。此参数是必需的。这是您的 Visual Studio Web 应用程序的名称。您可以在 Visual Studio 生成的 JSON 配置文件的 webDeployparameters 特性中找到此名称。

.PARAMETER WebDeployPackage
指定 Visual Studio 生成的 Web 部署包 ZIP 文件的路径及文件名。此参数是必需的。若要创建 Web 部署包 ZIP 文件，请参见 http://go.microsoft.com/fwlink/?LinkId=391353 上的“如何: 在 Visual Studio 中创建 Web 部署包”。

.PARAMETER Username
指定虚拟机管理员的用户名。此参数是必需的。

.PARAMETER Password
指定虚拟机管理员的密码。请以纯文本格式输入密码。不允许使用安全字符串。此参数是必需的。

.PARAMETER AllowUntrusted
允许与站点建立不受信任的 SSL 连接。此参数在调用 MSDeploy.exe 的过程中使用。此参数是必需的。

.PARAMETER ConnectionString
指定 SQL 数据库的连接字符串。此参数接受包含 Name 和 ConnectionString 键的哈希表作为值。Name 的值是数据库的名称。ConnectionString 的值是 JSON 配置文件中的 connectionStringName。

.INPUTS
无。此函数不从管道中获取任何输入。

.OUTPUTS
无。

.EXAMPLE
Publish-WebPackageToVM -VMDnsName contoso.cloudapp.net `
-IisWebApplicationName myTestWebApp `
-WebDeployPackage C:\Documents\Azure\ADWebApp.zip
-Username admin01 `
-Password pa$$word `
-AllowUntrusted:$False `
-ConnectionString @{Name='TestDB';ConnectionString='DefaultConnection'}

.LINK
Publish-WebPackage
#>
function Publish-WebPackageToVM
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [String]
        $VMDnsName,

        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [String]
        $IisWebApplicationName,

        [Parameter(Mandatory = $true)]
        [ValidateScript({Test-Path $_ -PathType Leaf})]
        [String]
        $WebDeployPackage,

        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [String]
        $UserName,

        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [String]
        $UserPassword,

        [Parameter(Mandatory = $true)]
        [Bool]
        $AllowUntrusted,
        
        [Parameter(Mandatory = $true)]
        [Hashtable]
        $ConnectionString
    )
    Write-VerboseWithTime 'Publish-WebPackageToVM: 开始'

    $VMDnsUrl = $VMDnsName -as [System.Uri]

    if (!$VMDnsUrl)
    {
        throw ('Publish-WebPackageToVM: URL 无效 ' + $VMDnsUrl)
    }

    $publishUrl = 'https://{0}:{1}/msdeploy.axd' -f $VMDnsUrl.Host, $WebDeployPort

    $result = Publish-WebPackage `
        -WebDeployPackage $WebDeployPackage `
        -PublishUrl $publishUrl `
        -SiteName $IisWebApplicationName `
        -UserName $UserName `
        -Password $UserPassword `
        -AllowUntrusted:$AllowUntrusted `
        -ConnectionString $ConnectionString

    Write-VerboseWithTime 'Publish-WebPackageToVM: 结束'
    return $result
}


<#
.SYNOPSIS
创建一个供您连接 Windows Azure SQL 数据库的字符串。

.DESCRIPTION
Get-AzureSQLDatabaseConnectionString 函数会对连接字符串进行汇编，以便连接到 Windows Azure SQL 数据库。

.PARAMETER  DatabaseServerName
指定 Windows Azure 订阅中现有数据库服务器的名称。所有 Windows Azure SQL 数据库都必须与 SQL 数据库服务器关联。要获取服务器名称，请使用 Get-AzureSqlDatabaseServer cmdlet (Azure 模块)。此参数是必需的。

.PARAMETER  DatabaseName
指定 SQL 数据库的名称。此名称可以是现有 SQL 数据库的名称，也可以是用于新 SQL 数据库的名称。此参数是必需的。

.PARAMETER  Username
指定 SQL 数据库管理员的名称。此用户名将为 $Username@DatabaseServerName。此参数是必需的。

.PARAMETER  Password
指定 SQL 数据库管理员的密码。请以纯文本格式输入密码。不允许使用安全字符串。此参数是必需的。

.INPUTS
无。

.OUTPUTS
System.String

.EXAMPLE
PS C:\> $ServerName = (Get-AzureSqlDatabaseServer).ServerName
PS C:\> Get-AzureSQLDatabaseConnectionString -DatabaseServerName $ServerName `
        -DatabaseName 'testdb' -UserName 'admin'  -Password 'pa$$word'

Server=tcp:bebad12345.database.windows.net,1433;Database=testdb;User ID=admin@bebad12345;Password=pa$$word;Trusted_Connection=False;Encrypt=True;Connection Timeout=20;
#>
function Get-AzureSQLDatabaseConnectionString
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [String]
        $DatabaseServerName,

        [Parameter(Mandatory = $true)]
        [String]
        $DatabaseName,

        [Parameter(Mandatory = $true)]
        [String]
        $UserName,

        [Parameter(Mandatory = $true)]
        [String]
        $Password
    )

    return ('Server=tcp:{0}.database.windows.net,1433;Database={1};' +
           'User ID={2}@{0};' +
           'Password={3};' +
           'Trusted_Connection=False;' +
           'Encrypt=True;' +
           'Connection Timeout=20;') `
           -f $DatabaseServerName, $DatabaseName, $UserName, $Password
}


<#
.SYNOPSIS
使用 Visual Studio 生成的 JSON 配置文件中的值创建 Windows Azure SQL 数据库。

.DESCRIPTION
Add-AzureSQLDatabases 函数从 JSON 文件的数据库部分中获取信息。Add-AzureSQLDatabases (复数)这个函数会对 JSON 文件中的每个 SQL 数据库调用 Add-AzureSQLDatabase (单数)函数。Add-AzureSQLDatabase (单数)会调用 New-AzureSqlDatabase cmdlet (Azure 模块)以创建 SQL 数据库。此函数不返回数据库对象，而是返回由用来创建数据库的值组成的哈希表。

.PARAMETER DatabaseConfig
接受一个由 PSCustomObjects 组成的数组作为参数，这些对象来自于当 JSON 文件有网站属性时 Read-ConfigFile 函数返回的 JSON 文件。该文件包含 environmentSettings.databases 属性。您可以通过管道将该列表传送给此函数。
PS C:\> $config = Read-ConfigFile <name>.json
PS C:\> $DatabaseConfig = $config.databases| where connectionStringName
PS C:\> $DatabaseConfig
connectionStringName: Default Connection
databasename : TestDB1
edition   :
size     : 1
collation  : SQL_Latin1_General_CP1_CI_AS
servertype  : New SQL Database Server
servername  : r040tvt2gx
user     : dbuser
password   : Test.123
location   : West US

.PARAMETER  DatabaseServerPassword
指定 SQL 数据库服务器管理员的密码。请输入包含 Name 和 Password 键的哈希表。Name 的值是 SQL 数据库服务器的名称。Password 的值是管理员密码。例如: @Name = "TestDB1"; Password = "pa$$word" 此参数是可选的。如果您省略此参数，或者 SQL 数据库服务器名称与该 $DatabaseConfig 对象的 serverName 属性的值不匹配，则此函数会将该 $DatabaseConfig 对象的 Password 属性用于连接字符串中的 SQL 数据库。

.PARAMETER CreateDatabase
确认您是否要创建数据库。此参数是可选的。

.INPUTS
System.Collections.Hashtable[]

.OUTPUTS
System.Collections.Hashtable

.EXAMPLE
PS C:\> $config = Read-ConfigFile <name>.json
PS C:\> $DatabaseConfig = $config.databases| where $connectionStringName
PS C:\> $DatabaseConfig | Add-AzureSQLDatabases

Name                           Value
----                           -----
ConnectionString               Server=tcp:testdb1.database.windows.net,1433;Database=testdb;User ID=admin@testdb1;Password=pa$$word;Trusted_Connection=False;Encrypt=True;Connection Timeout=20;
Name                           Default Connection
Type                           SQLAzure

.LINK
Get-AzureSQLDatabaseConnectionString

.LINK
Create-AzureSQLDatabase
#>
function Add-AzureSQLDatabases
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [PSCustomObject]
        $DatabaseConfig,

        [Parameter(Mandatory = $false)]
        [AllowNull()]
        [Hashtable[]]
        $DatabaseServerPassword,

        [Parameter(Mandatory = $false)]
        [Switch]
        $CreateDatabase = $true
    )

    begin
    {
        Write-VerboseWithTime 'Add-AzureSQLDatabases: 开始'
    }
    process
    {
        Write-VerboseWithTime ('Add-AzureSQLDatabases: 正在创建 ' + $DatabaseConfig.databaseName)

        if ($CreateDatabase)
        {
            # 使用 DatabaseConfig 值创建一个新 SQL 数据库(除非已存在这样的数据库)
            # 已禁止显示命令输出。
            Add-AzureSQLDatabase -DatabaseConfig $DatabaseConfig | Out-Null
        }

        $serverPassword = $null
        if ($DatabaseServerPassword)
        {
            foreach ($credential in $DatabaseServerPassword)
            {
               if ($credential.Name -eq $DatabaseConfig.serverName)
               {
                   $serverPassword = $credential.password             
                   break
               }
            }               
        }

        if (!$serverPassword)
        {
            $serverPassword = $DatabaseConfig.password
        }

        return @{
            Name = $DatabaseConfig.connectionStringName;
            Type = 'SQLAzure';
            ConnectionString = Get-AzureSQLDatabaseConnectionString `
                -DatabaseServerName $DatabaseConfig.serverName `
                -DatabaseName $DatabaseConfig.databaseName `
                -UserName $DatabaseConfig.user `
                -Password $serverPassword }
    }
    end
    {
        Write-VerboseWithTime 'Add-AzureSQLDatabases: 结束'
    }
}


<#
.SYNOPSIS
创建一个新的 Windows Azure SQL 数据库。

.DESCRIPTION
Add-AzureSQLDatabase 函数使用 Visual Studio 生成的 JSON 配置文件中的数据创建一个 Windows Azure SQL 数据库并返回新数据库。如果订阅在指定 SQL 数据库服务器中已经有一个使用指定的数据库名称的 SQL 数据库，则此函数将返回现有数据库。此函数会调用实际创建 SQL 数据库的 New-AzureSqlDatabase cmdlet (Azure 模块)。

.PARAMETER DatabaseConfig
接受一个 PSCustomObject 作为参数，它来自于当 JSON 文件有网站属性时 Read-ConfigFile 函数返回的 JSON 配置文件。该文件包含 environmentSettings.databases 属性。您不能通过管道将该对象传送给此函数。Visual Studio 会为所有 Web 项目都生成一个 JSON 配置文件，并将该文件存储在您的解决方案的 PublishScripts 文件夹中。

.INPUTS
无。此函数不从管道中获取任何输入

.OUTPUTS
Microsoft.WindowsAzure.Commands.SqlDatabase.Services.Server.Database

.EXAMPLE
PS C:\> $config = Read-ConfigFile <name>.json
PS C:\> $DatabaseConfig = $config.databases | where connectionStringName
PS C:\> $DatabaseConfig

connectionStringName    : Default Connection
databasename : TestDB1
edition      :
size         : 1
collation    : SQL_Latin1_General_CP1_CI_AS
servertype   : New SQL Database Server
servername   : r040tvt2gx
user         : dbuser
password     : Test.123
location     : West US

PS C:\> Add-AzureSQLDatabase -DatabaseConfig $DatabaseConfig

.LINK
Add-AzureSQLDatabases

.LINK
New-AzureSQLDatabase
#>
function Add-AzureSQLDatabase
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [ValidateNotNull()]
        [Object]
        $DatabaseConfig
    )

    Write-VerboseWithTime 'Add-AzureSQLDatabase: 开始'

    # 如果参数值没有 serverName 属性，或者未填入 serverName 属性值，则失败。
    if (-not (Test-Member $DatabaseConfig 'serverName') -or -not $DatabaseConfig.serverName)
    {
        throw 'Add-AzureSQLDatabase: DatabaseConfig 值中缺少数据库 serverName (必需)。'
    }

    # 如果参数值没有 databasename 属性，或者 databasename 属性内未填入值，则失败。
    if (-not (Test-Member $DatabaseConfig 'databaseName') -or -not $DatabaseConfig.databaseName)
    {
        throw 'Add-AzureSQLDatabase: DatabaseConfig 值中缺少 databasename (必需)。'
    }

    $DbServer = $null

    if (Test-HttpsUrl $DatabaseConfig.serverName)
    {
        $absoluteDbServer = $DatabaseConfig.serverName -as [System.Uri]
        $subscription = Get-AzureSubscription -Current -ErrorAction SilentlyContinue

        if ($subscription -and $subscription.ServiceEndpoint -and $subscription.SubscriptionId)
        {
            $absoluteDbServerRegex = 'https:\/\/{0}\/{1}\/services\/sqlservers\/servers\/(.+)\.database\.windows\.net\/databases' -f `
                                     $subscription.serviceEndpoint.Host, $subscription.SubscriptionId

            if ($absoluteDbServer -match $absoluteDbServerRegex -and $Matches.Count -eq 2)
            {
                 $DbServer = $Matches[1]
            }
        }
    }

    if (!$DbServer)
    {
        $DbServer = $DatabaseConfig.serverName
    }

    $db = Get-AzureSqlDatabase -ServerName $DbServer -DatabaseName $DatabaseConfig.databaseName -ErrorAction SilentlyContinue

    if ($db)
    {
        Write-HostWithTime ('Create-AzureSQLDatabase: 正在使用现有数据库 ' + $db.Name)
        $db | Out-String | Write-VerboseWithTime
    }
    else
    {
        $param = New-Object -TypeName Hashtable
        $param.Add('serverName', $DbServer)
        $param.Add('databaseName', $DatabaseConfig.databaseName)

        if ((Test-Member $DatabaseConfig 'size') -and $DatabaseConfig.size)
        {
            $param.Add('MaxSizeGB', $DatabaseConfig.size)
        }
        else
        {
            $param.Add('MaxSizeGB', 1)
        }

        # 如果 $DatabaseConfig 对象有一个排序规则属性，并且该属性既不为 Null，也不为空
        if ((Test-Member $DatabaseConfig 'collation') -and $DatabaseConfig.collation)
        {
            $param.Add('Collation', $DatabaseConfig.collation)
        }

        # 如果 $DatabaseConfig 对象有一个版本属性，并且该属性既不为 Null，也不为空
        if ((Test-Member $DatabaseConfig 'edition') -and $DatabaseConfig.edition)
        {
            $param.Add('Edition', $DatabaseConfig.edition)
        }

        # 将哈希表写入到详细流中
        $param | Out-String | Write-VerboseWithTime
        # 通过展开调用 New-AzureSqlDatabase (禁止显示输出)
        $db = New-AzureSqlDatabase @param
    }

    Write-VerboseWithTime 'Add-AzureSQLDatabase: 结束'
    return $db
}

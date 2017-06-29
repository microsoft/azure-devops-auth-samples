#  AzureWebAppPublishModule.psm1 is a Windows PowerShell script module. This module exports Windows PowerShell functions that automate life cycle management for web applications. You can use the functions as is or customize them for your application and publishing environment.





Set-StrictMode -Version 3

# A variable to save original subscription.
$Script:originalCurrentSubscription = $null

# A variable to save original storage account.
$Script:originalCurrentStorageAccount = $null

# A variable to save storage account of user specified subscription.
$Script:originalStorageAccountOfUserSpecifiedSubscription = $null

# A variable to save subscription name.
$Script:userSpecifiedSubscription = $null

# Web deploy port number
New-Variable -Name WebDeployPort -Value 8172 -Option Constant

<#
.SYNOPSIS
Prepends the date and time to a message.

.DESCRIPTION
Prepends the date and time to a message. This function is designed for messages written to the Error and Verbose streams.

.PARAMETER  Message
Specifies the messages without the date.

.INPUTS
System.String

.OUTPUTS
System.String

.EXAMPLE
PS C:\> Format-DevTestMessageWithTime -Message "Adding file $filename to the directory"
2/5/2014 1:03:08 PM - Adding file $filename to the directory

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
Writes an error message prefixed with the current time.

.DESCRIPTION
Writes an error message prefixed with the current time. This function calls the Format-DevTestMessageWithTime function to prepend the time before writing the message to the Error stream.

.PARAMETER  Message
Specifies the message in the error message call. You can pipe the message string to the function.

.INPUTS
System.String

.OUTPUTS
None. The function writes to the Error stream.

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
Writes a verbose message prefixed with the current time.

.DESCRIPTION
Writes a verbose message prefixed with the current time. Because it calls Write-Verbose, the message displays only when the script runs with the Verbose parameter or when the VerbosePreference preference is set to Continue.

.PARAMETER  Message
Specifies the message in the verbose message call. You can pipe the message string to the function.

.INPUTS
System.String

.OUTPUTS
None. The function writes to the Verbose stream.

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
Writes a host message prefixed with the current time.

.DESCRIPTION
This function writes a message to the host program (Write-Host) prefixed with the current time. The effect of writing to the host program varies. Most programs that host Windows PowerShell write these messages to standard output.

.PARAMETER  Message
Specifies the base message without the date. You can pipe the message string to the function.

.INPUTS
System.String

.OUTPUTS
None. The function writes the message to the host program.

.EXAMPLE
PS C:> Write-HostWithTime -Message "The operation succeeded."
1/27/2014 11:02:37 AM - The operation succeeded.

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
Returns $true if a property or method is a member of the object. Otherwise, $false.

.DESCRIPTION
Returns $true if the property or method is a member of the object. This function returns $false for static methods of the class and for views, such as PSBase and PSObject.

.PARAMETER  Object
Specifies the object in the test. Enter a variable that contains an object or an expression that returns an object. You cannot specify types, such as [DateTime] or pipe objects to this function.

.PARAMETER  Member
Specifies the name of the property or method in the test. When specifying a method, omit parentheses that follow the method name.

.INPUTS
None. This function does not take input from the pipeline.

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
Returns $true if the version of the Azure module is 0.7.4 or later. Else, $false.

.DESCRIPTION
Test-AzureModuleVersion returns $true if the version of the Azure module is 0.7.4 or later. It returns $false if the module isn't installed or is an earlier version. This function has no parameters.

.INPUTS
None

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
Returns $true if the installed Azure module version is 0.7.4 or later.

.DESCRIPTION
Test-AzureModule returns $true if the installed Azure module version is 0.7.4 or later. Returns $false if the module isn't installed or is an earlier version. This function has no parameters.

.INPUTS
None

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
Saves the current Windows Azure subscription in the $Script:originalSubscription variable in script scope.

.DESCRIPTION
The Backup-Subscription function saves the current Windows Azure subscription (Get-AzureSubscription -Current) and its storage account, and the subscription that is changed by this script ($UserSpecifiedSubscription) and its storage account, in script scope. By saving the values, you can use a function, such as Restore-Subscription, to restore the original current subscription and storage account to current status if the current status has changed.

.PARAMETER UserSpecifiedSubscription
Specifies the name of the subscription in which the new resources will be created and published. The function saves the names of the subscription and its storage accounts in script scope. This parameter is required.

.INPUTS
None

.OUTPUTS
None

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

    Write-VerboseWithTime 'Backup-Subscription: Start'

    $Script:originalCurrentSubscription = Get-AzureSubscription -Current -ErrorAction SilentlyContinue
    if ($Script:originalCurrentSubscription)
    {
        Write-VerboseWithTime ('Backup-Subscription: Original subscription is ' + $Script:originalCurrentSubscription.SubscriptionName)
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

    Write-VerboseWithTime 'Backup-Subscription: End'
}


<#
.SYNOPSIS
Restores to "current" status the Windows Azure subscription that is saved in the $Script:originalSubscription variable in script scope.

.DESCRIPTION
The Restore-Subscription function makes the subscription that is saved in the $Script:originalSubscription variable the current subscription (again). If the original subscription has a storage account, this function makes that storage account current for the current subscription.  The function restores the subscription only if there is a non-null $SubscriptionName variable in the environment. Otherwise, it exits.  If the $SubscriptionName is populated, but $Script:originalSubscription is $null, Restore-Subscription uses the Select-AzureSubscription cmdlet to clear the Current and Default settings for subscriptions in Windows Azure PowerShell.  This function doesn't have parameters, it takes no input, and it returns nothing (void). You can use -Verbose to write messages to the Verbose stream.

.INPUTS
None

.OUTPUTS
None

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

    Write-VerboseWithTime 'Restore-Subscription: Start'

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

    Write-VerboseWithTime 'Restore-Subscription: End'
}

<#
.SYNOPSIS
Finds a Windows Azure storage account named "devtest*" in the current subscription.

.DESCRIPTION
The Get-AzureVMStorage function returns the name of the first storage account with the name pattern "devtest*" (case insensitive) in the specified location or affinity group. If the "devtest*" storage account does not match the location or affinity group, the function ignores it. You must specify either a location or an affinity group.

.PARAMETER  Location
Specifies the location of the storage account. Valid values are the Windows Azure locations, such as "West US". You can enter a location or an affinity group, but not both.

.PARAMETER  AffinityGroup
Specifies the affinity group of the storage account. You can enter a location or an affinity group, but not both.

.INPUTS
None. You cannot pipe input to this function.

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

    Write-VerboseWithTime 'Get-AzureVMStorage: Start'

    $storages = @(Get-AzureStorageAccount -ErrorAction SilentlyContinue)
    $storageName = $null

    foreach ($storage in $storages)
    {
        # Get the first storage account whose name begins with "devtest"
        if ($storage.Label -like 'devtest*')
        {
            if ($storage.AffinityGroup -eq $AffinityGroup -or $storage.Location -eq $Location)
            {
                $storageName = $storage.Label

                    Write-HostWithTime ('Get-AzureVMStorage: Found devtest storage account ' + $storageName)
                    $storage | Out-String | Write-VerboseWithTime
                break
            }
        }
    }

    Write-VerboseWithTime 'Get-AzureVMStorage: End'
    return $storageName
}


<#
.SYNOPSIS
Creates a new Windows Azure storage account with a unique name that begins with "devtest".

.DESCRIPTION
The Add-AzureVMStorage function creates a new Windows Azure storage account in the current subscription. The name of the account begins with "devtest" followed by a unique alphanumeric string. The function returns the name of the new storage account. You must specify either a location or an affinity group for the new storage account.

.PARAMETER  Location
Specifies the location of the storage account. Valid values are the Windows Azure locations, such as "West US". You can enter a location or an affinity group, but not both.

.PARAMETER  AffinityGroup
Specifies the affinity group of the storage account. You can enter a location or an affinity group, but not both.

.INPUTS
None. You cannot pipe input to this function.

.OUTPUTS
System.String. The string is the name of the new storage account

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

    Write-VerboseWithTime 'Add-AzureVMStorage: Start'

    # Create a unique name by appending part of a GUID to "devtest"
    $name = 'devtest'
    $suffix = [guid]::NewGuid().ToString('N').Substring(0,24 - $name.Length)
    $name = $name + $suffix

    # Create a new Windows Azure storage account with location/affinity group
    if ($PSCmdlet.ParameterSetName -eq 'Location')
    {
        New-AzureStorageAccount -StorageAccountName $name -Location $Location | Out-Null
    }
    else
    {
        New-AzureStorageAccount -StorageAccountName $name -AffinityGroup $AffinityGroup | Out-Null
    }

    Write-HostWithTime ("Add-AzureVMStorage:  Created new storage acccount $name")
    Write-VerboseWithTime 'Add-AzureVMStorage: End'
    return $name
}


<#
.SYNOPSIS
Validates the config file and returns a hashtable of config file values.

.DESCRIPTION
The Read-ConfigFile function validates the JSON configuration file and returns a hash table of selected values.
-- It begins by converting the JSON file to a PSCustomObject.
-- It verifies that the environmentSettings property contains either a web site or cloud service property, but not both.
-- Creates and returns one of two types of hash tables; one for a web site; one for a cloud service. The web site hash table has the following keys:
-- IsAzureWebSite: $True. The config file is for a web site. 
-- Name: Web site name
-- Location: Web site location
-- Databases: Web site SQL databases
The cloud service hash table has the following keys:
-- IsAzureWebSite: $False. The config file is not for a website.
-- webdeployparameters : Optional. Might be $null or empty.
-- Databases: SQL databases

.PARAMETER  ConfigurationFile
Specifies the path and name of the JSON configuration file for your web project. Visual Studio generates the JSON file automatically when you create a web project and stores it in the PublishScripts folder in your solution.

.PARAMETER HasWebDeployPackage
Indicates that there is a web deploy package ZIP file for the web application. To specify a value of $true, use -HasWebDeployPackage or HasWebDeployPackage:$true. To specify a value of false, use HasWebDeployPackage:$false.This parameter is required.

.INPUTS
None. You cannot pipe input to this function.

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

    Write-VerboseWithTime 'Read-ConfigFile: Start'

    # Get the contents of the JSON file (-raw ignores line breaks) and convert it to a PSCustomObject
    $config = Get-Content $ConfigurationFile -Raw | ConvertFrom-Json

    if (!$config)
    {
        throw ('Read-ConfigFile: ConvertFrom-Json failed: ' + $error[0])
    }

    # Determine whether the environmentSettings object has 'webSite' or 'cloudService' properties (regardless of the property value)
    $hasWebsiteProperty =  Test-Member -Object $config.environmentSettings -Member 'webSite'
    $hasCloudServiceProperty = Test-Member -Object $config.environmentSettings -Member 'cloudService'

    if (!$hasWebsiteProperty -and !$hasCloudServiceProperty)
    {
        throw 'Read-ConfigFile: Malformed configuration file. Does not have webSite or cloudService'
    }
    elseif ($hasWebsiteProperty -and $hasCloudServiceProperty)
    {
        throw 'Read-ConfigFile: Malformed configuration file. Has both webSite and cloudService'
    }

    # Build a hash table from the values in the PSCustomObject
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

    Write-VerboseWithTime 'Read-ConfigFile: End'

    return $returnObject
}

<#
.SYNOPSIS
Adds new input endpoints to a virtual machine and returns the virtual machine with the new endpoint.

.DESCRIPTION
The Add-AzureVMEndpoints function adds new input endpoints to a virtual machine and returns the virtual machine with the new endpoints. This function calls the Add-AzureEndpoint cmdlet (Azure module).

.PARAMETER  VM
Specifies the virtual machine object. Enter an VM object, such as the type that the New-AzureVM or Get-AzureVM cmdlets return. You can pipe objects from Get-AzureVM to Add-AzureVMEndpoints.

.PARAMETER  Endpoints
Specifies an array of endpoints to add to the VM. Typically, these endpoints originate in the JSON configuration file that Visual Studio generates for web projects. Use the Read-ConfigFile function in this module to convert the file to a hash table. The endpoints are a property of the cloudservice key of the hash table ($<hashtable>.cloudservice.virtualmachine.endpoints). For example,
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

    Write-VerboseWithTime 'Add-AzureVMEndpoints: Start'

    # Add each endpoint from the JSON file to the VM
    $Endpoints | ForEach-Object `
    {
        $_ | Out-String | Write-VerboseWithTime
        Add-AzureEndpoint -VM $VM -Name $_.name -Protocol $_.protocol -LocalPort $_.privateport -PublicPort $_.publicport | Out-Null
    }

    Write-VerboseWithTime 'Add-AzureVMEndpoints: End'
    return $VM
}

<#
.SYNOPSIS
Creates all elements of a new virtual machine in a Windows Azure subscription.

.DESCRIPTION
This function creates a Windows Azure virtual machine (VM) and returns the URL of the deployed VM. The function sets up the prerequisites and then calls the New-AzureVM cmdlet (Azure module) to create a new VM. 
-- It calls the New-AzureVMConfig cmdlet (Azure module) to get a virtual machine configuration object. 
-- If you include the Subnet parameter to add the VM to an Azure subnet, it calls Set-AzureSubnet to set the subnet list for the VM. 
-- It calls the Add-AzureProvisioningConfig (Azure module) to add elements to the VM configuration. It creates a standalone Windows provisioning configuration (-Windows) with an admin account and password. 
-- It calls the Add-AzureVMEndpoints function in this module to add the endpoints specified by the Endpoints parameter. This function takes a VM object and returns a VM object with the added endpoints. 
-- It calls the Add-AzureVM cmdlet to create a new Windows Azure virtual machine and returns the new VM. The values of the function parameters are typically taken from the JSON configuration file that Visual Studio generates for Windows Azure-integrated web projects. The Read-ConfigFile function in this module converts the JSON file into a hash table. Save the cloudservice key of the hash table in a variable (as a PSCustomObject), and uses the properties of the custom object as parameter values.

.PARAMETER  UserName
Specifies an admin user name. This is submitted as the value of the AdminUserName parameter of Add-AzureProvisioningConfig. This parameter is required.

.PARAMETER  UserPassword
Specifies a password for the admin user account. This is submitted as the value of the Password parameter of Add-AzureProvisioningConfig. This parameter is required.

.PARAMETER  VMName
Specifies a name for the new VM. The VM name must be unique within the cloud service. This parameter is required.

.PARAMETER  VMSize
Specifies the size of the VM. Valid values are "ExtraSmall", "Small", "Medium", "Large", "ExtraLarge", "A5", "A6", and "A7". This value is submitted as the value of the InstanceSize parameter of New-AzureVMConfig. This parameter is required. 

.PARAMETER  ServiceName
Specifies an existing Windows Azure service or a name of a new Windows Azure service. This value is submitted to the ServiceName parameter of the New-AzureVM cmdlet, which adds the new virtual machine to an exiting Windows Azure service, or, if Location or AffinityGroup is specified, creates a new virtual machine and service in the current subscription. This parameter is required. 

.PARAMETER  ImageName
Specifies the name of the virtual machine image to use for the operating system disk. This parameter is submitted as the value of the ImageName parameter of the New-AzureVMConfig cmdlet. This parameter is required. 

.PARAMETER  Endpoints
Specifies an array of endpoints to add to the VM. This value is submitted to the Add-AzureVMEndpoints function that this module exports. This parameter is optional. Typically, these endpoints originate in the JSON configuration file that Visual Studio generates for web projects. Use the Read-ConfigFile function in this module to convert the file to a hash table. The endpoints are a property of the cloudService key of the hash table ($<hashtable>.cloudservice.virtualmachine.endpoints). 

.PARAMETER  AvailabilitySetName
Specifies the name of an availability set for the new VM. When you place multiple virtual machines in an availability set, Windows Azure tries to keep those virtual machines on separate hosts to improve continuity of service should one fail. This parameter is optional. 

.PARAMETER  VNetName
Specifies the name of the virtual network name where the new virtual machine is deployed. This value is submitted to the VNetName parameter of the Add-AzureVM cmdlet. This parameter is optional. 

.PARAMETER  Location
Specifies a location for the new VM. Valid values are the Windows Azure locations, such as "West US". The default is the location of the subscription. This parameter is optional. 

.PARAMETER  AffinityGroup
Specifies an affinity group for the new VM. An affinity group is a group of related resources. When you specify an affinity group, Windows Azure tries to keep the resources in the group together to improve efficiency. 

.PARAMETER EnableWebDeployExtension
Prepares the VM to for deployment. Prepares the VM to for deployment. This parameter is optional. If it is not specified, the VM is created, but it is not deployed. The value of this parameter is included in the JSON configuration file that Visual Studio generates for cloud services. The value of this parameter is included in the JSON configuration file that Visual Studio generates for cloud services.

.PARAMETER  Subnet
Specifies the subnet of the new VM configuration. This value is submitted to the Set-AzureSubnet cmdlet (Azure module) which takes a VM and an array of subnet names and returns a VM with the subnets in its configuration.

.INPUTS
None. This function does not take input from the pipeline.

.OUTPUTS
System.Url

.EXAMPLE
 This command calls the Add-AzureVM function. Many of the parameter values come from a $CloudServiceConfiguration object. This PSCustomObject is the cloudservice key and values of the hash table that the Read-ConfigFile function returns. The source is the JSON configuration file that Visual Studio generates for web projects.

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

    Write-VerboseWithTime 'Add-AzureVM: Start'

    # Create a new Windows Azure VM configuration object.
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
        throw 'Add-AzureVM: Failed to create Azure VM config.'
    }

    if ($Subnet)
    {
        # Set the subnet list for a virtual machine configuration.
        $subnetResult = Set-AzureSubnet -VM $vm -SubnetNames $Subnet

        if (!$subnetResult)
        {
            throw ('Add-AzureVM: Failed to set subnet ' + $Subnet)
        }
    }

    # Add configuration data to the VM configuration
    $VMWithConfig = Add-AzureProvisioningConfig -VM $vm -Windows -Password $UserPassword -AdminUserName $UserName

    if (!$VMWithConfig)
    {
        throw ('Add-AzureVM: Failed to create provisioning config.')
    }

    # Add input endpoints to the VM
    if ($Endpoints -and $Endpoints.Count -gt 0)
    {
        $VMWithConfig = Add-AzureVMEndpoints -Endpoints $Endpoints -VM $VMWithConfig
    }

    if (!$VMWithConfig)
    {
        throw ('Add-AzureVM: Failed to create endpoints.')
    }

    if ($EnableWebDeployExtension)
    {
        Write-VerboseWithTime 'Add-AzureVM: Add webdeploy extension'

        Write-VerboseWithTime 'To view the WebDeploy license, see http://go.microsoft.com/fwlink/?LinkID=389744 '

        $VMWithConfig = Set-AzureVMExtension `
            -VM $VMWithConfig `
            -ExtensionName WebDeployForVSDevTest `
            -Publisher 'Microsoft.VisualStudio.WindowsAzure.DevTest' `
            -Version '1.*' 

        if (!$VMWithConfig)
        {
            throw ('Add-AzureVM: Failed to add webdeploy extension.')
        }
    }

    # Create a hash table of parameters for splatting
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

    Write-HostWithTime ('Add-AzureVM: Created Virtual machine ' + $VMName)

    $url = [System.Uri](Get-AzureDeployment -ServiceName $ServiceName).Url

    if (!$url)
    {
        throw 'Add-AzureVM: Can not find VM Url.'
    }

    Write-HostWithTime ('Add-AzureVM: Publish Url https://' + $url.Host + ':' + $WebDeployPort + '/msdeploy.axd')

    Write-VerboseWithTime 'Add-AzureVM: End'

    return $url.AbsoluteUri
}


<#
.SYNOPSIS
Gets the specified Windows Azure virtual machine.

.DESCRIPTION
The Find-AzureVM function gets a Windows Azure virtual machine (VM) based on the service name and VM name. This function calls the Test-AzureName cmdlet (Azure module) to verify that the service name exists in Windows Azure. If it does, the function calls the Get-AzureVM cmdlet to get the VM. This function returns a hash table with vm and foundService keys.
-- FoundService: $True if Test-AzureName found the service. Otherwise, $False
-- VM: Contains the VM object when FoundService is true and Get-AzureVM return the VM object.

.PARAMETER  ServiceName
The name of an existing Windows Azure service. This parameter is required.

.PARAMETER  VMName
The name of a virtual machine in the service. This parameter is required.

.INPUTS
None. You cannot pipe input to this function.

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

    Write-VerboseWithTime 'Find-AzureVM: Start'
    $foundService = $false
    $vm = $null

    if (Test-AzureName -Service -Name $ServiceName)
    {
        $foundService = $true
        $vm = Get-AzureVM -ServiceName $ServiceName -Name $VMName
        if ($vm)
        {
            Write-HostWithTime ('Find-AzureVM: Found existing virtual machine ' + $vm.Name )
            $vm | Out-String | Write-VerboseWithTime
        }
    }

    Write-VerboseWithTime 'Find-AzureVM: End'
    return @{ VM = $vm; FoundService = $foundService }
}


<#
.SYNOPSIS
Finds or creates a virtual machine in the subscription that matches the values in the JSON configuration file.

.DESCRIPTION
The New-AzureVMEnvironment function finds or creates a virtual machine in the subscription that matches the values in the JSON configuration file that Visual Studio generates for web projects. It takes a PSCustomObject that is the cloudservice key of the hash table the Read-ConfigFile returns. This data originates in the JSON configuration file that Visual Studio generates. The function looks for a virtual machine (VM) in the subscription with a service name and virtual machine name that matches the values in the CloudServiceConfiguration custom object. If it cannot find a matching VM, it calls the Add-AzureVM function in this module and uses the values in the CloudServiceConfiguration object to create a VM. The virtual machine environment includes a storage account that has a name that begins with "devtest". If the function cannot find a storage account with that name pattern in the subscription, it creates one. The function returns a hashtable with VMUrl, userName, and Password keys and string values.

.PARAMETER  CloudServiceConfiguration
Takes a PSCustomObject that contains the cloudservice property of the hash table that the Read-ConfigFile function returns. All values originate in the JSON configuration file that Visual Studio generates for web projects. You can find this file in the PublishScripts folder in your solution. This parameter is required.
$config = Read-ConfigFile -ConfigurationFile <file>.json $cloudServiceConfiguration = $config.cloudService

.PARAMETER  VMPassword
Takes a hash table with name and password keys, such as: @{Name = "admin"; Password = "pa$$word"} This parameter is optional. If you omit it, the default values are the virtual machine user name and password in the JSON configuration file.

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

    Write-VerboseWithTime ('New-AzureVMEnvironment: Start')

    if ($CloudServiceConfiguration.location -and $CloudServiceConfiguration.affinityGroup)
    {
        throw 'New-AzureVMEnvironment: Malformed configuration file. Has both location and affinityGroup'
    }

    if (!$CloudServiceConfiguration.location -and !$CloudServiceConfiguration.affinityGroup)
    {
        throw 'New-AzureVMEnvironment: Malformed configuration file. Does not have location or affinityGroup'
    }

    # If the CloudServiceConfiguration object has the 'name' property (for service name) and the 'name' property has a value, use it. Otherwise, use the virtual machine name in the CloudServiceConfiguration object, which is always populated.
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

    # Get the VM name from the JSON file
    $findAzureVMResult = Find-AzureVM -ServiceName $serviceName -VMName $CloudServiceConfiguration.virtualMachine.name

    # If we can't find a VM with that name in that cloud service, create one.
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
            throw 'New-AzureVMEnvironment: Failed to get current Azure subscription.'
        }

        # Set the devtest* storage account to current
        Set-AzureSubscription `
            -SubscriptionName $currentSubscription.SubscriptionName `
            -CurrentStorageAccountName $storageAccountName

        Write-VerboseWithTime ('New-AzureVMEnvironment: Storage account is set to ' + $storageAccountName)

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

        # Create a VM with the values from the JSON file + parameter values
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

        Write-VerboseWithTime ('New-AzureVMEnvironment: End')

        return @{ 
            VMUrl = $VMUrl; 
            UserName = $userName; 
            Password = $userPassword; 
            IsNewCreatedVM = $true; }
    }
    else
    {
        Write-VerboseWithTime ('New-AzureVMEnvironment: Found an existing virtual machine ' + $findAzureVMResult.VM.Name)
    }

    Write-VerboseWithTime ('New-AzureVMEnvironment: End')

    return @{ 
        VMUrl = $findAzureVMResult.VM.DNSName; 
        UserName = $userName; 
        Password = $userPassword; 
        IsNewCreatedVM = $false; }
}


<#
.SYNOPSIS
Returns a command to run the MsDeploy.exe tool

.DESCRIPTION
The Get-MSDeployCmd function assembles and returns a valid command to run Web Deploy Tool, MSDeploy.exe. It finds the correct path to the tool on the local computer in a registry key. This function has no parameters.

.INPUTS
None

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
    Write-VerboseWithTime 'Get-MSDeployCmd: Start'
    $regKey = 'HKLM:\SOFTWARE\Microsoft\IIS Extensions\MSDeploy'

    if (!(Test-Path $regKey))
    {
        throw ('Get-MSDeployCmd: Can not find ' + $regKey)
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

    Write-VerboseWithTime 'Get-MSDeployCmd: End'
    return $msdeployPath
}


<#
.SYNOPSIS
Creates a Windows Azure web site.

.DESCRIPTION
Creates a Windows Azure web site with the specific name and location. This function calls the New-AzureWebsite cmdlet in the Azure module. If the subscription does not yet have a web site with the specified name, this function creates the web site and returns a web site object. Otherwise, it returns $null.

.PARAMETER  Name
Specifies a name for the new web site. The name must be unique in Windows Azure. This parameter is required.

.PARAMETER  Location
Specifies the location of the web site. Valid values are the Windows Azure locations, such as "West US". This parameter is required.

.INPUTS
NONE.

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

    Write-VerboseWithTime 'Add-AzureWebsite: Start'
    $website = Get-AzureWebsite -Name $Name -ErrorAction SilentlyContinue

    if ($website)
    {
        Write-HostWithTime ('Add-AzureWebsite: An existing web site ' +
        $website.Name + ' was found')
    }
    else
    {
        if (Test-AzureName -Website -Name $Name)
        {
            Write-ErrorWithTime ('Website {0} already exists' -f $Name)
        }
        else
        {
            $website = New-AzureWebsite -Name $Name -Location $Location
        }
    }

    $website | Out-String | Write-VerboseWithTime
    Write-VerboseWithTime 'Add-AzureWebsite: End'

    return $website
}

<#
.SYNOPSIS
Returns $True when the URL is absolute and its scheme is https.

.DESCRIPTION
The Test-HttpsUrl function converts the input URL to a System.Uri object. Returns $True when the URL is absolute (not relative) and its scheme is https. If either is false, or the input string cannot be converted to a URL, the function returns $false.

.PARAMETER Url
Specifies the URL to test. Enter a URL string,

.INPUTS
NONE.

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

    # If $uri cannot be converted to a System.Uri object, Test-HttpsUrl returns $false
    $uri = $Url -as [System.Uri]

    return $uri.IsAbsoluteUri -and $uri.Scheme -eq 'https'
}


<#
.SYNOPSIS
Deploys a web package to Windows Azure.

.DESCRIPTION
The Publish-WebPackage function uses MsDeploy.exe and a web deployment package ZIP file to deploy resources to a Windows Azure web site. This function does not generate any output. If the call to MSDeploy.exe fails, the function throws an exception. To get more detailed output, use the Verbose common parameter.

.PARAMETER  WebDeployPackage
Specifies the path and file name of a web deployment package ZIP file that Visual Studio generates. This parameter is required. To create a web deployment package ZIP file, see "How to: Create a Web Deployment Package in Visual Studio" at: http://go.microsoft.com/fwlink/?LinkId=391353.

.PARAMETER PublishUrl
Specifies the URL to which the resources are deployed. The URL must use HTTPS protocol and include the port. This parameter is required.

.PARAMETER SiteName
Specifies a name for the web site. This parameter is required.

.PARAMETER Username
Specifies the user name of the web site administrator. This parameter is required.

.PARAMETER Password
Specifies a password for the web site administrator. Enter a password in plain text. Secure strings are not permitted. This parameter is required.

.PARAMETER AllowUntrusted
Allows untrusted SSL connections to the site. This parameter is used in the call to MSDeploy.exe. This parameter is required.

.PARAMETER ConnectionString
Specifies a connection string for a SQL database. This parameter takes a hash table with Name and ConnectionString keys. The value of Name is the name of the database. The value of ConnectionString is the connectionStringName in the JSON configuration file.

.INPUTS
None. This function does not take input from the pipeline.

.OUTPUTS
None

.EXAMPLE
Publish-WebPackage -WebDeployPackage C:\Documents\Azure\ADWebApp.zip `
    -PublishUrl $publishUrl "https://contoso.cloudnet.net:8172/msdeploy.axd" `
    -SiteName 'Contoso Test Site' `
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

    Write-VerboseWithTime 'Publish-WebPackage: Start'

    $msdeployCmd = Get-MSDeployCmd

    if (!$msdeployCmd)
    {
        throw 'Publish-WebPackage: MsDeploy.exe cannot be found.'
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
         Write-VerboseWithTime ('Msdeploy.exe exited with error. ExitCode:' + $msdeployExecution.ExitCode)
    }

    Write-VerboseWithTime 'Publish-WebPackage: End'
    return ($msdeployExecution.ExitCode -eq 0)
}


<#
.SYNOPSIS
Deploys a virtual machine to Windows Azure.

.DESCRIPTION
The Publish-WebPackageToVM function is a helper function for that verifies the parameter values and then calls the Publish-WebPackage function.

.PARAMETER  VMDnsName
Specifies the DNS name of the Windows Azure virtual machine. This parameter is required.

.PARAMETER IisWebApplicationName
Specifies the name of an IIS web application for the the virtual machine. This parameter is required. This is the name of your Visual Studio web app. You can find the name in the webDeployparameters attribute of the JSON configuration file that Visual Studio generates.

.PARAMETER WebDeployPackage
Specifies the path and file name of a web deployment package ZIP file that Visual Studio generates. This parameter is required. To create a web deployment package ZIP file, see "How to: Create a Web Deployment Package in Visual Studio" at: http://go.microsoft.com/fwlink/?LinkId=391353.

.PARAMETER Username
Specifies the user name of the virtual machine administrator. This parameter is required.

.PARAMETER Password
Specifies a password for the virtual machine administrator. Enter a password in plain text. Secure strings are not permitted. This parameter is required.

.PARAMETER AllowUntrusted
Allows untrusted SSL connections to the site. This parameter is used in the call to MSDeploy.exe. This parameter is required.

.PARAMETER ConnectionString
Specifies a connection string for a SQL database. This parameter takes a hash table with Name and ConnectionString keys. The value of Name is the name of the database. The value of ConnectionString is the connectionStringName in the JSON configuration file.

.INPUTS
None. This function does not take input from the pipeline.

.OUTPUTS
None.

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
    Write-VerboseWithTime 'Publish-WebPackageToVM: Start'

    $VMDnsUrl = $VMDnsName -as [System.Uri]

    if (!$VMDnsUrl)
    {
        throw ('Publish-WebPackageToVM: Invalid url ' + $VMDnsUrl)
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

    Write-VerboseWithTime 'Publish-WebPackageToVM: End'
    return $result
}


<#
.SYNOPSIS
Creates a string that lets you connect to a Windows Azure SQL database.

.DESCRIPTION
The Get-AzureSQLDatabaseConnectionString function assembles a connection string to connect to a Windows Azure SQL database.

.PARAMETER  DatabaseServerName
Specifies the name of an existing database server in the Windows Azure subscription. All Windows Azure SQL databases must be associated with a SQL database server. To get the server name, use the Get-AzureSqlDatabaseServer cmdlet (Azure module). This parameter is required.

.PARAMETER  DatabaseName
Specifies the name for the SQL database. This can be an existing SQL database or a name used for a new SQL database. This parameter is required.

.PARAMETER  Username
Specifies the name of the SQL database administrator. The username will be $Username@DatabaseServerName. This parameter is required.

.PARAMETER  Password
Specifies a password for the SQL database administrator. Enter a password in plain text. Secure strings are not permitted. This parameter is required.

.INPUTS
None.

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
Creates Windows Azure SQL databases from the values in the JSON configuation file that Visual Studio generates.

.DESCRIPTION
The Add-AzureSQLDatabases function takes information from the databases section of the JSON file. This function, Add-AzureSQLDatabases (plural), calls the Add-AzureSQLDatabase (singular) function for each SQL database in the JSON file. Add-AzureSQLDatabase (singular) calls the New-AzureSqlDatabase cmdlet (Azure module), which creates the SQL databases. This function does not return a database object. It returns a hashtable of values that were used to create the databases.

.PARAMETER DatabaseConfig
 Takes an array of PSCustomObjects that originate in the JSON file that the Read-ConfigFile function returns when the JSON file has a web site property. It includes the environmentSettings.databases properties. You can pipe the list to this function.
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
Specifies the password for the SQL database server administrator. Enter a hashtable with Name and Password keys. The value of Name is the name of the SQL database server. The value of Password is the administrator password. For example: @Name = "TestDB1"; Password = "pa$$word" This parameter is optional. If you omit it or the SQL database server name doesn't match the value of the serverName property of the $DatabaseConfig object, the function uses the Password property of the $DatabaseConfig object for the SQL database in the connection string.

.PARAMETER CreateDatabase
Verifies that you want to create a database. This parameter is optional.

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
        Write-VerboseWithTime 'Add-AzureSQLDatabases: Start'
    }
    process
    {
        Write-VerboseWithTime ('Add-AzureSQLDatabases: Creating ' + $DatabaseConfig.databaseName)

        if ($CreateDatabase)
        {
            # Creates a new SQL database with the DatabaseConfig values (unless one already exists)
            # The command output is suppressed.
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
        Write-VerboseWithTime 'Add-AzureSQLDatabases: End'
    }
}


<#
.SYNOPSIS
Creates a new Windows Azure SQL database.

.DESCRIPTION
The Add-AzureSQLDatabase function creates a Windows Azure SQL database from the data in the JSON configuration file that Visual Studio generates and returns the new database. If the subscription already has a SQL database with the specified database name in the specified SQL database server, the function returns the existing database. This function calls the New-AzureSqlDatabase cmdlet (Azure module), which actually creates the SQL database.

.PARAMETER DatabaseConfig
Takes a PSCustomObject that originates in the JSON configuration file that the Read-ConfigFile function returns when the JSON file has a web site property. It includes the environmentSettings.databases properties. You cannot pipe the object to this function. Visual Studio generates a JSON configuration file for all web projects and stores it in the PublishScripts folder of your solution.

.INPUTS
None. This function does not take input from the pipeline

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

    Write-VerboseWithTime 'Add-AzureSQLDatabase: Start'

    # Fail if the parameter value doesn't have the serverName property, or the serverName property value isn't populated.
    if (-not (Test-Member $DatabaseConfig 'serverName') -or -not $DatabaseConfig.serverName)
    {
        throw 'Add-AzureSQLDatabase: The database serverName (required) is missing from the DatabaseConfig value.'
    }

    # Fail if the parameter value doesn't have the databasename property, or the databasename property value isn't populated.
    if (-not (Test-Member $DatabaseConfig 'databaseName') -or -not $DatabaseConfig.databaseName)
    {
        throw 'Add-AzureSQLDatabase: The databasename (required) is missing from the DatabaseConfig value.'
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
        Write-HostWithTime ('Create-AzureSQLDatabase: Using existing database ' + $db.Name)
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

        # If the $DatabaseConfig object has a collation property and it's not null or empty
        if ((Test-Member $DatabaseConfig 'collation') -and $DatabaseConfig.collation)
        {
            $param.Add('Collation', $DatabaseConfig.collation)
        }

        # If the $DatabaseConfig object has an edition property and it's not null or empty
        if ((Test-Member $DatabaseConfig 'edition') -and $DatabaseConfig.edition)
        {
            $param.Add('Edition', $DatabaseConfig.edition)
        }

        # Write the hash table to the Verbose stream
        $param | Out-String | Write-VerboseWithTime
        # Call New-AzureSqlDatabase with splatting (suppress the output)
        $db = New-AzureSqlDatabase @param
    }

    Write-VerboseWithTime 'Add-AzureSQLDatabase: End'
    return $db
}

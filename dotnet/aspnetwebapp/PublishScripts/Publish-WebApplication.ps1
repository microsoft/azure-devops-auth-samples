#Requires -Version 3.0

<#
.SYNOPSIS
Creates and deploys Windows Azure Web Sites, Virtual Machines, SQL Databases, and storage accounts for a Visual Studio web project.

.DESCRIPTION
The Publish-WebApplication.ps1 script creates the Windows Azure resources that you specify in a Visual Studio web project and (optionally) deploys them for you. It can create Windows Azure Web Sites, Virtual Machines, SQL Databases, and storage accounts.

To manage the entire application lifecycle of your web application in this script, implement the placeholder functions New-WebDeployPackage and Test-WebApplication.

If you specify the WebDeployPackage parameter with a valid web deploy package ZIP file, Publish-WebApplication.ps1 also deploys your the web pages or virtual machines that it creates.

This script requires Windows PowerShell 3.0 or greater and Windows Azure PowerShell version 0.7.4 or greater. For information about installing Windows Azure PowerShell and its Azure module, see http://go.microsoft.com/fwlink/?LinkID=350552. To find the version of your Azure module, type: (Get-Module -Name Azure -ListAvailable).version To find the version of Windows PowerShell,type: $PSVersionTable.PSVersion

Before running this script, run the Add-AzureAccount cmdlet to provide the credentials of your Windows Azure account to Windows PowerShell. Also, if you create SQL databases, you need to have an existing Windows Azure SQL database server. To create a SQL database, use the New-AzureSqlDatabaseServer cmdlet in the Azure module.

Also, if you have never run a script, use the Set-ExecutionPolicy cmdlet to an execution policy that allows you to run scripts. To run this cmdlet, start Windows PowerShell with the 'Run as administrator' option.

This Publish-WebApplication.ps1 script uses the JSON configuration file that Visual Studio generates when you create your web project. You can find the JSON file in the PublishScripts folder in your Visual Studio solution.

You can delete or edit the 'databases' object in your JSON configuration file. Do not delete the 'website' or 'cloudservice' objects or their attributes. However, you can delete the entire 'databases' object or delete the attributes that represent a database. To create a SQL database, but not deploy it, delete the "connectionStringName" attribute or its value.

It also uses functions in the AzureWebAppPublishModule.psm1 Windows PowerShell script module to create the resources in your Windows Azure subscription. You can find a copy of this script module in the PublishScripts folder in your Visual Studio solution.

You can use the Publish-WebApplication.ps1 script as is, or edit it to meet your needs. You can also use the functions in the AzureWebAppPublishModule.psm1 module independent of the script and edit them. For example, you can use the Invoke-AzureWebRequest function to call any REST API in the Windows Azure web service.

Once you have a script that creates the Windows Azure resources that you need, you can use it repeatedly to create environments and resources in Windows Azure.

For updates to this script, go to http://go.microsoft.com/fwlink/?LinkId=391217 .
To add support to build your Web Application project, please refer to the MSBuild documentation: http://go.microsoft.com/fwlink/?LinkId=391339 
To add support for running unit tests on your Web Application project, please refer to the VSTest.Console documentation: http://go.microsoft.com/fwlink/?LinkId=391340 

View the WebDeploy license terms:  http://go.microsoft.com/fwlink/?LinkID=389744 

.PARAMETER Configuration
Specifies the path and filename of the JSON configuration file that Visual Studio generates. This parameter is required. You can find this file in the PublishScripts folder of your Visual Studio solution. User can customize the JSON configuration files by modifying the attribute values and deleting optional SQL database objects For the script to run correctly, SQL database objects in web site and virtual machine configuration files may be deleted. Web site and cloud service objects and attributes cannot be deleted. If user does not wish to create or apply a SQL database to the connection string during publish, make sure the "connectionStringName" attribute in the SQL database object is empty or to delete the entire SQL database object.

NOTE: This script supports only Windows virtual hard disk (VHD) files for virtual machines. To use a Linux VHD, change the script so that it calls a cmdlet with a Linux parameter, such as New-AzureQuickVM or New-WAPackVM.

.PARAMETER SubscriptionName
Specifies the name of a subscription in your Windows Azure account. This parameter is option. The default is the current subscription (Get-AzureSubscription -Current). If you specify a subscription that is not the current one, the script temporarily changes the specified subscription to current, but restores the current subscription status before the script completes. If the script errors out before it completes, the specified subscription might still be set as current.

.PARAMETER WebDeployPackage
Specifies the path and file name of a web deployment package ZIP file that Visual Studio generates. This parameter is optional.

If you specify a valid web deployment package, this script uses MsDeploy.exe and the web deployment package to deploy the web site.

To create a web deployment package ZIP file, see "How to: Create a Web Deployment Package in Visual Studio" at: http://go.microsoft.com/fwlink/?LinkId=391353 .

For information about MSDeploy.exe, see Web Deploy Command Line Reference at http://go.microsoft.com/fwlink/?LinkId=391354 

.PARAMETER AllowUntrusted
Allows untrusted SSL connections to the Web Deploy endpoint on the virtual machine. This parameter is used in the call to MSDeploy.exe. This parameter is optional. The default value is False. This parameter is effective only you include the WebDeployPackage parameter with a valid ZIP file value. For information about MSDeploy.exe, see "Web Deploy Command Line Reference" at http://go.microsoft.com/fwlink/?LinkId=391354 

.PARAMETER VMPassword
Specifies a user name and password for the administrator of the Windows Azure virtual machine that the script creates. This parameter takes a hash table with Name and Password keys, such as:
@{Name = "admin"; Password = "pa$$word"}

This parameter is optional. If you omit it, the default values are the virtual machine user name and password in the JSON configuration file.

This parameter is effective only when the JSON configuration file is for a cloud service that includes virtual machines.

.PARAMETER DatabaseServerPassword
Sets the password for a Windows Azure SQL database server. This parameter takes an array of hash tables with Name (SQL database server name) and Password keys. Enter one hash table for each database server that your SQL databases use.

This parameter is optional. The default value is the SQL database server password in the JSON configuration file that Visual Studio generates.

This value is effective when the JSON configuration file includes databases and serverName attributes and the Name key in the hash table matches the serverName value.

.INPUTS
None. You cannot pipe parameter values to this script.

.OUTPUTS
None. This script returns no objects. For script status, use the Verbose parameter.

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
    #Write a function to build and package your web application

    #To build your web application, use MsBuild.exe. For help, see MSBuild Command-Line Reference at: http://go.microsoft.com/fwlink/?LinkId=391339
}

function Test-WebApplication
{
    #Edit this function to run unit test on your web application

    #Write a function to run unit tests on your web application, use VSTest.Console.exe. For help, see VSTest.Console Command-Line Reference at http://go.microsoft.com/fwlink/?LinkId=391340
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

    # If the JSON file has a 'webSite' element
    if ($Config.IsAzureWebSite)
    {
        Add-AzureWebsite -Name $Config.name -Location $Config.location | Out-String | Write-HostWithTime
        # Create the SQL databases. The connection string is used for deployment.
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
                Write-VerboseWithTime ($scriptName + ' Publishing to VM succeeded.')
            }
            elseif ($VMInfo.IsNewCreatedVM -and !$Config.cloudService.virtualMachine.enableWebDeployExtension)
            {
                Write-VerboseWithTime ($scriptName + ' You need to set "enableWebDeployExtension" to $true.')
            }
            elseif (!$VMInfo.IsNewCreatedVM)
            {
                Write-VerboseWithTime ($scriptName + ' Exising VM does not support Web Deploy.')
            }
            else
            {
                Write-VerboseWithTime ($scriptName + " Publishing to VM failed. Attempt $($attempts + 1) of $allAttempts.")
                Write-VerboseWithTime ($scriptName + " Publishing to VM will start after $waitingTime seconds.")
                
                Start-Sleep -Seconds $waitingTime
            }
             
             $attempts++
        
             #Try to publish again only for newly created virtual machine that has Web Deploy installed. 
        } While( !$result -and $VMInfo.IsNewCreatedVM -and $attempts -lt $allAttempts -and $Config.cloudService.virtualMachine.enableWebDeployExtension)
        
        if (!$result)
        {                    
            Write-Warning 'Publishing to the virtual machine failed. This can be caused by an untrusted or invalid certificate.  You can specify �AllowUntrusted to accept untrusted or invalid certificates.'
            throw ($scriptName + ' Publishing to VM failed.')
        }
    }
}


# Script main routine
Set-StrictMode -Version 3

# Import the current version of the AzureWebAppPublishModule.psm1 module
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
    
    Write-VerboseWithTime ($scriptName + ' Start')
    
    $Global:ErrorActionPreference = 'Stop'
    Write-VerboseWithTime ('{0} $ErrorActionPreference is set to {1}' -f $scriptName, $ErrorActionPreference)
    
    Write-Debug ('{0}: $PSCmdlet.ParameterSetName = {1}' -f $scriptName, $PSCmdlet.ParameterSetName)

    # Save the current subscription. It will be restored to Current status later in the script
    Backup-Subscription -UserSpecifiedSubscription $SubscriptionName
    
    # Verify that you have the Azure module, Version 0.7.4 or later.
    if (-not (Test-AzureModule))
    {
         throw 'You have an outdated version of Windows Azure PowerShell. To install the latest version, go to http://go.microsoft.com/fwlink/?LinkID=320552 .'
    }
    
    if ($SubscriptionName)
    {

        # If you provided a subscription name, verify that the subscription exists in your account.
        if (!(Get-AzureSubscription -SubscriptionName $SubscriptionName))
        {
            throw ("{0}: Cannot find the subscription name $SubscriptionName" -f $scriptName)

        }

        # Set the specified subscription to current.
        Select-AzureSubscription -SubscriptionName $SubscriptionName | Out-Null

        Write-VerboseWithTime ('{0}: Subscription is set to {1}' -f $scriptName, $SubscriptionName)
    }

    $Config = Read-ConfigFile $Configuration -HasWebDeployPackage:([Bool]$WebDeployPackage)

    #Build and package your web application
    #New-WebDeployPackage

    #Run unit test on your web application
    #Test-WebApplication

    #Create Azure environment described in the JSON configuration file
    $newEnvironmentResult = New-AzureWebApplicationEnvironment -Config $Config -DatabaseServerPassword $DatabaseServerPassword -VMPassword $VMPassword

    #Deploy Web Application package if $WebDeployPackage is specified by the user 
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

    # Restore the original current subscription to Current status
    Restore-Subscription

    Write-Output $Global:AzureWebAppPublishOutput    
    $Global:AzureWebAppPublishOutput = @()
}

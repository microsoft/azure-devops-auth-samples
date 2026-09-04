[CmdletBinding()]
param(
    [PSCredential] $Credential,
    [Parameter(Mandatory=$False, HelpMessage='Tenant ID (This is a GUID which represents the "Directory ID" of the AzureAD tenant into which you want to create the apps')]
    [string] $tenantId
)

Function ReplaceInLine([string] $line, [string] $key, [string] $value)
{
    $index = $line.IndexOf($key)
    if ($index -ige 0)
    {
        $index2 = $index+$key.Length
        $line = $line.Substring(0, $index) + $value + $line.Substring($index2)
    }
    return $line
}

Function ReplaceInTextFile([string] $configFilePath, [System.Collections.HashTable] $dictionary)
{
    $lines = Get-Content $configFilePath
    $index = 0
    while($index -lt $lines.Length)
    {
        $line = $lines[$index]
        foreach($key in $dictionary.Keys)
        {
            if ($line.Contains($key))
            {
                $lines[$index] = ReplaceInLine $line $key $dictionary[$key]
            }
        }
        $index++
    }

    Set-Content -Path $configFilePath -Value $lines -Force
}

Set-Content -Value "<html><body><table>" -Path createdApps.html
Add-Content -Value "<thead><tr><th>Application</th><th>AppId</th><th>Url in the Azure portal</th></tr></thead><tbody>" -Path createdApps.html

$ErrorActionPreference = "Stop"

Function ConfigureApplications
{
<#.Description
   This function creates the Azure AD applications for the sample in the provided Azure AD tenant and updates the
   configuration files in the client and service project  of the visual studio solution (App.Config and Web.Config)
   so that they are consistent with the Applications parameters
#> 
    # $tenantId is the Entra Tenant. This is a GUID which represents the "Directory ID" of the Entra tenant
    # into which you want to create the apps. Look it up in the Azure portal in the "Properties" of the Entra.

    try{
        # Login to Microsoft Graph PowerShell
        if ($TenantId)
        {
            $creds = Connect-MgGraph -TenantId $tenantId
        }
        else
        {
            $creds = Connect-MgGraph
        }

        if (!$tenantId)
        {
            $tenantId = $creds.Tenant.Id
        }

        $tenant = Get-MgOrganization
        $tenantName =  ($tenant.VerifiedDomains | Where { $_.IsDefault -eq $True }).Name

        # Get the user running the script to add the user as the app owner
        $user = Get-MgUser -UserId (Get-MgContext).Account

        # Create the pythonwebapp Entra application
        # create the application 

        $pythonwebappAadApplication = New-MgApplication -DisplayName "python-webapp" -Web @{ RedirectUris = @("https://localhost:5001/getAToken"); ImplicitGrantSettings = @{ EnableIdTokenIssuance = $true } } -SignInAudience "AzureADMyOrg"

        Write-Host "Creating the Entra application (python-webapp)"
        # create the service principal of the newly created application 
        $currentAppId = $pythonwebappAadApplication.AppId
        $pythonwebappServicePrincipal = New-MgServicePrincipal -AppId $currentAppId
        
        # add the user running the script as an app owner if needed
        $owner = Get-MgApplicationOwner -ApplicationId $pythonwebappAadApplication.Id
        if ($owner -eq $null)
        { 
            $NewOwner = @{
                "@odata.id"= "https://graph.microsoft.com/v1.0/directoryObjects/{"+ $user.Id +"}"
            }
            New-MgApplicationOwnerByRef -ApplicationId $pythonwebappAadApplication.Id -BodyParameter $NewOwner
            Write-Host "'$($user.UserPrincipalName)' added as an application owner to app '$($pythonwebappServicePrincipal.DisplayName)'"
        } 

        Write-Host "Done creating the pythonwebapp application (python-webapp)"

        $secret = Add-MgApplicationPassword -ApplicationId $pythonwebappAadApplication.Id
            
        # URL of the Entra application in the Azure portal
        # Future? $pythonwebappPortalUrl = "https://portal.azure.com/#@"+$tenantName+"/blade/Microsoft_AAD_RegisteredApps/ApplicationMenuBlade/Overview/appId/"+$pythonwebappAadApplication.AppId+"/objectId/"+$pythonwebappAadApplication.ObjectId+"/isMSAApp/"
        $pythonwebappPortalUrl = "https://portal.azure.com/#blade/Microsoft_AAD_RegisteredApps/ApplicationMenuBlade/CallAnAPI/appId/"+$pythonwebappAadApplication.AppId+"/objectId/"+$pythonwebappAadApplication.Id+"/isMSAApp/"
        Add-Content -Value "<tr><td>pythonwebapp</td><td>$currentAppId</td><td><a href='$pythonwebappPortalUrl'>python-webapp</a></td></tr>" -Path createdApps.html
    
        $requiredResourceAccess = @()

        $scopeId_UserBasicReadAll = Find-MgGraphPermission User.ReadBasic.All -ExactMatch -PermissionType Delegated | Select-Object -ExpandProperty Id
        # Add Required Resources Access (from 'pythonwebapp' to 'Microsoft Graph')

        $requiredResourceAccess = @{
        ResourceAppId = "00000003-0000-0000-c000-000000000000"
        ResourceAccess = @(
            @{
                Id = $scopeId_UserBasicReadAll
                Type = "Scope"
            }
        )
        }

    Update-MgApplication -ApplicationId $pythonwebappAadApplication.Id -RequiredResourceAccess $requiredResourceAccess
    Write-Host "Granted permissions."
    write-host "AppId: $($pythonwebappAadApplication.AppId)"
    write-host "Secret: $($secret.SecretText)"

    # Update config file for 'pythonwebapp'
    $configFile = Join-Path $pwd.Path -ChildPath ".." "app_config.py"
    Write-Host "Updating the sample code ($configFile)"
    $dictionary = @{ "Enter_the_Tenant_Name_Here" = $tenantName;"Enter_the_Client_Secret_Here" = $secret.SecretText;"Enter_the_Application_Id_here" = $pythonwebappAadApplication.AppId };
    ReplaceInTextFile -configFilePath $configFile -dictionary $dictionary

    Add-Content -Value "</tbody></table></body></html>" -Path createdApps.html  
    }
    finally {
        Disconnect-MgGraph
    }
}

# Pre-requisites
if ((Get-Module -ListAvailable -Name "Microsoft.Graph") -eq $null) { 
    Install-Module "Microsoft.Graph" -Scope CurrentUser 
}

Import-Module Microsoft.Graph

# Run interactively (will ask you for the tenant ID)
ConfigureApplications -tenantId $TenantId
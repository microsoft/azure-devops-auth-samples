[CmdletBinding()]
param(    
    [PSCredential] $Credential,
    [Parameter(Mandatory=$False, HelpMessage='Tenant ID (This is a GUID which represents the "Directory ID" of the AzureAD tenant into which you want to create the apps')]
    [string] $tenantId
)

if ($null -eq (Get-Module -ListAvailable -Name "Microsoft.Graph")) { 
    Install-Module "Microsoft.Graph" -Scope CurrentUser 
} 
Import-Module Microsoft.Graph
$ErrorActionPreference = "Stop"

Function Cleanup
{
<#
.Description
This function removes the Azure AD applications for the sample. These applications were created by the Configure.ps1 script
#>

    # $tenantId is the Active Directory Tenant. This is a GUID which represents the "Directory ID" of the AzureAD tenant 
    # into which you want to create the apps. Look it up in the Azure portal in the "Properties" of the Azure AD. 

    # Login to Microsoft Graph PowerShell (interactive if credentials are not already provided:
    # you'll need to sign-in with creds enabling your to create apps in the tenant)
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
        $tenantId = (Get-MgOrganization).Id
    }
    $tenant = Get-MgOrganization
    $tenantName =  ($tenant.VerifiedDomains | Where-Object { $_.IsDefault -eq $True }).Name
    
    # Removes the applications
    Write-Host "Cleaning-up applications from tenant '$tenantName'"

    Write-Host "Removing 'pythonwebapp' (python-webapp) if needed"
    Get-MgApplication -Filter "DisplayName eq 'python-webapp'"  | ForEach-Object {Remove-MgApplication -ApplicationId $_.Id }
    $apps = Get-MgApplication -Filter "DisplayName eq 'python-webapp'"
    if ($apps)
    {
        Remove-MgApplication -ApplicationId $apps.Id
    }

    foreach ($app in $apps) 
    {
        Remove-MgApplication -ApplicationId $app.Id
        Write-Host "Removed python-webapp.."
    }
    # also remove service principals of this app
    Get-MgServicePrincipal -Filter "DisplayName eq 'python-webapp'" | ForEach-Object {Remove-MgServicePrincipal -ServicePrincipalId $_.Id -Confirm:$false}
    
}

Cleanup -Credential $Credential -tenantId $TenantId
#=======Config [Edit these as per your settings]=======
$azureDevOpsOrgUrl = "https://dev.azure.com/<org>"      # change to the URL of your Azure DevOps account; NOTE: This must use HTTPS
$aadClientId = "<aadClientId>"                          # change to your AAD Application (client) ID
$redirectUri = "urn:ietf:wg:oauth:2.0:oob"              # change to your app registration's reply URI
$tenantId = "common"                                    # change to your org's tenant ID, else leave as is
#======================================================

#==============Constants [Leave as is]=================
$azureDevOpsResId = "499b84ac-1321-427f-aa17-267ca6975798"
$listProjectsApi = "_apis/projects?stateFilter=All&api-version=6.0-preview.4"
#======================================================

if (!(Get-Package ADAL.PS)) { 
    Install-Package -Name ADAL.PS -Scope CurrentUser
}

$authUrl = "https://login.microsoftonline.com/$tenantId"

$authResponse = Get-ADALToken -Resource $azureDevOpsResId -ClientId $aadClientId -RedirectUri $redirectUri -Authority $authUrl -PromptBehavior:Always
Write-Output "Access token expires on: $($authResponse.ExpiresOn)"

$headers = @{
    'Authorization' = "Bearer $($authResponse.AccessToken)"
}

$uri = "$azureDevOpsOrgUrl/$listProjectsApi"
$apiResponse = Invoke-RestMethod -Uri $uri -Method Get -Headers $headers

Write-Output $apiResponse.value
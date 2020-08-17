#========Config [Edit these with your settings]========
$azureDevOpsOrgUrl = "https://dev.azure.com/<org>"      #change to the URL of your Azure DevOps account; NOTE: This must use HTTPS
$aadClientId = "<aadClientId>"                          # change to your app registration's Application ID, unless you are an MSA backed account
$redirectUri = "urn:ietf:wg:oauth:2.0:oob"              # change to your app registration's reply URI, unless you are an MSA backed account
$tenantId = "common"                                    # change to your org's tenant ID, else leave as is
#======================================================

#==============Constants [Leave as is]=================
$azureDevOpsResId = "499b84ac-1321-427f-aa17-267ca6975798"
#======================================================

if (!(Get-Package ADAL.PS)) { 
    Install-Package -Name ADAL.PS -Scope CurrentUser
}

$authUrl = "https://login.microsoftonline.com/$tenantId"

$response = Get-ADALToken -Resource $azureDevOpsResId -ClientId $aadClientId -RedirectUri $redirectUri -Authority $authUrl -PromptBehavior:Always
$response.AccessToken | clip
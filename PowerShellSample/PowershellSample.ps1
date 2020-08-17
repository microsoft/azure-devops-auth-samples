#========Config [Edit these with your settings]========
$azureDevOpsOrgUrl = "https://dev.azure.com/<org>" #change to the URL of your Azure DevOps account; NOTE: This must use HTTPS
$aadClientId = "<client_id>"                       # change to your app registration's Application ID, unless you are an MSA backed account
$redirectUri = "urn:ietf:wg:oauth:2.0:oob"            # change to your app registration's reply URI, unless you are an MSA backed account
$tenantId = "common"                               # change to your org's tenant ID, else leave as is
#======================================================

#==============Constants [Leave as is]=================
$azureDevOpsScope = "499b84ac-1321-427f-aa17-267ca6975798"
#======================================================

$authUrl = "https://login.microsoftonline.com/$tenantId/oauth2/v2.0/authorize?response_type=token&response_mode=fragment&prompt=login&client_id=$aadClientId&redirect_uri=$redirectUri&scope=$azureDevOpsScope"

$Token = Invoke-RestMethod -Method Post -Uri $authUrl
Write-Output $Token
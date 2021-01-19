import os

# To configure this application, fill in your application (client) ID, client secret, 
# AAD tenant ID, and Azure DevOps collection name in the placeholders below.

CLIENT_ID = "Enter_the_Application_Id_here" 
# Application (client) ID of app registration

CLIENT_SECRET = "Enter_the_Client_Secret_here"
# In a production app, we recommend you use a more secure method of storing your secret,
# like Azure Key Vault. Or, use an environment variable as described in Flask's documentation:
# https://flask.palletsprojects.com/en/1.1.x/config/#configuring-from-environment-variables
# CLIENT_SECRET = os.getenv("CLIENT_SECRET")
# if not CLIENT_SECRET:
#     raise ValueError("Need to define CLIENT_SECRET environment variable")

AUTHORITY = "https://login.microsoftonline.com/Enter_the_Tenant_Name_Here"  # For multi-tenant app
# AUTHORITY = "https://login.microsoftonline.com/Enter_the_Tenant_Name_Here"

REDIRECT_PATH = "/getAToken"  # Used for forming an absolute URL to your redirect URI.
                              # The absolute URL must match the redirect URI you set
                              # in the app's registration in the Azure portal.


ENDPOINT = 'https://dev.azure.com/Enter_the_Collection_Name_Here/_apis/Tokens/Pats?api-version=6.1-preview' 
# fill in the url to the user's ADO collection name here

SCOPE = ["499b84ac-1321-427f-aa17-267ca6975798/.default"]
# Means "All scopes for the Azure DevOps API resource"

SESSION_TYPE = "filesystem"  
# Specifies the token cache should be stored in server-side session

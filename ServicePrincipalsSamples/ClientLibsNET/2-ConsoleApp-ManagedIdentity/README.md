# .NET Core console application using an Azure AD Managed Identity to get a work item

This sample shows how to get an Azure AD access token for a Managed Identity using [Azure Identity client library for .NET](https://learn.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme?view=azure-dotnet) and authenticate to Azure DevOps to create or get a work item.

## How to run this sample

**Prerequisites**

- [.NET Core SDK](https://dotnet.microsoft.com/en-us/download) - 6.0 or higher
- [Azure DevOps .NET client libraries](https://learn.microsoft.com/en-us/azure/devops/integrate/concepts/dotnet-client-libraries?view=azure-devops) - 19.219.0-preview or higher
- [Visual Studio / Visual Studio Code](https://aka.ms/vsdownload)

### Step 1: Clone or download this repository

From a shell or command line: 

```ps
git clone https://github.com/microsoft/azure-devops-auth-samples.git
```

### Step 2: Create an Azure VM with a Managed Identity assigned

To assign a Managed Identity during the [VM creation](https://learn.microsoft.com/en-us/azure/virtual-machines/windows/quick-create-portal) or to an existing one, see [Configure managed identities for Azure resources on a VM using the Azure portal](https://learn.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/qs-configure-portal-windows-vm).

### Step 3: Add the Managed Identity to your Azure DevOps Organization

Once the Managed Identity is created, [add it to your Azure DevOps organization](https://learn.microsoft.com/en-us/azure/devops/integrate/get-started/authentication/service-principal-managed-identity#step-by-step-configuration).

### Step 4: Configure the sample to use the Managed Identity

Update constants in the file `Program.cs` with the information about the Managed Identity and Azure DevOps organization.

```cs
public const string AdoOrgName = "Your organization name";

public const string AadTenantId = "Your Azure AD tenant id";
// ClientId for User Assigned Managed Identity. Leave null for System Assigned Managed Identity
public const string AadUserAssignedManagedIdentityClientId = null;
```

### Step 5: Run the sample

**Test in dev environment**

From the console run (your AAD credentials will be used):

```cmd
az login 
cd 2-ConsoleApp-ManagedIdentity
dotnet run
```

**Run in the Azure VM**

The managed identity assigned to the VM will be used in this case. From the console run:

```cmd
cd 2-ConsoleApp-ManagedIdentity
dotnet run
```

# References 
- [Azure.Identity - DefaultAzureCredential](https://learn.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme?view=azure-dotnet#defaultazurecredential)
targetScope = 'subscription'

@minLength(1)
@maxLength(50)
@description('Name of the the environment which is used to generate a short unique hash used in all resources.')
param name string

@minLength(1)
@maxLength(50)
@description('Name of the Azure Container Registry you want to create and use to store your repositories.')
param acrResourceName string

@minLength(1)
@description('Primary location for all resources')
param location string

resource resourceGroup 'Microsoft.Resources/resourceGroups@2020-06-01' = {
    name: '${name}rg'
    location: location
    tags: tags
}

var tags = {
    'azd-env-name': name
}

module resources 'resources.bicep' = {
    name: 'rpo'
    scope: resourceGroup
    params: {
        acrResourceName: acrResourceName
        location: location
    }
}

output AZURE_LOCATION string = location

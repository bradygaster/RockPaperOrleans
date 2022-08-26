targetScope = 'subscription'

@minLength(1)
@maxLength(50)
@description('Name of the the environment which is used to generate a short unique hash used in all resources.')
param name string

@minLength(1)
@description('Primary location for all resources')
param location string

resource resourceGroup 'Microsoft.Resources/resourceGroups@2020-06-01' = {
    name: name
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
        location: location
    }
}

resource acr 'Microsoft.ContainerRegistry/registries@2021-09-01' existing = {
    name: toLower('${resourceGroup.name}acr')
    scope: resourceGroup
}

output AZURE_LOCATION string = location
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = acr.properties.loginServer

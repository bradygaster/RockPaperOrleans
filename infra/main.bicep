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

output AZURE_LOCATION string = location
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = resources.outputs.AZURE_CONTAINER_REGISTRY_ENDPOINT
output AZURE_CONTAINER_REGISTRY_NAME string = resources.outputs.AZURE_CONTAINER_REGISTRY_NAME
output ORLEANS_AZURE_STORAGE_CONNECTION_STRING string = resources.outputs.ORLEANS_AZURE_STORAGE_CONNECTION_STRING
output APPLICATIONINSIGHTS_CONNECTION_STRING string = resources.outputs.APPLICATIONINSIGHTS_CONNECTION_STRING
output ACA_ENVIRONMENT_ID string = resources.outputs.ACA_ENVIRONMENT_ID

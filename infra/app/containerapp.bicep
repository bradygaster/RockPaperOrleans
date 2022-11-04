param environmentName string
param location string = resourceGroup().location
param tags object = {}

param applicationInsightsConnectionString string
param applicationInsightsInstrumentationKey string
param containerAppsEnvironmentName string
param containerRegistryName string
param external bool = false
param imageName string
param serviceName string
param storageAccountName string
param targetPort int = 80

module app '../core/host/container-app.bicep' = {
  name: '${serviceName}-container-app-shared-module'
  params: {
    name: '${environmentName}${serviceName}'
    location: location
    tags: union(tags, { 'azd-env-name': environmentName, 'azd-service-name': serviceName })
    containerAppsEnvironmentName: containerAppsEnvironmentName
    containerName: serviceName
    containerRegistryName: containerRegistryName
    env: [
      {
        name: 'ASPNETCORE_ENVIRONMENT'
        value: 'Development'
      }
      {
        name: 'AzureStorageConnectionString'
        value: 'DefaultEndpointsProtocol=https;AccountName=${storage.name};AccountKey=${storage.listKeys().keys[0].value};EndpointSuffix=${environment().suffixes.storage}'
      }
      {
        name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
        value: applicationInsightsConnectionString
      }
      {
        name: 'APPLICATIONINSIGHTS_INSTRUMENTATIONKEY'
        value: applicationInsightsInstrumentationKey
      }
      {
        name: 'ASPNETCORE_LOGGING__CONSOLE__DISABLECOLORS'
        value: 'true'
      }
    ]
    external: external
    imageName: !empty(imageName) ? imageName : 'nginx:latest'
    targetPort: targetPort
  }
}

resource storage 'Microsoft.Storage/storageAccounts@2022-05-01' existing = {
  name: storageAccountName
}

output CONTAINER_APP_IDENTITY_PRINCIPAL_ID string = app.outputs.identityPrincipalId
output CONTAINER_APP_NAME string = app.outputs.name
output CONTAINER_APP_URI string = app.outputs.uri
output CONTAINER_APP_IMAGE_NAME string = app.outputs.imageName

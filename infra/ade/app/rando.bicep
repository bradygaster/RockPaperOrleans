param environmentName string
param location string = resourceGroup().location
param tags object = {}

param applicationInsightsConnectionString string
param applicationInsightsInstrumentationKey string
param containerAppsEnvironmentName string
param containerRegistryName string
param imageName string = serviceName
param serviceName string = 'rando'
param storageAccountName string
param targetPort int = 30003
param external bool = false

module app 'containerapp.bicep' = {
  name: '${serviceName}-container-app-module'
  params: {
    environmentName: environmentName
    location: location
    tags: tags
    applicationInsightsConnectionString: applicationInsightsConnectionString
    applicationInsightsInstrumentationKey: applicationInsightsInstrumentationKey
    containerAppsEnvironmentName: containerAppsEnvironmentName
    containerRegistryName: containerRegistryName
    imageName: imageName
    serviceName: serviceName
    storageAccountName: storageAccountName
    targetPort: targetPort
    external: external
  }
}

output SERVICE_RANDO_IDENTITY_PRINCIPAL_ID string = app.outputs.CONTAINER_APP_IDENTITY_PRINCIPAL_ID
output SERVICE_RANDO_NAME string = app.outputs.CONTAINER_APP_NAME
output SERVICE_RANDO_URI string = app.outputs.CONTAINER_APP_URI
output SERVICE_RANDO_IMAGE_NAME string = app.outputs.CONTAINER_APP_IMAGE_NAME

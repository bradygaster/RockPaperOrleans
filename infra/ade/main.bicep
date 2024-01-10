targetScope = 'subscription'

@minLength(1)
@maxLength(50)
@description('Name of the the environment which is used to generate a short unique hash used in all resources.')
param environmentName string

@minLength(1)
@description('Primary location for all resources')
param location string

param applicationInsightsDashboardName string = ''
param applicationInsightsName string = ''
param containerAppsEnvironmentName string = ''
param containerRegistryName string = ''
param logAnalyticsName string = ''
param storageAccountName string = ''

var resourceToken = toLower(uniqueString(subscription().id, environmentName, location))
var tags = { 'azd-env-name': environmentName }
var abbrs = loadJsonContent('abbreviations.json')

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
    name: '${abbrs.resourcesResourceGroups}${environmentName}'
    location: location
    tags: tags
}

// Container apps host (including container registry)
module containerApps './core/host/container-apps.bicep' = {
    name: 'container-apps'
    scope: rg
    params: {
        name: 'app'
        containerAppsEnvironmentName: !empty(containerAppsEnvironmentName) ? containerAppsEnvironmentName : '${abbrs.appManagedEnvironments}${resourceToken}'
        containerRegistryName: !empty(containerRegistryName) ? containerRegistryName : '${abbrs.containerRegistryRegistries}${resourceToken}'
        location: location
        logAnalyticsWorkspaceName: monitoring.outputs.logAnalyticsWorkspaceName
    }
}

// Backing storage for Azure functions backend API
module storage './core/storage/storage-account.bicep' = {
    name: 'storage'
    scope: rg
    params: {
        name: !empty(storageAccountName) ? storageAccountName : '${abbrs.storageStorageAccounts}${resourceToken}'
        location: location
        tags: tags
    }
}

// Monitor application with Azure Monitor
module monitoring './core/monitor/monitoring.bicep' = {
    name: 'monitoring'
    scope: rg
    params: {
        location: location
        tags: tags
        logAnalyticsName: !empty(logAnalyticsName) ? logAnalyticsName : '${abbrs.operationalInsightsWorkspaces}${resourceToken}'
        applicationInsightsName: !empty(applicationInsightsName) ? applicationInsightsName : '${abbrs.insightsComponents}${resourceToken}'
        applicationInsightsDashboardName: !empty(applicationInsightsDashboardName) ? applicationInsightsDashboardName : '${abbrs.portalDashboards}${resourceToken}'
    }
}

output APPLICATIONINSIGHTS_CONNECTION_STRING string = monitoring.outputs.applicationInsightsConnectionString
output APPLICATIONINSIGHTS_INSTRUMENTATIONKEY string = monitoring.outputs.applicationInsightsInstrumentationKey
output APPLICATIONINSIGHTS_NAME string = monitoring.outputs.applicationInsightsName
output AZURE_CONTAINER_ENVIRONMENT_NAME string = containerApps.outputs.environmentName
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = containerApps.outputs.registryLoginServer
output AZURE_CONTAINER_REGISTRY_NAME string = containerApps.outputs.registryName
output AZURE_LOCATION string = location
output AZURE_STORAGE_ACCOUNT_NAME string = storage.outputs.name
output AZURE_TENANT_ID string = tenant().tenantId

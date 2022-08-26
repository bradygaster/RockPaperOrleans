param location string = resourceGroup().location 
param repositoryImage string = 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'

resource acr 'Microsoft.ContainerRegistry/registries@2021-09-01' = {
  name: toLower('${resourceGroup().name}acr')
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: true
  }
}

resource storage 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: toLower('${resourceGroup().name}stg')
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

resource logs 'Microsoft.OperationalInsights/workspaces@2021-06-01' = {
  name: toLower('${resourceGroup().name}logs')
  location: location
  properties: any({
    retentionInDays: 30
    features: {
      searchVersion: 1
    }
    sku: {
      name: 'PerGB2018'
    }
  })
}

resource ai 'Microsoft.Insights/components@2020-02-02' = {
  name: toLower('${resourceGroup().name}ai')
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logs.id
  }
}

resource env 'Microsoft.App/managedEnvironments@2022-03-01' = {
  name: toLower('${resourceGroup().name}rpoenv')
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logs.properties.customerId
        sharedKey: logs.listKeys().primarySharedKey
      }
    }
  }
}

module gamecontroller 'gamecontroller.bicep' = {
  name: toLower('${resourceGroup().name}gamecontroller')
  params: {
    location: location
    repositoryImage: repositoryImage
    azureStorage: format('DefaultEndpointsProtocol=https;AccountName=${storage.name};AccountKey=${k};EndpointSuffix=core.windows.net')
    ai: ai.properties.ConnectionString
    aiKey: ai.properties.InstrumentationKey
    registry: acr.name
    environmentId: env.id
  }
}

module leaderboard 'leaderboard.bicep' = {
  name: toLower('${resourceGroup().name}leaderboard')
  params: {
    location: location
    repositoryImage: repositoryImage
    azureStorage: format('DefaultEndpointsProtocol=https;AccountName=${storage.name};AccountKey=${k};EndpointSuffix=core.windows.net')
    ai: ai.properties.ConnectionString
    aiKey: ai.properties.InstrumentationKey
    registry: acr.name
    environmentId: env.id
  }
}

module players 'players.bicep' = {
  name: toLower('${resourceGroup().name}players')
  params: {
    location: location
    repositoryImage: repositoryImage
    azureStorage: format('DefaultEndpointsProtocol=https;AccountName=${storage.name};AccountKey=${k};EndpointSuffix=core.windows.net')
    ai: ai.properties.ConnectionString
    aiKey: ai.properties.InstrumentationKey
    registry: acr.name
    environmentId: env.id
  }
}

module rando 'rando.bicep' = {
  name: toLower('${resourceGroup().name}rando')
  params: {
    location: location
    repositoryImage: repositoryImage
    azureStorage: format('DefaultEndpointsProtocol=https;AccountName=${storage.name};AccountKey=${k};EndpointSuffix=core.windows.net')
    ai: ai.properties.ConnectionString
    aiKey: ai.properties.InstrumentationKey
    registry: acr.name
    environmentId: env.id
  }
}

var k = listKeys(storage.name, storage.apiVersion).keys[0].value
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = acr.properties.loginServer
output AZURE_CONTAINER_REGISTRY_NAME string = acr.name
output ORLEANS_AZURE_STORAGE_CONNECTION_STRING string = format('DefaultEndpointsProtocol=https;AccountName=${storage.name};AccountKey=${k};EndpointSuffix=core.windows.net')
output APPLICATIONINSIGHTS_CONNECTION_STRING string = ai.properties.ConnectionString
output APPLICATIONINSIGHTS_INSTRUMENTATIONKEY string = ai.properties.InstrumentationKey
output ACA_ENVIRONMENT_ID string = env.id



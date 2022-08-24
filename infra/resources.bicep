param location string = resourceGroup().location
param acrResourceName string 

resource acr 'Microsoft.ContainerRegistry/registries@2021-09-01' = {
  name: toLower(acrResourceName)
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

var shared_config = [
  {
    name: 'ASPNETCORE_ENVIRONMENT'
    value: 'Development'
  }
  {
    name: 'AzureStorageConnectionString'
    value: format('DefaultEndpointsProtocol=https;AccountName=${storage.name};AccountKey=${listKeys(storage.name, storage.apiVersion).keys[0].value};EndpointSuffix=core.windows.net')
  }
  {
    name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
    value: ai.properties.InstrumentationKey
  }
  {
    name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
    value: ai.properties.ConnectionString
  }
]

module containerApps 'modules/containerapps.bicep' = {
  name: 'rpocontainerapps'
  params: {
    containerAppEnvironmentId: env.id
    registry: acrResourceName
    registryPassword: acr.listCredentials().passwords[0].value
    registryUsername: acr.listCredentials().username
    envVars: shared_config
    location: location
  }
}

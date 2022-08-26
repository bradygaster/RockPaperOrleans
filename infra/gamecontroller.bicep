param location string = resourceGroup().location
param repositoryImage string

resource acr 'Microsoft.ContainerRegistry/registries@2021-09-01' existing = {
  name: toLower('${resourceGroup().name}acr')
  scope: resourceGroup(resourceGroup().name)
}

resource ai 'Microsoft.Insights/components@2020-02-02' existing = {
  name: toLower('${resourceGroup().name}ai')
  scope: resourceGroup(resourceGroup().name)
}

resource env 'Microsoft.App/managedEnvironments@2022-03-01' existing = {
  name: toLower('${resourceGroup().name}rpoenv')
  scope: resourceGroup(resourceGroup().name)
}

resource storage 'Microsoft.Storage/storageAccounts@2021-02-01' existing = {
  name: toLower('${resourceGroup().name}stg')
  scope: resourceGroup(resourceGroup().name)
}

var key = listKeys(storage.name, storage.apiVersion).keys[0].value

resource gamecontroller 'Microsoft.App/containerApps@2022-01-01-preview' = {
  name: toLower('${resourceGroup().name}gamecontroller')
  location: location
  properties: {
    managedEnvironmentId: env.id
    configuration: {
      activeRevisionsMode: 'single'
      secrets: [
        {
          name: 'container-registry-password'
          value: acr.listCredentials().passwords[0].value
        }
      ]
      registries: [
        {
          server: '${acr.name}.azurecr.io'
          username: acr.name
          passwordSecretRef: 'container-registry-password'
        }
      ]
      ingress: {
        external: true
        targetPort: 80
      }
    }
    template: {
      containers: [
        {
          image: repositoryImage
          name: 'gamecontroller'
          env: [
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: 'Development'
            }
            {
              name: 'AzureStorageConnectionString'
              value: format('DefaultEndpointsProtocol=https;AccountName=${storage.name};AccountKey=${key};EndpointSuffix=core.windows.net')
            }
            {
              name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
              value: ai.properties.ConnectionString
            }
            {
              name: 'APPLICATIONINSIGHTS_INSTRUMENTATIONKEY'
              value: ai.properties.InstrumentationKey
            }
            {
              name: 'ASPNETCORE_LOGGING__CONSOLE__DISABLECOLORS'
              value: 'true'
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 1
      }
    }
  }
}

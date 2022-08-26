param location string = resourceGroup().location
param repositoryImage string
param azureStorage string 
param ai string
param aiKey string
param registry string
param environmentId string 

resource acr 'Microsoft.ContainerRegistry/registries@2021-12-01-preview' existing = {
  name: registry
}

resource leaderboard 'Microsoft.App/containerApps@2022-01-01-preview' = {
  name: toLower('${resourceGroup().name}leaderboard')
  location: location
  properties: {
    managedEnvironmentId: environmentId
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
          server: '${registry}.azurecr.io'
          username: registry
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
          name: 'leaderboard'
          env: [
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: 'Development'
            }
            {
              name: 'AzureStorageConnectionString'
              value: azureStorage
            }
            {
              name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
              value: ai
            }
            {
              name: 'APPLICATIONINSIGHTS_INSTRUMENTATIONKEY'
              value: aiKey
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


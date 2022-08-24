param location string = resourceGroup().location
param containerAppEnvironmentId string
@secure()
param registryPassword string
param registry string
param registryUsername string
param repositoryImage string
param envVars array = []

resource containerApp 'Microsoft.App/containerApps@2022-01-01-preview' = {
  name: 'rporandos'
  location: location
  properties: {
    managedEnvironmentId: containerAppEnvironmentId
    configuration: {
      activeRevisionsMode: 'single'
      secrets: [
        {
          name: 'container-registry-password'
          value: registryPassword
        }
      ]
      registries: [
        {
          server: registry
          username: registryUsername
          passwordSecretRef: 'container-registry-password'
        }
      ]
    }
    template: {
      containers: [
        {
          image: repositoryImage
          name: 'randos'
          env: envVars
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 1
      }
    }
  }
}

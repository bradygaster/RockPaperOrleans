param location string = resourceGroup().location
param containerAppEnvironmentId string
@secure()
param registryPassword string
param registry string
param registryUsername string
param repositoryImage string = 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'
param envVars array = []

module gamecontroller 'gamecontroller.bicep' = {
  name: 'rpogamecontroller'
  params: {
    location: location
    containerAppEnvironmentId: containerAppEnvironmentId
    registry: registry
    registryPassword: registryPassword
    registryUsername: registryUsername
    envVars : envVars
    repositoryImage: repositoryImage
  }
}

module leaderboard 'leaderboard.bicep' = {
  name: 'rpoleaderboard'
  params: {
    location: location
    containerAppEnvironmentId: containerAppEnvironmentId
    registry: registry
    registryPassword: registryPassword
    registryUsername: registryUsername
    envVars : envVars
    repositoryImage: repositoryImage
  }
}

module players 'players.bicep' = {
  name: 'rpoplayers'
  params: {
    location: location
    containerAppEnvironmentId: containerAppEnvironmentId
    registry: registry
    registryPassword: registryPassword
    registryUsername: registryUsername
    envVars : envVars
    repositoryImage: repositoryImage
  }
}

module randos 'randos.bicep' = {
  name: 'rporandos'
  params: {
    location: location
    containerAppEnvironmentId: containerAppEnvironmentId
    registry: registry
    registryPassword: registryPassword
    registryUsername: registryUsername
    envVars : envVars
    repositoryImage: repositoryImage
  }
}

param name string
param location string
param image string

var containerAppName = 'rascals'

module containerApp 'containerapp.bicep' = {
  name: 'containerapp-${containerAppName}'
  params: {
    name: name
    location: location
    containerAppName: containerAppName
    image: image
    port: 30003
  }
}

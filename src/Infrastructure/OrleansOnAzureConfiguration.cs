﻿namespace Orleans.Hosting
{
    public static class OrleansOnAzureConfiguration
    {
        public static class EnvironmentVariableNames
        {
            public static string ClusterId => "OrleansClusterId";
            public static string ServiceId => "OrleansServiceId";
            public static string SiloPort => "OrleansSiloPort";
            public static string GatewayPort => "OrleansGatewayPort";
            public static string SiloNameTemplate => "OrleansSiloNameTemplate";
            public static string CosmosDbAccountEndpoint => "OrleansCosmosDbAccountEndpoint";
            public static string CosmosDbAccountKey => "OrleansCosmosDbAccountKey";
            public static string CosmosDbDatabase => "OrleansCosmosDbDatabase";
            public static string AzureStorageConnectionString => "AzureStorageConnectionString";
            public static string ApplicationInsights => "APPLICATIONINSIGHTS_INSTRUMENTATIONKEY";
        }
    }
}

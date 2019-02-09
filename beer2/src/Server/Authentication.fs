module Authentication

open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration

module AzureActiveDirectory =
    let configureAppConfiguration (context : WebHostBuilderContext) (config : IConfigurationBuilder) =
        config
            .AddJsonFile("appsettings.json", optional = false, reloadOnChange = true)
            .AddJsonFile(sprintf "appsettings.%s.json" context.HostingEnvironment.EnvironmentName, optional = true)
            .AddEnvironmentVariables()
        |> ignore
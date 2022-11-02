dapr run `
    --app-id serviceA `
    --app-port 7200 `
    --dapr-http-port 3500 `
    --components-path ../dapr/components `
    dotnet run
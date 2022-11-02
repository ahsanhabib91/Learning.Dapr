dapr run `
    --app-id serviceB `
    --app-port 7251 `
    --dapr-http-port 3501 `
    --components-path ../dapr/components `
    dotnet run
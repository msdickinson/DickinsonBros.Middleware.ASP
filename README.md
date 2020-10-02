# DickinsonBros.Middleware
<a href="https://www.nuget.org/packages/DickinsonBros.Middleware.ASP/">
    <img src="https://img.shields.io/nuget/v/DickinsonBros.Middleware.ASP">
</a>

Middleware for ASP.Net

Features

* Logs requests redacted
* Logs responses redacted and statuscode
* Adds Telemetry
* Handles correlation Ids
* Catch all uncaught exceptions and log them redacted

<a href="https://dev.azure.com/marksamdickinson/DickinsonBros/_build?definitionScope=%5CDickinsonBros.Middleware.ASP">Builds</a>

<h2>Example Ouput</h2>

      info: DickinsonBros.Middleware.ASP.MiddlewareService[1]
      + POST https://localhost:5001/Api/Sample
      Path: /Api/Sample
      Method: POST
      Scheme: https
      Prams: {}
      Body: {
        "Username": "Username",
        "Password": "***REDACTED***"
      }
      CorrelationId: c5bb0105-91c9-444d-98b5-b4a18365eb48

      info: DickinsonBros.Middleware.ASP.MiddlewareService[1]
      Response POST https://localhost:5001/Api/Sample
      Body: {
        "userId": "User100",
        "massiveString": "***REDACTED***"
      }
      StatusCode: 200
      CorrelationId: c5bb0105-91c9-444d-98b5-b4a18365eb48

      info: DickinsonBros.Middleware.ASP.MiddlewareService[1]
      - POST https://localhost:5001/Api/Sample
      ElapsedMilliseconds: 190
      CorrelationId: c5bb0105-91c9-444d-98b5-b4a18365eb48
      
![Alt text](https://raw.githubusercontent.com/msdickinson/DickinsonBros.Middleware/develop/TelemetryAPISample.PNG)

Note: Logs can be redacted via configuration (see https://github.com/msdickinson/DickinsonBros.Redactor)

Telemetry generated when using DickinsonBros.Telemetry and connecting it to a configured database for ITelemetry See https://github.com/msdickinson/DickinsonBros.Telemetry on how to configure DickinsonBros.Telemetry and setup the database.

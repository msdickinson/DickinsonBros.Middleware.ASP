# DickinsonBros.Middleware.ASP
<a href="https://dev.azure.com/marksamdickinson/dickinsonbros/_build/latest?definitionId=86&amp;branchName=master"> <img alt="Azure DevOps builds (branch)" src="https://img.shields.io/azure-devops/build/marksamdickinson/DickinsonBros/86/master"> </a> <a href="https://dev.azure.com/marksamdickinson/dickinsonbros/_build/latest?definitionId=86&amp;branchName=master"> <img alt="Azure DevOps coverage (branch)" src="https://img.shields.io/azure-devops/coverage/marksamdickinson/dickinsonbros/86/master"> </a><a href="https://dev.azure.com/marksamdickinson/DickinsonBros/_release?_a=releases&view=mine&definitionId=38"> <img alt="Azure DevOps releases" src="https://img.shields.io/azure-devops/release/marksamdickinson/b5a46403-83bb-4d18-987f-81b0483ef43e/38/39"> </a><a href="https://www.nuget.org/packages/DickinsonBros.Middleware.ASP/"><img src="https://img.shields.io/nuget/v/DickinsonBros.Middleware.ASP"></a>

Middleware for ASP.Net

Features

* Logs requests redacted
* Logs responses redacted and statuscode
* Adds Telemetry
* Handles correlation Ids
* Catch all uncaught exceptions and log them redacted

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

<b>Telemetry</b>
![Alt text](https://raw.githubusercontent.com/msdickinson/DickinsonBros.Middleware.ASP/master/MiddlewareTelemetry.PNG)
      
[Sample Runner](https://github.com/msdickinson/DickinsonBros.Middleware.ASP/tree/master/DickinsonBros.Middleware.ASP.Runner)

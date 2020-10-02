using DickinsonBros.DateTime.Abstractions;
using DickinsonBros.Guid.Abstractions;
using DickinsonBros.Logger.Abstractions;
using DickinsonBros.Stopwatch.Abstractions;
using DickinsonBros.Telemetry.Abstractions;
using DickinsonBros.Telemetry.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DickinsonBros.Middleware.ASP
{
    public class MiddlewareService
    {
        internal const string CORRELATION_ID = "X-Correlation-ID";
        private readonly RequestDelegate _next;
        internal readonly IServiceProvider _serviceProvider;
        internal readonly IDateTimeService _dateTimeService;
        internal readonly ITelemetryService _telemetryService;
        internal readonly IGuidService _guidService;
        internal readonly ILoggingService<MiddlewareService> _loggingService;

        internal readonly ICorrelationService _correlationService;
        public MiddlewareService(
            RequestDelegate next,
            IServiceProvider serviceProvider,
            IDateTimeService dateTimeService,
            ITelemetryService telemetryService,
            IGuidService guidService,
            ICorrelationService correlationService,
            ILoggingService<MiddlewareService> loggingService
        )
        {
            _next = next;
            _guidService = guidService;
            _loggingService = loggingService;
            _correlationService = correlationService;
            _serviceProvider = serviceProvider;
            _dateTimeService = dateTimeService;
            _telemetryService = telemetryService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var telemetryData = new TelemetryData
            {
                DateTime = _dateTimeService.GetDateTimeUTC(),
                Name = $"{context.Request.Method} {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}",
                TelemetryType = TelemetryType.API
            };
            var x = context.Request.PathBase;
            var stopwatchService = _serviceProvider.GetRequiredService<IStopwatchService>();
            stopwatchService.Start();
            _correlationService.CorrelationId = EnsureCorrelationId(context.Request);

            var requestBody = await FormatRequestAsync(context.Request);
            var originalBodyStream = context.Response.Body;

            if (context.Request.Path.Value.Contains("/api/", StringComparison.OrdinalIgnoreCase))
            {
                _loggingService.LogInformationRedacted
                (
                    $"+ {telemetryData.Name}",
                    new Dictionary<string, object>
                    {
                        { "Path", context.Request.Path.Value },
                        { "Method", context.Request.Method },
                        { "Scheme", context.Request.Scheme },
                        { "Prams", context.Request.Query.ToDictionary() },
                        { "Body", requestBody }
                    }
                );
            }

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;
            try
            {
                await _next(context);

                context.Response.Headers.TryAdd
                (
                    CORRELATION_ID,
                   _correlationService.CorrelationId
                );

                var responseBodyString = await FormatResponseAsync(context.Response);
                await responseBody.CopyToAsync(originalBodyStream);
                stopwatchService.Stop();

                if (context.Request.Path.Value.Contains("/api/", StringComparison.OrdinalIgnoreCase))
                {
                    _loggingService.LogInformationRedacted
                    (
                        $"Response {telemetryData.Name}",
                        new Dictionary<string, object>
                        {
                            { "Body", responseBodyString },
                            { "StatusCode", context.Response.StatusCode }
                        }
                    );
                }

                if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
                {
                    telemetryData.TelemetryState = TelemetryState.Successful;
                }
                else if (context.Response.StatusCode >= 400 && context.Response.StatusCode < 500)
                {
                    telemetryData.TelemetryState = TelemetryState.BadRequest;
                }
                else
                {
                    telemetryData.TelemetryState = TelemetryState.Failed;
                }


            }
            catch (Exception exception)
            {
                context.Response.StatusCode = 500;
                context.Response.Headers.TryAdd
                (
                    CORRELATION_ID,
                   _correlationService.CorrelationId
                );
                telemetryData.TelemetryState = TelemetryState.Failed;
                stopwatchService.Stop();

                _loggingService.LogErrorRedacted
                (
                    $"Unhandled exception {telemetryData.Name}",
                    exception,
                    new Dictionary<string, object>
                    {
                            { "StatusCode", context.Response.StatusCode }
                    }
                );
            }
            finally
            {
                telemetryData.ElapsedMilliseconds = (int)stopwatchService.ElapsedMilliseconds;

                _loggingService.LogInformationRedacted
                (
                    $"- {telemetryData.Name}",
                    new Dictionary<string, object>
                    {
                            { "ElapsedMilliseconds", telemetryData.ElapsedMilliseconds }
                    }
                );


                _telemetryService.Insert(telemetryData);
            }
        }

        internal string EnsureCorrelationId(HttpRequest request)
        {
            if (!request.Headers.Any(e => e.Key == CORRELATION_ID))
            {
                return _guidService.NewGuid().ToString();
            }

            return request.Headers.First(e => e.Key == CORRELATION_ID).Value;
        }

        internal async Task<string> FormatRequestAsync(HttpRequest request)
        {
            request.EnableBuffering();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];

            await request.Body.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

            var body = Encoding.UTF8.GetString(buffer);

            request.Body.Position = 0;

            return body;
        }

        internal async Task<string> FormatResponseAsync(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);

            string body = await new StreamReader(response.Body).ReadToEndAsync();

            response.Body.Seek(0, SeekOrigin.Begin);

            return body;
        }
    }
}

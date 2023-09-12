using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using VL.Health.API.Exceptions;

namespace VL.Health.Service.CustomExceptions
{
    public class CustomExceptionMiddleware
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly RequestDelegate _next;

        private HttpStatusCode _statusCode;
        private string _message;
        private List<string> _error;

        public CustomExceptionMiddleware(RequestDelegate next)
        {
            var configurationAppInsights = TelemetryConfiguration.CreateDefault();
            _telemetryClient = new TelemetryClient(configurationAppInsights);
            _telemetryClient.InstrumentationKey = Environment.GetEnvironmentVariable("ApplicationInsights_InstrumentationKey");
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _statusCode = HttpStatusCode.InternalServerError;
            _message = "Unexpected Error";
            _error = new List<string> { "UnexpectedError" };

            var requestTelemetry = new RequestTelemetry
            {
                Name = $"{context.Request.Method} {context.Request.Path.ToString()}"
            };
            requestTelemetry.ResponseCode = context.Response.StatusCode.ToString();
            requestTelemetry.Success = false;
            requestTelemetry.HttpMethod = context.Request.Method;

            var operation = _telemetryClient.StartOperation(requestTelemetry);
            try
            {
                var response = context.Response;
                response.ContentType = "application/json";

                if (exception is FunctionalException)
                {
                    var functionalException = (FunctionalException)exception;

                    _message = functionalException.Message;
                    _statusCode = (HttpStatusCode)functionalException.FunctionalError;
                    _error = functionalException.Errors;
                }
                else
                {
                    _telemetryClient.TrackException(exception);
                    _telemetryClient.Flush();
                }

                response.StatusCode = (int)_statusCode;

                var customErrorResponse = CreateCustomErrorResponse();
                var serialized = JsonConvert.SerializeObject(customErrorResponse);
                await response.WriteAsync(serialized);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _telemetryClient.StopOperation(operation);
            }
        }

        private CustomErrorResponse CreateCustomErrorResponse()
        {
            return new CustomErrorResponse
            {
                Message = _message,
                Error = _error
            };
        }
    }
}

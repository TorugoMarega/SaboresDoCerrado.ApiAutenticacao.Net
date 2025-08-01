﻿using System.Net;
using System.Text.Json;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu uma exceção não tratada: {Message}", ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var response = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "Ocorreu um erro interno no servidor. Por favor, entre em contato com o suporte."
                };

                var jsonResponse = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsPage.data;
using NewsPage.Models.entities;
using System;
using System.Threading.Tasks;

namespace NewsPage.Middleware
{
    public class PageVisitorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PageVisitorMiddleware> _logger;

        public PageVisitorMiddleware(RequestDelegate next, ILogger<PageVisitorMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            _logger.LogInformation($"PageVisitorMiddleware executing for: {context.Request.Method} {context.Request.Path}");
            
            // Proceed with the request
            await _next(context);

            _logger.LogInformation($"Request completed with status code: {context.Response.StatusCode}");

            // Only track successful page requests (status code 200) and exclude API and static file requests
            if (context.Response.StatusCode == 200)
            {
                _logger.LogInformation("Status code is 200");
                
                if (!context.Request.Path.StartsWithSegments("/swagger"))
                {
                    _logger.LogInformation("Path does not start with /swagger");
                    
                    if (context.Request.Method == "GET")
                    {
                        _logger.LogInformation("Method is GET - tracking page visit");
                        await TrackPageVisitAsync(dbContext);
                    }
                    else
                    {
                        _logger.LogInformation($"Method is {context.Request.Method} - not tracking");
                    }
                }
                else
                {
                    _logger.LogInformation("Path starts with /swagger - not tracking");
                }
            }
            else
            {
                _logger.LogInformation($"Status code is {context.Response.StatusCode} - not tracking");
            }
        }

        private async Task TrackPageVisitAsync(ApplicationDbContext dbContext)
        {
            // Check if we have a record for today
            var today = DateTime.Today;
            var pageVisit = await dbContext.PageVisitors
                .FirstOrDefaultAsync(p => p.CreateAt.Date == today);

            if (pageVisit == null)
            {
                // Create new record for today
                pageVisit = new PageVisitor
                {
                    Id = Guid.NewGuid(),
                    UserId = null,
                    AccessCount = 1,
                    CreateAt = today
                };
                dbContext.PageVisitors.Add(pageVisit);
                _logger.LogInformation("Created new page visit record for today");
            }
            else
            {
                // Increment existing counter
                pageVisit.AccessCount++;
                _logger.LogDebug("Incremented existing page visit counter");
            }

            await dbContext.SaveChangesAsync();
        }
    }

    // Extension method to make middleware registration cleaner
    public static class PageVisitorMiddlewareExtensions
    {
        public static IApplicationBuilder UsePageVisitorTracking(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PageVisitorMiddleware>();
        }
    }
}
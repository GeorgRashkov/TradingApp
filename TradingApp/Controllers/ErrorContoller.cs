using Microsoft.AspNetCore.Mvc;
using TradingApp.ViewModels.Error;

namespace TradingApp.Controllers
{
    public class ErrorContoller : Controller
    {
        public IActionResult StatusCodePage(int statusCode)
        {
            StatusCodeViewModel statusCodeModel;

            
            if(statusCode == 401)
            {
                statusCodeModel = new StatusCodeViewModel()
                {
                    Title = "Unauthorized",
                    Code = statusCode,
                    Description = "You must authenticate yourself to get the requested resource."
                };
            }
            else if (statusCode == 403) 
            {
                statusCodeModel = new StatusCodeViewModel()
                {
                    Title = "Forbidden",
                    Code = statusCode,
                    Description = "You do not have access to the requested resource."
                };
            }
            else if (statusCode == 404) 
            {
                statusCodeModel = new StatusCodeViewModel()
                {
                    Title = "Not Found",
                    Code = statusCode,
                    Description = "The server could not find the requested resource."
                };
            }
            else if(statusCode >= 400 && statusCode < 500)
            {
                statusCodeModel = new StatusCodeViewModel()
                {
                    Title = "Bad Request",
                    Code = statusCode,
                    Description = "The server could not understand the request due to invalid syntax."
                };
            }
            else if (statusCode >= 500 && statusCode < 600) 
            {
                statusCodeModel = new StatusCodeViewModel()
                {
                    Title = "Internal Server Error",
                    Code = statusCode,
                    Description = "There was a problem with the server. Please try again later."
                };
            }
            else
            {
                statusCodeModel = new StatusCodeViewModel()
                {
                    Title = "Error",
                    Code = statusCode,
                    Description = "An unexpected error occurred. Please try again later."
                };
            }

            return View(model:statusCodeModel);
        }
    }
}

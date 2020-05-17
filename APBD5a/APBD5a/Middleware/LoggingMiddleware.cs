using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APBD5a.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next) {

            _next = next;

        }

        public async Task InvokeAsync(HttpContext context) {

            context.Request.EnableBuffering();

            if (context.Request != null) {
                string path = context.Request.Path;
                string method = context.Request.Method;
                string queryString = context.Request.QueryString.ToString();
                string bodyStr = "";
                DateTime localDate = DateTime.Now;

                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true)) {
                    bodyStr = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;

                }

                string logFile = "requestsLog.txt";

                string textLog =
                    path 
                    + "date: "+ localDate.ToString()
                    + "method: " + method 
                    +" "+ queryString 
                    + " " + bodyStr 
                    + Environment.NewLine;


                if (File.Exists(logFile))
                {
                    using (StreamWriter sw = File.AppendText(logFile))
                    {
                        sw.WriteLine(textLog);
                    }
                }
                else
                {
                    File.WriteAllText(logFile, textLog);
                }
            }

            if (_next != null)
            {
                await _next(context);
            }
        }

    }


}

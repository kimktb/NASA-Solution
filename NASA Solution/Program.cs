using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NASA_Project;
using NASA_Project.Interfaces;
using NASA_Project.Models;
using NASA_Project.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NASA_Solution
{
    public class Program
    {
        private static string _dataFilePath = "C:\\Temp\\DataFiles\\";
        private static string _photoFilePath = $"{_dataFilePath}Photos\\";
        private static string _apiKey       = "DEMO_KEY";
        private static string _uri          = "https://api.nasa.gov/mars-photos/api/v1/rovers/curiosity/photos";
        private static INASA_FileService _fileService;
        private static INASA_API_Service _apiService;

        /// <summary>
        /// This method drives the NASA file download process
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            //CreateWebHostBuilder(args).Build().Run();
            string dataFilePath = _dataFilePath;
            _apiService = new NASA_API_Service(_uri, _apiKey);
            _fileService = new NASA_FileService();

            // Down NASA photos
            using (NASA_Driver driver = new NASA_Driver(_dataFilePath, 
                _apiService, _fileService))
            {
                driver.DownLoadNASAPhotos()
                         .ConfigureAwait(false)
                            .GetAwaiter()
                            .GetResult();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();        
    }
}

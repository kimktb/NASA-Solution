using Microsoft.VisualStudio.TestTools.UnitTesting;
using NASA_Project.Models;
using NASA_Project.Services;
using System;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;
using System.IO;
using NASA_Project.Interfaces;
using NASA_Project;
using Newtonsoft.Json;

namespace Test_NASA
{
    [TestClass]
    public class TestNASA
    {
        private string _uri;
        private string _apiKey;
        private INASA_API_Service _apiService;
        private INASA_FileService _fileService;
        private string _dataFilePath;

        [TestInitialize]
        public void Initialize()
        {
            _apiKey = "DEMO_KEY";
            _uri = "https://api.nasa.gov/mars-photos/api/v1/rovers/curiosity/photos";
            _apiService = new NASA_API_Service(_uri, _apiKey);
            _fileService = new NASA_FileService();
            _dataFilePath = "C:\\Temp\\DataFiles\\";
        }

        [TestMethod]
        public async Task Test_GetNASAPhotosAsync()
        {
            HTTPResponseDetails details;
            string photoDate = DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd");
            using (NASA_API_Service apiService = new NASA_API_Service(_uri, _apiKey))
            {
                details = await apiService.GetNASAPhotosAsync(photoDate);
            }
            Assert.AreEqual(details.ResponseStatus, HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task Test_ReadDatesFile()
        {
            List<string> dates;
            string fileNameWithPath = $"{_dataFilePath}Dates.txt"; 

            using (NASA_FileService fs = new NASA_FileService())
            {
                dates = fs.ReadDatesFile();
            }
            Assert.IsNotNull(dates);
        }
        [TestMethod]
        public async Task Test_SaveNASAPhoto()
        {
            string remoteFileIncludingPath = "https://codeproject.freetls.fastly.net/App_Themes/CodeProject/Img/logo250x135.gif";
            string localFileFolder = $"{_dataFilePath}Photos\\";
            string localFileIncludingPath = $"{localFileFolder}{Path.GetFileName(remoteFileIncludingPath)}";

            using (NASA_FileService fs = new NASA_FileService())
            {
                await fs.SaveNASAPhoto(remoteFileIncludingPath, localFileIncludingPath);
            }
        }

        [TestMethod]
        public async Task Test_NASA_Driver()
        {
            using (NASA_Driver driver = new NASA_Driver(_dataFilePath, _apiService, _fileService))
            {
                await driver.DownLoadNASAPhotos();
            }
        }

        [TestMethod]
        public void Test_ConvertDate()
        {
            List<string> inputDateList = new List<string>
            {
                "02/27/17",
                "June 2, 2018",
                "Jul-13-2016",
                "April 31, 2018", // This is an invalid date - Date does not exist
                "December 1, 2018"
            };
            // Expected dates in the format "yyyy-MM-dd" i.e. 2020 - 12 - 01
            List<string> expectedDateList = new List<string>
            {
                "2017-02-27",
                "2018-06-02",
                "2016-07-13",
                null, // This is an invalid date - Date does not exist
                "2018-12-01"
            };

            List<string> outputDateList = new List<string>();

            string outputDateString;

            using (NASA_Driver driver = new NASA_Driver(_dataFilePath, _apiService, _fileService))
            {
                foreach (string dateString in inputDateList)
                {
                    outputDateString = driver.ConvertDateString(dateString);
                    outputDateList.Add(outputDateString);
                }
            }
            Assert.AreEqual(JsonConvert.SerializeObject(expectedDateList),
                JsonConvert.SerializeObject(outputDateList));
        }

        [TestMethod]
        public async Task Test_CreateRequiredFolders()
        {            
            using (NASA_FileService fs = new NASA_FileService())
            {
                fs.CreateRequiredFolders();
            }
        }
    }
}

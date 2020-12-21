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

        //ReadTextFile(string fileName)
        [TestMethod]
        public async Task Test_ReadDatesFile()
        {
            List<string> dates;
            string fileNameWithPath = $"{_dataFilePath}DatesFile.txt"; // "DatesFileFolder\\DatesFile.txt";

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
                "April 31, 2018",
                "December 1, 2018"
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

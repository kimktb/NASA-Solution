using NASA_Project.Interfaces;
using NASA_Project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace NASA_Project
{
    public class NASA_Driver :IDisposable
    {
        private string _dataFilePath;
        private string _inputFileName;
        private string _inputFileNameAndPath;
        private string _outputFilePath;
        private INASA_API_Service _apiService;
        private INASA_FileService _fileService;

        public NASA_Driver(string dataFilePath, INASA_API_Service apiService, INASA_FileService fileService)
        {
            _apiService = apiService;
            _fileService = fileService;
            _dataFilePath = dataFilePath;
            _inputFileName = "DateFiles.txt";
            _inputFileNameAndPath = $"{_dataFilePath}{_inputFileName}";
            _outputFilePath = $"{_dataFilePath}Photos/";
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public async Task DownLoadNASAPhotos() 
        {
            try
            {
                _fileService.CreateRequiredFolders();

                List<string> dateList = _fileService.ReadDatesFile();

                foreach (string ds in dateList)
                {
                    string dateString = ConvertDateString(ds);
                    if (dateString != null)
                    {
                        HTTPResponseDetails responseDetails = await _apiService.GetNASAPhotosAsync(dateString);
                        if (responseDetails.ResponseStatus == System.Net.HttpStatusCode.OK)
                        {
                            NASA_ResponseObject nasaResponseObject = JsonConvert.DeserializeObject<NASA_ResponseObject>(responseDetails.ResponseResult);
                            foreach (Photo p in nasaResponseObject.photos)
                            {
                                string filename = Path.GetFileName(p.img_src);
                                await _fileService.SaveNASAPhoto(p.img_src, $"{_outputFilePath}{filename}");
                            }
                        }
                        else
                        {
                            // do some logging
                            throw new Exception();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                //Do Some logging, re throw
                throw;
            }
        }

        public string ConvertDateString(string inputDateString) 
        {
            DateTime dateResult;

            string[] formats = new string[] 
            {
                @"MM/dd/yy",        // Date format: "02/27/17"
                @"MM/dd/yyyy",      // Date format: "02/27/2017"
                @"MMMM d, yyyy",    // Date format: "June 2, 2018"
                @"MMM-d-yyyy",      // Date format: "Jul-13-2016"
                @"MMMM dd, yyyy"    // Date format: "April 31, 2018" -- Invalid date
            };

            CultureInfo[] cultures = { new CultureInfo("en-US")};

            if (DateTime.TryParseExact(inputDateString.Trim(), formats,
                System.Globalization.CultureInfo.CurrentCulture, 
                DateTimeStyles.None, out dateResult))
            {
                return dateResult.ToString("yyyy-MM-dd");
            }
            return null;
        }
    }
}

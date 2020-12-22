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
            _inputFileName = "Dates.txt";
            _inputFileNameAndPath = $"{_dataFilePath}{_inputFileName}";
            _outputFilePath = $"{_dataFilePath}Photos/";
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Download NASA files for each date in Dates.txt
        /// </summary>
        /// <returns></returns>
        public async Task DownLoadNASAPhotos() 
        {
            try
            {
                // Create the required folder structure
                _fileService.CreateRequiredFolders();

                // Retrieve list of dates from Dates.txt
                List<string> dateList = _fileService.ReadDatesFile();

                // Process each date
                foreach (string ds in dateList)
                {
                    // Convert each date to format "yyyy-MM-dd" i.e. 2020-12-01 
                    string dateString = ConvertDateString(ds);

                    // Check for date in the correct format
                    if (dateString != null)
                    {
                        // Call Mars Rover Photos API Request for the specified date
                        HTTPResponseDetails responseDetails = await _apiService.GetNASAPhotosAsync(dateString);
                        
                        // If the response is ok, deserialize to NASA_ResponseObject model
                        if (responseDetails.ResponseStatus == System.Net.HttpStatusCode.OK)
                        {
                            // Deserialize the response
                            NASA_ResponseObject nasaResponseObject = JsonConvert.DeserializeObject<NASA_ResponseObject>(responseDetails.ResponseResult);
                            
                            // Loop through the List<Photo>
                            foreach (Photo p in nasaResponseObject.photos)
                            {
                                // Get the file name part of the file name
                                string filename = Path.GetFileName(p.img_src);

                                // Save the photo to a local folder
                                await _fileService.SaveNASAPhoto(p.img_src, $"{_outputFilePath}{filename}");
                            }
                        }
                        else
                        {
                            // Do some logging
                            throw new Exception();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                //Do Some logging, rethrow
                throw;
            }
        }

        /// <summary>
        /// Convert each date to format "yyyy-MM-dd" i.e. 2020-12-01
        /// </summary>
        /// <param name="inputDateString"></param>
        /// <returns></returns>
        public string ConvertDateString(string inputDateString) 
        {
            DateTime dateResult;

            // Format type that we expect to interpret
            string[] formats = new string[] 
            {
                @"MM/dd/yy",        // Date format: "02/27/17"
                @"MM/dd/yyyy",      // Date format: "02/27/2017"
                @"MMMM d, yyyy",    // Date format: "June 2, 2018"
                @"MMM-d-yyyy",      // Date format: "Jul-13-2016"
                @"MMMM dd, yyyy"    // Date format: "April 31, 2018" -- Invalid date
            };

            // Define the culture for the dates
            CultureInfo[] cultures = { new CultureInfo("en-US")};

            // Attempt to parse out the date in the required format yyyy-MM-dd
            if (DateTime.TryParseExact(inputDateString.Trim(), formats,
                System.Globalization.CultureInfo.CurrentCulture, 
                DateTimeStyles.None, out dateResult))
            {
                // Sucessfully parsed the date
                return dateResult.ToString("yyyy-MM-dd");
            }
            // Unsucessful date parse
            return null;
        }
    }
}

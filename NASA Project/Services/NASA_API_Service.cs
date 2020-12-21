using NASA_Project.Interfaces;
using NASA_Project.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NASA_Project.Services
{
    public class NASA_API_Service : INASA_API_Service
    {
        private string _uri;
        private string _apiKey;

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public NASA_API_Service(string uri, string apiKey)
        {
            _uri    = uri;
            _apiKey = apiKey;
        }

        public async Task<HTTPResponseDetails> GetNASAPhotosAsync(string photoDate)
        {
            HTTPResponseDetails responseDetails = new HTTPResponseDetails();
            string response;
            Stream dataStream;
            string method = "GET";
            
            string endpoint = $"{_uri}?earth_date={photoDate}&api_key={_apiKey}"; //?earth_date=2020-12-11&api_key=DEMO_KEY

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);
                request.Method = method;
                request.Date = DateTime.Now;

                using (HttpWebResponse webResponse = (HttpWebResponse)await request.GetResponseAsync())
                {
                    // Get content received after request
                    dataStream = webResponse.GetResponseStream();

                    StreamReader reader = new StreamReader(dataStream);

                    // Read to the end of the stream
                    response = reader.ReadToEnd();

                    // Save the response and the response code into the object returned by this method
                    responseDetails.ResponseResult = response;
                    responseDetails.ResponseStatus = webResponse.StatusCode;
                }
            }
            catch (WebException ex)
            {
                if (ex?.Response != null)
                {
                    using (WebResponse exResponse = ex.Response)
                    {
                        HttpWebResponse exWebResponse = (HttpWebResponse)exResponse;
                        using (Stream reader = exResponse.GetResponseStream())
                        {
                            string responseStream = new StreamReader(reader).ReadToEnd();
                            responseDetails.ResponseResult = responseStream;
                        }
                        responseDetails.ResponseStatus = exWebResponse.StatusCode;
                    }
                }
                else
                {
                    responseDetails.ResponseResult = ex.ToString();
                }
            }
            catch (Exception ex)
            {
                //log here
            }
            return responseDetails;
        }
    }
}

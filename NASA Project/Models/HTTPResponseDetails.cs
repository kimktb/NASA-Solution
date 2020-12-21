using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NASA_Project.Models
{
    public class HTTPResponseDetails
    {
        public HttpStatusCode ResponseStatus { get; set; }
        public string ResponseResult { get; set; }
    }
}

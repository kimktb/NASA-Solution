using NASA_Project.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NASA_Project.Interfaces
{
    public interface INASA_API_Service : IDisposable
    {
        Task<HTTPResponseDetails> GetNASAPhotosAsync(string photoDate );
    }
}

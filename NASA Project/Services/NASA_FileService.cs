using NASA_Project.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NASA_Project.Services
{
    public class NASA_FileService : INASA_FileService
    {
        private static string _saveFilePathRoot = $"C:\\Temp\\DataFiles\\";
        private static string _dateFileNameWithPath = $"{_saveFilePathRoot}DatesFile.txt";
        private static string _saveFilePathPhotos = $"{_saveFilePathRoot}Photos\\";
        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public NASA_FileService()
        {

        }

        public List<string> ReadDatesFile()
        {
            List<string> dateList = null;
            try
            {
                string[] dates = System.IO.File.ReadAllLines(_dateFileNameWithPath);
                dateList = dates.ToList();
            }
            catch (Exception ex)
            {

            }
            return dateList;
        }

        public async Task SaveNASAPhoto(string remoteFileIncludingPath, string localFileIncludingPath)
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFileAsync(new Uri(remoteFileIncludingPath), localFileIncludingPath);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void CreateRequiredFolders()
        {
            string path;
            try
            {
                path = _saveFilePathRoot;
                // Determine whether the directory exists.
                if (Directory.Exists(path))
                {
                    Console.WriteLine($"That path exists already.{path}");
                }
                else
                {
                    // Try to create the directory.
                    DirectoryInfo root = Directory.CreateDirectory(path);
                    Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(path));
                }
                path = _saveFilePathPhotos;
                // Determine whether the directory exists.
                if (Directory.Exists(path))
                {
                    Console.WriteLine($"That path exists already.{path}");
                }
                else
                {
                    // Try to create the directory.
                    DirectoryInfo root = Directory.CreateDirectory(path);
                    Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(path));
                }
                if (Directory.Exists(path))
                { 
                    CreateDateFile();
                }
                Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(path));
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
            finally { }
        }

        private void CreateDateFile()
        {
            string path = _dateFileNameWithPath;
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("02/27/17");
                    sw.WriteLine("June 2, 2018");
                    sw.WriteLine("Jul-13-2016");                    
                    sw.WriteLine("April 31, 2018");
                }
            }
        }
    }
}

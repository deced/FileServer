using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Server.Entities;
using File = Server.Entities.File;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class FileController : ControllerBase
    {
        private static string startFolder = "Files/";
        private static List<File> files;
        
        
        [HttpGet]
        public async Task<string> GetFiles(string path)
        {
            string[] allfiles = Directory.GetFileSystemEntries(startFolder + path?.TrimStart('/'));
            string backwardsPath = "";
            if (path != null)
            {
                int lastIndex = path.LastIndexOf("/");
                backwardsPath = path.Substring(0, lastIndex);
            }

            files = new List<File>()
                {new File() {Name = "../", FileType = FileType.Backwards, BackPath = backwardsPath}};
            foreach (var filePath in allfiles)
            {
                File file = new File();
                if (System.IO.File.Exists(filePath))
                    file.FileType = FileType.File;
                else
                    file.FileType = FileType.Directory;
                file.Name = filePath.TrimStart("Files".ToCharArray());
                file.BackPath = path ?? "/";
                files.Add(file);
            }

            files = files.OrderByDescending(x => x.FileType).ToList();
            return JsonConvert.SerializeObject(files);
        }

        [HttpGet]
        public async Task<string> GetFile(string path)
        {
            return await System.IO.File.ReadAllTextAsync(startFolder + path.TrimStart('/'));
        }

        [HttpPut]
        public async Task AppendFile(string path, string text)
        {
            var sw = System.IO.File.AppendText(startFolder + path.TrimStart('/'));
            await sw.WriteAsync(text);
            sw.Close();
        }

        [HttpPost]
        public void Move(string source, string dest)
        {
            string fileName = Path.GetFileName(source);
            if(dest == "/")
                System.IO.File.Move(startFolder +source.TrimStart('/'),startFolder +fileName);
            else
                System.IO.File.Move(startFolder +source.TrimStart('/'),startFolder+dest.TrimStart('/').TrimEnd('/')+"/"+fileName);
        }
        
        [HttpPost]
        public void Copy(string source, string dest)
        {
            string fileName = Path.GetFileName(source);
            
            if(source == dest+fileName)
                return;
            
            if(dest == "/")
                System.IO.File.Copy(startFolder +source.TrimStart('/'),startFolder +fileName);
            else
                System.IO.File.Copy(startFolder +source.TrimStart('/'),startFolder+dest.TrimStart('/').TrimEnd('/')+"/"+fileName);
        }

        [HttpDelete]
        public async Task Delete(string path)
        {
            System.IO.File.Delete(startFolder+path.TrimStart('/'));
        }

        [HttpPost]
        public async Task CreateFile(string fileName)
        {
            System.IO.File.Create(startFolder+fileName).Close();
        }
        
        [HttpPost]
        public async Task CreateDirectory(string directoryName)
        {
            System.IO.Directory.CreateDirectory(startFolder + directoryName);
        }
    }
}
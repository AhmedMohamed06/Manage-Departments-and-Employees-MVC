using Microsoft.AspNetCore.Http;
using System.IO;
using System;

namespace Demo.Pl.Helper
{
    public class DocumentSettings
    {
        // Upload
        public static string UpLoadFile(IFormFile file, string FolderName)
        {
            //1-Get Located Folder Path
            //F:\Route\Route C42\Cycle42\05 MVC\G01\Session03\DemoMvc.G01\Demp.Pl\wwwroot\Files\Images\
            string FolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", FolderName);
            //2- Get File Name And Make It Unique
            string FileName = $"{Guid.NewGuid()}{file.FileName}";
            //3- Get File Path[Folder Path+ FileName]
            string FilePath = Path.Combine(FolderPath, FileName);
            //4- Save  File As Streams
            using var FileStream = new FileStream(FilePath, FileMode.Create);
            file.CopyTo(FileStream);
            //- Return File Name 
            return FileName;
        }




        // Delete
        public static void DeleteFile(string FileName, string FolderName)
        {
            //1- Get File Path
            string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", FolderName, FileName);
            //2- Check If File Exists Or Not 
            if (File.Exists(FilePath))
            {
                // If Exists Remove It
                File.Delete(FilePath);
            }

        }
    }
}

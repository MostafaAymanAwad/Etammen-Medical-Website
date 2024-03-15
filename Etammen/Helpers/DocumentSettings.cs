namespace Etammen.Helpers
{
    public static class DocumentSettings
    {
        public static string UploadFile(IFormFile file, string folderName)
        {
            // 1. Get Located Folder Path

            //string folderPath = "D:\\Route\\Cycle 39\\07 ASP.NET Core MVC\\Session 05\\Demos\\G01 Demo Solution\\Demo.PL\\wwwroot\\files\\";
            //string folderPath = Directory.GetCurrentDirectory() + "\\wwwroot\\files\\" + folderName;
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", folderName);

            // 2. Get File Name and Make it UINQUE
            string fileName = $"{Guid.NewGuid()}{file.FileName}";

            // 3. Get File Path

            string filePath = Path.Combine(folderPath, fileName);

            // 4. Save File as Streams : [Data Per Time]

            using var fs = new FileStream(filePath, FileMode.Create); //we are disposing the file it is like the connection to the database cause file is stored in the database so we use using like the dbconnection

            file.CopyTo(fs);

            return fileName;

        }


        public static void DeleteFile(string fileName, string folderName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", folderName, fileName);

            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}

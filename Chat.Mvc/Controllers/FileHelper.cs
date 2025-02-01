namespace Chat.Mvc.Controllers
{

    public class FileHelper
    {
        public static async Task<string> SaveFileAsync(IFormFile file, string uploadPath)
        {
            try
            {
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return fileName; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar el archivo: {ex.Message}");
                throw;
            }
        }

    }

}

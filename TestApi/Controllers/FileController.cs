using Microsoft.AspNetCore.Mvc;

namespace TestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController: ControllerBase
    {

        /// <summary>
        /// API to serve a file. Useful for authenticated downloads
        /// </summary>
        /// <returns></returns>
        [HttpGet("{fileName}") ]

        public async Task<IActionResult> Get(string fileName)
        {
            string localFileName = $"{Environment.CurrentDirectory}\\{fileName}";
            if (System.IO.File.Exists(localFileName))
            {
                using (var stream = System.IO.File.OpenRead(localFileName))
                {
                    string mimeType = "application/octet-stream";
                    var bytes = await System.IO.File.ReadAllBytesAsync(localFileName);
                    return File(bytes, mimeType, Path.GetFileName(localFileName));
                }
            }
            else
            {
                return NotFound();
            }
        }

    }
}

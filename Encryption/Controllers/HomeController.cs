using Encryption.Interface;
using Encryption.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Encryption.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        private readonly IExternalApiService _externalApiService;

        public HomeController(IExternalApiService externalApiService)
        {
            _externalApiService = externalApiService;
        }

        [HttpPost("send-encrypted")]
        public async Task<IActionResult> SendEncryptedData([FromBody] MyDataModel data)
        {
            try
            {
                string externalApiUrl = "https://localhost:7014/api/Home/receive-encrypted";

                // Send encrypted data and get decrypted response
                string response = await _externalApiService.SendEncryptedDataAsync(data, externalApiUrl);

                return Ok(new { message = "Success", response = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error", error = ex.Message });
            }
        }
    }
}

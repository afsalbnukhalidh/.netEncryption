using System.Text.Json;
using apiEndpoint.Model;
using apiEndpoint.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Encryption.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {

        private readonly AesEncryptionHelper _aesHelper;

        public HomeController()
        {
            // Key and IV must match the sender's encryption logic
            string aesKey = "IUU+w1HAa9k7bGtT9zb9asJJc42kBbq/l6o3DG2+3v0=";
            string aesIV = "I/hhunYukZED5bY1sJ/V1g==";
            _aesHelper = new AesEncryptionHelper(aesKey, aesIV);
        }

        [HttpPost("receive-encrypted")]
        public IActionResult ReceiveEncryptedData([FromBody] JsonElement payload)
        {
            try
            {

                // Extract the encryptedData correctly
                if (!payload.TryGetProperty("encryptedData", out JsonElement encryptedElement))
                {
                    return BadRequest(new { message = "Error", error = "Invalid input format: 'encryptedData' is missing" });
                }

                string encryptedData = encryptedElement.GetString();
                string decryptedData = _aesHelper.Decrypt(encryptedData);

                MyDataModel receivedData = Newtonsoft.Json.JsonConvert.DeserializeObject<MyDataModel>(decryptedData);

                var responseList = receivedData.data.Select(item => new DataReturn
                {
                    Name = item.FirstName.ToUpper() +" "+ item.LastName.ToUpper(),
                    Age = item.Age 
                }).ToList();

                var responseModel = responseList;

                string jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(responseModel);
                string encryptedResponse = _aesHelper.Encrypt(jsonResponse);

                return Ok(new { encryptedResponse });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error", error = ex.Message });
            }
        }


    }
}

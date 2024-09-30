using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly string reCaptchaSecretKey = Environment.GetEnvironmentVariable("SECRET_KEY");
        private readonly string GoogleVerifyUri = "https://www.google.com/recaptcha/api/siteverify";

        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            RestClient restClient = new RestClient();
            RestRequest restRequest = new RestRequest(GoogleVerifyUri, Method.Post);
            restRequest.AddParameter("secret", reCaptchaSecretKey);
            restRequest.AddParameter("response", loginDto.CaptchaResponse);

            var response = await restClient.ExecuteAsync(restRequest);
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<reCaptchaResponseDto>(response.Content);
                if (result.Success)
                    return Ok(response.Content);
            }

            return BadRequest(response.Content);
        }
    }
    public class LoginDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string CaptchaResponse { get; set; }
    }

    public class reCaptchaResponseDto
    {
        public bool Success { get; set; }
        public string[] ErrorCodes { get; set; }
    }
}

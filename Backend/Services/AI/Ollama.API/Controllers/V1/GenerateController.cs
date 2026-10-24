using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace Ollama.API.Controllers.V1
{

    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    public class GenerateController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public GenerateController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpPost]
        public async Task<OllamaResponse> Post([FromBody] string question)
        {
            var requestData = new
            {
                model = "llama3.2:1b",
                prompt = question,
                stream = false
            };

            string json = JsonSerializer.Serialize(requestData);

            using var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("http://host.docker.internal:11434/api/generate", content);

            string responseJson = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<OllamaResponse>(responseJson);
            return result ?? new OllamaResponse { response = "Error: Unable to deserialize response.", done = false };
        }

    }

    public class OllamaResponse
    {
        public string response { get; set; } = string.Empty;
        public bool done { get; set; }
    }
}

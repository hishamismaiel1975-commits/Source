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
            _httpClient.BaseAddress = new Uri("http://host.docker.internal:11434");
        }

        [HttpPost("Generate")]
        public async Task<OllamaResponse> Generate([FromBody] string question)
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

            var response = await _httpClient.PostAsync("/api/generate", content);

            string responseJson = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<OllamaResponse>(responseJson);
            return result ?? new OllamaResponse { response = "Error: Unable to deserialize response.", done = false };
        }

        [HttpPost("GenerateFromOurPolicy")]
        public async Task<OllamaResponse> GenerateFromOurPolicy([FromBody] string question)
        {

            // 1. Load document
            string filePath = Path.Combine(AppContext.BaseDirectory, "policy.txt");
            string document = await System.IO.File.ReadAllTextAsync(filePath);

            // 2. Split document into simple chunks
            var chunks = document.Split(
                '\n',
                StringSplitOptions.RemoveEmptyEntries |
                StringSplitOptions.TrimEntries);

            // 3. Create embeddings for all chunks
            var vectors = new List<float[]>();

            foreach (var chunk in chunks)
            {
                vectors.Add(await GetEmbedding(chunk));
            }

            // Convert question to an embedding
            var questionVector = await GetEmbedding(question);

            // Find the most similar chunk
            int bestIndex = 0;
            double bestScore = double.NegativeInfinity;

            for (int i = 0; i < vectors.Count; i++)
            {
                double score = CosineSimilarity(
                    questionVector, vectors[i]);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestIndex = i;
                }
            }

            // 5. Retrieve relevant context
            string context = chunks[bestIndex];

            // 6. Ask Llama to answer using that context
            string prompt = $"""
        Answer using ONLY the context below.
        If the answer is not present, say:
        "I couldn't find the answer in the document."

        Context:
        {context}

        Question:
        {question}
        """;

            var result = await Generate(prompt);
            return result;
        }

        private async Task<float[]> GetEmbedding(string text)
        {
            var response = await _httpClient.PostAsJsonAsync("api/embed",
                new
                {
                    model = "nomic-embed-text",
                    input = text
                });

            response.EnsureSuccessStatusCode();

            using var json = JsonDocument.Parse(
                await response.Content.ReadAsStringAsync());

            return json.RootElement
                .GetProperty("embeddings")[0]
                .EnumerateArray()
                .Select(x => x.GetSingle())
                .ToArray();
        }

        // Compare two vectors
        private double CosineSimilarity(float[] a, float[] b)
        {
            double dot = 0, normA = 0, normB = 0;

            for (int i = 0; i < a.Length; i++)
            {
                dot += a[i] * b[i];
                normA += a[i] * a[i];
                normB += b[i] * b[i];
            }

            return dot / (Math.Sqrt(normA) * Math.Sqrt(normB));
        }
    }

    public class OllamaResponse
    {
        public string response { get; set; } = string.Empty;
        public bool done { get; set; }
    }




}

using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace Ollama.API.Controllers.V1
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    public class SimpleAgentController : ControllerBase
    {
        private readonly HttpClient _http;

        public SimpleAgentController(HttpClient http)
        {
            _http = http;
            _http.BaseAddress = new Uri("http://host.docker.internal:11434");
        }

        [HttpPost("Ask")]
        public async Task<string> Ask([FromBody] string question)
        {
            // 1. The conversation (we resend it every time, the model has no memory)
            var messages = new JsonArray
            {
                new JsonObject { ["role"] = "user", ["content"] = question }
            };

            // 2. The tool list: tell the model that get_date exists
            var tools = new JsonArray
            {
                new JsonObject
                {
                    ["type"] = "function",
                    ["function"] = new JsonObject
                    {
                        ["name"] = "get_date",
                        ["description"] = "Get today's date. Use this when the user asks about the date or day.",
                        ["parameters"] = new JsonObject
                        {
                            ["type"] = "object",
                            ["properties"] = new JsonObject()   // this tool needs no input
                        }
                    }
                }
            };

            // 3. The loop: Ask -> Run -> Return -> Repeat (max 3 times)
            for (int i = 0; i < 3; i++)
            {
                // ASK: send the conversation + tools to the model
                var request = new JsonObject
                {
                    ["model"] = "llama3.2:3b",
                    ["messages"] = messages.DeepClone(),
                    ["tools"] = tools.DeepClone(),
                    ["stream"] = false
                };

                var response = await _http.PostAsJsonAsync("api/chat", request);
                var reply = JsonNode.Parse(await response.Content.ReadAsStringAsync());
                var message = reply["message"];

                // Did the model ask for a tool?
                var toolCalls = message["tool_calls"]?.AsArray();

                // NO tool request -> this is the final answer, we are done
                if (toolCalls == null || toolCalls.Count == 0)
                {
                    return message["content"].ToString();
                }

                // YES tool request -> save the model's message first
                messages.Add(message.DeepClone());

                foreach (var call in toolCalls)
                {
                    string toolName = call["function"]["name"].ToString();

                    // RUN: our code does the real work
                    string result = "Unknown tool";
                    if (toolName == "get_date")
                    {
                        result = GetDate();
                    }

                    // RETURN: give the result back to the model
                    messages.Add(new JsonObject
                    {
                        ["role"] = "tool",
                        ["content"] = result
                    });
                }
                // REPEAT: the loop runs again, now the model can see the result
            }

            return "Agent stopped: too many steps.";
        }

        // The tool: just a normal C# method
        private string GetDate()
        {
            return DateTime.UtcNow.ToString();
        }
    }
}
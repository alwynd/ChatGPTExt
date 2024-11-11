using System.Net.Http.Headers;
using System.Text.Json;
using TextCopy;
using static System.Net.Mime.MediaTypeNames;

namespace ChatGPTExt
{
    /// <summary>
    /// Provides ChatGPT functionality.
    /// </summary>
    public class ChatGPT
    {
        public string Model { get; set; } = "gpt-4o-mini";
        public string ModelsUrl { get; set; } = "https://api.openai.com/v1/models";
        public string Url { get; set; } = "https://api.openai.com/v1/chat/completions";

        public string OrganizationId { get; set; } = "";
        public string ProjectId { get; set; } = "";
        public string ApiKey { get; set; } = "";
        public bool AppendCodeFromClipboard { get; set; } = true;

        public string SystemMessage { get; set; } =
            "You are an expert code reviewer and assistant specialized in optimization, efficiency, and best practices. Your tasks include: \n" +
            " - Reviewing code with a focus on optimization, readability, maintainability, and adherence to best practices. \n" +
            " - Providing specific recommendations to improve performance, memory usage, and overall efficiency, without altering the code's intended functionality. \n" +
            " - Clearly explaining the reasoning behind suggested changes, such as code patterns, language-specific features, or potential edge cases. \n" +
            " - Include unmodified code in your response so that users can easily replace the code in question from your response, since it will be copied from the clipboard in most cases. \n" +
            " - Being concise and direct, addressing only necessary modifications without excessive explanation, unless clarification is requested. \n" +
            "Provide a thorough yet efficient review to help developers write optimal, production-ready code, when asked to do so.";

        /// <summary>
        /// ChatGPT invocation.
        /// The request will be amended as follows:
        /// - If there is text in the clipboard, it will be assumed to be code, and will be appended to the contents of the request file such as:
        ///     request text from requestFile
        ///     ```
        ///     clipboard text
        ///     ```
        /// </summary>
        public async Task<string> InvokeChatGPT(string requestFile)
        {
            Program.Debug($"{GetType().Name}.InvokeChatGPT:-- START, , model: {Model}, system: {SystemMessage}, requestFile: {requestFile}");
            string request = await File.ReadAllTextAsync(requestFile);

            if (AppendCodeFromClipboard)
            {
                try
                {
                    Clipboard history = new();
                    string text = await history.GetTextAsync();
                    Program.Debug($"{GetType().Name}.InvokeChatGPT got clipboard text: {text}");

                    if (!string.IsNullOrEmpty(text))
                    {
                        Program.Debug($"{GetType().Name}.InvokeChatGPT APPENDING FROM CLIPBOARD: {text}");
                        request += $"{Environment.NewLine}```code{Environment.NewLine}{text}{Environment.NewLine}```{Environment.NewLine}";
                    }
                }
                catch (Exception ex)
                {
                    Program.Debug($"{GetType().Name}.InvokeChatGPT Warning: Clipboard does not contain text. {ex.Message}");
                }
            }

            Program.Debug($"{GetType().Name}.InvokeChatGPT request: {request.Length}, request: {request.Length}");

            if (string.IsNullOrEmpty((ApiKey))) throw new Exception("API Key is mandatory.");
                if (string.IsNullOrEmpty((SystemMessage))) throw new Exception("System Message is mandatory.");

                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
                if (!string.IsNullOrEmpty(OrganizationId)) client.DefaultRequestHeaders.Add("OpenAI-Organization", OrganizationId);
                if (!string.IsNullOrEmpty(ProjectId)) client.DefaultRequestHeaders.Add("OpenAI-Project", ProjectId);

                string json = JsonSerializer.Serialize(new
                {
                    model = Model,
                    messages = new[]
                    {
                        new { role = "system", content = SystemMessage },
                        new { role = "user", content = request }
                    },
                });

                int random = Math.Abs(new Random().Next());
                HttpResponseMessage response = await client.PostAsync(Url, new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
                Program.Debug($"{GetType().Name}.InvokeChatGPT, request: {request.Length}, response.StatusCode: {response.StatusCode}");

                Program.Debug($"{GetType().Name}.InvokeChatGPT, request#: {random} request: {request}");
                Program.Debug($"{GetType().Name}.InvokeChatGPT, request#: {random} response: {response}");

                string result = await response.Content.ReadAsStringAsync();
                Program.Debug($"{GetType().Name}.InvokeChatGPT, request#: {random} result: {result}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(result);
                }

                string content = string.Empty;
                using (JsonDocument doc = JsonDocument.Parse(result))
                {
                    JsonElement root = doc.RootElement;
                    if (root.TryGetProperty("choices", out JsonElement choices) && choices.GetArrayLength() > 0)
                    {
                        content = choices[0].GetProperty("message").GetProperty("content").GetString();
                    }
                }

                Program.Debug($"{GetType().Name}.InvokeChatGPT, request#: {random} content: {content}");
                return content;

        }


        /// <summary>
        /// List available gpt models.
        /// </summary>
        public async Task<List<string>> ListModels()
        {
            Program.Debug($"{GetType().Name}.ListModels:-- START");

            if (string.IsNullOrEmpty((ApiKey))) throw new Exception("API Key is mandatory.");

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);

            var response = await client.GetAsync(ModelsUrl);
            Program.Debug($"{GetType().Name}.ListModels, response.StatusCode: {response.StatusCode}");
            Program.Debug($"{GetType().Name}.ListModels, response: {response}");

            string result = await response.Content.ReadAsStringAsync();
            Program.Debug($"{GetType().Name}.ListModels, result: {result}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(result);
            }

            List<string> models = new();
            using (JsonDocument doc = JsonDocument.Parse(result))
            {
                foreach (var element in doc.RootElement.GetProperty("data").EnumerateArray())
                {
                    models.Add(element.GetProperty("id").GetString());
                }
            }

            Program.Debug($"{GetType().Name}.ListModels models: {models.Count}");
            models.OrderBy(x => x).ToList().ForEach(model =>
            {
                Program.Debug($"{GetType().Name}.ListModels Model: {model}");
            });

            return models;
        }

    }


}

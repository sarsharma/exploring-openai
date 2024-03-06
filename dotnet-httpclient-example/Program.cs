using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text;

class Program
{
    static string extractionPrompt = "Extract the person name, company name, location and phone number from the text below and create it into structured json with keys person_name, company_name, location, phone_number";
    static async Task Main(string[] args)
    {
        // Load configuration
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        string openAIEndpoint = configuration.GetSection("AzureOpenAI:Endpoint").Value;
        string openAIKey = configuration.GetSection("AzureOpenAI:Key").Value;

        // Read text file
        string inputFile = "input.txt";
        string[] entries = File.ReadAllText(inputFile).Split(new[] { Environment.NewLine, "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

        List<OpenAIRequest> openAIRequests = new List<OpenAIRequest>();

        // Create OpenAI requests for each entry [Context window limit]
        foreach (var entry in entries)
        {
            openAIRequests.Add(new OpenAIRequest(entry.Trim()));
        }

        // Make OpenAI API requests and extract structured data
        List<string> structuredDataList = await ProcessOpenAIRequestsAsync(openAIRequests, openAIEndpoint, openAIKey);

        // Print structured data, we can now use this structured data for further processing
        for (int i = 0; i < structuredDataList.Count; i++)
        {
            Console.WriteLine($"Entry {i + 1}: {structuredDataList[i]}");
        }
    }

    static async Task<List<string>> ProcessOpenAIRequestsAsync(List<OpenAIRequest> openAIRequests, string OpenAIEndpoint, string OpenAIKey)
    {
        List<string> structuredDataList = new List<string>();

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("api-key", $"{OpenAIKey}");

            foreach (var request in openAIRequests)
            {
                string json_converted_properties = await MakeOpenAIRequestAsync(client, OpenAIEndpoint, request);

                structuredDataList.Add(json_converted_properties);
            }
        }

        return structuredDataList;
    }

    static async Task<string> MakeOpenAIRequestAsync(HttpClient client, string apiUrl, OpenAIRequest request)
    {
        var requestBody = new
        {
            messages = new[] { new 
            { role = "user", content = extractionPrompt + "\n\n" + request.Text }
            },
            max_tokens = 150
        };

        var jsonRequest = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(apiUrl, content);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        JsonElement root = JsonDocument.Parse(jsonResponse).RootElement;
        JsonElement choicesElement = root.GetProperty("choices")[0];
        JsonElement messageElement = choicesElement.GetProperty("message");
        string resultContent = messageElement.GetProperty("content").GetString();
        return resultContent;
    }

}

class OpenAIRequest
{
    public string Text { get; set; }

    public OpenAIRequest(string text)
    {
        Text = text;
    }
}

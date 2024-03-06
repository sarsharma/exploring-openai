using Azure;
using Azure.AI.OpenAI;
using Azure.Core;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Text.Json;

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
        string deploymentName = configuration.GetSection("AzureOpenAI:DeploymentName").Value;

        // Create OpenAI client using SDK

        OpenAIClient client = new OpenAIClient(
                new Uri(openAIEndpoint),
                new AzureKeyCredential(openAIKey));


        // Read text file
        string inputFile = "input.txt";
        string[] entries = File.ReadAllText(inputFile).Split(new[] { Environment.NewLine, "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

        List<string> structuredDataList = new List<string>();

        foreach (var entry in entries)
        {
            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                DeploymentName = deploymentName,
                Messages =
                {
                    // The system message represents instructions or other guidance about how the assistant should behave
                    new ChatRequestSystemMessage(extractionPrompt),
                    // User messages represent current or historical input from the end user
                    new ChatRequestUserMessage(entry),
                },
                //MaxTokens = 200,
                //Temperature = (float?)0.7,
            };


            Response<ChatCompletions> response = await client.GetChatCompletionsAsync(chatCompletionsOptions);
            ChatResponseMessage responseMessage = response.Value.Choices[0].Message;
            structuredDataList.Add(responseMessage.Content.ToString());
        }
        // Print structured data, we can now use this structured data for further processing
        for (int i = 0; i < structuredDataList.Count; i++)
        {
            Console.WriteLine($"Entry {i + 1}: {structuredDataList[i]}");
        }
    }
}

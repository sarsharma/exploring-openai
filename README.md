# Synergies with AI - Intro to Azure Open AI SDKs and Frameworks

## Creating Azure OpenAI resource

* Currently, Azure OpenAI is invite only and we need to fill this application to onboard our subscriptions.
  * <https://aka.ms/oai/access>
* Once the subscription is onboarded, we can follow this doc to create openai resources.
  * <https://learn.microsoft.com/en-us/azure/ai-services/openai/how-to/create-resource?pivots=web-portal>

### Using Azure OpenAI API

#### With cURL

* Hello world example

```bash
curl "https://<your-azure-openai-resource-name>.openai.azure.com/openai/deployments/<deployment-name>/chat/completions?api-version=2024-02-15-preview" \
  -H "Content-Type: application/json" \
  -H "api-key: <your-api-key>" \
  -d "{
  \"messages\": [{\"role\":\"user\",\"content\":\"Write a python code to take username as input and print Hello username to the console\"}]
}"

```

* Generating content for your brand via a prompt
```bash
curl "https://<your-azure-openai-resource-name>.openai.azure.com/openai/deployments/<deployment-name>/chat/completions?api-version=2024-02-15-preview" \
  -H "Content-Type: application/json" \
  -H "api-key: <your-api-key>" \
  -d "{
  \"messages\": [{\"role\":\"system\",\"content\":\"You are a marketing writing assistant. You help come up with creative content ideas and content like marketing emails, blog posts, tweets, ad copy and product descriptions. You write in a friendly yet professional tone but can tailor your writing style that best works for a user-specified audience. If you do not know the answer to a question, respond by saying I do not know the answer to your question.\"},{\"role\":\"user\",\"content\":\"My company sells sustainable bamboo water bottles. The company is launching a new line of bottles with vibrant, eye-catching designs. Please write a compelling social media post promoting the new collection.\"}],
  \"max_tokens\": 800,
  \"temperature\": 0.7,
  \"frequency_penalty\": 0,
  \"presence_penalty\": 0,
  \"top_p\": 0.95,
}"
```
### Using the API with your apps

For these examples, we will be utilizing azure openai api to get structured data out of text paragraphs.

* [.NET Example with HTTP Client](./dotnet-httpclient-example/)
* [.NET Example using Azure OpenAI SDK](./dotnet-sdk-example/)

# References
* https://learn.microsoft.com/en-us/azure/ai-services/openai/reference
* https://learn.microsoft.com/en-us/dotnet/api/overview/azure/ai.openai-readme?view=azure-dotnet-preview
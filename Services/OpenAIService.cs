using System.Net.Http;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OpenAiProject.Models;
using OpenAiProject.Services;

public class OpenAiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly EmailService _emailService;

    public OpenAiService(HttpClient httpClient, IOptions<OpenAiSettings> options, EmailService emailService)
    {
        _httpClient = httpClient;
        _apiKey = options.Value.ApiKey;
        _emailService = emailService;
    }

    public async Task<QuizResponse> GenerateQuizAsync(string topic, string difficulty, string language)
    {
        var requestUri = "https://api.openai.com/v1/chat/completions";

        var messages = new[]
        {
        new { role = "system", content = "You are a quiz generator that responds with JSON data." },
        new { role = "user", content = $@"
            Generate a quiz on the topic '{topic}' with a difficulty level of '{difficulty}' in the following language: '{language}'. 
            The quiz should contain 10 multiple-choice questions, each with 4 options labeled A, B, C, and D. 
            Provide the correct answer index (0 for the first option, 1 for the second, etc.). 
            Please respond in the following JSON format:

            {{
                ""questions"": [
                    {{
                        ""text"": ""Question text"",
                        ""options"": [""Option A"", ""Option B"", ""Option C"", ""Option D""],
                        ""correctOptionIndex"": 0
                    }},
                    ...
                ]
            }}"
        }
    };

        var requestBody = new
        {
            model = "gpt-4",
            messages = messages,
            max_tokens = 1500
        };



        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        var response = await _httpClient.PostAsync(requestUri, content);

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Received response from OpenAI:");
            Console.WriteLine(responseBody); // Log response data

            var jsonResponse = JsonSerializer.Deserialize<JsonDocument>(responseBody);

            // Assuming OpenAI returns the response in JSON as specified
            var quizResponse = ParseQuizResponse(jsonResponse);
            return quizResponse;
        }
        else
        {
            var statusCode = response.StatusCode;
            var reasonPhrase = response.ReasonPhrase;
            var errorContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Error from OpenAI: {statusCode} - {reasonPhrase}");
            Console.WriteLine(errorContent);

            throw new Exception("Error in fetching quiz data from OpenAI.");
        }
    }


    private QuizResponse ParseQuizResponse(JsonDocument jsonResponse)
    {
        var quizResponse = new QuizResponse { Questions = new List<Question>() };

        // Extract the JSON-formatted quiz content
        var content = jsonResponse.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        Console.WriteLine("Extracted content:");
        Console.WriteLine(content);

        try
        {
            // Ensure JSON formatting is valid by parsing it
            using var contentJson = JsonDocument.Parse(content);
            Console.WriteLine("Parsed JSON content:");
            Console.WriteLine(contentJson.RootElement.GetRawText());
            
            foreach(var item in contentJson.RootElement.GetProperty("questions").EnumerateArray())
            {
                var question = new Question
                {
                    Text = item.GetProperty("text").GetString(),
                    CorrectOptionIndex = item.GetProperty("correctOptionIndex").GetInt32()
                };

                foreach (var option in item.GetProperty("options").EnumerateArray())
                {
                    question.Options.Add(option.GetString());
                }

                quizResponse.Questions.Add(question);
            }

            // Deserialize to QuizResponse explicitly
            if (quizResponse == null || quizResponse.Questions == null || quizResponse.Questions.Count == 0)
            {
                Console.WriteLine("Deserialization resulted in an empty or null QuizResponse.");
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine("Failed to parse JSON from OpenAI response.");
            Console.WriteLine(ex.Message);
        }

        return quizResponse;
    }

    public async Task<int> Send(string score, string totalQuestions, string receiverEmail)
    {
        EmailModel emailModelQuiz = new EmailModel();
        emailModelQuiz.From = "abbasiharis1997@gmail.com";
        emailModelQuiz.To = receiverEmail;
        emailModelQuiz.Subject = "Quiz Result";
        emailModelQuiz.Body = $"Quiz Score: You got {score} out of {totalQuestions}.";
        await _emailService.SendEmailAsync(emailModelQuiz);
        return 1;
    }


}

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AiReviewBot
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            // Environment Variables నుండి టోకెన్లు మరియు వివరాలు తెచ్చుకుంటుంది
            string? githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
            string? geminiApiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            string? prNumberStr = Environment.GetEnvironmentVariable("PR_NUMBER");
            string? repository = Environment.GetEnvironmentVariable("GITHUB_REPOSITORY"); // Owner/RepoName రూపంలో ఉంటుంది

            if (string.IsNullOrEmpty(githubToken) || string.IsNullOrEmpty(geminiApiKey) || string.IsNullOrEmpty(prNumberStr) || string.IsNullOrEmpty(repository))
            {
                Console.WriteLine("Error: Missing required environment variables.");
                return;
            }

            Console.WriteLine($"Starting review for PR #{prNumberStr} in {repository}...");

            // 1. GitHub PR నుండి మార్చబడిన కోడ్ (Diff) ని తెచ్చుకోవడం
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", githubToken);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("DotNet-AI-Bot");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3.diff"));

            string diffUrl = $"https://api.github.com/repos/{repository}/pulls/{prNumberStr}";
            HttpResponseMessage diffResponse = await client.GetAsync(diffUrl);

            if (!diffResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to fetch PR diff. Status: {diffResponse.StatusCode}");
                return;
            }

            string codeDiff = await diffResponse.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(codeDiff))
            {
                Console.WriteLine("No code changes found in this PR.");
                return;
            }

            // 2. Gemini API ని కాల్ చేసి కోడ్ రివ్యూ అడగడం
            Console.WriteLine("Sending code changes to Gemini AI...");
            string geminiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={geminiApiKey}";

            var promptText = $"You are an expert code reviewer. Review the following GitHub code diff. Identify security vulnerabilities, bugs, and performance issues. Provide your review as a summary with clean markdown formatting. Keep it concise.\n\nCode Diff:\n{codeDiff}";

            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = promptText } } }
                }
            };

            string jsonBody = JsonSerializer.Serialize(requestBody);
            using var geminiRequest = new HttpRequestMessage(HttpMethod.Post, geminiUrl);
            geminiRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            HttpResponseMessage geminiResponse = await client.SendAsync(geminiRequest);
            if (!geminiResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"Gemini API error. Status: {geminiResponse.StatusCode}");
                return;
            }

            string geminiJsonResult = await geminiResponse.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(geminiJsonResult);
            string aiReviewComment = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "AI could not generate a review.";

            // 3. వచ్చిన AI రివ్యూని GitHub PR లో కామెంట్‌గా పోస్ట్ చేయడం
            Console.WriteLine("Posting review comment back to GitHub PR...");
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", githubToken);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("DotNet-AI-Bot");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));

            string commentUrl = $"https://api.github.com/repos/{repository}/issues/{prNumberStr}/comments";
            var commentBody = new { body = $"### 🤖 Gemini AI Code Review\n\n{aiReviewComment}" };
            string jsonComment = JsonSerializer.Serialize(commentBody);

            using var commentRequest = new HttpRequestMessage(HttpMethod.Post, commentUrl);
            commentRequest.Content = new StringContent(jsonComment, Encoding.UTF8, "application/json");

            HttpResponseMessage commentResponse = await client.SendAsync(commentRequest);
            if (commentResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Successfully posted review comment!");
            }
            else
            {
                Console.WriteLine($"Failed to post comment. Status: {commentResponse.StatusCode}");
            }
        }
    }
}

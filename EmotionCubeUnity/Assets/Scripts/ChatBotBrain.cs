// ChatbotBrain.cs
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


// Authors: Joel Puthankalam, Tymon Vu, Nick Perlich
// Chatbot brain that does the servies request to Groq's Llama 3.3 model
public class ChatbotBrain
{
    private readonly string apiKey = Blackboard.Instance.GetGrokAPIKey();

    // (emotion, reply)
    public event Action<string, string> OnOutput;

    private const string BaseUrl = "https://api.groq.com/openai/v1/chat/completions";
    private static readonly HttpClient httpClient = new HttpClient();

    /// <summary>
    /// Initialize and subscribe to blackboard emotion change events
    /// </summary>
    public ChatbotBrain()
    {
        // Subscribe to blackboard emotion changes
        Blackboard.Instance.OnEmotionChanged += HandleEmotionChanged;
    }

    /// <summary>
    /// Cleanup event subscriptions
    /// </summary>
    public void Dispose()
    {
        Blackboard.Instance.OnEmotionChanged -= HandleEmotionChanged;
    }

    /// <summary>
    /// Handle emotion change events from the blackboard
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <param name="emotion"></param>
    private void HandleEmotionChanged(int slotIndex, string emotion)
    {
        // Fire-and-forget async task
        if (slotIndex != 0)
        {
            return;
        }
        _ = ProcessEmotionAsync(emotion);
    }


    /// <summary>
    /// Process the detected emotion by calling the Groq API
    /// </summary>
    /// <param name="emotion"></param>
    /// <returns></returns>
    private async Task ProcessEmotionAsync(string emotion)
    {
        string prompt =
            $"The system detected that the user is feeling \"{emotion}\". " +
            "Respond with a short, supportive message (1–2 sentences).";

        string reply;
        try
        {
            reply = await CallGroqAsync(prompt);
        }
        catch (Exception ex)
        {
            reply = $"[Error contacting model: {ex.Message}]";
        }

        // Invoke the output event for the ChatbotController to handle
        OnOutput?.Invoke(emotion, reply);
    }
    /// <summary>
    /// Call Groq's Llama 3.3 model with the given prompt
    /// </summary>
    /// <param name="prompt"></param>
    /// <returns></returns>
    private async Task<string> CallGroqAsync(string prompt)
    {
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        // Manual JSON – no extra packages
        string json =
            "{\n" +
            "  \"model\": \"llama-3.3-70b-versatile\",\n" +
            "  \"messages\": [\n" +
            "    { \"role\": \"user\", \"content\": \"" + Escape(prompt) + "\" }\n" +
            "  ]\n" +
            "}";

        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(BaseUrl, content);
        string body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return $"[Groq error {response.StatusCode}]\n{body}";
        }

        return ExtractAssistantReply(body);
    }

    /// <summary>
    /// Escape special characters in JSON string
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    private string Escape(string s) =>
        s.Replace("\\", "\\\\").Replace("\"", "\\\"");

    /// <summary>
    /// Extract the assistant's reply from the Groq API JSON response
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    private string ExtractAssistantReply(string json)
    {
        const string marker = "\"content\":";
        int idx = json.IndexOf(marker, StringComparison.Ordinal);
        if (idx < 0) return "(No reply)";

        idx = json.IndexOf("\"", idx + marker.Length, StringComparison.Ordinal);
        if (idx < 0) return "(No reply)";

        int end = json.IndexOf("\"", idx + 1, StringComparison.Ordinal);
        if (end < 0) return "(No reply)";

        string extracted = json.Substring(idx + 1, end - idx - 1);
        extracted = extracted.Replace("\\n", "\n").Replace("\\\"", "\"");
        return extracted.Trim();
    }
}

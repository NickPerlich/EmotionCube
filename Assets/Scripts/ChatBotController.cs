using UnityEngine;
using TMPro; 

// Authors: Joel Puthankalam, Tymon Vu, Nick Perlich
// Chatbot Controller that manages UI updates based on ChatbotBrain output
// Implements typewriter effect for chatbot replies
public class ChatbotController : MonoBehaviour
{

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI emotionText;
    [SerializeField] private TextMeshProUGUI chatbotText;

    [Header("Typewriter Settings")]
    [SerializeField] private float charDelay = 0.03f;  // seconds between characters

    private ChatbotBrain brain;

    // Buffers for cross-thread handoff
    private string pendingEmotion;
    private string pendingReply;

    private Coroutine typewriterCoroutine;

    /// <summary>
    /// Initialize ChatbotBrain and subscribe to its output event
    /// </summary>
    private void Start()
    {
        brain = new ChatbotBrain();
        brain.OnOutput += HandleBotOutput;
    }

    /// <summary>
    /// Cleanup subscriptions and dispose ChatbotBrain
    /// </summary>
    private void OnDestroy()
    {
        if (brain != null)
        {
            brain.OnOutput -= HandleBotOutput;
            brain.Dispose();
        }
    }

    /// <summary>
    /// Handle output from ChatbotBrain (worker thread)
    /// </summary>
    /// <param name="emotion"></param>
    /// <param name="reply"></param>
    private void HandleBotOutput(string emotion, string reply)
    {
        pendingEmotion = emotion;
        pendingReply = reply;
    }

    private void Update()
    {
        // Apply updates to UI on Unity main thread
        if (!string.IsNullOrEmpty(pendingReply))
        {
            string emotion = pendingEmotion;
            string reply = pendingReply;

            pendingEmotion = null;
            pendingReply = null;

            // Update emotion label instantly
            if (emotionText != null)
                emotionText.text = $"Emotion: {emotion}";

            // Start typewriter effect for chatbot text
            if (chatbotText != null)
            {
                if (typewriterCoroutine != null)
                    StopCoroutine(typewriterCoroutine);

                typewriterCoroutine = StartCoroutine(TypewriterCoroutine(chatbotText, reply, charDelay));
            }

            Debug.Log($"[Chatbot] Emotion={emotion}, Reply={reply}");
        }
    }
    /// <summary>
    /// Typewriter effect coroutine
    /// </summary>
    /// <param name="target"></param>
    /// <param name="fullText"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    private System.Collections.IEnumerator TypewriterCoroutine(TMP_Text target, string fullText, float delay)
    {
        target.text = "";
        foreach (char c in fullText)
        {
            target.text += c;
            yield return new WaitForSeconds(delay);
        }
    }
}

using UnityEngine;
using TMPro;

// Authors: Joel Puthankalam, Tymon Vu, Nick Perlich
// Controller for the start screen UI where the user inputs their client ID and GROK API key
// Listens to the blackboard event when the local client is resolved
public class StartScreenController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField clientIdInput; // Input field for clientID
    [SerializeField] private TMP_InputField grokAPIInput; // Input field for GROK API key
    [SerializeField] private GameObject mainRoot;      // parent of your main scene objects
    [SerializeField] private GameObject waitingLabel;  // optional "Waiting for data..." text

    /// <summary>
    /// Initialize the start screen
    /// </summary>
    private void Start()
    {
        if (mainRoot != null)
            mainRoot.SetActive(false);

        if (waitingLabel != null)
            waitingLabel.SetActive(false);

        // Observe the event when the local client is resolved
        Blackboard.Instance.OnLocalClientResolved += HandleLocalClientResolved;
    }

    /// <summary>
    /// Cleanup event subscriptions
    /// </summary>
    private void OnDestroy()
    {
        Blackboard.Instance.OnLocalClientResolved -= HandleLocalClientResolved;
    }


    /// <summary>
    /// Called when the Confirm button is clicked
    /// </summary>
    public void OnClickConfirm()
    {
        string id = clientIdInput.text.Trim();
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning("[StartScreen] clientId is empty!");
            return;
        }

        string grokID = grokAPIInput.text.Trim();
        if (string.IsNullOrEmpty(grokID))
        {
            Debug.LogWarning("[StartScreen] Grok API Key is empty!");
            return;
        }

        // Store the chosen clientId in the blackboard
        Blackboard.Instance.SetLocalClientId(id);
        Debug.Log("[StartScreen] Set local clientId = " + id);

        // Set the GrokAPI key in the blackboard
        if (!string.IsNullOrEmpty(grokID))
            Blackboard.Instance.SetGrokApiKey(grokID);

        // Optionally show a "waiting for data" message
        if (waitingLabel != null)
            waitingLabel.SetActive(true);
    }

    /// <summary>
    /// Handles the event when the local client is resolved to a slot
    /// </summary>
    /// <param name="slotIndex"></param>
    private void HandleLocalClientResolved(int slotIndex)
    {
        Debug.Log("[StartScreen] Local client resolved to slot " + slotIndex);

        // Hide the start screen UI
        gameObject.SetActive(false);

        // Enable the main scene objects (cubes, chatbot UI, etc.)
        if (mainRoot != null)
            mainRoot.SetActive(true);
    }
}

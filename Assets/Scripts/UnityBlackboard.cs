using System;
using UnityEditor;
using UnityEngine;


// Authors: Joel Puthankalam, Tymon Vu, Nick Perlich
// Blackboard object that manages global data
// Is an observable for emotion changes

public class Blackboard : MonoBehaviour
{
    // -------------------------
    //  Singleton Implementation
    // -------------------------

    private static readonly Lazy<Blackboard> _instance =
        new Lazy<Blackboard>(() => new Blackboard());

    public static Blackboard Instance => _instance.Value;

    private Blackboard() { }  // Private constructor ensures singleton


    // -------------------------
    //  Blackboard Event System
    // -------------------------

    /// <summary>
    /// Event fired whenever the dominant interpreted emotion changes.
    /// Observers subscribe to this to react to system state updates.
    /// </summary>
    public event Action<int, string> OnEmotionChanged;
    public event Action<int> OnPlayerAdded;
    public event Action<int> OnPlayerRemoved;
    public event Action<int> OnLocalClientResolved;

    public string LocalClientId { get; private set; }

    private string grokAPIKey = "";
    private string[] playerEmotions = new string[4];
    private string[] players = new string[4];
    private static int numPlayers = 0;

    private static Boolean isInitialized = false;


    // -------------------------
    //  Public API
    // -------------------------

    /// <summary>
    /// Pushes new JSON emotion data into the blackboard.  
    /// The data is processed to determine the dominant emotional state.
    /// If the dominant state changes, observers are notified.
    /// </summary>
    /// <param name="json">
    /// <param name="slotIndex">"
    /// Raw BCI metrics (e.g., focus, calmness, stress) encoded as JSON.
    /// </param>
    public void PushEmotionJson(string json, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= playerEmotions.Length)
        {
            Debug.Log("[Blackboard] Invalid slot index!");
            return;
        }


        var raw = DataProcessor.ParseBCI(json);
        var processed = DataProcessor.ComputeEmotion(raw);
        string dominant = DataProcessor.GetDominantEmotion(processed);

        Debug.Log($"[Blackboard] Slot {slotIndex} - Dominant Emotion: {dominant}");

        string currentEmotion = playerEmotions[slotIndex];

        Debug.Log($"[Blackboard] Current Emotion {currentEmotion}");

        if (dominant != currentEmotion && isInitialized)
        {
           Debug.Log($"[Blackboard] Emotion changed for slot {slotIndex}: {currentEmotion} -> {dominant}");
            playerEmotions[slotIndex] = dominant;
            OnEmotionChanged?.Invoke(slotIndex,dominant);
        }
        
    }

    /// <summary>
    /// Sets the local client ID for this instance.
    /// </summary>
    /// <param name="clientId"></param>
    public void SetLocalClientId(string clientId)
    {
        LocalClientId = clientId;
    }

    /// <summary>
    /// Sets the Grok API key for this instance.
    /// </summary>
    /// <param name="apiKey"></param>
    public void SetGrokApiKey(string apiKey)
    {
        grokAPIKey = apiKey;
    }

    /// <summary>
    /// Gets the Grok API key for this instance (used by chatbotbrain).
    /// </summary>
    /// <returns></returns>
    public string GetGrokAPIKey()
    {
        return grokAPIKey;
    }

    /// <summary>
    /// Gets or adds a player by client ID.
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    public int GetOrAddPlayer(string clientId)
    {

        // check if we have the localclientid
        if (LocalClientId == clientId)
        {
            for (int i = 0; i < numPlayers; i++)
            {
                if (players[i] == clientId)
                {
                    Debug.Log("[Blackboard] Local client already assigned to slot " + i);
                    if (isInitialized == false)
                    {
                        isInitialized = true;
                        Debug.Log("[Blackboard] Firing OnLocalClientResolved for slot " + i);
                        OnLocalClientResolved?.Invoke(i);
                    }
                    
                    return i;
                }
            }
        }

        // check if we already have the client id
        for (int i = 0; i < numPlayers; i++)
        {
            if (players[i] == clientId)
            {
                return i;
            }
        }
        // if not lets add them if space
        if (numPlayers >= players.Length)
        {
            return -1;
        }

        int playerId = numPlayers;
        players[numPlayers] = clientId;
        numPlayers += 1;
        OnPlayerAdded?.Invoke(playerId);
        return playerId;
        
    }
    /// <summary>
    /// Removes a player by client ID.
    /// </summary>
    /// <param name="clientId"></param>
    public void RemovePlayer(string clientId)
    {
        for (int i = 0; i < numPlayers; i++)
        {
            if (players[i] == clientId)
            {
                players[i] = null;
                OnPlayerRemoved?.Invoke(i);
                break;
            }
        }
    }

   
    /// <summary>
    /// Registers an observer callback to be notified whenever the
    /// interpreted emotional state changes.
    /// </summary>
    public void RegisterObserver(Action<int, string> observer)
    {
        OnEmotionChanged += observer;
    }

    /// <summary>
    /// Removes a previously registered observer.
    /// </summary>
    public void UnregisterObserver(Action<int, string> observer)
    {
        OnEmotionChanged -= observer;
    }
}

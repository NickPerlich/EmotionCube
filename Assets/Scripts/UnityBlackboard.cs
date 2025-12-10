using UnityEngine;
using System;

/// <summary>
/// The UnityBlackboard acts as the central shared knowledge hub in the system.
/// It receives raw emotion data from external sources (e.g., MQTTSubscriber),
/// processes it using the DataProcessor, determines the dominant interpreted
/// emotional state, and notifies all registered observers.
/// 
/// This class serves as the "Model" in the MVC architecture and the core
/// coordination mechanism in the Blackboard Pattern, where multiple independent
/// components can read from and react to shared state.
/// </summary>
public class UnityBlackboard : MonoBehaviour
{
    /// <summary>
    /// Event triggered whenever the interpreted emotional state changes.
    /// Observers (e.g., CubePanel) register callbacks to respond to emotion updates.
    /// The string parameter represents the dominant emotion label
    /// (e.g., "Happy", "Sad", "Upset", "Stress", "Fear").
    /// </summary>
    public event Action<string> OnEmotionChanged;

    /// <summary>
    /// Tracks the most recent dominant emotion to prevent duplicate notifications.
    /// </summary>
    private string currentEmotion = "none";

    /// <summary>
    /// Receives raw JSON emotion data (typically from MQTTSubscriber),
    /// delegates parsing and emotion derivation to the DataProcessor,
    /// determines the dominant interpreted emotion, and broadcasts it
    /// to all registered observers if it has changed.
    /// </summary>
    /// <param name="json">A JSON string containing raw BCI metrics such as focus, calm, and stress.</param>
    public void PushEmotionJson(string json)
    {
        var raw = DataProcessor.ParseBCI(json);
        var processed = DataProcessor.ComputeEmotion(raw);
        string dominant = DataProcessor.GetDominantEmotion(processed);

        // Only notify if the emotion is different from last update
        if (dominant != currentEmotion)
        {
            currentEmotion = dominant;
            OnEmotionChanged?.Invoke(dominant);
        }
    }

    /// <summary>
    /// Registers a new observer callback that will be invoked every time
    /// the dominant interpreted emotion changes.
    /// 
    /// This enables loose coupling between components, as observers
    /// do not need direct references to the publisher or to each other.
    /// </summary>
    /// <param name="observer">
    /// A method that accepts a string emotion name to respond to updates.
    /// Example: UpdateCubeColor("Happy")
    /// </param>
    public void RegisterObserver(Action<string> observer)
    {
        OnEmotionChanged += observer;
    }
}

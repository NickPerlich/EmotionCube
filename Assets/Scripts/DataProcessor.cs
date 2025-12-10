using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// The DataProcessor is a stateless utility module responsible for transforming
/// raw BCI input values (focus, calm, stress) into interpreted emotional values.
/// 
/// This class acts as the core "emotion engine" for the system. It:
/// - Parses raw JSON sent by the MQTT subscriber
/// - Computes 5 higher-level emotional interpretations:
///     Happy, Sad, Upset, Stress, Fear
/// - Selects the strongest (dominant) emotion for display
/// 
/// It contains no Unity lifecycle functions and is not attached to any GameObject.
/// This ensures the emotion model remains modular, testable, and scalable.
/// </summary>
public static class DataProcessor
{
    /// <summary>
    /// Raw incoming brain–computer interface data.
    /// These values reflect the emotional or cognitive metrics published via MQTT.
    /// </summary>
    [Serializable]
    public class RawBCIData
    {
        public float focus;   // 0–1 representing cognitive engagement
        public float calm;    // 0–1 representing relaxation
        public float stress;  // 0–1 representing stress arousal
    }

    /// <summary>
    /// Contains computed emotional interpretation scores derived from RawBCIData.
    /// Each emotion is normalized between 0–1 and represents the strength of that state.
    /// </summary>
    [Serializable]
    public class EmotionOutput
    {
        public float happy;
        public float sad;
        public float upset;
        public float stressVal;
        public float fear;
    }

    /// <summary>
    /// Converts a JSON string (received from the MQTTSubscriber)
    /// into a <see cref="RawBCIData"/> instance using Unity's JSON utility.
    /// </summary>
    /// <param name="json">A JSON string expected to contain focus, calm, and stress values.</param>
    /// <returns>A populated RawBCIData object, or null if parsing fails.</returns>
    public static RawBCIData ParseBCI(string json)
    {
        try
        {
            return JsonUtility.FromJson<RawBCIData>(json);
        }
        catch (Exception ex)
        {
            Debug.LogError("BCI JSON parse failed: " + ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Computes five higher-order emotional states using weighted combinations of:
    /// focus, calm, and stress.
    /// 
    /// These formulas represent a simple affective model:
    /// - Happy: increases with calm and focus, decreases with stress
    /// - Sad: increases with low calm, low focus, and moderate stress
    /// - Upset: increases with stress and decreases with calm
    /// - Stress: simply amplifies the raw stress signal
    /// - Fear: increases with stress and both low calm and low focus
    /// 
    /// All emotion values are clamped between 0–1 for stability.
    /// </summary>
    /// <param name="d">The raw focus/calm/stress values.</param>
    /// <returns>A computed EmotionOutput instance.</returns>
    public static EmotionOutput ComputeEmotion(RawBCIData d)
    {
        if (d == null) return null;

        EmotionOutput e = new EmotionOutput();

        // 1. HAPPY = calm + focus - stress
        e.happy = 0.6f * d.calm + 0.3f * d.focus - 0.5f * d.stress;

        // 2. SAD = low calm + low focus + some stress
        e.sad = 0.5f * (1f - d.calm) + 0.3f * (1f - d.focus) + 0.2f * d.stress;

        // 3. UPSET = high stress + low calm
        e.upset = 0.8f * d.stress + 0.2f * (1f - d.calm);

        // 4. STRESS = raw stress amplified slightly
        e.stressVal = 1.1f * d.stress;

        // 5. FEAR = high stress + low calm + low focus
        e.fear = 0.6f * d.stress 
               + 0.2f * (1f - d.calm) 
               + 0.2f * (1f - d.focus);

        // Normalize values to [0, 1]
        e.happy     = Mathf.Clamp01(e.happy);
        e.sad       = Mathf.Clamp01(e.sad);
        e.upset     = Mathf.Clamp01(e.upset);
        e.stressVal = Mathf.Clamp01(e.stressVal);
        e.fear      = Mathf.Clamp01(e.fear);

        return e;
    }

    /// <summary>
    /// Selects the dominant emotion from the computed emotion scores.
    /// The emotion with the highest numerical value is returned.
    /// </summary>
    /// <param name="e">The computed emotion scores.</param>
    /// <returns>
    /// A string representing the strongest interpreted emotion:
    /// "Happy", "Sad", "Upset", "Stress", or "Fear".
    /// </returns>
    public static string GetDominantEmotion(EmotionOutput e)
    {
        if (e == null) return "none";

        var dict = new Dictionary<string, float>
        {
            { "Happy",  e.happy },
            { "Sad",    e.sad },
            { "Upset",  e.upset },
            { "Stress", e.stressVal },
            { "Fear",   e.fear }
        };

        float max = float.NegativeInfinity;
        string best = "none";

        foreach (var kvp in dict)
        {
            if (kvp.Value > max)
            {
                max = kvp.Value;
                best = kvp.Key;
            }
        }

        return best;
    }
}

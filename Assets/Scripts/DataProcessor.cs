using UnityEngine;
using System;
using System.Collections.Generic;

// Authors: Joel Puthankalam, Tymon Vu, Nick Perlich
// Data Processor that handles parsing and interpreting BCI data
// Computes emotional states from raw BCI metrics
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
        public float focus;
        public float calm;
        public float stress;
        
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
    /// Computes the Emotion --> can expand to compute more complex emotions later.
    /// </summary>
    /// <param name="d">The raw focus/calm/stress values.</param>
    /// <returns>A computed EmotionOutput instance.</returns>
    public static EmotionOutput ComputeEmotion(RawBCIData d)
    {
        if (d == null) return null;

        EmotionOutput e = new EmotionOutput();

        e.focus = d.focus;
        e.calm = d.calm;
        e.stress = d.stress;

        return e;
    }

    /// <summary>
    /// Selects the dominant emotion from the computed emotion scores.
    /// The emotion with the highest numerical value is returned.
    /// </summary>
    /// <param name="e">The computed emotion scores.</param>
    /// <returns>
    /// A string representing the strongest interpreted emotion
    /// </returns>
    public static string GetDominantEmotion(EmotionOutput e)
    {
        if (e == null) return "none";

        var dict = new Dictionary<string, float>
        {
            { "Focus",  e.focus},
            { "Calm",    e.calm},
            { "Stress",  e.stress},
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

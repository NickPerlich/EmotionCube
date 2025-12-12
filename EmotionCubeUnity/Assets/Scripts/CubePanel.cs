using UnityEngine;

// Authors: Joel Puthankalam, Tymon Vu, Nick Perlich
// CubePanel that represents a player's emotional state
// Listens to a blackboard event when the emotion changes

public class CubePanel : MonoBehaviour
{
    [Header("Which player slot does this cube represent? (0–3)")]
    [SerializeField] private int slotIndex = 0;

    // used by the CubeManager to add a cube at a given slot
    public int SlotIndex            
    {
        get => slotIndex;
        set => slotIndex = value;
    }

    private Renderer cubeRenderer;

    /// <summary>
    /// Initialize and subscribe to blackboard emotion change events
    /// </summary>
    private void Start()
    {
        cubeRenderer = GetComponent<Renderer>();
        Blackboard.Instance.OnEmotionChanged += HandleEmotionChanged;
    }

    /// <summary>
    /// Cleanup event subscriptions
    /// </summary>
    private void OnDestroy()
    {
        Blackboard.Instance.OnEmotionChanged -= HandleEmotionChanged;
    }

    /// <summary>
    /// Handle emotion change events from the blackboard
    /// </summary>
    /// <param name="idx"></param>
    /// <param name="emotion"></param>
    private void HandleEmotionChanged(int idx, string emotion)
    {
        Debug.Log(EmotionColor(emotion));
        if (idx != slotIndex)
            return;

        cubeRenderer.material.color = EmotionColor(emotion);
        Debug.Log($"[CubePanel slot {slotIndex}] Emotion changed -> {emotion}");
    }

    /// <summary>
    /// Get the color associated with a given emotion
    /// </summary>
    /// <param name="emotion"></param>
    /// <returns></returns>
    private Color EmotionColor(string emotion)
    {
        return emotion switch
        {
            "Focus" => Color.yellow,
            "Calm" => Color.blue,
            "Stress" => Color.red,
            _ => Color.gray,
        };
    }
}

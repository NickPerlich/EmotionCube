using UnityEngine;

/// <summary>
/// The CubePanel acts as the View in the MVC architecture.
/// It observes the UnityBlackboard for emotion updates and reflects
/// those emotions visually by changing the cube's color.
/// 
/// Responsibilities:
/// - Register itself as an observer of the UnityBlackboard
/// - Update cube material color when the interpreted emotion changes
/// - Log emotion transitions for debugging and verification
/// </summary>
public class CubePanel : MonoBehaviour
{
    /// <summary>
    /// Cached reference to the cube's Renderer component.
    /// Used to apply color changes based on emotional state.
    /// </summary>
    private Renderer cubeRenderer;

    /// <summary>
    /// Unity Start() callback.
    /// Initializes renderer reference and registers this panel
    /// as an observer of the UnityBlackboard.
    /// </summary>
    void Start()
    {
        cubeRenderer = GetComponent<Renderer>();

        // Auto-discover the active Blackboard instance in the scene
        UnityBlackboard bb = FindObjectOfType<UnityBlackboard>();
        if (bb == null)
        {
            Debug.LogError("UnityBlackboard not found in scene!");
            return;
        }

        // Register callback to respond to emotion changes
        bb.RegisterObserver(UpdateCubeColor);
    }

    /// <summary>
    /// Callback invoked by UnityBlackboard whenever the interpreted
    /// emotional state changes. Updates cube color and prints debug information.
    /// </summary>
    /// <param name="emotion">The interpreted emotional state (e.g., "Happy", "Sad")</param>
    private void UpdateCubeColor(string emotion)
    {
        cubeRenderer.material.color = EmotionColor(emotion);
        Debug.Log("MQTT SUBSCRIBE: {New Emotion: " + emotion + "}");
    }

    /// <summary>
    /// Maps emotion labels to specific colors for visualization.
    /// Modify this method to adjust emotional color styling.
    /// </summary>
    /// <param name="emotion">Emotion name being visualized.</param>
    /// <returns>A UnityEngine.Color representing the emotion.</returns>
    private Color EmotionColor(string emotion)
    {
        return emotion switch
        {
            "Happy"  => Color.yellow,
            "Sad"    => Color.blue,
            "Upset"  => new Color(1f, 0.3f, 0.3f),
            "Stress" => Color.red,
            "Fear"   => new Color(0.4f, 0f, 0.4f),
            _        => Color.gray,
        };
    }
}

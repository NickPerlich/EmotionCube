using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Text;

/// <summary>
/// The MQTTSubscriber acts as the Controller within the MVC architecture.
/// It connects to an MQTT broker, listens for incoming raw BCI emotional data,
/// and forwards the received JSON payloads to the UnityBlackboard.
/// 
/// Since MQTT callbacks occur on a background thread, this class uses the
/// UnityMainThreadDispatcher to safely forward work back onto the Unity main thread,
/// ensuring compliance with Unity's threading restrictions.
/// </summary>
public class MQTTSubscriber : MonoBehaviour
{
    /// <summary>
    /// The hostname or IP address of the MQTT broker providing the emotion data.
    /// </summary>
    public string broker = "broker.hivemq.com";

    /// <summary>
    /// Port number for the MQTT broker connection (typically 1883 for non-SSL).
    /// </summary>
    public int port = 1883;

    /// <summary>
    /// Topic string on which this subscriber listens for emotion data packets.
    /// Example: "bci/emotions"
    /// </summary>
    public string topic = "bci/emotions";

    /// <summary>
    /// MQTT client instance that manages the connection and message reception.
    /// </summary>
    private MqttClient client;

    /// <summary>
    /// Reference to the UnityBlackboard, which receives and processes incoming emotion JSON.
    /// </summary>
    private UnityBlackboard blackboard;

    /// <summary>
    /// Unity Start() callback.
    /// Discovers the Blackboard, initializes the MQTT client, connects to the broker,
    /// and subscribes to the configured topic.
    /// </summary>
    void Start()
    {
        // Locate the single active Blackboard instance in the scene
        blackboard = FindObjectOfType<UnityBlackboard>();
        if (blackboard == null)
        {
            Debug.LogError("UnityBlackboard not found in scene!");
            return;
        }

        // Create MQTT client and assign message callback
        client = new MqttClient(broker, port, false, null, null, MqttSslProtocols.None);
        client.MqttMsgPublishReceived += OnMessageReceived;

        // Connect using a unique client ID
        string clientId = System.Guid.NewGuid().ToString();
        client.Connect(clientId);

        // Subscribe to the target topic with QoS 0 (at most once delivery)
        client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });

        Debug.Log("MQTTSubscriber connected & subscribed.");
    }

    /// <summary>
    /// Callback executed whenever a message arrives on the subscribed MQTT topic.
    /// This is invoked on a background MQTT thread, so Unity API calls are not allowed here.
    /// Instead, the JSON payload is forwarded via UnityMainThreadDispatcher to be processed
    /// safely on the Unity main thread.
    /// </summary>
    /// <param name="sender">MQTT client instance that received the message.</param>
    /// <param name="e">Event arguments containing the topic and message payload.</param>
    private void OnMessageReceived(object sender, MqttMsgPublishEventArgs e)
    {
        // Convert byte payload into UTF-8 JSON string
        string json = Encoding.UTF8.GetString(e.Message);

        // Forward JSON to Blackboard on the Unity main thread
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            if (blackboard != null)
                blackboard.PushEmotionJson(json);
        });
    }
}

using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Text;
using UnityEditor.PackageManager;



// Authors: Joel Puthankalam, Tymon Vu, Nick Perlich
// MQTT Subscriber that listens to emotion data from external clients
// Uses BCIMessage format to parse incoming JSON messages

public class MQTTSubscriber : MonoBehaviour
{
    public string broker = "broker.hivemq.com";
    public int port = 1883;
    public string topic = "bci/emotions";
    private string clientId = "";

    private MqttClient client;

    /// <summary>
    /// Initialize and connect to MQTT broker
    /// </summary>
    private void Start()
    {
        // Set up MQTT client
        client = new MqttClient(broker, port, false, null, null, MqttSslProtocols.None);
        client.MqttMsgPublishReceived += OnMessageReceived;

        clientId = System.Guid.NewGuid().ToString();
        client.Connect(clientId);

        client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });

        Debug.Log("[MQTT] Connected and subscribed.");
    }

    /// <summary>
    /// Callback when a message is received from the MQTT broker
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnMessageReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string json = Encoding.UTF8.GetString(e.Message);
        Debug.Log("[MQTT] Message received: " + json);
        // Pass data to the pure C# blackboard through main thread dispatcher

        // Check who the client is and assign the slot accordingly
        BciMessage msg;
        try
        {
            msg = JsonUtility.FromJson<BciMessage>(json);
        }
        catch
        {
            Debug.LogWarning("[MQTT] Failed to parse JSON: " + json);
            return;
        }
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            if (msg.clientId != null)
            {
                int slotIndex = Blackboard.Instance.GetOrAddPlayer(msg.clientId);
                Debug.Log("[MQTT] Assigned slot index: " + slotIndex + " for clientId: " + msg.clientId);
                if (slotIndex == -1) return;
                Blackboard.Instance.PushEmotionJson(json, slotIndex);
            }
            
        });

    }

    /// <summary>
    /// Cleanup on destroy
    /// </summary>
    private void OnDestroy(){
        if (client != null && client.IsConnected)
        {
            client.Disconnect();
        }

        if (!string.IsNullOrEmpty(clientId))
        {
            Blackboard.Instance.RemovePlayer(clientId);
        }
    }
}

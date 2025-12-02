using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Text;
using System.Collections.Generic;

public class MQTTSubscriber : MonoBehaviour
{
    public string broker = "broker.hivemq.com";
    public int port = 1883;
    public string topic = "bci/emotions";

    private MqttClient client;
    private Renderer cubeRenderer;

    private string pendingEmotion = null;
    private readonly object lockObj = new object();

    [System.Serializable]
    public class EmotionData
    {
        public float happy;
        public float sad;
        public float angry;
        public float calm;
        public float fear;
        public float surprise;
    }

    void Start()
    {
        cubeRenderer = GetComponent<Renderer>();

        // M2Mqtt constructor (host, port, secure=false, cert=null, cert=null, sslprotocol default)
        client = new MqttClient(broker, port, false, null, null, MqttSslProtocols.None);

        client.MqttMsgPublishReceived += OnMessageReceived;

        string clientId = System.Guid.NewGuid().ToString();
        client.Connect(clientId);

        client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });

        Debug.Log("M2Mqtt connected + subscribed!");
    }

    void OnMessageReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string json = Encoding.UTF8.GetString(e.Message);

        EmotionData data = JsonUtility.FromJson<EmotionData>(json);
        string dom = GetDominantEmotion(data);

        lock (lockObj)
        {
            pendingEmotion = dom;
        }
    }

    void Update()
    {
        string emotion = null;

        lock (lockObj)
        {
            if (pendingEmotion != null)
            {
                emotion = pendingEmotion;
                pendingEmotion = null;
            }
        }

        if (emotion != null)
        {
            cubeRenderer.material.color = EmotionColor(emotion);
        }
    }

    private string GetDominantEmotion(EmotionData d)
    {
        var dict = new Dictionary<string, float>
        {
            {"happy", d.happy},
            {"sad", d.sad},
            {"angry", d.angry},
            {"calm", d.calm},
            {"fear", d.fear},
            {"surprise", d.surprise}
        };

        string best = "none";
        float max = float.NegativeInfinity;

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

    private Color EmotionColor(string e)
    {
        return e switch
        {
            "happy" => Color.yellow,
            "sad" => Color.blue,
            "angry" => Color.red,
            "calm" => Color.cyan,
            "fear" => new Color(0.4f, 0f, 0.4f),
            "surprise" => Color.magenta,
            _ => Color.gray,
        };
    }
}

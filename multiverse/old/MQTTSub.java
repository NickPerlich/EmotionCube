import org.eclipse.paho.client.mqttv3.*;

public class MQTTSub implements Runnable, MqttCallback {

    private final String broker = Main.BROKER_URL; 
    private final String topic  = Main.TOPIC;       
    private final String myId   = Main.ID;          

    private final String clientId = "sub-" + System.currentTimeMillis();
    private MqttClient client;

    public MQTTSub() {
        try {
            client = new MqttClient(broker, clientId);
            client.setCallback(this);
        } catch (MqttException e) { e.printStackTrace(); }
    }

    @Override
    public void run() {
        try {
            client.connect();
            System.out.println("SUB connected → " + broker);

            client.subscribe(topic);
            System.out.println("SUBscribed → " + topic);

        } catch (MqttException e) { e.printStackTrace(); }
    }

    @Override 
    public void connectionLost(Throwable cause) {
        System.out.println("SUB lost connection");
    }

    @Override
    public void messageArrived(String topic, MqttMessage message) {
        try {
            String json = new String(message.getPayload()).trim();

            // Example JSON:
            // {"happy":0.2,"sad":0.1,"angry":0.5,"calm":0.1,"fear":0.05,"surprise":0.05}

            float happy     = extract(json, "happy");
            float sad       = extract(json, "sad");
            float angry     = extract(json, "angry");
            float calm      = extract(json, "calm");
            float fear      = extract(json, "fear");
            float surprise  = extract(json, "surprise");

            // Determine dominant emotion
            float max = -1;
            String dom = "none";

            if (happy    > max) { max = happy; dom = "happy"; }
            if (sad      > max) { max = sad; dom = "sad"; }
            if (angry    > max) { max = angry; dom = "angry"; }
            if (calm     > max) { max = calm; dom = "calm"; }
            if (fear     > max) { max = fear; dom = "fear"; }
            if (surprise > max) { max = surprise; dom = "surprise"; }

            System.out.printf("RX Emotion → %s (%.2f)%n", dom, max);

        } catch (Exception ex) {
            System.out.println("Bad JSON: " + new String(message.getPayload()));
        }
    }

    @Override public void deliveryComplete(IMqttDeliveryToken token) {}

    /** simple manual JSON parsing (no dependencies) */
    private float extract(String json, String key) {
        try {
            // key":"value"
            String search = "\"" + key + "\":";
            int idx = json.indexOf(search);
            if (idx == -1) return 0f;

            int start = idx + search.length();
            int end = json.indexOf(",", start);
            if (end == -1) end = json.indexOf("}", start);

            return Float.parseFloat(json.substring(start, end));
        } catch (Exception e) {
            return 0f;
        }
    }
}

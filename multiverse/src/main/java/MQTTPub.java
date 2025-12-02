import org.eclipse.paho.client.mqttv3.*;
import java.util.Random;

public class MQTTPub implements Runnable {

    private final String broker = Main.BROKER_URL;
    private final String topic  = Main.TOPIC;
    private final String myId   = Main.ID;

    private final String clientId = "pub-" + System.currentTimeMillis();
    private MqttClient client;

    private Random rand = new Random();

    public MQTTPub() {
        try {
            client = new MqttClient(broker, clientId);
        } catch (MqttException e) { e.printStackTrace(); }
    }

    @Override
    public void run() {
        try {
            client.connect();
            System.out.println("PUB connected → " + broker);

            while (true) {
                String json =
                        "{"
                                + "\"happy\":" + rand.nextFloat() + ","
                                + "\"sad\":" + rand.nextFloat() + ","
                                + "\"angry\":" + rand.nextFloat() + ","
                                + "\"calm\":" + rand.nextFloat() + ","
                                + "\"fear\":" + rand.nextFloat() + ","
                                + "\"surprise\":" + rand.nextFloat()
                                + "}";

                MqttMessage msg = new MqttMessage(json.getBytes());
                msg.setQos(0);

                client.publish(topic, msg);
                System.out.println("PUBLISHED → " + json);

                Thread.sleep(1000);
            }

        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}

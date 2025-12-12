import java.beans.PropertyChangeEvent;
import java.beans.PropertyChangeListener;
import java.util.UUID;
import java.util.concurrent.BlockingQueue;
import java.util.concurrent.LinkedBlockingQueue;

import org.eclipse.paho.client.mqttv3.*;

/**
 * Authors: Joel Puthankalam, Tymon Vu, Nick Perlich
 * Publishes emotion state updates to an MQTT broker on a separate thread.
 *
 * <p>This class observes the {@link EmotionBlackboard} for emotion state
 * changes and publishes each update as a message to an MQTT topic.
 * Publishing is performed on a dedicated thread to avoid blocking
 * the UI or blackboard notification flow.</p>
 *
 * <p>A {@link BlockingQueue} is used to safely transfer emotion state
 * updates from the blackboard observer callback to the publishing loop
 * running in the publisher thread.</p>
 */
public class MQTTPublisher implements PropertyChangeListener, Runnable {

    /** MQTT broker connection URL. */
    public static final String BROKER_URL = "tcp://broker.hivemq.com:1883";

    /** MQTT topic to which emotion data is published. */
    public static final String TOPIC = "bci/emotions";

    /** MQTT client identifier. */
    public static final String ID = UUID.randomUUID().toString();

    private final BlockingQueue<EmotionState> queue = new LinkedBlockingQueue<>();
    private IMqttClient client;

    /**
     * Constructs the MQTT publisher, establishes a connection to the broker,
     * and registers this instance as a listener with the emotion blackboard.
     */
    public MQTTPublisher() {
        connect();
        EmotionBlackboard.getInstance().addPropertyChangeListener(this);
    }

    /**
     * Receives emotion state change notifications from the blackboard.
     *
     * <p>When a new emotion state is published to the blackboard, it is
     * placed onto the internal queue for asynchronous processing by
     * the publisher thread.</p>
     *
     * @param evt property change event carrying the updated emotion state
     */
    @Override
    public void propertyChange(PropertyChangeEvent evt) {
        if ("emotionState".equals(evt.getPropertyName())) {
            queue.offer((EmotionState) evt.getNewValue());
        }
    }

    /**
     * Runs the publisher loop on a dedicated thread.
     *
     * <p>This method blocks while waiting for emotion state updates
     * and publishes each received state to the configured MQTT topic.</p>
     */
    @Override
    public void run() {
        while (true) {
            try {
                EmotionState state = queue.take();
                publish(state);
            } catch (InterruptedException e) {
                Thread.currentThread().interrupt();
                break;
            }
        }
    }

    /**
     * Establishes a connection to the MQTT broker.
     */
    private void connect() {
        try {
            client = new MqttClient(BROKER_URL, ID);
            client.connect();
        } catch (MqttException e) {
            throw new RuntimeException(e);
        }
    }

    /**
     * Publishes an emotion state update to the MQTT topic.
     *
     * @param state emotion state to publish
     */
    private void publish(EmotionState state) {
        try {
            String payload = serialize(state);
            client.publish(TOPIC, new MqttMessage(payload.getBytes()));
            System.out.println("MQTT PUBLISH -> " + payload);
        } catch (MqttException e) {
            System.err.println("Publish failed: " + e.getMessage());
        }
    }

    /**
     * Serializes an emotion state into a string payload.
     *
     * @param state emotion state to serialize
     * @return serialized emotion state representation
     */
    private String serialize(EmotionState state) {
        return String.format(
                "{ \"clientId\": \"%s\", \"focus\": %.2f, \"calm\": %.2f, \"stress\": %.2f }",
                EmotionBlackboard.getInstance().getClientId(),
                state.getFocus(),
                state.getCalm(),
                state.getStress()
        );
    }
}

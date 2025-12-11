import javax.swing.*;
import java.awt.*;

/**
 * Authors: Joel Puthankalam, Tymon Vu, Nick Perlich
 * Application entry point for the BCI Emotion Simulator.
 *
 * <p>This class is responsible solely for assembling and wiring together
 * the application components, including the UI view, input listener,
 * controller, and MQTT publisher. It contains no business logic.</p>
 *
 * <p>The UI is initialized on the Swing Event Dispatch Thread (EDT)
 * to ensure thread safety.</p>
 */
public class Main {

    /**
     * Launches the BCI Emotion Simulator application.
     *
     * <p>This method initializes the Swing frame, connects the emotion
     * slider view to its listener and controller, creates the MQTT
     * publisher, and starts the publisher thread.</p>
     *
     * @param args command-line arguments (not used)
     */
    public static void main(String[] args) {
        SwingUtilities.invokeLater(() -> {
            JFrame frame = new JFrame("BCI Emotion Simulator");
            frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
            frame.setLayout(new BorderLayout());

            EmotionSliderView view = new EmotionSliderView();
            EmotionProcessorController controller = new EmotionProcessorController();
            EmotionInputListener listener = new EmotionInputListener(controller);

            view.setEmotionChangeListener(listener);

            ClientIdPanel clientIdPanel = new ClientIdPanel();

            MQTTPublisher publisher = new MQTTPublisher();
            new Thread(publisher, "MQTT-Publisher").start();

            frame.add(clientIdPanel, BorderLayout.NORTH);
            frame.add(view,  BorderLayout.CENTER);
            frame.pack();
            frame.setVisible(true);
        });
    }
}

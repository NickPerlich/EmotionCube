import javax.swing.*;
import java.awt.*;

/**
 * Authors: Joel Puthankalam, Tymon Vu, Nick Perlich
 * ClientID panel to manually change the clientID sent via MQTT.
 * Once id is entered press submit button
 */
public class ClientIdPanel extends JPanel {

    private final JTextField clientIdField;

    public ClientIdPanel() {
        setLayout(new FlowLayout());

        clientIdField = new JTextField(15);
        JButton submitButton = new JButton("Set Client ID");

        add(new JLabel("Client ID:"));
        add(clientIdField);
        add(submitButton);

        // When user clicks submit, update the blackboard
        submitButton.addActionListener(e -> {
            String id = clientIdField.getText().trim();

            if (!id.isEmpty()) {
                EmotionBlackboard.getInstance().setClientId(id);
                System.out.println("Client ID set -> " + id);
            } else {
                JOptionPane.showMessageDialog(
                        this,
                        "Client ID cannot be empty.",
                        "Input Error",
                        JOptionPane.WARNING_MESSAGE
                );
            }
        });
    }
}

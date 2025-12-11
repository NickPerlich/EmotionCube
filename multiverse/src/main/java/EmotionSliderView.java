import javax.swing.*;
import javax.swing.event.ChangeListener;
import java.awt.*;

/**
 *  Authors: Joel Puthankalam, Tymon Vu, Nick Perlich
 *  Swing-based view providing sliders for user-controlled emotion input.
 *
 * <p>This class is responsible solely for rendering the UI components
 * and reporting finalized emotion values through an
 * {@link EmotionChangeListener}.</p>
 *
 * <p>Emotion change events are only dispatched after slider adjustments
 * are completed to avoid flooding downstream systems with transient
 * input values.</p>
 */
public class EmotionSliderView extends JPanel {

    private EmotionChangeListener listener;

    private JSlider focusSlider;
    private JSlider calmSlider;
    private JSlider stressSlider;

    private JTextField clientId;

    /**
     * Constructs the emotion slider view and initializes UI components.
     */
    public EmotionSliderView() {
        setLayout(new GridLayout(3, 1));
        setPreferredSize(new Dimension(500, 300));
        initSliders();
    }

    /**
     * Registers a listener to be notified when emotion values change.
     *
     * @param listener listener to notify of finalized emotion updates
     */
    public void setEmotionChangeListener(EmotionChangeListener listener) {
        this.listener = listener;
    }

    /**
     * Initializes and adds all emotion sliders to the view.
     */
    private void initSliders() {
        focusSlider = createSlider("Focus");
        calmSlider = createSlider("Calm");
        stressSlider = createSlider("Stress");

        add(labeledSlider("Focus", focusSlider));
        add(labeledSlider("Calm", calmSlider));
        add(labeledSlider("Stress", stressSlider));
    }

    /**
     * Creates a configured slider for a given emotion.
     *
     * @param name name of the emotion represented by the slider
     * @return configured {@link JSlider}
     */
    private JSlider createSlider(String name) {
        JSlider slider = new JSlider(0, 100, 50);
        slider.setMajorTickSpacing(25);
        slider.setPaintTicks(true);
        slider.setPaintLabels(true);

        ChangeListener changeListener = e -> notifyListener();
        slider.addChangeListener(changeListener);

        return slider;
    }

    /**
     * Creates a labeled panel for a slider component.
     *
     * @param labelText text label describing the slider
     * @param slider    slider component
     * @return panel containing the label and slider
     */
    private JPanel labeledSlider(String labelText, JSlider slider) {
        JPanel panel = new JPanel(new BorderLayout());
        panel.add(new JLabel(labelText), BorderLayout.NORTH);
        panel.add(slider, BorderLayout.CENTER);
        return panel;
    }

    /**
     * Notifies the registered listener of emotion changes once
     * slider adjustment has completed.
     */
    private void notifyListener() {
        if (listener == null) return;

        if (focusSlider.getValueIsAdjusting()
                || calmSlider.getValueIsAdjusting()
                || stressSlider.getValueIsAdjusting()) {
            return; // user is still dragging
        }

        listener.onEmotionChanged(
                focusSlider.getValue() / 100.0,
                calmSlider.getValue() / 100.0,
                stressSlider.getValue() / 100.0
        );
    }
}

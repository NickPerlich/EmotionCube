/**
 *
 * Authors: Joel Puthankalam, Tymon Vu, Nick Perlich
 * Listener implementation that receives emotion change events from the UI.
 *
 * <p>This class acts as a bridge between the emotion slider view and the
 * emotion processing controller. It forwards raw emotion values without
 * performing any processing or state management.</p>
 * 
 */
public class EmotionInputListener implements EmotionChangeListener {

    private final EmotionProcessorController controller;

    /**
     * Constructs an emotion input listener.
     *
     * @param controller the controller responsible for processing
     *                   emotion input values
     */
    public EmotionInputListener(EmotionProcessorController controller) {
        this.controller = controller;
    }

    /**
     * Called when the user updates emotion values through the UI.
     *
     * <p>This method forwards the received emotion values to the
     * {@link EmotionProcessorController} for interpretation and handling.</p>
     *
     * @param focus  focus value in the range [0.0, 1.0]
     * @param calm   calm value in the range [0.0, 1.0]
     * @param stress stress value in the range [0.0, 1.0]
     */
    @Override
    public void onEmotionChanged(double focus, double calm, double stress) {
        controller.processEmotionInput(focus, calm, stress);
    }

}

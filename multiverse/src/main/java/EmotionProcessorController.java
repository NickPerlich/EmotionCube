/**
 * Authors: Joel Puthankalam, Tymon Vu, Nick Perlich
 * Processes raw emotion input and updates the blackboard.
 */
public class EmotionProcessorController {

    /**
     * Handles raw emotion input values.
     *
     * @param focus  focus value (0.0 - 1.0)
     * @param calm   calm value (0.0 - 1.0)
     * @param stress stress value (0.0 - 1.0)
     */
    public void processEmotionInput(double focus, double calm, double stress) {

        // Convert raw values to domain state
        EmotionState state = new EmotionState(focus, calm, stress);

        // Update blackboard
        EmotionBlackboard.getInstance().setEmotionState(state);
    }
}

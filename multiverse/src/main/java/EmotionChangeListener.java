/**
 * Listener interface for emotion slider changes.
 */
public interface EmotionChangeListener {

    /**
     * Called when any emotion slider value changes.
     *
     * @param focus  focus value (0.0 - 1.0)
     * @param calm   calm value (0.0 - 1.0)
     * @param stress stress value (0.0 - 1.0)
     */
    void onEmotionChanged(double focus, double calm, double stress);
}
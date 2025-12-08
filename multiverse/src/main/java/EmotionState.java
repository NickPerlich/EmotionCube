/**
 * Immutable snapshot representing the current emotional state.
 *
 * <p>This class serves as a lightweight domain object used to transport
 * emotion values through the system. Instances are immutable to ensure
 * thread safety and to prevent shared state from being modified after
 * publication.</p>
 */
public class EmotionState {

    private final double focus;
    private final double calm;
    private final double stress;

    /**
          * Creates a new immutable emotion state.
     *
     * @param focus  focus value in the range [0.0, 1.0]
     * @param calm   calm value in the range [0.0, 1.0]
     * @param stress stress value in the range [0.0, 1.0]
     */
    public EmotionState(double focus, double calm, double stress) {
        this.focus = focus;
        this.calm = calm;
        this.stress = stress;
    }

    /**
     * Returns the focus value.
     *
     * @return focus level
     */
    public double getFocus() {
        return focus;
    }

    /**
     * Returns the calm value.
     *
     * @return calm level
     */
    public double getCalm() {
        return calm;
    }

    /**
     * Returns the stress value.
     *
     * @return stress level
     */
    public double getStress() {
        return stress;
    }

    /**
     * Returns a string representation of the emotion state.
     *
     * @return formatted emotion state string
     */
    @Override
    public String toString() {
        return "EmotionState{" +
                "focus=" + focus +
                ", calm=" + calm +
                ", stress=" + stress +
                '}';
    }
}

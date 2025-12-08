import java.beans.PropertyChangeListener;
import java.beans.PropertyChangeSupport;

/**
 * Singleton blackboard for storing and broadcasting emotion state.
 */
public class EmotionBlackboard {

    private static EmotionBlackboard instance;

    private EmotionState emotionState;
    private final PropertyChangeSupport pcs;

    /**
     * Private constructor to enforce Singleton.
     */
    private EmotionBlackboard() {
        this.pcs = new PropertyChangeSupport(this);
    }

    /**
     * Returns the single instance of the blackboard.
     *
     * @return EmotionBlackboard instance
     */
    public static synchronized EmotionBlackboard getInstance() {
        if (instance == null) {
            instance = new EmotionBlackboard();
        }
        return instance;
    }

    /**
     * Updates the emotion state and notifies observers.
     *
     * @param newState new emotion state
     */
    public void setEmotionState(EmotionState newState) {
        EmotionState oldState = this.emotionState;
        this.emotionState = newState;
        pcs.firePropertyChange("emotionState", oldState, newState);
    }

    /**
     * Returns the current emotion state.
     *
     * @return current emotion state
     */
    public EmotionState getEmotionState() {
        return emotionState;
    }

    /**
     * Adds a property change listener.
     *
     * @param listener listener to add
     */
    public void addPropertyChangeListener(PropertyChangeListener listener) {
        pcs.addPropertyChangeListener(listener);
    }

    /**
     * Removes a property change listener.
     *
     * @param listener listener to remove
     */
    public void removePropertyChangeListener(PropertyChangeListener listener) {
        pcs.removePropertyChangeListener(listener);
    }
}

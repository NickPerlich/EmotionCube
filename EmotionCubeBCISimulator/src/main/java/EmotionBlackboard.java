import java.beans.PropertyChangeListener;
import java.beans.PropertyChangeSupport;

/**
 * Authors: Joel Puthankalam, Tymon Vu, Nick Perlich
 * Singleton blackboard for storing and broadcasting emotion state.
 */
public class EmotionBlackboard {

    private static EmotionBlackboard instance;
    private String clientId = "joelp";
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
     * Returns the clientID
     *
     */
    public String getClientId() {
        return clientId;
    }

    public void setClientId(String clientId) {
        System.out.println(clientId);
        String oldClientId = this.clientId;
        this.clientId = clientId;
        pcs.firePropertyChange("clientId", oldClientId, clientId);
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

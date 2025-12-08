public class old {

    public static final String BROKER_URL = "tcp://broker.hivemq.com:1883";
    public static final String TOPIC = "bci/emotions";
    public static final String ID = "tymonvu"; 

    public static void main(String[] args) {

        // Start subscriber
        new Thread(new MQTTSub()).start();

        // Start publisher (fake BCI)
        new Thread(new MQTTPub()).start();
    }
}

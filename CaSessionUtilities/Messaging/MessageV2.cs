namespace CaSessionUtilities.Messaging;

public class MessageV2 {
    private string note;
    private RdeSessionArgs rdeSessionArgs;
    private MessageFile[] files;

    public MessageV2(string note, RdeSessionArgs rdeSessionArgs, MessageFile[] objects) {

        this.note = note;
        this.rdeSessionArgs = rdeSessionArgs;
        this.files = objects;
    }

    public string getNote() {
        return note;
    }

    public MessageFile[] getFiles() {
        return files;
    }

    public RdeSessionArgs getRdeSessionArgs() {
        return rdeSessionArgs;
    }
}

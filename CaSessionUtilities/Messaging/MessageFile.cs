namespace CaSessionUtilities.Messaging;

public class MessageFile {
    private string filename;
    private byte[] content;

    public MessageFile(string filename, byte[] content) {
        this.filename = filename;
        this.content = content;
    }

    public string getFilename() {
        return filename;
    }

    public byte[] getContent() {
        return content;
    }
}

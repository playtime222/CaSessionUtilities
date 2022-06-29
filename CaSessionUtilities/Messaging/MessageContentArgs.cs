namespace CaSessionUtilities.Messaging;

/**
 * Pass this and a crypto spec to a message formatter
 * TODO use a builder?
 */
public class MessageContentArgs
{
    private string unencryptedNote;
    private List<FileArgs> files = new();

    public string getUnencryptedNote() { return unencryptedNote; }
    public void setUnencryptedNote(string unencryptedNote)
    {
        this.unencryptedNote = unencryptedNote;
    }

    //public FileArgs getFileArgs(int index) { return files.get(index); }
    public FileArgs[] getFileArgs()
    {

        return files.ToArray();
    }

    public void add(FileArgs file)
    {
        files.Add(file);
    }
}


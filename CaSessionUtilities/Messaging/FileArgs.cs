namespace CaSessionUtilities.Messaging;

//TODO validation
public class FileArgs
{
    private string name;
    private byte[] content;

    public FileArgs()
    {
    }

    public FileArgs(string name, byte[] content)
    {
        this.name = name;
        this.content = content;
    }

    public string getName()
    {
        return name;
    }

    public void setName(string name)
    {
        this.name = name;
    }

    public byte[] getContent()
    {
        return content;
    }

    public void setContent(byte[] content)
    {
        this.content = content;
    }

    //TODO MimeType?
}



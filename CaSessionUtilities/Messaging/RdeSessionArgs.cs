namespace CaSessionUtilities.Messaging;

/**
 * This is the part that will go WITH the encrypted message and is NOT encrypted, put the rest in DocumentEnrollment and message/conversation config.
 * NB Fcont is ONLY needed on the server to simulate the RB call to get the SecretKey
 * <p>
 * Serialised to JSON
 */
public class RdeSessionArgs {
    //The document the receiver needs to decrypt
    public string documentDisplayName;

    public string cipher;
    public string iv;
    public string encryptionAlgorithm;

    //Parameters for obtaining the decryption key
    public string caEncryptedCommand; //Need this to obtain
    public string pcdPublicKey; //from EAC CA session

    //Could get these 2 by doing starting a standard CA session before the decryption call
    public string caProtocolOid; //from enrollment EAC CA session

    //public string getCaProtocolOid() {
    //    return caProtocolOid;
    //}

    //public void setCaProtocolOid(string caProtocolOid) {
    //    this.caProtocolOid = caProtocolOid;
    //}

    //public string getDocumentDisplayName() {
    //    return documentDisplayName;
    //}

    //public void setDocumentDisplayName(string documentDisplayName) {
    //    this.documentDisplayName = documentDisplayName;
    //}

    //public string getCipher() {
    //    return cipher;
    //}

    //public void setCipher(string cipher) {
    //    this.cipher = cipher;
    //}

    //public byte[] getCaEncryptedCommand() {
    //    return caEncryptedCommand;
    //}

    //public void setCaEncryptedCommand(byte[] caEncryptedCommand) {
    //    this.caEncryptedCommand = caEncryptedCommand;
    //}

    //public byte[] getPcdPublicKey() {
    //    return pcdPublicKey;
    //}

    //public void setPcdPublicKey(byte[] pcdPublicKey) {
    //    this.pcdPublicKey = pcdPublicKey;
    //}

    //public byte[] getIv() {
    //    return iv;
    //}

    //public void setIv(byte[] iv) {
    //    this.iv = iv;
    //}

    //public string getEncryptionAlgorithm() {
    //    return encryptionAlgorithm;
    //}

    //public void setEncryptionAlgorithm(string encryptionAlgorithm) {
    //    this.encryptionAlgorithm = encryptionAlgorithm;
    //}
}

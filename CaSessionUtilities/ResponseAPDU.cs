using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilities;

/**
     * A response APDU as defined in ISO/IEC 7816-4. It consists of a conditional
     * body and a two byte trailer.
     * This class does not attempt to verify that the APDU encodes a semantically
     * valid response.
     *
     * <p>Instances of this class are immutable. Where data is passed in or out
     * via byte arrays, defensive cloning is performed.
     *
     * @see CommandAPDU
     * @see CardChannel#transmit CardChannel.transmit
     *
     * @since   1.6
     * @author  Andreas Sterbenz
     * @author  JSR 268 Expert Group
     */
public class ResponseAPDU
{

    private static long serialVersionUID = 6962744978375594225L;

    /** @serial */
    private byte[] apdu;

    /**
         * Constructs a ResponseAPDU from a byte array containing the complete
         * APDU contents (conditional body and trailed).
         *
         * <p>Note that the byte array is cloned to protect against subsequent
         * modification.
         *
         * @param apdu the complete response APDU
         *
         * @throws NullPointerException if apdu is null
         * @throws IllegalArgumentException if apdu.Length is less than 2
         */
    public ResponseAPDU(byte[] apdu)
    {
        if (apdu.Length < 2)
            throw new ArgumentException("apdu must be at least 2 bytes long");

        this.apdu = apdu;
    }


    /**
         * Returns the number of data bytes in the response body (Nr) or 0 if this
         * APDU has no body. This call is equivalent to
         * <code>getData().Length</code>.
         *
         * @return the number of data bytes in the response body or 0 if this APDU
         * has no body.
         */
    public int getNr()
    {
        return apdu.Length - 2;
    }

    /**
         * Returns a copy of the data bytes in the response body. If this APDU as
         * no body, this method returns a byte array with a length of zero.
         *
         * @return a copy of the data bytes in the response body or the empty
         *    byte array if this APDU has no body.
         */
    public byte[] getData()
    {
        byte[] data = new byte[apdu.Length - 2];
        Array.Copy(apdu, 0, data, 0, data.Length);
        return data;
    }

    /**
         * Returns the value of the status byte SW1 as a value between 0 and 255.
         *
         * @return the value of the status byte SW1 as a value between 0 and 255.
         */
    public int getSW1()
    {
        return apdu[apdu.Length - 2] & 0xff;
    }

    /**
         * Returns the value of the status byte SW2 as a value between 0 and 255.
         *
         * @return the value of the status byte SW2 as a value between 0 and 255.
         */
    public int getSW2()
    {
        return apdu[apdu.Length - 1] & 0xff;
    }

    /**
         * Returns the value of the status bytes SW1 and SW2 as a single
         * status word SW.
         * It is defined as
         * {@code (getSW1() << 8) | getSW2()}
         *
         * @return the value of the status word SW.
         */
    public int getSW()
    {
        return (getSW1() << 8) | getSW2();
    }

    /**
         * Returns a copy of the bytes in this APDU.
         *
         * @return a copy of the bytes in this APDU.
         */

    /**
     * Returns a string representation of this response APDU.
     *
     * @return a string representation of this response APDU.
     */
    //public string toString()
    //{
    //    return "ResponseAPDU: " + apdu.Length + " bytes, SW="
    //        + Hex.toHexString(getSW());
    //}

    /**
     * Compares the specified object with this response APDU for equality.
     * Returns true if the given object is also a ResponseAPDU and its bytes are
     * identical to the bytes in this ResponseAPDU.
     *
     * @param obj the object to be compared for equality with this response APDU
     * @return true if the specified object is equal to this response APDU
     */
    //public bool equals(Object obj)
    //{
    //    if (this == obj)
    //    {
    //        return true;
    //    }
    //    if (obj is ResponseAPDU == false) {
    //        return false;
    //    }
    //    ResponseAPDU other = (ResponseAPDU)obj;
    //    return Array.Equals(this.apdu, other.apdu);
    //}

    ///**
    // * Returns the hash code value for this response APDU.
    // *
    // * @return the hash code value for this response APDU.
    // */
    //public int hashCode()
    //{
    //    return Arrays.hashCode(apdu);
    //}

    //private void readObject(java.io.ObjectInputStream in)
    //        throws java.io.IOException, ClassNotFoundException {
    //    apdu = (byte[])in.readUnshared();
    //    check(apdu);
    //}

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Utilities;

namespace CaSessionUtilities.Wrapping.Implementation
{

    /**
     * A command APDU following the structure defined in ISO/IEC 7816-4.
     * It consists of a four byte header and a conditional body of variable length.
     * This class does not attempt to verify that the APDU encodes a semantically
     * valid command.
     *
     * <p>Note that when the expected length of the response APDU is specified
     * in the {@linkplain #CommandAPDU(int,int,int,int,int) constructors},
     * the actual length (Ne) must be specified, not its
     * encoded form (Le). Similarly, {@linkplain #getNe} returns the actual
     * value Ne. In other words, a value of 0 means "no data in the response APDU"
     * rather than "maximum length."
     *
     * <p>This class supports both the short and extended forms of length
     * encoding for Ne and Nc. However, note that not all terminals and Smart Cards
     * are capable of accepting APDUs that use the extended form.
     *
     * <p>For the header bytes CLA, INS, P1, and P2 the Java type <code>int</code>
     * is used to represent the 8 bit unsigned values. In the constructors, only
     * the 8 lowest bits of the <code>int</code> value specified by the application
     * are significant. The accessor methods always return the byte as an unsigned
     * value between 0 and 255.
     *
     * <p>Instances of this class are immutable. Where data is passed in or out
     * via byte arrays, defensive cloning is performed.
     *
     * @see ResponseAPDU
     * @see CardChannel#transmit CardChannel.transmit
     *
     * @since   1.6
     * @author  Andreas Sterbenz
     * @author  JSR 268 Expert Group
     */
    public class CommandAPDU //implements java.io.Serializable
    {

        //  @SuppressWarnings("unused")
        //private static int MAX_APDU_SIZE = 65544;

        /** @serial */
        private readonly byte[] _Content;

        //Not serialised
        // value of _Nc
        private readonly int _Nc;

        //Not serialised
        // value of _Ne
        private readonly int _Ne;

        //Not serialised
        // index of start of data within the _Content array
        private readonly byte _DataOffset;

        private void checkArrayBounds(byte[] b, int ofs, int len)
        {
            if (ofs < 0 || len < 0)
                throw new ArgumentException("Offset and length must not be negative");

            if (b == null)
            {
                if (ofs != 0 && len != 0)
                    throw new ArgumentException("offset and length must be 0 if array is null");
            }
            else
            {
                if (ofs > b.Length - len)
                    throw new ArgumentException("Offset plus length exceed array size");
            }
        }

        /**
         * Constructs a CommandAPDU from the four header bytes. This is case 1
         * in ISO 7816, no command body.
         *
         * @param cla the class byte CLA
         * @param ins the instruction byte INS
         * @param p1 the parameter byte P1
         * @param p2 the parameter byte P2
         */
        //public CommandAPDU(int cla, int ins, int p1, int p2): this(cla, ins, p1, p2, null, 0, 0, 0)
        //{
        //}

        /**
         * Constructs a CommandAPDU from the four header bytes and the expected
         * response data length. This is case 2 in ISO 7816, empty command data
         * field with Ne specified. If Ne is 0, the APDU is encoded as ISO 7816
         * case 1.
         *
         * @param cla the class byte CLA
         * @param ins the instruction byte INS
         * @param p1 the parameter byte P1
         * @param p2 the parameter byte P2
         * @param _Ne the maximum number of expected data bytes in a response APDU
         *
         * @throws ArgumentException if _Ne is negative or greater than
         *   65536
         */
        public CommandAPDU(int cla, int ins, int p1, int p2, int ne) : this(cla, ins, p1, p2, null, 0, 0, ne)
        {
        }

        /**
         * Constructs a CommandAPDU from the four header bytes and command data.
         * This is case 3 in ISO 7816, command data present and Ne absent. The
         * value Nc is taken as data.Length. If <code>data</code> is null or
         * its length is 0, the APDU is encoded as ISO 7816 case 1.
         *
         * <p>Note that the data bytes are copied to protect against
         * subsequent modification.
         *
         * @param cla the class byte CLA
         * @param ins the instruction byte INS
         * @param p1 the parameter byte P1
         * @param p2 the parameter byte P2
         * @param data the byte array containing the data bytes of the command body
         *
         * @throws ArgumentException if data.Length is greater than 65535
         */
        //public CommandAPDU(int cla, int ins, int p1, int p2, byte[] data) : this(cla, ins, p1, p2, data, 0, arrayLength(data), 0)
        //{
        //}

        /**
         * Constructs a CommandAPDU from the four header bytes and command data.
         * This is case 3 in ISO 7816, command data present and Ne absent. The
         * value Nc is taken as dataLength. If <code>dataLength</code>
         * is 0, the APDU is encoded as ISO 7816 case 1.
         *
         * <p>Note that the data bytes are copied to protect against
         * subsequent modification.
         *
         * @param cla the class byte CLA
         * @param ins the instruction byte INS
         * @param p1 the parameter byte P1
         * @param p2 the parameter byte P2
         * @param data the byte array containing the data bytes of the command body
         * @param _DataOffset the offset in the byte array at which the data
         *   bytes of the command body begin
         * @param dataLength the number of the data bytes in the command body
         *
         * @throws NullPointerException if data is null and dataLength is not 0
         * @throws ArgumentException if _DataOffset or dataLength are
         *   negative or if _DataOffset + dataLength are greater than data.Length
         *   or if dataLength is greater than 65535
         */
        //public CommandAPDU(int cla, int ins, int p1, int p2, byte[] data, int dataOffset, int dataLength) : this(cla, ins, p1, p2, data, dataOffset, dataLength, 0)
        //{
        //}

        /**
         * Constructs a CommandAPDU from the four header bytes, command data,
         * and expected response data length. This is case 4 in ISO 7816,
         * command data and Ne present. The value Nc is taken as data.Length
         * if <code>data</code> is non-null and as 0 otherwise. If Ne or Nc
         * are zero, the APDU is encoded as case 1, 2, or 3 per ISO 7816.
         *
         * <p>Note that the data bytes are copied to protect against
         * subsequent modification.
         *
         * @param cla the class byte CLA
         * @param ins the instruction byte INS
         * @param p1 the parameter byte P1
         * @param p2 the parameter byte P2
         * @param data the byte array containing the data bytes of the command body
         * @param _Ne the maximum number of expected data bytes in a response APDU
         *
         * @throws ArgumentException if data.Length is greater than 65535
         *   or if _Ne is negative or greater than 65536
         */
        public CommandAPDU(int cla, int ins, int p1, int p2, byte[] data, int ne) : this(cla, ins, p1, p2, data, 0, data?.Length ?? 0, ne)
        {
        }

        /**
         * Command APDU encoding options:
         *
         * case 1:  |CLA|INS|P1 |P2 |                                 len = 4
         * case 2s: |CLA|INS|P1 |P2 |LE |                             len = 5
         * case 3s: |CLA|INS|P1 |P2 |LC |...BODY...|                  len = 6..260
         * case 4s: |CLA|INS|P1 |P2 |LC |...BODY...|LE |              len = 7..261
         * case 2e: |CLA|INS|P1 |P2 |00 |LE1|LE2|                     len = 7
         * case 3e: |CLA|INS|P1 |P2 |00 |LC1|LC2|...BODY...|          len = 8..65542
         * case 4e: |CLA|INS|P1 |P2 |00 |LC1|LC2|...BODY...|LE1|LE2|  len =10..65544
         *
         * LE, LE1, LE2 may be 0x00.
         * LC must not be 0x00 and LC1|LC2 must not be 0x00|0x00
         */


        /**
         * Constructs a CommandAPDU from the four header bytes, command data,
         * and expected response data length. This is case 4 in ISO 7816,
         * command data and Le present. The value Nc is taken as
         * <code>dataLength</code>.
         * If Ne or Nc
         * are zero, the APDU is encoded as case 1, 2, or 3 per ISO 7816.
         *
         * <p>Note that the data bytes are copied to protect against
         * subsequent modification.
         *
         * @param cla the class byte CLA
         * @param ins the instruction byte INS
         * @param p1 the parameter byte P1
         * @param p2 the parameter byte P2
         * @param data the byte array containing the data bytes of the command body
         * @param _DataOffset the offset in the byte array at which the data
         *   bytes of the command body begin
         * @param dataLength the number of the data bytes in the command body
         * @param _Ne the maximum number of expected data bytes in a response APDU
         *
         * @throws NullPointerException if data is null and dataLength is not 0
         * @throws ArgumentException if _DataOffset or dataLength are
         *   negative or if _DataOffset + dataLength are greater than data.Length,
         *   or if _Ne is negative or greater than 65536,
         *   or if dataLength is greater than 65535
         */
        private CommandAPDU(int cla, int ins, int p1, int p2, byte[] data, int dataOffset, int dataLength, int ne)
        {
            checkArrayBounds(data, dataOffset, dataLength);
            if (dataLength > 65535)
                throw new ArgumentException("dataLength is too large");

            if (ne < 0 || ne > 65536)
                throw new ArgumentOutOfRangeException(nameof(ne));

            _Ne = ne;

            _Nc = dataLength;

            if (dataLength == 0)
            {
                if (ne == 0)
                {
                    // case 1
                    _Content = new byte[4];
                    SetHeader(cla, ins, p1, p2);
                }
                else
                {
                    // case 2s or 2e
                    if (ne <= 256)
                    {
                        // case 2s
                        // 256 is encoded as 0x00
                        var len = (byte)(ne != 256 ? ne : 0);
                        _Content = new byte[5];
                        SetHeader(cla, ins, p1, p2);
                        _Content[4] = len;
                    }
                    else
                    {
                        // case 2e
                        byte l1, l2;
                        // 65536 is encoded as 0x00 0x00
                        if (ne == 65536)
                        {
                            l1 = 0;
                            l2 = 0;
                        }
                        else
                        {
                            l1 = (byte)(ne >> 8);
                            l2 = (byte)ne;
                        }
                        _Content = new byte[7];
                        SetHeader(cla, ins, p1, p2);
                        _Content[5] = l1;
                        _Content[6] = l2;
                    }
                }
            }
            else
            {
                if (ne == 0)
                {
                    // case 3s or 3e
                    if (dataLength <= 255)
                    {
                        // case 3s
                        _Content = new byte[4 + 1 + dataLength];
                        SetHeader(cla, ins, p1, p2);
                        _Content[4] = (byte)dataLength;
                        _DataOffset = 5;
                        Array.Copy(data, dataOffset, _Content, 5, dataLength);
                    }
                    else
                    {
                        // case 3e
                        _Content = new byte[4 + 3 + dataLength];
                        SetHeader(cla, ins, p1, p2);
                        _Content[4] = 0;
                        _Content[5] = (byte)(dataLength >> 8);
                        _Content[6] = (byte)dataLength;
                        _DataOffset = 7;
                        Array.Copy(data, dataOffset, _Content, 7, dataLength);
                    }
                }
                else
                {
                    // case 4s or 4e
                    if (dataLength <= 255 && ne <= 256)
                    {
                        // case 4s
                        _Content = new byte[4 + 2 + dataLength];
                        SetHeader(cla, ins, p1, p2);
                        _Content[4] = (byte)dataLength;
                        _DataOffset = 5;
                        Array.Copy(data, dataOffset, _Content, 5, dataLength);
                        _Content[^1] = (byte)(ne != 256 ? ne : 0);
                    }
                    else
                    {
                        // case 4e
                        _Content = new byte[4 + 5 + dataLength];
                        SetHeader(cla, ins, p1, p2);
                        _Content[4] = 0;
                        _Content[5] = (byte)(dataLength >> 8);
                        _Content[6] = (byte)dataLength;
                        _DataOffset = 7;
                        Array.Copy(data, dataOffset, _Content, 7, dataLength);
                        if (ne != 65536)
                        {
                            var leOfs = _Content.Length - 2;
                            _Content[leOfs] = (byte)(ne >> 8);
                            _Content[leOfs + 1] = (byte)ne;
                        } // else le == 65536: no need to fill in, encoded as 0
                    }
                }
            }
        }

        private void SetHeader(int cla, int ins, int p1, int p2)
        {
            _Content[0] = (byte)cla;
            _Content[1] = (byte)ins;
            _Content[2] = (byte)p1;
            _Content[3] = (byte)p2;
        }

        /// <summary>
        /// class byte CLA.
        /// </summary>
        public int CLA => _Content[0] & 0xff;

        /// <summary>
        /// class byte CLA.
        /// </summary>
        public int INS => _Content[1] & 0xff;

        /// <summary>
        /// parameter byte P1
        /// </summary>
        public int P1 => _Content[2] & 0xff;

        /// <summary>
        /// parameter byte P2.
        /// </summary>
        public int P2 => _Content[3] & 0xff;

        /**
         * Returns the number of data bytes in the command body (Nc) or 0 if this
         * APDU has no body. This call is equivalent to
         * <code>getData().Length</code>.
         *
         * @return the number of data bytes in the command body or 0 if this APDU
         * has no body.
         */

        /// <summary>
        /// class byte CLA.
        /// </summary>
        public int Nc => _Nc;

        /**
         * Returns a copy of the data bytes in the command body. If this APDU as
         * no body, this method returns a byte array with length zero.
         *
         * @return a copy of the data bytes in the command body or the empty
         *    byte array if this APDU has no body.
         */
        //TODO pretty sure read length is not command 'data'
        public byte[] getData()
        {
            var data = new byte[_Nc];
            Array.Copy(_Content, _DataOffset, data, 0, _Nc);
            return data;
        }

        /**
         * Returns the maximum number of expected data bytes in a response
         * APDU (Ne).
         *
         * @return the maximum number of expected data bytes in a response APDU.
         */
        public int Ne => _Ne;

        /// <summary>
        /// Contents
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            return _Content;
            //return Arrays.CopyOf(_Content, _Content.Length);
        }

        /**
         * Returns a string representation of this command APDU.
         *
         * @return a string representation of this command APDU.
         */
        //public override string ToString()
        //{
        //    return "CommmandAPDU: " + _Content.Length + " bytes, _Nc=" + _Nc + ", _Ne=" + _Ne;
        //}

        /**
         * Compares the specified object with this command APDU for equality.
         * Returns true if the given object is also a CommandAPDU and its bytes are
         * identical to the bytes in this CommandAPDU.
         *
         * @param obj the object to be compared for equality with this command APDU
         * @return true if the specified object is equal to this command APDU
         */
        //public override bool Equals(object obj)
        //{
        //    if (this == obj)
        //    {
        //        return true;
        //    }
        //    if (obj instanceof CommandAPDU == false) {
        //        return false;
        //    }
        //    CommandAPDU other = (CommandAPDU)obj;
        //    return Arrays.equals(_Content, other._Content);
        //}

        /**
         * Returns the hash code value for this command APDU.
         *
         * @return the hash code value for this command APDU.
         */
        //      @Override
        //public int hashCode()
        //      {
        //          return Arrays.hashCode(_Content);
        //      }

        //private void readObject(java.io.ObjectInputStream in)
        //{
        //      _Content = (byte[])in.readUnshared();
        //      // initialize transient fields
        //      parse();
        //  }

    }

}


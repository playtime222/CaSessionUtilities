namespace CaSessionUtilities;

public static class ISO7816
{
    public const byte OFFSET_CLA = (byte)0;
    public const byte OFFSET_INS = (byte)1;
    public const byte OFFSET_P1 = (byte)2;
    public const byte OFFSET_P2 = (byte)3;
    public const byte OFFSET_LC = (byte)4;
    public const byte OFFSET_CDATA = (byte)5;

    public const byte CLA_ISO7816 = (byte)0x00;
    public const byte CLA_COMMAND_CHAINING = (byte)0x10;

    public const byte INVALIDATE_CHV = 0x04;
    public const byte INS_ERASE_BINARY = 0x0E;
    public const byte INS_VERIFY = 0x20;
    public const byte INS_CHANGE_CHV = 0x24;
    public const byte INS_UNBLOCK_CHV = 0x2C;
    public const byte INS_DECREASE = 0x30;
    public const byte INS_INCREASE = 0x32;
    public const byte INS_DECREASE_STAMPED = 0x34;
    public const byte INS_REHABILITATE_CHV = 0x44;
    public const byte INS_MANAGE_CHANNEL = 0x70;
    public const byte INS_EXTERNAL_AUTHENTICATE = (byte)0x82;
    public const byte INS_MUTUAL_AUTHENTICATE = (byte)0x82;
    public const byte INS_GET_CHALLENGE = (byte)0x84;
    public const byte INS_ASK_RANDOM = (byte)0x84;
    public const byte INS_GIVE_RANDOM = (byte)0x86;
    public const byte INS_INTERNAL_AUTHENTICATE = (byte)0x88;
    public const byte INS_SEEK = (byte)0xA2;
    public const byte INS_SELECT = (byte)0xA4;
    public const byte INS_SELECT_FILE = (byte)0xA4;
    public const byte INS_CLOSE_APPLICATION = (byte)0xAC;
    public const byte INS_READ_BINARY = (byte)0xB0;
    public const byte INS_READ_BINARY2 = (byte)0xB1;
    public const byte INS_READ_RECORD = (byte)0xB2;
    public const byte INS_READ_RECORD2 = (byte)0xB3;
    public const byte INS_READ_RECORDS = (byte)0xB2;
    public const byte INS_READ_BINARY_STAMPED = (byte)0xB4;
    public const byte INS_READ_RECORD_STAMPED = (byte)0xB6;
    public const byte INS_GET_RESPONSE = (byte)0xC0;
    public const byte INS_ENVELOPE = (byte)0xC2;
    public const byte INS_GET_DATA = (byte)0xCA;
    public const byte INS_WRITE_BINARY = (byte)0xD0;
    public const byte INS_WRITE_RECORD = (byte)0xD2;
    public const byte INS_UPDATE_BINARY = (byte)0xD6;
    public const byte INS_LOAD_KEY_FILE = (byte)0xD8;
    public const byte INS_PUT_DATA = (byte)0xDA;
    public const byte INS_UPDATE_RECORD = (byte)0xDC;
    public const byte INS_CREATE_FILE = (byte)0xE0;
    public const byte INS_APPEND_RECORD = (byte)0xE2;
    public const byte INS_DELETE_FILE = (byte)0xE4;
    public const byte INS_PSO = (byte)0x2A;
    public const byte INS_MSE = (byte)0x22;

    public const short SW_BYTES_REMAINING_00 = (short)0x6100;
    public const short SW_STATE_NON_VOLATILE_MEMORY_UNCHANGED_NO_INFORMATION_GIVEN = (short)0x6200;
    public const short SW_END_OF_FILE = (short)0x6282;
    public const short SW_LESS_DATA_RESPONDED_THAN_REQUESTED = (short)0x6287;
    public const short SW_NON_VOLATILE_MEMORY_CHANGED_NO_INFORMATION_GIVEN = (short)0x6300;
    public const short SW_NON_VOLATILE_MEMORY_CHANGED_FILE_FILLED_UP_BY_LAST_WRITE = (short)0x6381;
    public const short SW_NON_VOLATILE_MEMORY_CHANGED_COUNTER_0 = (short)0x63C0;
    public const short SW_WRONG_LENGTH = (short)0x6700;
    public const short SW_LOGICAL_CHANNEL_NOT_SUPPORTED = (short)0x6881;
    public const short SW_SECURE_MESSAGING_NOT_SUPPORTED = (short)0x6882;
    public const short SW_LAST_COMMAND_EXPECTED = (short)0x6883;
    public const short SW_SECURITY_STATUS_NOT_SATISFIED = (short)0x6982;
    public const short SW_FILE_INVALID = (short)0x6983;
    public const short SW_DATA_INVALID = (short)0x6984;
    public const short SW_CONDITIONS_NOT_SATISFIED = (short)0x6985;
    public const short SW_COMMAND_NOT_ALLOWED = (short)0x6986;
    public const short SW_EXPECTED_SM_DATA_OBJECTS_MISSING = (short)0x6987;
    public const short SW_SM_DATA_OBJECTS_INCORRECT = (short)0x6988;
    public const short SW_APPLET_SELECT_FAILED = (short)0x6999;
    public const short SW_KEY_USAGE_ERROR = (short)0x69C1;
    public const short SW_WRONG_DATA = (short)0x6A80;
    public const short SW_FILEHEADER_INCONSISTENT = (short)0x6A80;
    public const short SW_FUNC_NOT_SUPPORTED = (short)0x6A81;
    public const short SW_FILE_NOT_FOUND = (short)0x6A82;
    public const short SW_RECORD_NOT_FOUND = (short)0x6A83;
    public const short SW_FILE_FULL = (short)0x6A84;
    public const short SW_OUT_OF_MEMORY = (short)0x6A84;
    public const short SW_INCORRECT_P1P2 = (short)0x6A86;
    public const short SW_KEY_NOT_FOUND = (short)0x6A88;
    public const short SW_WRONG_P1P2 = (short)0x6B00;
    public const short SW_CORRECT_LENGTH_00 = (short)0x6C00;
    public const short SW_INS_NOT_SUPPORTED = (short)0x6D00;
    public const short SW_CLA_NOT_SUPPORTED = (short)0x6E00;
    public const short SW_UNKNOWN = (short)0x6F00;
    public const short SW_CARD_TERMINATED = (short)0x6FFF;
    public const ushort SW_NO_ERROR = (ushort)0x9000;

    /**
         * ISO 7816-4 Secure Messaging tag for
         * data object for confidentiality, BER-TLV encoded, but not SM-related data objects.
         * See 5.6.4.
         */
    public const int TAG_SM_ENCRYPTED_DATA = 0x85;

    /**
         * ISO 7816-4 Secure Messaging tag for
         * data object for confidentiality, padding indicator byte followed by cryptogram (plain not coded in BER-TLV).
         * See 5.6.4.
         */
    public const int TAG_SM_ENCRYPTED_DATA_WITH_PADDING_INDICATOR = 0x87;

    /**
         * ISO 7816-4 Secure Messaging tag for
         * date object with cryptographic checksum (at least 4 bytes).
         * See 5.6.3.
         */
    public const int TAG_SM_CRYPTOGRAPHIC_CHECKSUM = 0x8E;

    /**
         * ISO 7816-4 Secure Messaging tag for
         * data object with expected response APDU length.
         * Specified elsewhere.
         */
    public const int TAG_SM_EXPECTED_LENGTH = 0x97;

    /**
         * ISO 7816-4 Secure Messaging tag for
         * data object containing secure messaging status information (SW1-SW2).
         * See 5.6.2.
         */
    public const int TAG_SM_STATUS_WORD = 0x99;

}
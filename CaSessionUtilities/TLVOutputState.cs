namespace CaSessionUtilities
{
    /*
     * This file is part of the SCUBA smart card framework.
     *
     * This library is free software; you can redistribute it and/or
     * modify it under the terms of the GNU Lesser General Public
     * License as published by the Free Software Foundation; either
     * version 2.1 of the License, or (at your option) any later version.
     *
     * This library is distributed in the hope that it will be useful,
     * but WITHOUT ANY WARRANTY; without even the implied warranty of
     * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
     * Lesser General Public License for more details.
     *
     * You should have received a copy of the GNU Lesser General Public
     * License along with this library; if not, write to the Free Software
     * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
     *
     * Copyright (C) 2009 - 2018  The SCUBA team.
     *
     * $Id: TLVOutputState.java 320 2020-09-22 19:43:54Z martijno $
     */

    //package net.sf.scuba.tlv;

    //import java.io.ByteArrayOutputStream;
    //import java.io.IOException;
    //import java.util.ArrayDeque;
    //import java.util.Deque;
    //import java.util.logging.Level;
    //import java.util.logging.Logger;

    //import net.sf.scuba.util.Hex;

    /**
     * State to keep track of where we are in a TLV stream.
     * This variant also stores values that were encountered, to be used in
     * {@link TLVOutputStream}.
     *
     * @author Martijn Oostdijk (martijn.oostdijk@gmail.com)
     *
     * @version $Revision: 320 $
     */
    class TLVOutputState
    {

        ///**
        // * Encoded the tags, lengths, and (partial) values.
        // */
        private Queue<TLVStruct> state = new();

        //      /*
        //       * Encoded position, only one can be true.
        //       *
        //       * TFF: ^TLVVVVVV
        //       * FTF: T^LVVVVVV
        //       * FFT: TL^VVVVVV
        //       * FFT: TLVVVV^VV
        //       * TFF: ^
        //       */
        private bool isAtStartOfTag;
        private bool isAtStartOfLength;
        private bool isReadingValue;

        //      public TLVOutputState()
        //      {
        //          this(new ArrayDeque<TLVStruct>(), true, false, false);
        //      }

        //      public TLVOutputState(TLVOutputState original)
        //      {
        //          this(original.getDeepCopyOfState(), original.isAtStartOfTag, original.isAtStartOfLength, original.isReadingValue);
        //      }

        //      private TLVOutputState(Deque<TLVStruct> state, boolean isAtStartOfTag, boolean isAtStartOfLength, boolean isReadingValue)
        //      {
        //          this.state = state;
        //          this.isAtStartOfTag = isAtStartOfTag;
        //          this.isAtStartOfLength = isAtStartOfLength;
        //          this.isReadingValue = isReadingValue;
        //      }

        public bool getIsAtStartOfTag()
        {
            return isAtStartOfTag;
        }

        public bool getIsAtStartOfLength()
        {
            return isAtStartOfLength;
        }

        //      public boolean isProcessingValue()
        //      {
        //          return isReadingValue;
        //      }

        //      public int getTag()
        //      {
        //          if (state.isEmpty())
        //          {
        //              throw new InvalidOperationException("Tag not yet read.");
        //          }
        //          TLVStruct currentObject = state.peek();
        //          return currentObject.getTag();
        //      }

        //      public int getLength()
        //      {
        //          if (state.isEmpty())
        //          {
        //              throw new InvalidOperationException("Length not yet known.");
        //          }
        //          TLVStruct currentObject = state.peek();
        //          int length = currentObject.getLength();
        //          if (length < 0)
        //          {
        //              throw new InvalidOperationException("Length not yet knwon.");
        //          }
        //          return length;
        //      }

        //      public int getValueBytesProcessed()
        //      {
        //          TLVStruct currentObject = state.peek();
        //          return currentObject.getValueBytesProcessed();
        //      }

        //      public int getValueBytesLeft()
        //      {
        //          if (state.isEmpty())
        //          {
        //              throw new InvalidOperationException("Length of value is unknown.");
        //          }
        //          TLVStruct currentObject = state.peek();
        //          int currentLength = currentObject.getLength();
        //          int valueBytesRead = currentObject.getValueBytesProcessed();
        //          return currentLength - valueBytesRead;
        //      }

        public void setTagProcessed(int tag)
        {
            /* Length is set to MAX INT, we will update it when caller calls our setLengthProcessed. */
            TLVStruct obj = new TLVStruct(tag);
            if (state.Count != 0)
            {
                TLVStruct parent = state.Peek();
                byte[] tagBytes = TLVUtil.getTagAsBytes(tag);
                parent.write(tagBytes, 0, tagBytes.Length);
            }
            state.Enqueue(obj);
            this.isAtStartOfTag = false;
            isAtStartOfLength = true;
            isReadingValue = false;
        }

        /**
         * We've passed the length in the stream, but we don't know what it is yet...
         */
        public void setDummyLengthProcessed()
        {
            isAtStartOfTag = false;
            isAtStartOfLength = false;
            isReadingValue = true;
            /* NOTE: doesn't call setLength, so that isLengthSet in stackFrame will remain false. */
        }

        public bool isDummyLengthSet()
        {
            if (state.Count == 0)
            {
                return false;
            }
            return !state.Peek().isLengthSet();
        }

        public void setLengthProcessed(int length)
        {
            if (length < 0)
                throw new ArgumentException("Cannot set negative length (length = " + length + ").");

            TLVStruct obj = state.Dequeue();
            if (state.Count != 0)
            {
                TLVStruct parent = state.Peek();
                byte[] lengthBytes = TLVUtil.getLengthAsBytes(length);
                parent.write(lengthBytes, 0, lengthBytes.Length);
            }
            obj.setLength(length);
            state.Enqueue(obj);
            isAtStartOfTag = false;
            isAtStartOfLength = false;
            isReadingValue = true;
        }

        //      public void updatePreviousLength(int byteCount)
        //      {
        //          if (state.isEmpty())
        //          {
        //              return;
        //          }
        //          TLVStruct currentObject = state.peek();

        //          if (currentObject.isLengthSet && currentObject.getLength() == byteCount)
        //          {
        //              return;
        //          }

        //          currentObject.setLength(byteCount);

        //          if (currentObject.getValueBytesProcessed() == currentObject.getLength())
        //          {
        //              /* Update parent. */
        //              state.pop();
        //              byte[] lengthBytes = TLVUtil.getLengthAsBytes(byteCount);
        //              byte[] value = currentObject.getValue();
        //              updateValueBytesProcessed(lengthBytes, 0, lengthBytes.Length);
        //              updateValueBytesProcessed(value, 0, value.Length);
        //              isAtStartOfTag = true;
        //              isAtStartOfLength = false;
        //              isReadingValue = false;
        //          }
        //      }

        public void updateValueBytesProcessed(byte[] bytes, int offset, int length)
        {
            if (state.Count == 0)
            {
                return;
            }
            TLVStruct currentObject = state.Peek();
            long bytesLeft = currentObject.getLength() - currentObject.getValueBytesProcessed();
            if (length > bytesLeft)
            {
                throw new ArgumentException("Cannot process " + length + " bytes! Only " + bytesLeft + " bytes left in this TLV object " + currentObject);
            }

            currentObject.write(bytes, offset, length);

            if (currentObject.getValueBytesProcessed() == currentObject.getLength())
            {
                /* Stand back! I'm going to try recursion! Update parent(s)... */
                state.Dequeue();
                updateValueBytesProcessed(currentObject.getValue(), 0, currentObject.getLength());
                isAtStartOfTag = true;
                isAtStartOfLength = false;
                isReadingValue = false;
            }
            else
            {
                /* We already have these values?!? */
                isAtStartOfTag = false;
                isAtStartOfLength = false;
                isReadingValue = true;
            }
        }

        //      public byte[] getValue()
        //      {
        //          if (state.isEmpty())
        //          {
        //              throw new InvalidOperationException("Cannot get value yet.");
        //          }
        //          return state.peek().getValue();
        //      }

        //      @Override
        //    public string toString()
        //      {
        //          return state.toString();
        //      }

        //      /*
        //       * TODO: ?? canBeWritten() <==> (state.size() == 1 && state.peek().isLengthSet()
        //       */
        public bool canBeWritten()
        {
            foreach (var stackFrame in state)
            {
                if (!stackFrame.isLengthSet())
                {
                    return false;
                }
            }
            return true;
        }

        //      private Deque<TLVStruct> getDeepCopyOfState()
        //      {
        //          Deque<TLVStruct> newStack = new ArrayDeque<TLVStruct>(state.size());
        //          for (TLVStruct tlvStruct: state)
        //          {
        //              newStack.add(new TLVStruct(tlvStruct));
        //          }
        //          return newStack;
        //      }

        private class TLVStruct
        {

            private int tag;
            private int length;
            private bool _isLengthSet;
            private MemoryStream value;

            public TLVStruct(TLVStruct original) : this(original.tag, original.length, original._isLengthSet, original.getValue())
            {
            }

            public TLVStruct(int tag) : this(tag, Int32.MaxValue, false, null)
            {
            }

            public TLVStruct(int tag, int length, bool isLengthSet, byte[] value)
            {
                this.tag = tag;
                this.length = length;
                this._isLengthSet = isLengthSet;
                this.value = new MemoryStream();
                if (value != null)
                {
                    this.value.Write(value);
                }
            }

            public void setLength(int length)
            {
                this.length = length;
                this._isLengthSet = true;
            }

            public int getTag()
            {
                return tag;
            }

            public int getLength()
            {
                return length;
            }

            public bool isLengthSet()
            {
                return _isLengthSet;
            }

            public long getValueBytesProcessed()
            {
                return value.Length;
            }

            public byte[] getValue()
            {
                return value.ToArray();
            }

            public void write(byte[] bytes, int offset, int length)
            {
                value.Write(bytes, offset, length);
            }

            //  @Override
            //public string toString()
            //  {
            //      byte[] valueBytes = value.toByteArray();
            //      return "[TLVStruct " + Integer.toHexString(tag) + ", " + (isLengthSet ? length : "UNDEFINED") + ", " + Hex.bytesToHexString(valueBytes) + "(" + valueBytes.Length + ") ]";
            //  }
        }
    }
}

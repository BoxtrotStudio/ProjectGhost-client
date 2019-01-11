using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using UnityEngine;

public class Packet {

    private byte[] udpData;
    private MemoryStream tempWriter;
    protected IPlayerClient client;
    protected bool Ended { get; private set; }
    public int Position { get; set; }

    private bool preserve;
    private byte[] preservedData;

    public Packet AttachPacket(byte[] data) {
        this.udpData = data;
        Position = 0;
        return this;
    }

    public void ReuseFor(IPlayerClient client) {
        this.client = client;
        Ended = false;
        Position = 0;
    }

    public void EndTCP() {
        if (client == null)
            return;

        if (Ghost.IsDebug) {
            Debug.Log("Sending " + GetType().FullName);
        }

        byte[] data = EndBytes();
        try {
            client.write(data);
        } catch (IOException e) {
            if (e.Message.Contains("Connection reset"))
            {
                client.disconnect();
            }
            else if (Ghost.IsDebug)
            {
                Debug.Log(e);
            }
        }
    }

    public void EndTCPFlush() {
        if (client == null)
            return;

        byte[] data = EndBytes();
        try {
            client.write(data);
            client.flush();
        } catch (IOException e) {
            if (e.Message.Contains("Connection reset"))
            {
                client.disconnect();
            }
            else if (Ghost.IsDebug)
            {
                Debug.Log(e);
            }
        }

        client = null;
    }

    public void EndUdp() {
        if (client == null)
            return;

        byte[] data = EndBytes();
        client.writeUDP(data);
    }

    public byte[] EndBytes() {
        if (preserve && preservedData != null)
            return preservedData;

        byte[] toReturn = new byte[0];
        if (tempWriter != null)
        {
            toReturn = tempWriter.ToArray();
            try {
                tempWriter.Close();
                tempWriter.Dispose();
            } catch (IOException e) {
                Debug.Log(e);
            }
        }

        if (!preserve) {
            End();
        } else {
            preservedData = toReturn;
        }

        return toReturn;
    }

    private void End() {
        tempWriter = null;
        Ended = true;
    }

    private void ValidateTempStream() {
        if (tempWriter == null)
            tempWriter = new MemoryStream();
    }

    /**
     * Read a certain amount of data as a {@link ConsumedData}. This can be used to
     * transform the read data into a Java primitive
     * @param length How much data to read
     * @return A {@link ConsumedData} object to allow easy transformation of the data
     * @throws IOException If there was a problem reading the data
     * @see ConsumedData
     */
    protected ConsumedData Consume(int length) {
        if (Ended)
            throw new IOException("This packet has already ended!");

        if (udpData == null) {
            byte[] data = new byte[length];
            int endPos = Position + length;
            int i = 0;
            while (Position < endPos) {
                int r = client.read(data, i, length - i);
                if (r == -1)
                    throw new IndexOutOfRangeException("Ran out of data to consume! (Consumed " + i + '/' + length + " bytes)");
                Position += r;
                i += r;
            }

            return new ConsumedData(data);
        } else {
            byte[] data = new byte[length];

            if (Position + length > this.udpData.Length)
                throw new IndexOutOfRangeException("Not enough data to consume! (Expected: " + length + " bytes, only " + (this.udpData.Length - Position - 1) + " bytes left)");

            Array.Copy(this.udpData, Position, data, 0, length);
            Position += length;

            return new ConsumedData(data);
        }
    }

    /**
     * Read a single byte or the entire packet as a {@link ConsumedData}. This can be used to
     * transform the read data into a Java primitive. Whether this method reads a single byte or the entire packet depends on
     * whether this packet is reading from a {@link java.io.InputStream} or a byte array. If from a {@link java.io.InputStream}, then
     * it will return a single byte, otherwise the entire packet
     * @return A {@link ConsumedData} object to allow easy transformation of the data
     * @throws IOException If there was a problem reading the data
     * @see ConsumedData
     */
    protected ConsumedData Consume() {
        if (Ended)
            throw new IOException("This packet has already ended!");

        if (udpData == null) {
            byte[] data = new byte[1];
            int r = client.read(data, 0, 1);
            Position += data.Length;
            return new ConsumedData(data);
        } else {
            int toRead = udpData.Length - Position;
            return Consume(toRead);
        }
    }

    public Packet Write<T>(T? obj) where T: struct 
    {
        if (obj.HasValue)
        {   
            return Write(obj.Value);
        }
        else
        {
            throw new ArgumentException("Cannot write a null type!");
        }
    }

    public Packet Write<T>(T obj) {
        //Handle nullable types
        if (obj == null)
            throw new ArgumentException("Cannot write a null type!");
        
        if (preserve && preservedData != null)
            return this;

        String json = JsonUtility.ToJson(obj);
        byte[] toWrite = Encoding.ASCII.GetBytes(json);

        if (toWrite.Length > 600) { //Only ever gzip the json if it's bigger than 0.6kb
            using (MemoryStream stream = new MemoryStream(toWrite.Length))
            {
                using (GZipStream gzip = new GZipStream(stream, CompressionMode.Compress))
                {
                    gzip.Write(toWrite, 0, toWrite.Length);
                }

                byte[] data = stream.ToArray();

                Write(4 + data.Length); //The size of this chunk
                Write(toWrite.Length); //The size of the uncompressed json
                Write(data); //The compressed json
            }
        } else {
            Write(4 + toWrite.Length); //The size of this chunk
            Write(toWrite.Length); //Size of json string
            Write(toWrite); //json
        }
        return this;
    }

    public Packet Write(byte[] val) {
        if (preserve && preservedData != null)
            return this;

        ValidateTempStream();
        tempWriter.Write(val, 0, val.Length);
        return this;
    }

    public Packet Write(byte[] val, int offset, int length) {
        if (preserve && preservedData != null)
            return this;

        ValidateTempStream();
        tempWriter.Write(val, offset, length);
        return this;
    }

    /**
     * Write a byte into this packet
     * @param val The byte value
     * @return This packet
     */
    public Packet Write(byte val) {
        if (preserve && preservedData != null)
            return this;

        ValidateTempStream();
        tempWriter.WriteByte(val);
        return this;
    }
    
    /**
     * Write a byte into this packet
     * @param val The byte value
     * @return This packet
     */
    public Packet Write(byte? val) {
        if (val.HasValue)
        {
            return Write(val.Value);
        }
        else
        {
            throw new ArgumentException("Cannot write a null type!");
        }
    }

    /**
     * Write an int into this packet
     * @param val The int value
     * @throws IOException if there was a problem writing the value
     * @return This packet
     */
    public Packet Write(int val) {
        if (preserve && preservedData != null)
            return this;

        ValidateTempStream();

        byte[] data = BitConverter.GetBytes(val);
        tempWriter.Write(data, 0, data.Length);
        return this;
    }
    
    /**
     * Write a byte into this packet
     * @param val The byte value
     * @return This packet
     */
    public Packet Write(int? val) {
        if (val.HasValue)
        {
            return Write(val.Value);
        }
        else
        {
            throw new ArgumentException("Cannot write a null type!");
        }
    }

    /**
     * Write a float into this packet
     * @param val The float value
     * @throws IOException if there was a problem writing the value
     * @return This packet
     */
    public Packet Write(float val) {
        if (preserve && preservedData != null)
            return this;

        ValidateTempStream();
        byte[] data = BitConverter.GetBytes(val);
        tempWriter.Write(data, 0, data.Length);
        return this;
    }
    
    /**
     * Write a byte into this packet
     * @param val The byte value
     * @return This packet
     */
    public Packet Write(float? val) {
        if (val.HasValue)
        {
            return Write(val.Value);
        }
        else
        {
            throw new ArgumentException("Cannot write a null type!");
        }
    }

    /**
     * Write a double into this packet
     * @param val The double value
     * @throws IOException if there was a problem writing the value
     * @return This packet
     */
    public Packet Write(double val) {
        if (preserve && preservedData != null)
            return this;

        ValidateTempStream();
        byte[] data = BitConverter.GetBytes(val);
        tempWriter.Write(data, 0, data.Length);
        return this;
    }
    
    /**
     * Write a byte into this packet
     * @param val The byte value
     * @return This packet
     */
    public Packet Write(double? val) {
        if (val.HasValue)
        {
            return Write(val.Value);
        }
        else
        {
            throw new ArgumentException("Cannot write a null type!");
        }
    }

    /**
     * Write a long into this packet
     * @param val The long value
     * @throws IOException if there was a problem writing the value
     * @return This packet
     */
    public Packet Write(long val) {
        if (preserve && preservedData != null)
            return this;

        ValidateTempStream();
        byte[] data = BitConverter.GetBytes(val);
        tempWriter.Write(data, 0, data.Length);
        return this;
    }
    
    /**
     * Write a byte into this packet
     * @param val The byte value
     * @return This packet
     */
    public Packet Write(long? val) {
        if (val.HasValue)
        {
            return Write(val.Value);
        }
        else
        {
            throw new ArgumentException("Cannot write a null type!");
        }
    }

    /**
     * Write a short into this packet
     * @param val The short value
     * @throws IOException if there was a problem writing the value
     * @return This packet
     */
    public Packet Write(short val) {
        if (preserve && preservedData != null)
            return this;

        ValidateTempStream();
        byte[] data = BitConverter.GetBytes(val);
        tempWriter.Write(data, 0, data.Length);
        return this;
    }
    
    /**
     * Write a byte into this packet
     * @param val The byte value
     * @return This packet
     */
    public Packet Write(short? val) {
        if (val.HasValue)
        {
            return Write(val.Value);
        }
        else
        {
            throw new ArgumentException("Cannot write a null type!");
        }
    }

    /**
     * Write a String into this packet, encoded as ASCII
     * @param string The String value
     * @throws IOException if there was a problem writing the value
     * @return This packet
     */
    public Packet Write(string val) {
        if (preserve && preservedData != null)
            return this;

        ValidateTempStream();
        byte[] data = Encoding.ASCII.GetBytes(val);
        tempWriter.Write(data, 0, data.Length);
        return this;
    }

    /**
     * Write a String into this packet encoded as a given {@link Charset}
     * @param string The String value
     * @param charset The charset to encode the String as
     * @throws IOException if there was a problem writing the value
     * @return This packet
     */
    public Packet Write(string val, Encoding charset) {
        if (preserve && preservedData != null)
            return this;

        ValidateTempStream();
        byte[] data = charset.GetBytes(val);
        tempWriter.Write(data, 0, data.Length);
        return this;
    }

    /**
     * Write a boolean into this packet. This will write 1 byte, 1 being true and 0 being false
     * @param value The boolean value
     * @throws IOException if there was a problem writing the value
     * @return This packet
     */
    public Packet Write(bool value) {
        if (preserve && preservedData != null)
            return this;

        ValidateTempStream();
        tempWriter.WriteByte(value ? (byte)1 : (byte)0);
        return this;
    }
    
    /**
     * Write a byte into this packet
     * @param val The byte value
     * @return This packet
     */
    public Packet Write(bool? val) {
        if (val.HasValue)
        {
            return Write(val.Value);
        }
        else
        {
            throw new ArgumentException("Cannot write a null type!");
        }
    }

    /**
     * Append the current size of this packet to the front of the packet. This is useful for dynamic length packets
     * @return This packet
     * @throws IOException If there was an error creating the packet
     */
    public Packet AppendSizeToFront() {
        if (preserve && preservedData != null)
            return this;

        if (tempWriter == null)
            throw new ArgumentException("No data written!");

        byte[] currentData = tempWriter.ToArray();
        tempWriter.Close();
        tempWriter.Dispose();
        tempWriter = null; //Reset writer

        Write(currentData[0]); //Write opCode first
        //We add 4 to include the space for this new info (the packet size)
        Write(currentData.Length + 4); //Then append size of packet to front of packet
        Write(currentData, 1, currentData.Length - 1); //Then write rest of packet
        return this;
    }

    /**
     * Read the contents of this packet and perform logic
     * @return This packet
     * @throws IOException If there was a problem reading the packet
     */
    public Packet HandlePacket(IPlayerClient client) {
        this.client = client;
        Ended = false;
        Handle();
        return this;
    }

    /**
     * Start writing this packet with the given data
     * @param args The data for this packet
     * @return This packet
     * @throws IOException If there was a problem reading the packet
     */
    public Packet WritePacket(IPlayerClient client, params System.Object[] args) {
        this.client = client;

        preserve = false;
        preservedData = null;
        Write(args);
        return this;
    }

    public Packet WriteAndPreservePacket(params System.Object[] args)  {
        preserve = true;
        Write(args);
        return this;
    }

    protected virtual void Handle() {
        throw new NotImplementedException("This packet does not handle data!");
    }

    protected virtual void Write(params System.Object[] args) {
        throw new NotImplementedException("This packet does not write data!");
    }
}

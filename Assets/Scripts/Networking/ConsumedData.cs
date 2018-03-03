using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

/**
 * Represented {@link ConsumedData} that can be transformed into a Java primitive
 */
public class ConsumedData {
    private byte[] data;
    private int index = 0;

    internal ConsumedData(byte[] data) {
        this.data = data;
    }

    /**
     * Transform this data to an int
     * @return The int value
     */
    public int AsInt()
    {
        if (index + 4 >= data.Length)
            throw new IOException("No more data!");
        
        int i =  BitConverter.ToInt32(data, index);
        index += 4;
        return i;
    }

    /**
     * Transform this data into a long
     * @return The long value
     */
    public long AsLong() {
        if (index + 8 >= data.Length)
            throw new IOException("No more data!");
        
        long i = BitConverter.ToInt64(data, index);
        index += 8;
        return i;
    }

    /**
     * Transform this data into a float
     * @return The float value
     */
    public float AsFloat() {
        if (index + 4 >= data.Length)
            throw new IOException("No more data!");
        
        float i = BitConverter.ToSingle(data, index);
        index += 4;
        return i;
    }

    /**
     * Transform this data into a double
     * @return The double value
     */
    public double AsDouble() {
        if (index + 8 >= data.Length)
            throw new IOException("No more data!");
        
        double i = BitConverter.ToDouble(data, index);
        index += 8;
        return i;
    }

    /**
     * Transform this data into a short
     * @return The short value
     */
    public short AsShort() {
        if (index + 2 >= data.Length)
            throw new IOException("No more data!");
        
        short i = BitConverter.ToInt16(data, index);
        index += 2;
        return i;
    }

    /**
     * Transform this data into a boolean. This read a single byte and returns true if the value is 1, or false if the value is 0
     * @return The boolean value
     */
    public bool AsBool() {
        if (index >= data.Length)
            throw new IOException("No more data!");
            
        bool i = data[index] == 1;
        index++;
        return i;
    }

    /**
     * Transform this data into a String, decoded using ASCII
     * @return The String value
     */
    public String AsString()
    {
        return BitConverter.ToString(data);
    }

    /**
     * Transform this data into a String, decoded using the provided {@link Charset}
     * @param charset The charset to use for decoding
     * @return The String value
     */
    public String AsString(Encoding charset)
    {
        return charset.GetString(data);
    }

    /**
     * Transform this data into a single byte
     * @return The byte value
     */
    public byte AsByte() 
    {
        if (index >= data.Length)
            throw new IOException("No more data!");
        
        byte b = data[index];
        index++;
        return b;
    }

    public int Remaining
    {
        get { return data.Length - index; }
    }

    public T AsType<T>()
    {
        int uncompressedLength = AsInt();
        int remain = Remaining;
        byte[] data = new byte[remain];

        Array.Copy(this.data, index, data, 0, data.Length);

        String json;
        if (uncompressedLength > 600) 
        {
            using (MemoryStream tempStream = new MemoryStream(data))
            {
                using (GZipStream gzip = new GZipStream(tempStream, CompressionMode.Decompress))
                {
                    byte[] uncompressedData = new byte[uncompressedLength];

                    int i = 0;
                    while (i < uncompressedLength)
                    {
                        int read = gzip.Read(uncompressedData, i, uncompressedLength - i);
                        i += read;
                    }

                    json = Encoding.ASCII.GetString(uncompressedData);
                }
            }

            data = null;
        } 
        else
        {
            json = Encoding.ASCII.GetString(data);
        }

        return JsonUtility.FromJson<T>(json);
    }

    public byte[] Raw() {
        return data;
    }
}

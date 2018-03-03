using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerClient
{
    void writeUDP(byte[] data);

    void write(byte[] data);

    void flush();

    int read(byte[] data, int index, int length);

    void disconnect();
    
    long SendCount { get; set; }
    
    long LastRead { get; set; }
}

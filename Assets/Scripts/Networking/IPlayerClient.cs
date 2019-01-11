using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerClient
{
    //TODO
    //method names are kept in their original casing
    //to make port simpler. These functions names can be refacotred
    //once networking code is stable
    
    void writeUDP(byte[] data);

    void write(byte[] data);

    void flush();

    int read(byte[] data, int index, int length);

    void disconnect();
    
    int SendCount { get; set; }
    
    int LastRead { get; set; }
    
    void GotOk(bool isOk);

    void JoinMatch(MatchInfo info);
}

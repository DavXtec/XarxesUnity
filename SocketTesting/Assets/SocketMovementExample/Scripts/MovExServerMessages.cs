using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MovExServerMessages {
    public enum MessageType { Welcome, NewPlayer, DisconnectPlayer, OldPlayers, Answer, Forfeit, PlayerUpdate, Length };
    public static string[] MessageTypeStrings = { "Welcome", "NewPlayer", "DisconnectPlayer", "OldPlayers", "Answer", "Forfeit", "PlayerUpdate" };
    public enum DataFields { MessageType, PlayerIndex, PlayerPositionX, PlayerPositionY, PlayerPositionZ, PlayerOrientation };

    public static string[] DataFieldStrings = { "MessageType", "PlayerIndex", "PlayerPositionX", "PlayerPositionY", "PlayerPositionZ", "PlayerOrientation" };

    public static MessageType GetTypeFromString(string typeString) {
        MessageType messageType = MessageType.Length;
        for (int i = 0; i < (int)MessageType.Length && messageType == MessageType.Length; i++) {
            if (((MessageType)i).ToString() == typeString) {
                messageType = ((MessageType)i);
            }
        }
        return messageType;
    }
}

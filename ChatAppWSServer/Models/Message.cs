using System;
using MessagePack;

namespace ChatAppWSServer.Models
{
    [MessagePackObject]
    public class Message
    {
        [Key(0)]
        public MessageType Type { get; set; }
        [Key(1)]
        public DateTime Timestamp;
        [Key(2)]
        public byte[] Data;
    }
}
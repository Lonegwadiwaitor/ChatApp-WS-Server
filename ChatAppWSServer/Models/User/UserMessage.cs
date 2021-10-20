using System;
using System.Collections.Generic;
using MessagePack;

namespace ChatAppWSServer.Models.User
{
    [MessagePackObject]
    public class DirectUserMessage
    {
        [Key(0)] public string Participant { get; set; } = "";
        [Key(1)] public string Content { get; set; } = "";
        [Key(2)] public DateTime Timestamp { get; set; }
        [Key(3)] public List<byte[]>? Attachments { get; set; } = null;
    }
}
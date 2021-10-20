using System;
using MessagePack;

namespace ChatAppWSServer.Models.User
{
    [MessagePackObject]
    public class GetDMResult
    {
        [Key(0)] public Opcode result { get; set; } = Opcode.GETDIRECTMESSAGES_FAIL;
        [Key(1)] public DirectUserMessage[]? Messages { get; set; } = Array.Empty<DirectUserMessage>();
    }

    [MessagePackObject]
    public class CreateDMResult
    {
        [Key(0)] public Opcode result { get; set; } = Opcode.CREATEDIRECTMESSAGE_FAIL;
    }
}
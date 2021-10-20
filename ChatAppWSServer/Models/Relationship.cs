using MessagePack;

namespace ChatAppWSServer.Models;

[MessagePackObject]
public class Relationship
{
    [Key(0)] public ulong UserUID { get; set; }
    [Key(1)] public ulong FriendUID { get; set; }

}

[MessagePackObject]
public class RelationshipResult
{
    [Key(0)] public Opcode result { get; set; } = Opcode.ADDFRIEND_FAIL;
}
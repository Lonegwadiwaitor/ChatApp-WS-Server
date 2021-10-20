using System;
using System.Linq;
using System.Runtime.CompilerServices;
using ChatAppWSServer.Models;
using ChatAppWSServer.Models.Login;
using ChatAppWSServer.PrivateServices;
using MessagePack;
using WebSocketSharp;
using WebSocketSharp.Server;
using Opcode = ChatAppWSServer.Models.Opcode;

namespace ChatAppWSServer.Services
{
    public class Connection : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            var message = MessagePackSerializer.Deserialize<Message>(e.RawData);
            var dbSvc = Globals.GetService<DatabaseService>();
            var rndSvc = Globals.GetService<RandomService>();

            switch (message.Type)
            {
                case MessageType.GETPROFILE:
                    
                    break;
                case MessageType.CREATEMESSAGE:
                    
                    break;
                case MessageType.OPENCHANNEL:

                    break;
                case MessageType.ADDFRIEND:
                    var friendData = MessagePackSerializer.Deserialize<Relationship>(message.Data);

                    var addFriendResult = dbSvc.AddRelationship(friendData.UserUID, friendData.FriendUID);

                    message.Data = MessagePackSerializer.Serialize(new RelationshipResult
                    {
                        result = addFriendResult ? Opcode.ADDFRIEND_SUCCESS : Opcode.ADDFRIEND_FAIL
                    });
                    break;
                case MessageType.LOGIN:
                    var dbLoginData = MessagePackSerializer.Deserialize<UserCredentials>(message.Data);

                    var loginResult = dbSvc.LoginAttempt(dbLoginData);

                    message.Data = MessagePackSerializer.Serialize(new LoginResult
                    {
                        result = loginResult ? Opcode.LOGIN_SUCCESS : Opcode.LOGIN_FAIL,
                        token = loginResult ? dbSvc.GetUser(dbLoginData.username)?.GetOrGenerateLoginToken()?.key ?? "" : ""
                    });
                    break;
                case MessageType.REGISTER:
                    var dbRegisterData = MessagePackSerializer.Deserialize<UserCredentials>(message.Data);

                    var registerResult = dbSvc.RegisterAttempt(dbRegisterData);

                    message.Data = MessagePackSerializer.Serialize(new LoginResult
                    {
                        result = registerResult ? Opcode.REGISTER_SUCCESS : Opcode.REGISTER_FAIL,
                        token = registerResult ? dbSvc.GetUser(dbRegisterData.username)?.GetOrGenerateLoginToken()?.key ?? "" : ""
                    });
                    break;
            }

            if (message.Data.Length != 0)
                Send(MessagePackSerializer.Serialize(message));

            base.OnMessage(e);
        }
    }
}
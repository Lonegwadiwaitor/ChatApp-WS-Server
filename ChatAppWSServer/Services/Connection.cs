using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ChatAppWSServer.Models;
using ChatAppWSServer.Models.Login;
using ChatAppWSServer.Models.Serverside;
using ChatAppWSServer.Models.User;
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
                    /*var data = MessagePackSerializer.Deserialize<UserCredentials>(e.RawData);

                    var user = dbSvc.GetUser(data.username);
                    
                    message.Data = MessagePackSerializer.Serialize(new )*/
                    break;
                case MessageType.CREATEDIRECTMESSAGE:
                    var createdMessage = MessagePackSerializer.Deserialize<DirectUserMessage>(message.Data);

                    if (Globals.ConnectedUsers.All(x => x.IP != Context.UserEndPoint.Address.ToString()))
                        break; // not verified? no direct messages!

                    var currentUser = Globals.ConnectedUsers
                        .First(x => x.IP == Context.UserEndPoint.Address.ToString()).user;

                    var storeDmResult = dbSvc.StoreDirectMessage(currentUser?.UID ?? 0, createdMessage);

                    message.Data = MessagePackSerializer.Serialize(new CreateDMResult
                    {
                        result = storeDmResult ? Opcode.CREATEDIRECTMESSAGE_SUCCESS : Opcode.CREATEDIRECTMESSAGE_FAIL
                    });

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

                    if (loginResult)
                    {
                        if (Globals.ConnectedUsers.Any(x => x.IP == Context.UserEndPoint.Address.ToString()))
                            break;
                        
                        Globals.ConnectedUsers.Add(new ConnectedUser
                        {
                            user = dbSvc.GetUser(dbLoginData.username),
                            Port = Context.UserEndPoint.Port,
                            IP = Context.UserEndPoint.Address.ToString()
                        });
                    }
                    
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
                case MessageType.GETFRIENDS:
                    var user = Globals.ConnectedUsers.FirstOrDefault(x => x.IP == Context.UserEndPoint.Address.ToString())?.user;

                    if (user is null) break;

                    List<UserCredentials> users = user.RelationshipUIDs.Select(userRelationshipUiD => dbSvc.GetUserByUID(userRelationshipUiD)).Select(relationshipUser => new UserCredentials {username = relationshipUser.username, UID = relationshipUser.UID}).ToList();

                    message.Data = MessagePackSerializer.Serialize(users.ToArray());
                    
                    break;
                case MessageType.GETFRIEND:
                    break;
                case MessageType.GETDIRECTMESSAGES:
                    var dmData = MessagePackSerializer.Deserialize<Relationship>(message.Data);

                    var friend = dbSvc.GetUserByUID(dmData.FriendUID);
                    var you = dbSvc.GetUserByUID(dmData.UserUID);

                    var friendMessages = friend?.DirectUserMessages.Where(x => x.Participant == you?.username);
                    var yourMessages = you?.DirectUserMessages.Where(x => x.Participant == friend?.username);

                    var messages = friendMessages?.Concat(yourMessages ?? new List<DirectUserMessage>());

                    message.Data = MessagePackSerializer.Serialize(new GetDMResult
                    {
                        result = (messages is null || friend is null || you is null) ? Opcode.GETDIRECTMESSAGES_FAIL :  Opcode.GETDIRECTMESSAGES_SUCCESS,
                        Messages = messages?.ToArray()
                    });
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (message.Data.Length != 0)
                Send(MessagePackSerializer.Serialize(message));

            base.OnMessage(e);
        }
    }
}
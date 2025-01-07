using Newtonsoft.Json;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services
{
    public class ComandService
    {
        private User currentUser;
        public ComandService()
        {
            currentUser = new User();
        }

        public async Task<Message> HandleComand(Message commandMessage, IPEndPoint? iP)
        {
            try
            {
                switch (commandMessage.Comand)
                {
                    case "Login":
                        return await Login(commandMessage);

                    case "SignUp":
                        return await SignUp(commandMessage);

                    case "ListGroups":
                        return await ListGroups(commandMessage);

                    case "ListUsers":
                        return await ListUsers(commandMessage);

                    case "UsersConection":
                        return await UsersConection(commandMessage);

                    case "Message":
                        return await GetIp(commandMessage);

                    case "GroupMessage":
                        return await GroupMessage(commandMessage);

                    case "GetGroupHistory":
                        return await GetGroupHistory(commandMessage);




                    case "Create group":
                        return await CreateGroup(commandMessage);

                    case "Delete group":
                        return await DeleteGroup(commandMessage);

                    case "Change group name":
                        return await ChangeGroupName(commandMessage);

                    case "ListUsersInGroup":
                        return await ListUsersInGroup(commandMessage);

                    case "ListUsersNotInGroup":
                        return await ListUsersNotInGroup(commandMessage);
                        
                    case "AddToGroup":
                        return await AddToGroup(commandMessage);

                    case "DeleteFromGroup":
                        return await DeleteFromGroup(commandMessage);


                    case "Exiting":
                       return await Exit(commandMessage);

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleComand: {ex.Message}");
            }
            return null;
        }

        public async Task<Message> GroupMessage(Message commandMessage)
        {
            await DataBaseService.setGroupHistory(commandMessage);
            commandMessage.Data = "NULL";
            return commandMessage;
        }


        public async Task<Message> GetGroupHistory(Message commandMessage)
        {
            var list = await DataBaseService.getGroupHistory(commandMessage);
            commandMessage.Data = JsonConvert.SerializeObject(list);
            return commandMessage;
        }

        public async Task<Message> DeleteFromGroup(Message commandMessage)
        {
            await DataBaseService.deleteFromGroup(commandMessage);
            commandMessage.Data = "NULL";
            return commandMessage;
        }

        public async Task<Message> AddToGroup(Message commandMessage)
        {
            await DataBaseService.addToGroup(commandMessage);
            commandMessage.Data = "NULL";
            return commandMessage;
        }

        public async Task<Message> ListUsersNotInGroup(Message commandMessage)
        {
            var list = await DataBaseService.UsersNotInGroup(commandMessage);
            commandMessage.Data = JsonConvert.SerializeObject(list);
            return commandMessage;
        }

        public async Task<Message> ListUsersInGroup(Message commandMessage)
        {
            var list = await DataBaseService.UsersInGroup(commandMessage);
            commandMessage.Data = JsonConvert.SerializeObject(list);
            return commandMessage;
        }

        public async Task<Message> ChangeGroupName(Message commandMessage)
        {
            var message = new Message(null, commandMessage.ReciverIP, commandMessage.SenderIP, null);
            if (await DataBaseService.isOwner(message))
            {
                await DataBaseService.change_groupName(commandMessage);
            }
            commandMessage.Data = "NULL";
            return commandMessage;

        }

        public  async Task<Message> CreateGroup(Message commandMessage)
        {
            await DataBaseService.createGroup(commandMessage);
            commandMessage.Data = "NULL";
            return commandMessage;
        }

        public async Task<Message> DeleteGroup(Message commandMessage)
        {
            if(await DataBaseService.isOwner(commandMessage))
            {
                await DataBaseService.deleteGroup(commandMessage);
            }
            commandMessage.Data = "NULL";
            return commandMessage;
        }

        public async Task<Message> GetIp(Message commandMessage)
        {
            commandMessage.ReciverIP = await DataBaseService.GetIPAsync(commandMessage.ReciverIP);
            await DataBaseService.SetUsersHistory(commandMessage);
            commandMessage.SenderIP = await DataBaseService.GetLoginByIPAsync(commandMessage.SenderIP);

            if (!await DataBaseService.isOnline(commandMessage.ReciverIP))
            {
               commandMessage.Comand = "NULL";
            }

            return commandMessage;
        }

        public async Task<Message> UsersConection(Message commandMessage)
        {
            var deserializedUsers = JsonConvert.DeserializeObject<List<string>>(commandMessage.Data);
            if (deserializedUsers == null)
            {
                commandMessage.Data = "Invalid user data";
                return commandMessage;
            }

            if (await DataBaseService.IsConnectionExistsAsync(deserializedUsers)){
                commandMessage.Data = JsonConvert.SerializeObject(await DataBaseService.GetUsersHistory(deserializedUsers));
                commandMessage.Comand = "History";
            }
            else{ 
                await DataBaseService.ConectUsers(deserializedUsers);
                commandMessage.Data = "There is no chat history";
            }

            return commandMessage;
        }

        public async Task<Message> Exit(Message commandMessage)
        {
            var deserializedUser = JsonConvert.DeserializeObject<User>(commandMessage.Data);
            if (deserializedUser == null)
            {
                commandMessage.Data = "Invalid user data";
                return commandMessage;
            }

            currentUser = deserializedUser;
            DataBaseService.Exit(currentUser);
            commandMessage.Data = "Exit";
            return commandMessage;
        }
        public async Task<Message> ListUsers(Message commandMessage)
        {
            var deserializedUser = JsonConvert.DeserializeObject<User>(commandMessage.Data);
            if (deserializedUser == null)
            {
                commandMessage.Data = "Invalid user data";
                return commandMessage;
            }

            currentUser = deserializedUser;

            var list = await DataBaseService.LoadUsersAsync(currentUser);
            commandMessage.Data = JsonConvert.SerializeObject(list);
            return commandMessage;
        }

        public async Task<Message> ListGroups(Message commandMessage)
        {
            var deserializedUser = JsonConvert.DeserializeObject<User>(commandMessage.Data);
            if (deserializedUser == null)
            {
                commandMessage.Data = "Invalid user data";
                return commandMessage;
            }

            currentUser = deserializedUser;

            var list = await DataBaseService.LoadGroupsAsync(currentUser);
            commandMessage.Data = JsonConvert.SerializeObject(list);
            return commandMessage;
        }

        public async Task<Message> SignUp(Message commandMessage)
        {
            var deserializedUser = JsonConvert.DeserializeObject<User>(commandMessage.Data);
            if (deserializedUser == null)
            {
                commandMessage.Data = "Invalid user data";
                return commandMessage;
            }

            currentUser = deserializedUser;

            currentUser.IP = commandMessage.SenderIP;
            if (! await DataBaseService.UserExistsAsync(currentUser))
            {
                await DataBaseService.SignUpAsync(currentUser);
                await DataBaseService.SetOnlineAsync(currentUser);
                commandMessage.Data = "true";

            }
            else commandMessage.Data = "Such a user already exists";

            return commandMessage;
        }

        public async Task<Message> Login(Message commandMessage)
        {
            var deserializedUser = JsonConvert.DeserializeObject<User>(commandMessage.Data);
            if (deserializedUser == null)
            {
                commandMessage.Data = "Invalid user data";
                return commandMessage;
            }

            currentUser = deserializedUser;

            if (await DataBaseService.UserExistsAsync(currentUser))
            {
                commandMessage.Data = "true";
                await DataBaseService.SetOnlineAsync(currentUser);
                await DataBaseService.UpdateIPAsync(currentUser, commandMessage.SenderIP);
            }
            else
            {
                commandMessage.Data = "No such user exists";
            }

            return commandMessage;
        }

    }
}

using Newtonsoft.Json;
using System.Net;
public class ComandsHandler
{
    private CurrentUser currentUser;
    List<ChatHistory> chatHistories;
    public ComandsHandler()
    {
        currentUser = new CurrentUser();
    }
    public CommandMessage HandleComand(CommandMessage commandMessage, IPEndPoint? iP)
    {
        try
        {
            switch (commandMessage.Comand)
            {
                case "Login":
                    return Login(commandMessage, iP);

                case "SignUp":
                    return SignUp(commandMessage, iP);

                case "Exiting":
                    DataBaseHelper.Exit(commandMessage.Data);
                    return commandMessage;

                case "ListUsers":
                    return List(commandMessage);

                case "Message":
                    return Message(commandMessage);

                case "History":
                    History(commandMessage);
                    return commandMessage;

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
    public CommandMessage Login(CommandMessage commandMessage, IPEndPoint? iP)
    {
        currentUser = JsonConvert.DeserializeObject<CurrentUser>(commandMessage.Data);
        if (DataBaseHelper.ValidatePasswordLogin(currentUser.UserName, currentUser.Password))
        {
            commandMessage.Data = "true";
            DataBaseHelper.UpdatePort(currentUser.UserName, iP.Port.ToString());
        }
        else commandMessage.Data = "No such user exists";

        return commandMessage;
    }
    public CommandMessage SignUp(CommandMessage commandMessage, IPEndPoint? iP)
    {
        currentUser = JsonConvert.DeserializeObject<CurrentUser>(commandMessage.Data);
        currentUser.IP = iP.Port.ToString();
        if (!DataBaseHelper.UserExists(currentUser.UserName))
        {
            DataBaseHelper.SignUp(currentUser);
            commandMessage.Data = "true";
        }
        else commandMessage.Data = "Such a user already exists";

        return commandMessage;
    }
    public CommandMessage List(CommandMessage commandMessage)
    {
        currentUser = JsonConvert.DeserializeObject<CurrentUser>(commandMessage.Data);
        var list = DataBaseHelper.LoadUsers(currentUser);
        commandMessage.Data = JsonConvert.SerializeObject(list);
        return commandMessage;
    }
    public CommandMessage History(CommandMessage commandMessage)
    {
        ChatHistory history = new ChatHistory(commandMessage.Sender, commandMessage.Reciver, null);

        if (chatHistories == null)
        {
            chatHistories = new List<ChatHistory>();
        }

        foreach (var item in chatHistories)
        {
            if (item.Equals(history))
            {
                if(item.Chat != null)
                {
                    commandMessage.Data = item.Chat;
                    return commandMessage;
                }
            }
        }

        chatHistories.Add(history);
        return null;
    }

    public CommandMessage Message(CommandMessage commandMessage)
    {
        ChatHistory history = new ChatHistory(commandMessage.Sender, commandMessage.Reciver, null);

        if (chatHistories == null)
        {
            chatHistories = new List<ChatHistory>();
        }

        bool historyExists = false;
        foreach (var item in chatHistories)
        {
            if (item.Equals(history))
            {
                item.Chat += $"{commandMessage.Sender}: {commandMessage.Data}\n";
                historyExists = true;
                break;
            }
        }

        if (!historyExists)
        {
            history.Chat = $"{commandMessage.Sender}: {commandMessage.Data}";
            chatHistories.Add(history);
        }

        commandMessage.Reciver = DataBaseHelper.GetPort(commandMessage.Reciver);
        return commandMessage;
    }

}

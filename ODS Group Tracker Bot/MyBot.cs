using System;
using System.Globalization;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Discord;
using Discord.Commands;


namespace DiscordBot1
{
    class MyBot
    {
        DiscordClient discord;
        CommandService commands;

        List<Group> groups;
        List<Invite> invites;

        public MyBot()
        {
            groups = new List<Group>();
            invites = new List<Invite>();

            discord = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });

            discord.UsingCommands(x =>
            {
                x.PrefixChar = '!';
                x.AllowMentionPrefix = true;
            });

            commands = discord.GetService<CommandService>();

            leaveGroupCommand();
            createGroupCommand();
            deleteGroupCommand();
            listGroupsCommand();
            viewMyGroupCommand();
            requestJoinCommand();
            declineJoinCommand();
            acceptJoinCommand();
            invtePlayerCommand();
            declineInviteCommand();
            acceptInviteCommand();
            requestMergeCommand();
            acceptMergeCommand();
            pingCommand();

            discord.ExecuteAndWait(async () =>
            {
                while (true)
                {
                    try
                    {
                        await discord.Connect("MjcwODc5MTA0NTg3MjY4MDk2.C1_PWw.FDQaDmRxYsQgZAb59B6Ysx0VewI", TokenType.Bot);
                        break;
                    }
                    catch
                    {
                        await Task.Delay(100);
                    }
                }
            });

            var timer = new Timer(
                e => clean(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(30));

            discord.Dispose();
        }

        private void clean()
        {
            for (int g = 0; g < groups.Count; g++)
                if (DateTime.Now.Minute == groups[g].GetExpireTime())
                    groups.RemoveAt(g);
            for (int i = 0; i < invites.Count; i++)
                if (DateTime.Now.Minute == invites[i].GetExpireTime())
                    invites.RemoveAt(i);
        }

        private void pingCommand()
        {
            commands.CreateCommand("ping")
                .Description("Pings your current group with a message you type, must be owner")
                .Parameter("Message", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    string name = e.User.Name;
                    if (e.User.Nickname != null)
                        name = e.User.Nickname;

                    bool found = false;

                    foreach (Group g in groups)
                        if(g.getOwner().ToLower().Equals(name.ToLower()))
                        {
                            if(g.getNumMembers() <= 1)
                            {
                                await e.Channel.SendMessage("No one in group to ping");
                                return;
                            }

                            found = true;
                            string message = "";
                            foreach (string s in g.members)
                                foreach (User user in e.Server.Users)
                                {
                                    if (user.Equals(e.User) || s == null)
                                        continue;

                                    string userName = user.Name;
                                    if (user.Nickname != null)
                                        userName = user.Nickname;

                                    if (userName.ToLower().Equals(s.ToLower()))
                                        message += "" + user.NicknameMention + ", ";
                                }

                            message = message.Substring(0, message.Length - 2);
                            message += ": "+ e.GetArg(0);

                            await e.Channel.SendMessage(message);
                        }

                    if (!found)
                        await e.Channel.SendMessage("You do not own a group");
                });
        }

        private void acceptMergeCommand()
        {
            commands.CreateCommand("acceptMerge")
                .Description("Accepts a request to merge with a group")
                .Parameter("Owner", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    string name = e.User.Name;
                    if (e.User.Nickname != null)
                        name = e.User.Nickname;

                    string owner = e.GetArg(0);

                    int x = invites.IndexOf(new Invite(name, owner, "", InviteType.GroupMerge));

                    if (x < 0)
                    {
                        await e.Channel.SendMessage("They have not requested a merge.");
                        return;
                    }

                    int g = groups.IndexOf(new Group(0, owner, ""));
                    int g2 = groups.IndexOf(new Group(0, name, ""));

                    string id = "";
                    string userName = "";

                    foreach (User user in e.Server.Users)
                    {
                        userName = user.Name;
                        if (user.Nickname != null)
                            userName = user.Nickname;

                        if (userName.ToLower().Equals(owner.ToLower()))
                        {
                            id = user.NicknameMention;
                            break;
                        }
                    }

                    if (Group.MergeGroups(groups[g], groups[g2]))
                    {
                        await e.Channel.SendMessage("" + e.User.NicknameMention + "\'s group has merged with " + id + "\'s group!");

                        for (int i = 0; i < invites.Count; i++)
                            if (invites[i].GetTo().Equals(name))
                                invites.RemoveAt(i);
                    }

                    else await e.Channel.SendMessage("Sorry, that group does not have enough room");
                });
        }

        private void requestMergeCommand()
        {
            commands.CreateCommand("requestMerge")
                .Description("Requests another owner to merge groups")
                .Alias("merge")
                .Parameter("Owner", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    string name = e.User.Name;
                    if (e.User.Nickname != null)
                        name = e.User.Nickname;

                    int n = groups.IndexOf(new Group(0, name, ""));

                    if (n < 0)
                    {
                        await e.Channel.SendMessage("You do not own a group");
                        return;
                    }

                    string owner = e.GetArg(0);

                    int g = groups.IndexOf(new Group(0, owner, ""));

                    if (g < 0)
                    {
                        await e.Channel.SendMessage("They do not own a group");
                        return;
                    }

                    if (!Group.canMergeGroups(groups[n], groups[g]))
                    {
                        await e.Channel.SendMessage("Too many players to merge");
                        return;
                    }

                    string id = "";
                    string userName = "";

                    foreach (User user in e.Server.Users)
                    {
                        userName = user.Name;
                        if (user.Nickname != null)
                            userName = user.Nickname;

                        if (userName.ToLower().Equals(owner.ToLower()))
                        {
                            id = user.NicknameMention;
                            break;
                        }
                    }

                    invites.Add(new Invite(owner, name, groups[n].ToString(), InviteType.GroupMerge));
                    await e.Channel.SendMessage("" + name + " has requested to merge groups with " + id+"\n"+groups[n].ToString());
                });
        }

        private void acceptInviteCommand()
        {
            commands.CreateCommand("acceptInv")
                .Description("Accepts the invite that was sent by owner")
                .Parameter("Owner", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    string name = e.User.Name;
                    if (e.User.Nickname != null)
                        name = e.User.Nickname;

                    string owner = e.GetArg(0);

                    int x = invites.IndexOf(new Invite(name, owner, "", InviteType.GroupInvite));

                    if (x < 0)
                    {
                        await e.Channel.SendMessage("They have not invited you to a group.");
                        return;
                    }

                    int g = groups.IndexOf(new Group(0, owner, ""));

                    if (groups[g].addMember(name))
                    {
                        await e.Channel.SendMessage("" + e.User.NicknameMention + " has joined " + owner + "\'s group!");

                        for (int i = 0; i < invites.Count; i++)
                            if(invites[i].GetTo().Equals(name))
                                invites.RemoveAt(i);
                    }

                    else await e.Channel.SendMessage("Sorry, that group is full!");
                });
        }

        private void declineInviteCommand()
        {
            commands.CreateCommand("declineInv")
                .Description("Someone who has been invited to a group can choose to decline the invitaion")
                .Parameter("Owner", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    string name = e.User.Name;
                    if (e.User.Nickname != null)
                        name = e.User.Nickname;

                    string owner = e.GetArg(0);

                    int i = invites.IndexOf(new Invite(name, owner, "", InviteType.GroupInvite));

                    if(i < 0)
                    {
                        await e.Channel.SendMessage("They have not invited you to a group.");
                        return;
                    }

                    invites.RemoveAt(i);
                });
        }

        private void invtePlayerCommand()
        {
            commands.CreateCommand("invite")
                .Description("The owner of a group can invite another player to their group")
                .Alias("inv")
                .Parameter("Invitee", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    string owner = e.User.Name;
                    if (e.User.Nickname != null)
                        owner = e.User.Nickname;

                    int i = groups.IndexOf(new Group(0, owner, ""));

                    if (i < 0)
                    {
                        await e.Channel.SendMessage("You do not own a group");
                        return;
                    }

                    string name = e.GetArg(0);

                    string id = "";
                    string userName = "";

                    foreach(User user in e.Server.Users)
                    {
                        userName = user.Name;
                        if (user.Nickname != null)
                            userName = user.Nickname;

                        if (userName.ToLower().Equals(name.ToLower()))
                        {
                            id = user.NicknameMention;
                            break;
                        }
                    }

                    if (id.Equals(""))
                    {
                        await e.Channel.SendMessage("No such user exists");
                        return;
                    }

                    await e.Channel.SendMessage("" + id + " has been invited to: " + groups[i].ToString());

                    invites.Add(new Invite(userName, owner, groups[i].ToString(), InviteType.GroupInvite));
                });
        }

        private void acceptJoinCommand()
        {
            commands.CreateCommand("acceptReq")
                .Description("The owner accepts a request to join the group")
                .Parameter("Requestee",ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    string owner = e.User.Name;
                    if (e.User.Nickname != null)
                        owner = e.User.Nickname;

                    int i = groups.IndexOf(new Group(0, owner, ""));

                    if (i < 0)
                    {
                        await e.Channel.SendMessage("You do not own a group");
                        return;
                    }

                    string name = e.GetArg(0);

                    string id = "";
                    string userName = "";

                    foreach (User user in e.Server.Users)
                    {
                        userName = user.Name;
                        if (user.Nickname != null)
                            userName = user.Nickname;

                        if (userName.ToLower().Equals(name.ToLower()))
                        {
                            id = user.NicknameMention;
                            break;
                        }
                    }

                    if (groups[i].acceptRequest(name))
                        await e.Channel.SendMessage("" + id + " has been added to " + e.User.NicknameMention + " \'s group!");
                });
        }

        private void declineJoinCommand()
        {
            commands.CreateCommand("declineReq")
                .Description("The owner of a group can decline an request to join their group")
                .Parameter("Requestee", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    string owner = e.User.Name;
                    if (e.User.Nickname != null)
                        owner = e.User.Nickname;

                    int i = groups.IndexOf(new Group(0, owner, ""));

                    if (i < 0)
                    {
                        await e.Channel.SendMessage("You do not own a group");
                        return;
                    }

                    string name = e.GetArg(0);

                    groups[i].declineRequest(name);

                });  
        }

        private void requestJoinCommand()
        {
            commands.CreateCommand("join")
                .Description("Requests the owner of the group to join")
                .Alias("request","req")
                .Parameter("Owner Name", ParameterType.Unparsed)
                .Do(async (e) =>
                 {
                     string owner = e.GetArg(0);

                     int i = groups.IndexOf(new Group(0, owner, ""));

                     if (i < 0)
                     {
                         await e.Channel.SendMessage("They do not own a group");
                         return;
                     }

                     string name = e.User.Name;
                     if (e.User.Nickname != null)
                         name = e.User.Nickname;

                     groups[i].requestJoin(name);

                     string id = "";
                     string userName = "";

                     foreach (User user in e.Server.Users)
                     {
                         userName = user.Name;
                         if (user.Nickname != null)
                             userName = user.Nickname;

                         if (userName.ToLower().Equals(owner.ToLower()))
                         {
                             id = user.NicknameMention;
                             break;
                         }
                     }

                     await e.Channel.SendMessage(""+id+", "+e.User.NicknameMention+" has requested to join your group!");
                 });
        }

        private void viewMyGroupCommand()
        {
            commands.CreateCommand("view")
                .Description("View all current members of your group")
                .Do(async (e) =>
                {
                    string name = e.User.Name;
                    if (e.User.Nickname != null)
                        name = e.User.Nickname;

                    bool found = false;

                    foreach (Group g in groups)
                        for (int i = 0; i < g.members.Length; i++)
                            if (g.members[i].ToLower().Equals(name.ToLower()))
                            {
                                string s = g.ToString() + "\n";
                                for (int z = 0; z < g.getSize(); z++)
                                    s += "" + (z + 1) + ": " + g.members[z] + "\n";

                                await e.Channel.SendMessage(s);
                                found = true;
                            }

                    if (!found)
                        await e.Channel.SendMessage("You are not in a group");
                });
        }

        private void listGroupsCommand()
        {
            commands.CreateCommand("list")
                .Description("Lists all current groups")
                .Parameter("Group Type", ParameterType.Optional)
                .Do(async (e) =>
                {
                    GroupType type = Group.translateType(e.GetArg("Group Type").ToLower());

                    string s = "Groups:\n";
                    int x = 0;
                    for (int i = 0; i < groups.Count; i++)
                        if(type == GroupType.invalid || groups[i].getType() == type)
                        {
                            s += groups[i].ToString() + "\n";
                            x++;
                        }

                    if (x < 1)
                    {
                        await e.Channel.SendMessage("No current groups.");
                        return;
                    }

                    await e.Channel.SendMessage(s);
                });
        }

        private void createGroupCommand()
        {
            commands.CreateCommand("create")
                .Description("Creates a group")
                .Alias("group")
                .Parameter("Group Type", ParameterType.Required)
                .Parameter("District", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    GroupType type = Group.translateType(e.GetArg("Group Type").ToLower());
                    if (type == GroupType.invalid)
                    {
                        await e.Channel.SendMessage("Invalid Group Type");
                        return;
                    }

                    string name = e.User.Name;
                    if (e.User.Nickname != null)
                        name = e.User.Nickname;

                    foreach(Group g in groups)
                        foreach(string s in g.members)
                            if(s != null && name.ToLower().Equals(s.ToLower()))
                            {
                                await e.Channel.SendMessage("You are already in a group");
                                return;
                            }

                    string dist = e.GetArg("District");
                    TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                    dist = textInfo.ToTitleCase(dist);

                    groups.Add(new Group(type, name, dist));
                    await e.Channel.SendMessage("" + name + " has created a group");
                });
        }

        private void deleteGroupCommand()
        {
            commands.CreateCommand("delete")
                .Description("Disbands the current group you own")
                .Alias("disband","del")
                .Do(async (e) =>
                {
                    string owner = e.User.Name;
                    if (e.User.Nickname != null)
                        owner = e.User.Nickname;

                    if (!groups.Remove(new Group(0, owner, "")))
                        await e.Channel.SendMessage("You do not own a group");
                    else
                        await e.Channel.SendMessage("Your group has been disbanded");
                });
        }

        private void leaveGroupCommand()
        {
            commands.CreateCommand("leave")
               .Description("Leaves the current group you are in")
               .Do(async (e) =>
               {
                   string name = e.User.Name;
                   if (e.User.Nickname != null)
                       name = e.User.Nickname;

                   int found = 0;

                   foreach (Group g in groups)
                   {
                       for (int i = 0; i < g.members.Length; i++)
                       {
                           if (g.members[i].Equals(name))
                           {
                               found = g.delMember(name);
                               break;
                           }
                       }
                   }

                   if (found == 0)
                       await e.Channel.SendMessage("You are not in a group");

                   else if (found == -1)
                       await e.Channel.SendMessage("You must disband your group to leave it");

                   else await e.Channel.SendMessage("" + e.User.NicknameMention + " has left their group");
               });
        }

        private void Log(Object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}

using System;


namespace DiscordBot1
{
    class Invite
    {
        private string to, from, desc;
        private long timeExpires;
        private InviteType type;

        public Invite(string to, string from, string desc, InviteType type)
        {
            this.to = to;
            this.from = from;
            this.desc = desc;
            this.timeExpires = DateTime.Now.AddMinutes(30).Minute;
            this.type = type;
        }

        public InviteType GetInviteType()
        {
            return type;
        }

        public long GetExpireTime()
        {
            return timeExpires;
        }

        public string GetTo()
        {
            return to;
        }

        public string GetFrom()
        {
            return from;
        }

        public string GetDesc()
        {
            return desc;
        }

        public override bool Equals(object obj)
        {
            return (to.ToLower().Equals((obj as Invite).to.ToLower())) && (from.ToLower().Equals((obj as Invite).from.ToLower()) && type == (obj as Invite).type);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "@" + from + " invited " + "@" + to + " to their group!\n" + desc;
        }
    }
}

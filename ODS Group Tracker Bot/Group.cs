using System;
using System.Collections.Generic;

namespace DiscordBot1
{
    class Group
    { 
        
        private GroupType type;
        private string owner;
        private string district;
        public string[] members;
        public List<string> requests;
        private int numMembers;
        private long timeExpires;

        public Group(GroupType t, string o, string d)
        {
            this.type = t;
            this.owner = o;
            this.district = d;
            this.numMembers = 0;
            this.requests = new List<string>();
            this.timeExpires = DateTime.Now.AddMinutes(30).Minute;
            if (t > GroupType.cio)
                members = new string[4];
            else
                members = new string[8];

            this.addMember(o);
        }

        public long GetExpireTime()
        {
            return timeExpires;
        }

        public bool requestJoin(string name)
        {
            if (numMembers == getSize()||requests.IndexOf(name.ToLower())>=0)
                return false;

            requests.Add(name.ToLower());

            return true;
        }

        public bool acceptRequest(string name)
        {
            if (requests.Count == 0 || requests.IndexOf(name.ToLower()) < 0)
                return false;

            requests.Remove(name.ToLower());
            addMember(name);

            return true;
        }

        public bool declineRequest(string name)
        {
            if (requests.Count == 0|| requests.IndexOf(name) < 0)
                return false;

            requests.Remove(name);

            return true;
        }

        public int getNumMembers()
        {
            return numMembers;
        }

        public int getSize()
        {
            if (this.type > GroupType.cio)
                return 4;
            return 8;
        }

        public string getOwner()
        {
            return owner;
        }

        public string getDistrict()
        {
            return district;
        }

        public GroupType getType()
        {
            return type;
        }
        public static GroupType translateType(string s)
        {
            switch (s)
            {
                case "vp":
                case "v.p.":
                case "v.p":
                    return GroupType.vp;
                case "shortfactory":
                case "factoryshort":
                case "shortfact":
                case "sfact":
                case "facts":
                case "factshort":
                    return GroupType.fact_short;
                case "longfactory":
                case "factorylong":
                case "fact":
                case "longfact":
                case "lfact":
                case "factl":
                case "factlong":
                    return GroupType.fact_long;
                case "fatalfactory":
                case "factoryfatal":
                case "ffact":
                case "factf":
                case "factfatal":
                case "fatalfact":
                    return GroupType.fact_fatal;
                case "cfo":
                case "c.f.o":
                case "c.f.o.":
                    return GroupType.cfo;
                case "bullionmint":
                case "bull":
                case "bullion":
                case "mintbullion":
                case "bullmint":
                case "mintbull":
                    return GroupType.mint_bullion;
                case "dollarmint":
                case "doll":
                case "dollar":
                case "mintdollar":
                case "mintdoll":
                    return GroupType.mint_dollar;
                case "cointmint":
                case "coin":
                case "mintcoin":
                    return GroupType.mint_coin;
                case "cj":
                case "c.j.":
                case "c.j":
                    return GroupType.cj;
                case "officea":
                case "aoffice":
                    return GroupType.office_a;
                case "officeb":
                case "boffice":
                    return GroupType.office_b;
                case "officec":
                case "coffice":
                    return GroupType.office_c;
                case "officed":
                case "doffice":
                    return GroupType.office_d;
                case "ceo":
                case "c.e.o":
                case "c.e.o.":
                    return GroupType.ceo;
                case "cio":
                case "c.i.o":
                case "c.i.o.":
                    return GroupType.cio;
                case "front3":
                case "frontthree":
                case "front":
                    return GroupType.front3;
                case "middle6":
                case "middlesix":
                case "mid6":
                case "midsix":
                case "mid":
                case "middle":
                    return GroupType.middle6;
                case "back9":
                case "backnine":
                case "back":
                    return GroupType.back9;
                case "1sell":
                    return GroupType.building_1s;
                case "2sell":
                    return GroupType.building_2s;
                case "3sell":
                    return GroupType.building_3s;
                case "4sell":
                    return GroupType.building_4s;
                case "5sell":
                    return GroupType.building_5s;
                case "1cash":
                    return GroupType.building_1c;
                case "2cash":
                    return GroupType.building_2c;
                case "3cash":
                    return GroupType.building_3c;
                case "4cash":
                    return GroupType.building_4c;
                case "5cash":
                    return GroupType.building_5c;
                case "1law":
                    return GroupType.building_1l;
                case "2law":
                    return GroupType.building_2l;
                case "3law":
                    return GroupType.building_3l;
                case "4law":
                    return GroupType.building_4l;
                case "5law":
                    return GroupType.building_5l;
                case "1boss":
                    return GroupType.building_1b;
                case "2boss":
                    return GroupType.building_2b;
                case "3boss":
                    return GroupType.building_3b;
                case "4boss":
                    return GroupType.building_4b;
                case "5boss":
                    return GroupType.building_5b;
                case "1tech":
                    return GroupType.building_1t;
                case "2tech":
                    return GroupType.building_2t;
                case "3tech":
                    return GroupType.building_3t;
                case "4tech":
                    return GroupType.building_4t;
                case "5tech":
                    return GroupType.building_5t;
                case "1any":
                    return GroupType.building_1a;
                case "2any":
                    return GroupType.building_2a;
                case "3any":
                    return GroupType.building_3a;
                case "4any":
                    return GroupType.building_4a;
                case "5any":
                    return GroupType.building_5a;
                case "sellfo":
                case "sellfieldoffice":
                case "fosell":
                case "fieldofficesell":
                    return GroupType.field_office_sell;
                case "lawfo":
                case "lawfieldoffice":
                case "folaw":
                case "fieldofficelaw":
                    return GroupType.field_office_law;
                case "racing":
                case "race":
                case "kart":
                case "kartrace":
                case "racekart":
                    return GroupType.racing;
                case "golfing":
                case "golf":
                    return GroupType.golfing;

                default: return GroupType.invalid;
            }
        }
        public static string translateType(GroupType t)
        {
            switch (t)
            {
                case GroupType.vp:
                    return "VP";
                case GroupType.fact_short:
                    return "Short Factory";
                case GroupType.fact_long:
                    return "Long Factory";
                case GroupType.fact_fatal:
                    return "Fatal Factory";
                case GroupType.cfo:
                    return "CFO";
                case GroupType.mint_coin:
                    return "Coin Mint";
                case GroupType.mint_dollar:
                    return "Dollar Mint";
                case GroupType.mint_bullion:
                    return "Bullion Mint";
                case GroupType.cj:
                    return "CJ";
                case GroupType.office_a:
                    return "DA Office A";
                case GroupType.office_b:
                    return "DA Office B";
                case GroupType.office_c:
                    return "DA Office C";
                case GroupType.office_d:
                    return "DA Office D";
                case GroupType.ceo:
                    return "ceo";
                case GroupType.front3:
                    return "Front 3";
                case GroupType.middle6:
                    return "Middle 6";
                case GroupType.back9:
                    return "Back 9";
                case GroupType.cio:
                    return "CIO";
                case GroupType.building_1s:
                    return "1 Story Sellbot Building";
                case GroupType.building_2s:
                    return "2 Story Sellbot Building";
                case GroupType.building_3s:
                    return "3 Story Sellbot Building";
                case GroupType.building_4s:
                    return "4 Story Sellbot Building";
                case GroupType.building_5s:
                    return "5 Story Sellbot Building";
                case GroupType.building_1c:
                    return "1 Story Cashbot Building";
                case GroupType.building_2c:
                    return "2 Story Cashbot Building";
                case GroupType.building_3c:
                    return "3 Story Cashbot Building";
                case GroupType.building_4c:
                    return "4 Story Cashbot Building";
                case GroupType.building_5c:
                    return "5 Story Cashbot Building";
                case GroupType.building_1l:
                    return "1 Story Lawbot Building";
                case GroupType.building_2l:
                    return "2 Story Lawbot Building";
                case GroupType.building_3l:
                    return "3 Story Lawbot Building";
                case GroupType.building_4l:
                    return "4 Story Lawbot Building";
                case GroupType.building_5l:
                    return "5 Story Lawbot Building";
                case GroupType.building_1b:
                    return "1 Story Bossbot Building";
                case GroupType.building_2b:
                    return "2 Story Bossbot Building";
                case GroupType.building_3b:
                    return "3 Story Bossbot Building";
                case GroupType.building_4b:
                    return "4 Story Bossbot Building";
                case GroupType.building_5b:
                    return "5 Story Bossbot Building";
                case GroupType.building_1t:
                    return "1 Story Techbot Building";
                case GroupType.building_2t:
                    return "2 Story Techbot Building";
                case GroupType.building_3t:
                    return "3 Story Techbot Building";
                case GroupType.building_4t:
                    return "4 Story Techbot Building";
                case GroupType.building_5t:
                    return "5 Story Techbot Building";
                case GroupType.building_1a:
                    return "1 Story Building";
                case GroupType.building_2a:
                    return "2 Story Building";
                case GroupType.building_3a:
                    return "3 Story Building";
                case GroupType.building_4a:
                    return "4 Story Building";
                case GroupType.building_5a:
                    return "5 Story Building";
                case GroupType.field_office_sell:
                    return "Sellbot Field Office";
                case GroupType.field_office_law:
                    return "Lawbot Field Office";
                case GroupType.racing:
                    return "Racing";
                case GroupType.golfing:
                    return "Golfing";
                default: return "invalid";
            }
        }
        public string translateType()
        {
            return translateType(this.type);
        }

        public bool addMember(string mem)
        {
            for(int i = 0; i < members.Length; i++)
                if(members[i] == null)
                {
                    members[i] = mem;
                    numMembers++;
                    return true;
                }
            return false;
        }

        public int delMember(string mem)
        {
            mem = mem.ToLower();
            if (mem.Equals(owner.ToLower()))
                return -1;
            for (int i = 0; i < members.Length; i++)
            {
                if (members[i].ToLower().Equals(mem))
                {
                    members[i] = null;
                    numMembers--;
                    return 1;
                }
            }
            return 0;
        }
        public string delMember(int mem)
        {
            string s = members[mem];
            if (members[mem] != null)
            {
                members[mem] = null;
                numMembers--;
                return s;
            }

            return null;
        }

        public static bool canMergeGroups(Group g1, Group g2)
        {
            return (g1.getType() == g2.getType() && g1.getSize()- g1.getNumMembers() >= g2.getNumMembers());
        }

        public static bool MergeGroups(Group g1, Group g2)
        {
            if (!Group.canMergeGroups(g1, g2))
                return false;

            for(int i = 0; i < g2.getSize();i++)
            {
                if (g2.members[i] != null)
                    g1.addMember(g2.delMember(i));
            }

            g2 = null;

            return true;
        }

        public override bool Equals(object obj)
        {
            return (obj as Group).getOwner().Equals(owner);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
           return "" + owner + "\'s group(" + numMembers + "/" + this.getSize() + "): " + Group.translateType(type) + " in " + district;
        }
    }
}

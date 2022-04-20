using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportsZone.Helpers
{
    public class IsExist
    {
        private Entities context = new Entities();
        internal bool User(string email)
        {
            var isthere = (from e in context.users where e.email == email select e);
            if (isthere == null) return false;
            else return true;
        }
        internal bool NotBan(string email)
        {
            bool? isBan = (from i in context.users where i.email == email && i.C_status == true select i.C_status).SingleOrDefault();
            bool ban = isBan ?? false;
            return ban;
        }
    }
}
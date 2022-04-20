using SportsZone.Security;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SportsZone.Security
{
    // validation Ok!
    internal class Authenticate
    {
        //-- Secrete **Hard Coded String** which will be cancatinated with each password
        private string HC_InputString = "fx,i[(MS";
        /// <summary>
        /// Encrypt the user password into a Hash with a Salt and a Workfactor by using Enhanced BCrypt algorithm
        /// </summary>
        /// <param name="Password">Plan user entered password</param>
        /// <returns></returns>
        internal string GenPassword(string Password)
        {
            if (Password == null) { return null; }
            else
            {
                string HC_Password = Password + HC_InputString;
                return BCrypt.HashPassword(HC_Password, 13);
            }

        }
        /// <summary>
        /// Verifies either the user is exists in System and validated
        /// </summary>
        /// <param name="Phone"> User Phone to which against user was registered</param>
        /// <param name="Password">User Password</param>
        /// <returns></returns>
        //-- PreVerify the user from a public method for less outside interaction //-- please don't modify the access specifiers of this function, else it will become a sequrity hole.
        internal bool Verify( string email, string password)
        {
            string HC_Password = password + HC_InputString;
            return BCrypt.Verify(HC_Password, GetHash(email));
        }
        //-- return hash agais a phone number //-- please don't modify the access specifiers of this function, else it will become a sequrity hole.
        private string GetHash(string email)
        {
            using (var context = new Entities())
            {
                string hash = context
                    .users
                    .Where(u => u.email.Equals(email))
                    .Select(u => u.passwd)
                    .SingleOrDefault();
                return hash;
            }
        }

    }
}

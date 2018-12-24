using IntelliMail.Data.DAL;
using System;
using System.Text.RegularExpressions;

namespace SQ1_1107
{
    class Program
    {
        static void Main(string[] args)
        {
            UserCollection users = new SubSonic.Select()
                .From(Tables.User)
                .Where(User.Columns.IsDeleted).IsEqualTo(1)
                    .AndExpression(User.Columns.Email).Like("%\\_OLD%")
                    .Or(User.Columns.Login).Like("%\\_OLD%")
                .CloseExpression()
                .ExecuteAsCollection<UserCollection>();

            foreach (User user in users)
            {
                string cleanEmail = GetClean(user.Email);
                string finalEmail = $"{cleanEmail}_{user.Id}_DELETED"; // homan@sendible.com_176721_DELETED

                string cleanLogin = GetClean(user.Login);
                string finalLogin = $"{cleanLogin}_{user.Id}_DELETED"; // homan_176721_DELETED

                user.Email = finalEmail;
                user.Login = finalLogin;

                // user.Save(); - will uncomment this and run this Save() call if this script passes code review.
            }

            Console.Read();
        }

        private static string GetClean(string element)
        {
            // normalised
            // homan@sendible.com_176721_OLD_DELETED_OLD -> homan@sendible.com_176721_DELETED_DELETED_DELETED
            string normalised = Regex.Replace(element, "_OLD", "_DELETED", RegexOptions.IgnoreCase);

            // withoutDeleted
            // homan@sendible.com_176721_DELETED_DELETED_DELETED -> homan@sendible.com_176721
            string withoutDeleted = Regex.Replace(normalised, "_DELETED", "", RegexOptions.IgnoreCase);

            // homan@sendible.com_176721 -> homan@sendible.com
            return Regex.Replace(withoutDeleted, "_\\d+$", "", RegexOptions.IgnoreCase);
        }
    }
}

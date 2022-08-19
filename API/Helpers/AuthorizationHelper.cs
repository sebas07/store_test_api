namespace API.Helpers
{
    public class AuthorizationHelper
    {

        public enum Roles
        {

            Administrator,
            Manager,
            Employee

        }

        public const Roles default_user_role = Roles.Employee;

    }
}

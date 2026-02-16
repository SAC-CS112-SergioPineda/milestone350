namespace CST_350_MilestoneProject.Models
{
    public interface IUserManager
    {
        bool RegisterUser(UserModel model);
        bool EmailExists(string email);
        bool UsernameExists(string username);
        UserEntity? ValidateUser(string username, string password);
    }
}

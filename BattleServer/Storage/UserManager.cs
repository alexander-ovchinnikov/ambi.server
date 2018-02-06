using System;

namespace BattleServer.Storage
{
    public class UserManager
    {
        private readonly string path;

        private UserManager(string path)
        {
            this.path = path;
        }

        public static UserManager Init(string path)
        {
            return new UserManager(path);
        }

        public string GetUserID()
        {
            return Guid.NewGuid().ToString();
        }

        public UserDB Load(string id = null)
        {
            if (id == null) id = GetUserID();
            return UserDB.Load(path, id);
        }
    }
}
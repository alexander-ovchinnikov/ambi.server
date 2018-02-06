using System.IO;
using System.Xml.Serialization;

namespace BattleServer.Storage
{
    public class UserDB
    {
        private string dbpath;
        public string Id { get; set; }
        public int Wins { get; set; }
        public int Losts { get; set; }


        public static UserDB Load(string dir_path, string user_id)
        {
            var path = Path.Combine(dir_path, user_id);
            UserDB udb;
            try
            {
                Directory.CreateDirectory(dir_path);
                var s = new XmlSerializer(typeof(UserDB));
                using (var reader = File.OpenText(path))
                {
                    udb = (UserDB) s.Deserialize(reader);
                }
            }
            catch
            {
                udb = new UserDB();
            }

            udb.Id = user_id;
            udb.dbpath = path;

            return udb;
        }

        public void Win()
        {
            Wins++;
            Save();
        }

        public void Lost()
        {
            Losts++;
            Save();
        }

        public void Save()
        {
            var s = new XmlSerializer(typeof(UserDB));
            var writer = File.CreateText(dbpath);
            s.Serialize(writer, this); //, ns);
            writer.Close();
        }
    }
}
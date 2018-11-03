using System;

namespace Server.Business
{
    public class BaseBusiness
    {
        public string ConnectionString 
        {
            get 
            {
                var dbDir = System.Environment.GetEnvironmentVariable("LITEDB_CONN", EnvironmentVariableTarget.Machine);
                dbDir = !string.IsNullOrEmpty(dbDir)?dbDir:"/var/Whatsappdb";
                var connstring = $"Filename={dbDir}/Server.dat;Mode=Exclusive";
                return connstring;
            }
        }
    }
}
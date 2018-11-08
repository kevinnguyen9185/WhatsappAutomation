using System;
using System.Runtime.InteropServices;

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
                bool isMac = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
                var connstring = $"Filename={dbDir}/Server.dat";
                if(isMac){
                    connstring += ";Mode=Exclusive";
                }
                return connstring;
            }
        }
    }
}
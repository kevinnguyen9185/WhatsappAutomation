using System;
using LiteDB;
using WebApi.Business.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace WebApi.Business
{
    public class ScheduleBusiness: BaseBusiness
    {
        public List<Schedule> ListSchedule(string username)
        {
            using(var db = new LiteRepository(ConnectionString))
            {
                return db.Query<Schedule>()
                    .Where(x=>x.Username == username)
                    .ToList();
            }
        }

        public bool UpsertSchedule(Schedule schedule)
        {
            using(var db = new LiteRepository(ConnectionString))
            {
                if(string.IsNullOrEmpty(schedule._id))
                {
                    schedule._id = Guid.NewGuid().ToString();
                    return (db.Insert<Schedule>(schedule)!=null);
                }
                else 
                {
                    return db.Update<Schedule>(schedule);
                }
            }
        }

        public bool DeleteSchedule(string id)
        {
            using(var db = new LiteRepository(ConnectionString))
            {
                return db.Delete<Schedule>(id);
            }
        }
        public async Task<string> SaveBase64ToLocalAsync(string base64Img)
        {
            var sel_shared_folder = GlobalVariable.SharedSelFolder;
            if(string.IsNullOrEmpty(GlobalVariable.SharedSelFolder))
            {
                var sel_shared_folder_env = System.Environment.GetEnvironmentVariable("SEL_SHARED_FOLDER", EnvironmentVariableTarget.Machine);
                if(sel_shared_folder_env==null)
                {
                    sel_shared_folder = sel_shared_folder_env.ToString();
                }
            }
            String hostpath = Path.Combine(sel_shared_folder, "robot_images");

            //Check if directory exist
            if (!System.IO.Directory.Exists(hostpath))
            {
                System.IO.Directory.CreateDirectory(hostpath); //Create directory if it doesn't exist
            }

            var imgFileName = $"{Guid.NewGuid().ToString()}.png";
            var imgPath = Path.Combine(hostpath, imgFileName);

            byte[] imageBytes = Convert.FromBase64String(base64Img);

            File.WriteAllBytes(imgPath, imageBytes);

            return imgFileName;
        }

        public async Task<string> GetBase64Async(string photoId)
        {
            var sel_shared_folder = GlobalVariable.SharedSelFolder;
            if(string.IsNullOrEmpty(GlobalVariable.SharedSelFolder))
            {
                var sel_shared_folder_env = System.Environment.GetEnvironmentVariable("SEL_SHARED_FOLDER", EnvironmentVariableTarget.Machine);
                if(sel_shared_folder_env==null)
                {
                    sel_shared_folder = sel_shared_folder_env.ToString();
                }
            }
            String hostpath = Path.Combine(sel_shared_folder, "robot_images");

            var bytes = await File.ReadAllBytesAsync(Path.Combine(hostpath, photoId));

            return Convert.ToBase64String(bytes);
        }
    }
}
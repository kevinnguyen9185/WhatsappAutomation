using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Client.Automation;
using System.Linq;
using OpenQA.Selenium.Interactions;

namespace Client.Automation
{
    public class ChatPage : Page
    {
        public bool IsLogin 
        { 
            get
            {
                return CheckLoginStatus();
            }
        }
        public ChatPage(string title)
        {
            PageTitle = title;
        }

        public bool CheckLoginStatus()
        {
            try
            {
                var elm = Driver.FindElement(By.CssSelector("span[data-icon='menu']"));
                return elm != null;
            }
            catch(NoSuchElementException ex)
            {
                return false;
            }
        }

        public List<string> GetContactList()
        {
            List<string> lstContact = new List<string>();
            try
            {
                var contactList = Driver.FindElements(By.CssSelector("div[class='_2wP_Y']"));
                for (int i = 0;i<contactList.Count;i++)
                {
                    var contactElm = contactList[i];
                    try
                    {
                        var contactName = contactElm.FindElement(By.CssSelector("span[class='_1wjpf']"));
                        lstContact.Add(contactName.Text);
                        //Console.WriteLine(contactName.Text);
                    }
                    catch{}
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return lstContact;
        }

        public async Task<List<string>> GetContactListAll()
        {
            Driver.FindElement(By.CssSelector("span[data-icon='menu']")).Click();
            await Task.Delay(500);
            Driver.FindElement(By.CssSelector("div[title='New group']")).Click();
            await Task.Delay(500);
            var nextGet = await GetContactListFromGroup();
            nextGet = nextGet.OrderByDescending(f=>f.Key).ToList();
            var contactList = new List<string>();
            contactList.AddRange(nextGet.Select(f=>f.Key));
            int differenceNo = contactList.Count;
            while(true)
            {                
                var actions = new Actions(Driver);
                actions.MoveToElement(nextGet[0].Value);
                actions.Perform();
                await Task.Delay(500);
                nextGet = await GetContactListFromGroup();
                nextGet = nextGet.OrderByDescending(f=>f.Key).ToList();
                var intersectNo = nextGet.Select(n=>n.Key).Intersect(contactList).Count();
                differenceNo = nextGet.Count - intersectNo;
                if(differenceNo == 0)
                {
                    break;
                }
                else
                {
                    var newList = nextGet.Where(n=>!contactList.Contains(n.Key)).Select(n=>n.Key).ToList();
                    contactList.AddRange(newList);
                }
            }
            return contactList;
        }

        public async Task<List<KeyValuePair<string, IWebElement>>> GetContactListFromGroup()
        {
            List<KeyValuePair<string, IWebElement>> lstContact = new List<KeyValuePair<string, IWebElement>>();
            try
            {
                var contactContainerElms = Driver.FindElements(By.CssSelector("div[class='_3q4NP k1feT']"));
                var realContactsContainers = new List<IWebElement>();
                foreach(var contactContainerElm in contactContainerElms)
                {
                    if(contactContainerElm.GetAttribute("innerHTML").Contains("Add group participants")){
                        //Find all contacts
                        try
                        {
                            var tempContainers = contactContainerElm.FindElements(By.CssSelector("div[class='_2EXPL']"));
                            foreach(var tempContainerElm in tempContainers)
                            {
                                var cssSelectorLinkedList = new LinkedList<string>(new string[]{"span[class='_3TEwt']", "span[class='_1wjpf']"});
                                var contactElms = Support.GetRecursiveElementByCssSelector(cssSelectorLinkedList.First , tempContainerElm);
                                //lstContact.Add(contactElm.GetAttribute("title"));
                                if(contactElms.Count>0)
                                {
                                    lstContact.Add(new KeyValuePair<string, IWebElement>(contactElms[0].GetAttribute("title"), contactElms[0]));
                                }
                            }
                        }
                        catch(Exception ex)
                        {

                        }
                        lstContact = lstContact.OrderBy(l=>l.Key).ToList();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return lstContact;
        }

        public async Task<bool> SendWhatsappMess(string contactName, string chatMessage, string[] fileContents)
        {
            try
            {
                var contactList = Driver.FindElements(By.CssSelector("div[class='_2wP_Y']"));
                for (int i = 0;i<contactList.Count;i++)
                {
                    var contactElm = contactList[i];
                    var htmlContent = contactElm.GetAttribute("innerHTML");
                    if(htmlContent.Contains(contactName))
                    {
                        try
                        {
                            contactElm.Click();
                            await Task.Delay(500);
                            contactElm.Click();
                            await Task.Delay(500);
                            if(fileContents.Length > 0)
                            {
                                List<string> filePath= new List<string>();
                                //Save file to local disk
                                for (int j = 0;j<fileContents.Length;j++)
                                {
                                    //If file item is filepath
                                    var filecontent = fileContents[j];
                                    var outGuidResult = Guid.Empty;
                                    Guid.TryParse(filecontent.Replace(".png",""), out outGuidResult);
                                    if(outGuidResult!=Guid.Empty)
                                    {
                                        String hostpath = Path.Combine(GetSelSharedFolder(), "robot_images");
                                        filePath.Add(Path.Combine(hostpath, filecontent));
                                    }
                                    else
                                    {
                                        var fileItem =await SaveBase64ToLocal(filecontent);
                                        filePath.Add(fileItem);
                                        Console.WriteLine($"Temp file saved {fileItem}");
                                    }
                                }
                                //Append image
                                await OpenLocalFile(filePath.ToArray());
                                await Task.Delay(500);
                                var chatboxElm = Driver.FindElement(By.CssSelector("div[class='_2S1VP copyable-text selectable-text']"));
                                //Console.WriteLine("Chat box found");
                                chatboxElm.SendKeys(Keys.Enter);
                                await Task.Delay(2000);
                                chatboxElm = Driver.FindElement(By.CssSelector("div[class='_2S1VP copyable-text selectable-text']"));
                                chatboxElm.SendKeys(chatMessage);
                                await Task.Delay(500);
                                chatboxElm.SendKeys(Keys.Enter);
                                //_3hV1n yavlE
                                // var sendWithImgBut = Driver.FindElementByCssSelector("div[class='_3hV1n yavlE']");
                                // //Console.WriteLine("Found button and click");
                                // sendWithImgBut.Click();
                                //Driver.GetScreenshot().SaveAsFile($"/Users/kevinng/Ansarada/Selenium_docker/Chuanbigui_{Guid.NewGuid().ToString()}.png",ScreenshotImageFormat.Png);
                                
                            }
                            else
                            {
                                var chatboxElm = Driver.FindElement(By.CssSelector("div[class='_2S1VP copyable-text selectable-text']"));
                                //Append text
                                chatboxElm.SendKeys(chatMessage);
                                chatboxElm.SendKeys(Keys.Return);
                            }
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            return false;
                        }
                    }
                }
            }
            catch(Exception ex){
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        private async Task<string> SaveBase64ToLocal(string base64Img)
        {
            var sel_shared_folder = GetSelSharedFolder();
            String hostpath = Path.Combine(sel_shared_folder, "robot_images");
            String dockerpath = "/home/seluser";

            //Check if directory exist
            if (!System.IO.Directory.Exists(hostpath))
            {
                System.IO.Directory.CreateDirectory(hostpath); //Create directory if it doesn't exist
            }

            var imgFileName = $"{Guid.NewGuid().ToString()}.png";
            var imgPath = Path.Combine(hostpath, imgFileName);

            byte[] imageBytes = Convert.FromBase64String(base64Img);

            File.WriteAllBytes(imgPath, imageBytes);

            if (IsRemoteDriver)
            {
                return Path.Combine(dockerpath, imgFileName);
            }
            else
            {
                return imgPath;
            }
        }

        private string GetSelSharedFolder()
        {
            var sel_shared_folder = Program.SharedSelFolder;
            if(string.IsNullOrEmpty(Program.SharedSelFolder))
            {
                var sel_shared_folder_env = System.Environment.GetEnvironmentVariable("SEL_SHARED_FOLDER", EnvironmentVariableTarget.Machine);
                if(sel_shared_folder_env==null)
                {
                    sel_shared_folder = sel_shared_folder_env.ToString();
                }
            }
            return sel_shared_folder;
        }

        private async Task OpenLocalFile(string[] filePath)
        {
            try
            {
                var inputs = "";
                for (int i = 0;i<filePath.Length;i++)
                {
                    if(i<filePath.Length-1)
                    {
                        inputs += filePath[i] + "\n";
                    }
                    else
                    {
                        inputs += filePath[i];
                    }
                }
                //Console.WriteLine(inputs);
                var attachButton = Driver.FindElement(By.CssSelector("div[title='Attach']"));
                attachButton.Click();
                await Task.Delay(1000);
                if(IsRemoteDriver)
                {
                    //Console.WriteLine("Using remote");
                    var allowsDetection = Driver as IAllowsFileDetection;
                    if(allowsDetection!=null)
                    {
                        allowsDetection.FileDetector = new LocalFileDetector();
                        //Console.WriteLine("Set allow detection");
                    }
                }
                var inputFile = Driver.FindElement(By.CssSelector("input[accept='image/*,video/mp4,video/3gpp']"));
                //Console.WriteLine(inputFile.TagName);
                inputFile.SendKeys(inputs);
                //inputFile.SendKeys("/home/seluser/2ngo-thanh-van.jpg");
                //((RemoteWebDriver)Driver).GetScreenshot().SaveAsFile($"/Users/kevinng/Ansarada/Selenium_docker/Draghinh_{Guid.NewGuid().ToString()}.png",ScreenshotImageFormat.Png);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
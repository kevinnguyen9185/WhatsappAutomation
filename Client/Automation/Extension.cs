using System.Collections.Generic;
using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace Client.Automation
{
    public static class Support
    {
        public static ReadOnlyCollection<IWebElement> GetRecursiveElementByCssSelector(LinkedListNode<string> level, IWebElement elm)
        {
            var elms = elm.FindElements(By.CssSelector(level.Value));
            if(level.Next!=null && elms.Count>0)
            {
                foreach(var tempElm in elms)
                {
                    return GetRecursiveElementByCssSelector(level.Next, tempElm);
                }
            }
            else 
            {
                return elms;
            }
            return null;
        }
    }
}
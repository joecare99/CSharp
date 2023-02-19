using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace FoxCon.Model
{
    public static class FoxConMain
    {
        private static IWebDriver driver;
        public static string homeURL;

        static FoxConMain()
        {
            homeURL = "https://trauer.rnz.de/traueranzeigen-suche/aktuelle-ausgabe";
            driver = new EdgeDriver();

        }
        public static void Login_is_on_home_page()
        {
            driver.Navigate().GoToUrl(homeURL);
            WebDriverWait wait = new WebDriverWait(driver,System.TimeSpan.FromSeconds(15));
            wait.Until(driver =>driver.FindElement(By.XPath("//a[@href='/beta/login']")));
            IWebElement element =driver.FindElement(By.XPath("//a[@href='/beta/login']"));

        }
    }
}

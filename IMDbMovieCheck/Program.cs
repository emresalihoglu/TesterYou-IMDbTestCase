using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IMDbMovieCheck
{
    public class MyObject
    {
        public string SITE_URL = "https://www.imdb.com";
        public string[] theCircus = new string[]
        {
            "The Circus",
            "1928",
            "//*[@id=\"__next\"]/main/div/section[1]/section/div[3]/section/section/div[3]/div[2]/div[1]/div[3]/ul/li[1]/div/ul/li/a",
            "//*[@id=\"__next\"]/main/div/section[1]/section/div[3]/section/section/div[3]/div[2]/div[1]/div[3]/ul/li[2]/div/ul/li/a",
            "//*[@id=\"__next\"]/main/div/section[1]/section/div[3]/section/section/div[3]/div[2]/div[1]/div[3]/ul/li[3]/div/ul/li/a",
        };
        public string[] theJazzSinger = new string[]
        {
            "The Jazz Singer",
            "1927",
            "//*[@id=\"__next\"]/main/div/section[1]/section/div[3]/section/section/div[3]/div[2]/div[1]/div[3]/ul/li[1]/div/ul/li/a",
            "//*[@id=\"__next\"]/main/div/section[1]/section/div[3]/section/section/div[3]/div[2]/div[1]/div[3]/ul/li[2]/div/ul/li/a",
            "//*[@id=\"__next\"]/main/div/section[1]/section/div[3]/section/section/div[3]/div[2]/div[1]/div[3]/ul/li[3]/div/ul/li/a",
        };
        public string[] messages = new string[]
        {
            "#     - Director information matches correctly.",
            "#     - Director information doesn't match!",
            "#     - Writer information matches correctly.",
            "#     - Writer information doesn't match!",
            "#     - Stars information matches correctly.",
            "#     - Stars information doesn't match!",
            "#     - All links are working correctly.",
            "#     - Broken link was found!",
        };
        public List<string> getDirectors = new List<string>();
        public List<string> getWriters = new List<string>();
        public List<string> getStars = new List<string>();
        public List<string> checkDirectors = new List<string>();
        public List<string> checkWriters = new List<string>();
        public List<string> checkStars = new List<string>();
        public List<string> photoLinks = new List<string>();
        public List<string> pageList = new List<string>();
        public int index = 0;
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            MyObject myObject = new MyObject();
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--start-maximized");
            chromeOptions.AddArgument("--log-level=3");
            IWebDriver driver = new ChromeDriver(chromeOptions);

            // The Circus
            AwardsAndEvents (driver, myObject.theCircus, myObject);
            SearchTitle (driver, myObject.theCircus, myObject);
            CheckCredits (myObject);
            SeeAllPhotos (driver, myObject);
            CheckPhotoLinks (myObject);

            // The Jazz Singer
            AwardsAndEvents (driver, myObject.theJazzSinger, myObject);
            SearchTitle (driver, myObject.theJazzSinger, myObject);
            CheckCredits (myObject);
            SeeAllPhotos (driver, myObject);
            CheckPhotoLinks (myObject);
        }

        /*
         * Arama çubuğunun sol tarafında bulunan Menü butonuna basılır.
         * Gelen ekranda "Awards & Events" başlığı altında bulunan "Oscars" butonuna tıklanır.
         * "Event History" başlığı altında "1929" değeri seçilir.
         * "Honorary Award" başlığı altnda "The Circus (Charles Chaplin)" seçilir.
         * 
         * Gelen ekranda, The Circus'a ait;
         *   a. Director: bilgisi kayıt edilir.
         *   b. Writer: bilgisi kayıt edilir.
         *   c. Stars: bilgisi kayıt edilir.
         * 
         * Ekranın sol üstünde bulunan "IMDb" butonuna tıklanarak "Anasayfa'ya" dönülür.
         * Arama çubuğuna "The Circus" yazılır.
         * Sonuçlar arasında gelen "The Circus'a" tıklanır.
         * 
         * Gelen ekranda;
         *   a. Director: bilgisi kayıt edilen Director'le aynı mı kontrol edilir.
         *   b. Writer: bilgisi kayıt edilen Writer'la aynı mı kontrol edilir.
         *   c. Stars: bilgisi kayıt edilen Stars'la aynı mı kontrol edilir.
         */
        public static void AwardsAndEvents (IWebDriver driver, string[] title, MyObject myObject)
        {
            Console.WriteLine("# Testing: \"" + title[0] + "\"\n#   Checking: Movie Credits");
            driver.Url = myObject.SITE_URL;
            System.Threading.Thread.Sleep(2000);
            driver.FindElement(By.Id("imdbHeader-navDrawerOpen")).Click();
            System.Threading.Thread.Sleep(1000);
            IList<IWebElement> awards = driver.FindElements(By.XPath("//nav[@id=\"imdbHeader\"]//child::a"));
            awards.First(element => element.Text == "Oscars" && element.GetAttribute("href").Contains("oscars")).Click();
            driver.FindElement(By.XPath("//div[@class='event-history-widget__years']//child::a[text()='1929']")).Click();
            System.Threading.Thread.Sleep(2000);
            driver.FindElement(By.XPath("//div[text()='Honorary Award']//following::a[text()='" + title[0] + "']")).Click();

            GetCredits(driver, title, myObject, 1);
        }
        public static void SearchTitle (IWebDriver driver, string[] title, MyObject myObject)
        {
            driver.FindElement(By.Id("home_img_holder")).Click();
            System.Threading.Thread.Sleep(2000);
            driver.FindElement(By.XPath("//input[@placeholder='Search IMDb']")).SendKeys(title[0]);
            System.Threading.Thread.Sleep(2000);
            driver.FindElement(By.XPath("//input[@placeholder='Search IMDb']")).Submit();
            System.Threading.Thread.Sleep(2000);
            driver.FindElement(By.XPath("//label[text()='" + title[1] + "']//ancestor::ul[1]//preceding-sibling::a[text()='" + title[0] + "']")).Click();
            System.Threading.Thread.Sleep(2000);
            GetCredits(driver, title, myObject, 0);
        }
        public static void GetCredits (IWebDriver driver, string[] title, MyObject myObject, int isFirst)
        {
            IList<IWebElement> directorList = driver.FindElements(By.XPath(title[2]));
            IList<IWebElement> writerList = driver.FindElements(By.XPath(title[3]));
            IList<IWebElement> starList = driver.FindElements(By.XPath(title[4]));

            if (directorList.Count != 0)
            {
                foreach (IWebElement director in directorList)
                {
                    if (isFirst == 1) myObject.getDirectors.Add(director.Text);
                    else myObject.checkDirectors.Add(director.Text);
                }
            }
            if (writerList.Count != 0)
            {
                foreach (IWebElement writer in writerList)
                {
                    if (isFirst == 1) myObject.getWriters.Add(writer.Text);
                    else myObject.checkWriters.Add(writer.Text);
                }
            }
            if (starList.Count != 0)
            {
                foreach (IWebElement star in starList)
                {
                    if (isFirst == 1) myObject.getStars.Add(star.Text);
                    else myObject.checkStars.Add(star.Text);
                }
            }
        }
        public static void CheckCredits (MyObject myObject)
        {
            if (myObject.checkDirectors.SequenceEqual(myObject.getDirectors)) Console.WriteLine(myObject.messages[0]);
            else Console.WriteLine(myObject.messages[1]);

            if (myObject.checkWriters.SequenceEqual(myObject.getWriters)) Console.WriteLine(myObject.messages[2]);
            else Console.WriteLine(myObject.messages[3]);

            if (myObject.checkStars.SequenceEqual(myObject.getStars)) Console.WriteLine(myObject.messages[4]);
            else Console.WriteLine(myObject.messages[5]);

            myObject.getDirectors.Clear();
            myObject.getWriters.Clear();
            myObject.getStars.Clear();
            myObject.checkDirectors.Clear();
            myObject.checkWriters.Clear();
            myObject.checkStars.Clear();
        }

        /*
         * Kontroller yapıldıktan sonra "See all .. photos" linkine tıklanır.
         * Gelen ekranda;
         *   a. Bütün fotoğrafların linklerinin kırık olmadığı kontrol edilir.
         */
        public static void SeeAllPhotos (IWebDriver driver, MyObject myObject)
        {
            Console.WriteLine("#   Checking: Photo links");
            driver.FindElement(By.XPath("//section[@data-testid='Photos']//child::h3")).Click();

            IList<IWebElement> pages = driver.FindElements(By.XPath("//span[@class='page_list']//child::a"));
            if (pages.Count == 0)
            {
                GetPhotoLinks(driver, myObject);
            }
            else
            {
                myObject.pageList.Add("1");
                for (int i = 0; i < pages.Count / 2; i++)
                {
                    myObject.pageList.Add(pages[i].Text);
                }
                GetPhotoLinks(driver, myObject);
                myObject.index++;

                while (myObject.index < myObject.pageList.Count)
                {
                    driver.FindElement(By.LinkText(myObject.pageList[myObject.index])).Click();
                    System.Threading.Thread.Sleep(1000);
                    GetPhotoLinks(driver, myObject);
                    myObject.index++;
                }
            }
        }
        public static void GetPhotoLinks (IWebDriver driver, MyObject myObject)
        {
            IList<IWebElement> photoList = driver.FindElements(By.XPath("//*[@id='media_index_thumbnail_grid']/a/img"));
            foreach (IWebElement photo in photoList)
            {
                myObject.photoLinks.Add(photo.GetAttribute("src"));
            }
        }
        public static void CheckPhotoLinks (MyObject myObject)
        {
            var client = new HttpClient();
            int brokenLink = 0;

            foreach (var photoLink in myObject.photoLinks)
            {
                var response = client.GetAsync(photoLink).GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode) brokenLink++;
            }

            if (brokenLink == 0) Console.WriteLine(myObject.messages[6] + " (" + myObject.photoLinks.Count + ")\n# Done!");
            else Console.WriteLine(myObject.messages[7] + " (" + brokenLink + " in " + myObject.photoLinks.Count + ")\n# Done!");

            myObject.photoLinks.Clear();
            myObject.pageList.Clear();
            myObject.index = 0;
        }
    }
}

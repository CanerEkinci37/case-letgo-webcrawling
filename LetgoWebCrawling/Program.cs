using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using LetgoWebCrawling;
internal class Program
{
    private static void Main(string[] args)
    {
        // Chrome Driver Path'ini belirtme ve Konfigürasyonu
        string driverPath = "ChromeDriver dizinini girin";
        var options = new ChromeOptions();
        // Handshake failed warninglerini almamak için yapılan işlem. 
        options.AddArgument("log-level=3");
        // Ip Ban'a yönelik yapılan user-agent konfigürasyonu.
        string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3";
        options.AddArgument($"--user-agent={userAgent}");
        var driver = new ChromeDriver(driverPath, options);
        // Driver'i tam ekrana ayarlama.
        driver.Manage().Window.Maximize();

        // Siteye Yönlendirme 
        driver.Navigate().GoToUrl("https://letgo.com");
        Thread.Sleep(2000);

        // Yerel Değişkenler
        List<Product> products = new List<Product>(); // Class yoluyla ürün listesi.
        int tiklamaSayisi = 2; // Daha fazla yükle butonuna tıklama sayısını ifade eder.
        // Butonlarda oluşan InterceptionException hatasına yönelik click amaçlı kullanılan js executor.
        IJavaScriptExecutor javascriptExecutor = (IJavaScriptExecutor)driver;

        //Cookies kısmını kapatma işlemi
        IWebElement btnCookieClose = driver.FindElement(By.XPath("//*[@id=\"onetrust-close-btn-container\"]/button"));
        javascriptExecutor.ExecuteScript("arguments[0].click()", btnCookieClose);
        Thread.Sleep(1000);
        
        // Anasayfada ürün sayısını arttırmak için yapılan işlem.
        IWebElement btnDahaFazlaYukle = driver.FindElement(By.CssSelector("[data-aut-id='btnLoadMore']"));
        int beforeLoadMoreCounter = driver.FindElements(By.ClassName("_1DNjI")).ToList().Count;
        for (int j = 0; j < tiklamaSayisi; j++)
        {
            javascriptExecutor.ExecuteScript("arguments[0].click();", btnDahaFazlaYukle);
            Thread.Sleep(1000);
        }
        int afterLoadMoreCounter = driver.FindElements(By.ClassName("_1DNjI")).ToList().Count; // Güncel Ürün Sayısı


        for (int i = 0; i < afterLoadMoreCounter; i++)
        {
            // Ürünleri çekip indeks bazlı ürünün detay sayfasına gitme işlemi.
            var liElements = driver.FindElements(By.CssSelector("[data-aut-id='itemBox']")).ToList();
            IWebElement link = liElements[i].FindElement(By.TagName("a"));
            javascriptExecutor.ExecuteScript("arguments[0].click();", link);
            Thread.Sleep(2000);
            // Product nesnesi oluşturma işlemi.
            Product product = new Product();
            IWebElement productPriceElement;
            IWebElement productOwnerElement;
            IWebElement productNameElement;

            // Her ürünün bir fiyatı olmak zorunda. Eğer detay sayfasında fiyat yoksa kırık linktir.
            // Kırık Linkten geri dönüp diğer ürüne geçme işlemi.
            // Fiyatı varsa işleme devam edilir.
            try
            {
                productPriceElement = driver.FindElement(By.CssSelector("[data-aut-id='itemPrice']"));
            }
            catch (NoSuchElementException)
            {
                driver.Navigate().Back();
                Thread.Sleep(1500);
                Console.WriteLine("Kırık Link ile karşılaşıldı!");
                continue;
            }
            product.productPrice = float.Parse(productPriceElement.Text.Replace("TL", "").Replace(" ", ""));
            // Nesnenin diğer niteliklerine varsayılan değer atama.
            product.productName = "";
            product.productOwner = "Placeholder Kullanıcı";

            // Ürünlerin başlığı bulunmakta fakat bazı sayfalarda sahipleri gözükmemekte.
            // Gözükmüyorsa Placeholder Kullanıcı olarak tanımlanmakta ve ürünün başlığı sayfadan alınmakta.
            try
            {
                productOwnerElement = driver.FindElement(By.ClassName("eHFQs"));
                product.productOwner = productOwnerElement.Text;
                productNameElement = driver.FindElement(By.CssSelector("[data-aut-id='itemTitle']"));
                product.productName = productNameElement.Text;
            }
            catch (NoSuchElementException)
            {
                productNameElement = driver.FindElement(By.CssSelector("[data-aut-id='itemTitle']"));
                product.productName = productNameElement.Text;
            }
            // Ürün listesine atama ve geri dönme işlemi.
            /* Daha fazla yükle dediğimiz kısımda genel olarak 15 ürünün verisini çekip geri döndüğümüzde 
             * Daha fazla yükleye tıklanarak arttırılan ürünler geri gitmekte ve geri getirmek için 15 üründe
             * bir daha fazla yükle butonuna tıklanarak out of range hatasını almaması sağlanır. */
            finally
            {
                products.Add(product);
                driver.Navigate().Back();
                if ((i + 1) % 15 == 0 && beforeLoadMoreCounter < afterLoadMoreCounter)
                {
                    IWebElement loopBtn = driver.FindElement(By.CssSelector("[data-aut-id='btnLoadMore']"));
                    javascriptExecutor.ExecuteScript("arguments[0].click();", loopBtn);
                }
                Thread.Sleep(1500);
            }

            Console.WriteLine(product.productOwner + "\t" + product.productName + "\t" + product.productPrice);
        }
        driver.Quit();


        // Dosyaya Yazma İşlemi
        try
        {
            // Dosyanın oluşturulması
            StreamWriter sw = new StreamWriter("Dizin girin");
            // Ürünlerin bilgilerinin yazılması.
            foreach (var product in products)
            {
                sw.WriteLine(product.productOwner + "\t" + product.productName + "\t" + product.productPrice);
            }
            sw.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine("Dosyaya yazarken hata oluştu");
        }
        // Ortalama fiyatın hesabı.
        float averagePrice = products.Average(product => product.productPrice);
        Console.WriteLine("Ortalama Fiyat: " + averagePrice);
        Console.ReadLine();
    }
}
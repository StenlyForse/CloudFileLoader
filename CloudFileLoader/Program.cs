using System;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace CloudFileLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            string login = "developingTest";
            string pass = "xWTRa829eKNM";
            string directory = @"D:\Test";
            bool stop = true;

            //получаем текущую дату формата дд/мм/гггг чч/мм/сс
            DateTime currDate = DateTime.Now;
            //коневертируем дату в строку с нужным форматом
            string dateString = currDate.ToString("dd/MM/yy");

            //путь к драйверу
            //нужно его инициализировать,т.к. стандартный не поддерживает хром 95, драйверлежит в папке проекта
            string driverPath = Directory.GetCurrentDirectory();
            IWebDriver driver = new ChromeDriver(driverPath)
            {
                Url = @"https://disk.yandex.ru/"
            };
            //устанавливаем ожидание, чтобы если страница или элемент прогружаются, селениум ждал
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            //кликаем по кнопке логина
            driver.FindElement(By.XPath("//*[contains(@class, 'button button_login header__login-link')]")).Click();
            driver.FindElement(By.XPath("//*[@id='passp-field-login']")).SendKeys(login);
            //кликаем по кнопке войти
            driver.FindElement(By.XPath("//*[contains(@class, 'Button2 Button2_size_l Button2_view_action Button2_width_max Button2_type_submit')]")).Click();
            driver.FindElement(By.Id("passp-field-passwd")).SendKeys(pass);

            //иногда при заходе на диск вылезает баннер с предложением скачать диск, надо его закрывать, поэтому используем блок трай кетч
            try
            {
                //кликаем по кнопке войти
                //driver.FindElement(By.XPath("//*[contains(@class, 'Button2 Button2_size_l Button2_view_action Button2_width_max Button2_type_submit')]")).Click();
                driver.FindElement(By.CssSelector(".Button2.Button2_size_l.Button2_view_action.Button2_width_max.Button2_type_submit")).Click();

                //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

                //кликает на кнопку файлы
                //driver.FindElement(By.XPath(@"/html/body/div[1]/div/div/div[3]/div[2]/div/div[1]/div[2]/a")).Click();
                driver.FindElement(By.Id("/disk")).Click();
            }
            catch (Exception)
            {
                //закрываем баннер
                driver.FindElement(By.XPath("/html/body/div[3]/div[2]/div/div/div/div/button")).Click();
            }
            finally
            {
                driver.FindElement(By.Id("/disk")).Click();
            }
            
            //кликаем на кнопку "Создать"
            //тк имя класса составное (Button2 Button2_view_raised Button2_size_m Button2_width_max), то конвертируем его в кссСелектор
            driver.FindElement(By.CssSelector(".Button2.Button2_view_raised.Button2_size_m.Button2_width_max")).Click();
            //клик по "Папку"
            driver.FindElement(By.CssSelector(".file-icon.file-icon_size_m.file-icon_dir_plus.create-resource-button__icon")).Click();

            //устанавливаем название папки текущей датой
            //стираем с поля названия папки "Новая папка"
            driver.FindElement(By.XPath("/html/body/div/div/div/div/div/div/div/div/div/form/span/input")).SendKeys(Keys.Backspace);
            //останавливаем поток, иначе оно стирает только один символ, а не всю надпись
            Thread.Sleep(1000);
            //закидываем дату
            driver.FindElement(By.XPath("/html/body/div/div/div/div/div/div/div/div/div/form/span/input")).SendKeys(dateString);
            //нажимаем на кнопку "Сохранить"
            driver.FindElement(By.CssSelector(".Button2.Button2_theme_raised.Button2_view_action.Button2_size_m.confirmation-dialog__button.confirmation-dialog__button_submit")).Click();
            //ищем папку с текущей датой
            IWebElement folder = driver.FindElement(By.XPath("//*[contains(text()," + "'" + dateString  + "'" + ")]"));

            //инициализируем экшон для doubleclick
            OpenQA.Selenium.Interactions.Actions action = new OpenQA.Selenium.Interactions.Actions(driver);
            action.DoubleClick(folder).Perform();

            //обозначаем кнопку загрузки, туда будем посылать пути к файлам
            IWebElement loader = driver.FindElement(By.ClassName("upload-button__attach"));

            //получаем все файлы в папке и проходимся по ним, отправляя в кнопку загрузки
            string[] filenames = Directory.GetFiles(directory, "*" ,SearchOption.AllDirectories);
            string[] fasdf = Directory.GetDirectories(directory);

            //если нет файлов в папке
            if (filenames.Length == 0)
            {
                Console.WriteLine("Файлы отсутствуют в папке");
                driver.Quit();
                Environment.Exit(0);
            }

            foreach(string file in filenames)
            {
                loader.SendKeys(file);
            }

            //тут проверяем текст в окошке загрузки, затем проходимся по каждому и кидаем его в загрузку
            string loadtext = driver.FindElement(By.ClassName("uploader-progress__progress-primary")).Text;
            
            //ждем пока загрузятся все файлы или выйдет ошибка и прекращаем выполнение => закрываем браузер, выходим из драйвера
            while (stop == true)
            {
                loadtext = driver.FindElement(By.ClassName("uploader-progress__progress-primary")).Text;
                if (loadtext == "Все файлы загружены" || loadtext == "Загрузка остановлена")
                    stop = false;
                Console.WriteLine(loadtext);
                //чтобы не спамило в консоль
                Thread.Sleep(3000);
            }

            Console.WriteLine("end");
            driver.Quit();

            if (loadtext == "Все файлы загружены")
            {
                //удаляем все файлы из папки
                foreach (string file in filenames)
                {
                    File.Delete(file);
                }
                foreach (string file in fasdf)
                    Directory.Delete(file);
            }

            Console.WriteLine(loadtext);
            Environment.Exit(0);
        }
    }
}

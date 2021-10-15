using System;
using System.IO;
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

            //путь к драйверу
            //нужно его инициализировать,т.к. стандартный не поддерживает хром 95, драйверлежит в папке проекта
            string driverPath = Directory.GetCurrentDirectory();
            IWebDriver driver = new ChromeDriver(driverPath);

            driver.Url = @"https://disk.yandex.ru/";

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

            //кликаем по кнопке логина
            driver.FindElement(By.XPath("//*[contains(@class, 'button button_login header__login-link')]")).Click();
            //driver.FindElement(By.ClassName("button button_login header__login-link")).Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

            driver.FindElement(By.XPath("//*[@id='passp-field-login']")).SendKeys(login);
            //кликаем по кнопке войти
            driver.FindElement(By.XPath("//*[contains(@class, 'Button2 Button2_size_l Button2_view_action Button2_width_max Button2_type_submit')]")).Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            driver.FindElement(By.Id("passp-field-passwd")).SendKeys(pass);

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

            //кликаем по кнопке войти
            driver.FindElement(By.XPath("//*[contains(@class, 'Button2 Button2_size_l Button2_view_action Button2_width_max Button2_type_submit')]")).Click();

            Console.ReadKey();

            driver.Quit();
        }
    }
}

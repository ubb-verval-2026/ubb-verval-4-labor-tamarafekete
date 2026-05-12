using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Text;
using NUnit.Framework;

namespace DatesAndStuff.Web.Tests;

[TestFixture]
public class BlazeDemoTests
{
    private IWebDriver driver;
    private StringBuilder verificationErrors;
    private const string BaseURL = "https://blazedemo.com";
    private const double PriceLimit = 300.0;
    private readonly string ScreenshotFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "BlazeDemoScreenshots");
    [SetUp]
    public void SetupTest()
    {
        driver = new ChromeDriver();
        verificationErrors = new StringBuilder();
        if (!Directory.Exists(ScreenshotFolder))
        {
            Directory.CreateDirectory(ScreenshotFolder);
        }
    }

    [TearDown]
    public void TeardownTest()
    {
        driver.Quit();
        driver.Dispose();
    }

    [Test]
    public void FlightSearch_MexicoCityToDublin_ShouldHaveAtLeastThreeFlights()
    {
        // Arrange
        driver.Navigate().GoToUrl(BaseURL);
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        var fromElem = wait.Until(ExpectedConditions.ElementIsVisible(By.Name("fromPort")));
        SelectElement fromPort = new SelectElement(fromElem);
        fromPort.SelectByValue("Mexico City");

        var toElem = wait.Until(ExpectedConditions.ElementIsVisible(By.Name("toPort")));
        SelectElement toPort = new SelectElement(toElem);
        toPort.SelectByValue("Dublin");

        // Act
        wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input[type='submit']"))).Click();

        // Assert
        wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("table")));
        var flightRows = driver.FindElements(By.XPath("//table[@class='table']/tbody/tr"));
        flightRows.Count.Should().BeGreaterThanOrEqualTo(3);

        bool foundFlight = false;
        foreach (var row in flightRows)
        {
            var priceText = row.FindElement(By.XPath("./td[6]")).Text;
            var cleanPriceText = priceText.Replace("$", "").Trim();
            if (double.TryParse(cleanPriceText, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double actualPrice))
            {
                if (actualPrice < PriceLimit)
                {
                    foundFlight = true;
                    break;
                }
            }
            
        }
        if (foundFlight)
        {
            TakeScreenshot("CheapFlightFound");
        }
    }

    private void TakeScreenshot(string fileName)
    {
        try
        {
            ITakesScreenshot? ssdriver = driver as ITakesScreenshot;
            Screenshot screenshot = ssdriver.GetScreenshot();

            string filePath = Path.Combine(ScreenshotFolder, $"{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            screenshot.SaveAsFile(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
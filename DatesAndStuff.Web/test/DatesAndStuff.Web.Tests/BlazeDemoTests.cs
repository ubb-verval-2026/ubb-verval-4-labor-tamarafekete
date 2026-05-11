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

    [SetUp]
    public void SetupTest()
    {
        driver = new ChromeDriver();
        verificationErrors = new StringBuilder();
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
    }
}
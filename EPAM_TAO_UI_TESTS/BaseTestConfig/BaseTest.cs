using System;
using System.Configuration;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using EPAM_TAO_CORE_UI_TAF.TestSetup;
using EPAM_TAO_CORE_UI_TAF.UI_Helpers;
using System.Reflection;
using EPAM_TAO_UI_POM.Pages.Products;
using EPAM_TAO_UI_POM.Pages.Cart;
using EPAM_TAO_UI_POM.Pages.Checkout;

namespace EPAM_TAO_UI_TESTS.BaseTestConfig
{
    [TestFixture]
    public abstract class BaseTest : TestHookup
    {
        private string strBrowser { get { return ConfigurationManager.AppSettings["Browser"].ToString(); } }
        private string strSiteURL { get { return ConfigurationManager.AppSettings["SiteURL"].ToString(); } }
        private string strAUT { get { return ConfigurationManager.AppSettings["AUT"].ToString(); } }
        public string strUserName { get { return ConfigurationManager.AppSettings["UserName"].ToString(); } }
        public string strPassword { get { return ConfigurationManager.AppSettings["Password"].ToString(); } }
        public Exception testEx { get; set; }
        public ProductsPage productsPage {get; set;}
        public CartPage cartPage { get; set; }
        public CheckoutPage checkoutPage { get; set; }

        [SetUp]
        public void setupDriverSession()
        {
            try
            {
                driver = InitBrowser((BrowserType)Enum.Parse(typeof(BrowserType), strBrowser.ToUpper()));
                CommonUtilities.commonUtilities.NavigateToURL(driver, strSiteURL);
                CommonUtilities.commonUtilities.MaximizeWindow(driver);

                ExtentReportHelper.GetInstance(strAUT, driver).CreateTest(TestContext.CurrentContext.Test.Name);
            }
            catch(Exception ex)
            {
                ErrorLogger.errorLogger.ErrorLog(MethodBase.GetCurrentMethod().Name, ex, driver);
                ExtentReportHelper.GetInstance(strAUT, driver).SetTestStatusFail($"<br>{ex.Message}<br>Stack Trace: <br>{ex.StackTrace}<br>");
            }
        }

        [TearDown]
        public void closeDriverSession()
        {
            try
            {
                var status = TestContext.CurrentContext.Result.Outcome.Status;                

                switch (status)
                {
                    case TestStatus.Failed:
                        var stacktrace = TestContext.CurrentContext.Result.StackTrace;
                        var errorMessage = "<pre>" + TestContext.CurrentContext.Result.Message + "</pre>";

                        ErrorLogger.errorLogger.ErrorLog(MethodBase.GetCurrentMethod().Name, testEx, driver);
                        ExtentReportHelper.GetInstance(strAUT, driver).SetTestStatusFail($"<br>{errorMessage}<br>Stack Trace: <br>{stacktrace}<br>", CommonUtilities.commonUtilities.TakeScreenshot(driver, TestContext.CurrentContext.Test.Name));
                        break;
                    case TestStatus.Skipped:
                        ExtentReportHelper.GetInstance(strAUT, driver).SetTestStatusSkipped();
                        break;
                    default:
                        ExtentReportHelper.GetInstance(strAUT, driver).SetTestStatusPass();
                        break;
                }                
            }
            catch(Exception ex)
            {
                ErrorLogger.errorLogger.ErrorLog(MethodBase.GetCurrentMethod().Name, ex, driver);
                ExtentReportHelper.GetInstance(strAUT, driver).SetTestStatusFail($"<br>{ex.Message}<br>Stack Trace: <br>{ex.StackTrace}<br>");
            }
            finally
            {
                CloseBrowser();
            }
        }

        [OneTimeTearDown]
        public void CloseAll()
        {
            try
            {
                ExtentReportHelper.GetInstance(strAUT, driver).CloseExtentReport();
            }
            catch(Exception ex)
            {
                ErrorLogger.errorLogger.ErrorLog(MethodBase.GetCurrentMethod().Name, ex);
                ExtentReportHelper.GetInstance(strAUT, driver).SetTestStatusFail($"<br>{ex.Message}<br>Stack Trace: <br>{ex.StackTrace}<br>");
            }
        }
    }
}

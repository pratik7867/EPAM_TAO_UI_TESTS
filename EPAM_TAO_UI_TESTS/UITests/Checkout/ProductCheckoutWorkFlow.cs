using System;
using System.Configuration;
using NUnit.Framework;
using EPAM_TAO_UI_TESTS.BaseTestConfig;
using EPAM_TAO_UI_POM.Pages.Login;

namespace EPAM_TAO_UI_TESTS.UITests.Checkout
{
    [TestFixture]
    public class ProductCheckoutWorkFlow : BaseTest
    {
        [Test]
        public void ProductCheckoutTest()
        {
            try
            {
                //DATA Arrangements
                var productName = ConfigurationManager.AppSettings["ProductName"].ToString();
                var firstName = ConfigurationManager.AppSettings["FirstName"].ToString();
                var lastName = ConfigurationManager.AppSettings["LastName"].ToString();
                var postalCode = ConfigurationManager.AppSettings["PostalCode"].ToString();

                //Log in to application
                productsPage = LogInPage.GetInstance(driver).LogIntoApplication(strUserName, strPassword);

                //Add Product to Cart
                var productPrice = productsPage.getProductPrice(productName);
                productsPage.AddToCart(productName);

                //Navigate to Cart Page and Validate Price of the added Product
                cartPage = productsPage.ClickOnShoppingCart();
                Assert.AreEqual(productPrice, cartPage.GetProductPrice());                

                //Navigate to Checkout Page and Fill up mandatory details to conitune
                checkoutPage = cartPage.ClickOnCheckout();
                checkoutPage.FillUpCheckoutDetailsAndContiue(firstName, lastName, postalCode);

                //Validate Name and Price of the added Product
                Assert.AreEqual(productName, checkoutPage.GetProductName());
                Assert.AreEqual(productPrice, checkoutPage.GetProductPrice());

                //Click on Finish button
                checkoutPage.ClickOnFinish();
            }
            catch(Exception ex)
            {
                testEx = ex;
                Assert.Fail(ex.Message);
            }
        }
    }
}

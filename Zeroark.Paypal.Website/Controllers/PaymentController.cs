using System;
using System.Collections.Generic;
using System.Web.Mvc;
using PayPal.Api;

namespace Zeroark.Paypal.Website.Controllers
{
    public class PaymentController : Controller
    {
        // GET: Payment
        public ActionResult Index()
        {
            // Get a reference to the config
            var config = ConfigManager.Instance.GetProperties();

            // Use OAuthTokenCredential to request an access token from PayPal
            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            var apiContext = new APIContext(accessToken) { Config = ConfigManager.Instance.GetProperties() };

            // Define any custom configuration settings for calls that will use this object.
            apiContext.Config["connectionTimeout"] = Convert.ToString(10 * 1000); // Quick timeout for testing purposes

            // Define any HTTP headers to be used in HTTP requests made with this APIContext object
            if (apiContext.HTTPHeaders == null)
            {
                apiContext.HTTPHeaders = new Dictionary<string, string>();
            }

            apiContext.HTTPHeaders["some-header-name"] = "some-value";

            string payerId = Request.Params["PayerID"];

            if (string.IsNullOrEmpty(payerId))
            {
                var itemList = new ItemList()
                {
                    items = new List<Item>()
                    {
                        new Item()
                        {
                            name = "Item Name",
                            currency = "USD",
                            price = "15",
                            quantity = "5",
                            sku = "sku"
                        }
                    }
                };

                var payer = new Payer() { payment_method = "paypal" };

                var baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Payment/Index?";
                var guid = Convert.ToString((new Random()).Next(100000));
                var redirectUrl = baseURI + "guid=" + guid;
                var redirUrls = new RedirectUrls()
                {
                    cancel_url = redirectUrl + "&cancel=true",
                    return_url = redirectUrl
                };

                var details = new Details()
                {
                    tax = "15",
                    shipping = "10",
                    subtotal = "75"
                };

                var amount = new Amount()
                {
                    currency = "USD",
                    total = "100.00", // Total must be equal to sum of shipping, tax and subtotal.
                    details = details
                };

                var transactionList = new List<Transaction>();

                transactionList.Add(new Transaction()
                {
                    description = "Transaction description.",
                    invoice_number = Convert.ToString(new Random().Next(1000000)),
                    amount = amount,
                    item_list = itemList
                });

                var payment = new Payment()
                {
                    intent = "sale",
                    payer = payer,
                    transactions = transactionList,
                    redirect_urls = redirUrls
                };

                var createdPayment = payment.Create(apiContext);

                var links = createdPayment.links.GetEnumerator();
                while (links.MoveNext())
                {
                    var link = links.Current;
                    if (link.rel.ToLower().Trim().Equals("approval_url"))
                    {
                        Session.Add(guid, createdPayment.id);
                        return Redirect(link.href);
                    }
                }
            }
            else
            {
                var guid = Request.Params["guid"];

                var paymentId = Session[guid] as string;
                var paymentExecution = new PaymentExecution() { payer_id = payerId };
                var payment = new Payment() { id = paymentId };

                var executedPayment = payment.Execute(apiContext, paymentExecution);
            }

            return View();
        }

        // GET: Payment/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Payment/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Payment/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Payment/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Payment/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Payment/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Payment/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace HeadSetChallange
{
    class Program
    {
        static void Main(string[] args)
        {
            string urlAddress = "https://headsetdev.blob.core.windows.net/hiring/code-chanllenges/Headset-POS-Challenge.json";
            
            using (WebClient client = new WebClient())
            {
                using (StreamReader sr = new StreamReader(client.OpenRead(urlAddress)))
                {
                    using (JsonReader reader = new JsonTextReader(sr)) 
                    {

                        StringBuilder resultData = new StringBuilder();

                        JsonSerializer serializer = new JsonSerializer();

                        IList<transaction> result = serializer.Deserialize<List<transaction>>(reader);


                        /*
                        Count of
                            Sales
                            Returns
                            Transfers
                        */
                        resultData.Append("Number of Total Transactions: " + result.Where(x => x.transactionType.Equals("Retail")).Count() + "\n");
                        resultData.Append("\tNumber of Sales: " + result.Where(x => x.total > 0 && x.transactionType.Equals("Retail")).Count() + "\n");
                        resultData.Append("\tNumber of Returns: " + result.Where(x => x.total < 0 && x.transactionType.Equals("Retail")).Count() + "\n");
                        resultData.Append("\tNumber of Transfers: " + result.Where(x => x.transactionType.Equals("Transfer")).Count() + "\n");

                        //Total Number of Items Sold
                        var totalNumber = result.Where(x => x.transactionType.Equals("Retail")).Sum(x => x.items.Sum(p => p.quantity).Value);

                        resultData.Append("Total Number Of Items Sold: " + totalNumber.ToString("#") + "\n");

                        //Total revenue and profit for the time period
                        //Total Revenue
                        var totalRevenue = result.Where(x => x.transactionType.Equals("Retail")).Sum(x => (x.subtotal - x.totalDiscount).Value);

                        resultData.Append("Total Revenue: " + totalRevenue.ToString("#.##") + "\n");

                        //Total Profit
                        var totalProfit = result.Where(x => x.transactionType.Equals("Retail")).Sum(x => x.items.Sum(k => ((k.totalPrice - k.totalDiscount - k.unitCost) * k.quantity))).Value;
                        resultData.Append("Total Profit: " + totalProfit.ToString("#.##") + "\n");

                        //Total dollar amount of discounts applied
                        resultData.Append("Total Amout of Discounts: " + result.Sum(x => x.totalDiscount) + "\n\n");

                        //Total revenue and profit for each day of the time period

                        var dailyRevenueList = result.Where(x => x.transactionDate.HasValue && x.transactionType.Equals("Retail")).Select(x => new { date = x.transactionDate, revenue = x.subtotal - x.totalDiscount, profit = x.items.Sum(k => ((k.totalPrice - k.totalDiscount - k.unitCost) * k.quantity)), items = x.items.Sum(k => k.quantity) })
                            .GroupBy(x => x.date.Value.Date).Select(x => new { date = x.Key, dailyProfit = x.Sum(p => p.profit).Value, dailyRevenue = x.Sum(p => p.revenue).Value, items = x.Sum(p => p.items).Value }).OrderBy(x => x.date);

                        foreach (var day in dailyRevenueList)
                        {
                            resultData.Append("Date: " + day.date.ToShortDateString() + " Revenue: " + day.dailyRevenue.ToString("#.##") + " Profit: " + day.dailyProfit.ToString("#.##") + " Number Of Items Sold: " + day.items.ToString("#") + "\n");
                        }

                        using (StreamWriter writer = new StreamWriter("output.txt"))
                        {
                            writer.WriteLine(resultData.ToString());
                        }

                        Console.ReadLine();
                    }
                }
            }
            
        }

        public class transaction
        {
            public DateTime? transactionDate { get; set; }
            public double? giftPaid { get; set; }
            public bool isVoid { get; set; }
            public double? loyaltySpent {get;set;}
            public double? creditPaid { get; set; }
            public double? checkPaid { get; set; }
            public double? changeDue { get; set; }
            public int? customerTypeId { get; set; }
            public double? total { get; set; }
            public int? employeeId { get; set; }
            public string orderType { get; set; }
            public DateTime? voidDate { get; set; }
            public discount[] discounts { get; set; }
            public string orderSource { get; set; }
            public double? cashPaid { get; set; }
            public int? returnOnTransactionId { get; set; }
            public string transactionType { get; set; }
            public double? tax { get; set; }
            public DateTime? checkInDate { get; set; }
            public double? paid { get; set; }
            public double? totalDiscount { get; set; }
            public string invoiceName { get; set; }
            public double? nonRevenueFeesAndDonations { get; set; }
            public double? subtotal { get; set; }
            public int? transactionId { get; set; }
            public bool wasPreOrdered { get; set; }
            public string authCode { get; set; }
            public item[] items { get; set; }
            public string[] orderIds { get; set; }
            public int? customerId { get; set; }
            public double? totalBeforeTax { get; set; }
            public bool isReturn { get; set; }
            public string[] taxSummary { get; set; }
            public double? totalCredit { get; set; }
            public int? totalItems { get; set; }
            public DateTime? lastModifiedDateUTC { get; set; }
            public int? adjustmentForTransactionId { get; set; }
            public double? debitPaid { get; set; }

        }

        public class item
        {
            public double? totalPrice { get; set; }
            public int? returnedByTransactionId { get; set; }
            public double? flowerEquivalent { get; set; }
            public string flowerEquivalentUnit { get; set; }
            public double? unitCost { get; set; }
            public DateTime? returnDate { get; set; }
            public double? taxes { get; set; }
            public string unitWeightUnit { get; set; }
            public bool isReturned { get; set; }
            public discount[] discounts { get; set; }
            public double? totalDiscount { get; set; }
            public bool isCoupon { get; set; }
            public string packageId { get; set; }
            public int? unitId { get; set; }
            public double? unitWeight { get; set; }
            public int? transactionId { get; set; }
            public int? transactionItemId { get; set; }
            public double? unitPrice { get; set; }
            public string returnReason { get; set; }
            public double? quantity { get; set; }
            public int? productId { get; set; }
        }
        

        public class discount
        {
            public double? amount { get; set; }
            public string discountReason { get; set; }
            public string discountName { get; set; }
            public int? transactionItemId { get; set; }
        }
        
    }
}

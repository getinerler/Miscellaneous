using System;
using System.Collections.Generic;
using System.Xml;

public class TcmbCurrenciesHelper
{
    public static void Main()
    {
        try
        {
            List<TcmbModel> todaysCurrencies = GetCurrencyRatesByTcmb(DateTime.Today);
            foreach (TcmbModel currency in todaysCurrencies)
            {
                Console.WriteLine($"{currency.Name} {currency.Rate}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
        Console.ReadLine();
    }

    public static List<TcmbModel> GetCurrencyRatesByTcmb(DateTime date)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(GetDateString(date));

        XmlNodeList nodes = xmlDoc.SelectNodes("Tarih_Date/Currency");
        if (nodes == null)
        {
            throw new Exception($"{date:dd-MM-yyyy} kuru bulunamadı.");
        }

        List<TcmbModel> rates = new List<TcmbModel>();
        foreach (XmlNode item in nodes)
        {
            if (item == null)
            {
                continue;
            }

            XmlNode nameNode = item.SelectSingleNode("Isim");
            XmlNode rateNode = item.SelectSingleNode("BanknoteSelling");

            if (nameNode == null || rateNode == null)
            {
                continue;
            }

            string name = nameNode.InnerXml;
            string rate = rateNode.InnerXml;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(rate))
            {
                continue;
            }

            if (!decimal.TryParse(rate, out decimal rateDecimal))
            {
                continue;
            }

            rates.Add(new TcmbModel()
            {
                Name = name,
                Rate = rateDecimal
            });
        }
        return rates;
    }

    private static string GetDateString(DateTime date)
    {
        string httpStr;
        if (date.Date == DateTime.Today)
        {
            httpStr = "http://www.tcmb.gov.tr/kurlar/today.xml";
        }
        else
        {
            httpStr = $"http://www.tcmb.gov.tr/kurlar/{date:yyyyMM}/{date:ddMMyyyy}.xml";
        }
        return httpStr;
    }
}

public class TcmbModel
{
    public string Name { get; set; }
    public decimal Rate { get; set; }
}
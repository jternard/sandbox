using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using html=HtmlAgilityPack;

namespace Scraper
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.In.Read();
            
        }

        static void BastienEmploi()
        {
            html.HtmlWeb web = new html.HtmlWeb();
            html.HtmlDocument doc = web.Load("http://www.jobup.ch/search/joblist.asp?cmd=showresults&subcategories=79,151,211,77&cantons=GE1,GE2,GE3,VD3,GE&&p=1");


            int pageMax;
            Int32.TryParse(doc.DocumentNode.SelectSingleNode("//a[@class='C_LAST']").GetAttributeValue("href", "lol").Last().ToString(), out pageMax);

            for (int currentPage = 1; currentPage <= pageMax; currentPage++)
            {
                doc = web.Load("http://www.jobup.ch/search/joblist.asp?cmd=showresults&subcategories=79,151,211,77&cantons=GE1,GE2,GE3,VD3,GE&&p=" + currentPage.ToString());
                var HeaderNames = doc.DocumentNode.SelectNodes("//a[@class='C_URL']").ToList();
                foreach (var item in HeaderNames)
                    Console.WriteLine(item.InnerText);
            }


            /*
            html.HtmlNode node = doc.DocumentNode.FirstChild;
            while (node != doc.DocumentNode.LastChild)
            {
                try
                {
                    Console.WriteLine(node.InnerText);
                    node = node.NextSibling;
                }
                catch (Exception e)
                { Console.WriteLine(e.Message); }
            }
            */
        }
    }
}
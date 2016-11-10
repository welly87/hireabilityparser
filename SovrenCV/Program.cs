using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceReference1;

namespace SovrenCV
{
    public class Program
    {
        private const string AccountId = "XXX";
        private const string ServiceKey = "XXX";

        public static void Main(string[] args)
        {
            byte[] fileBytes = File.ReadAllBytes(@"C:\Users\welly\AppData\Roaming\Skype\My Skype Received Files\CV.pdf");

            // Optionally, compress the bytes to reduce network delays
            //fileBytes = Gzip(fileBytes);
            //var endpointConfiguration = new ParsingServiceSoapClient.EndpointConfiguration();
            
            var client = new ParsingServiceSoapClient(new ParsingServiceSoapClient.EndpointConfiguration());

            ParseResumeRequest request = new ParseResumeRequest
            {
                // Required parameters
                AccountId = AccountId,
                ServiceKey = ServiceKey,
                FileBytes = fileBytes,

                // Optional parameters
                //Configuration = "", // Paste string from Parser Config String Builder.xls spreadsheet
                //OutputHtml = true, // Convert to HTML
                //OutputRtf = true, // Convert to RTF
                //OutputWordXml = true, // Convert to WordXml
                //RevisionDate = "2011-05-15", // Parse assuming a historical date for "current"
            };

            // Perform the parse. The first request will be slow due to WCF initializing
            // the connection and SOAP/XML serialization, but subsequent calls will be fast.
            ParseResumeResponse response = client.ParseResumeAsync(request).Result;

            // Display the results
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Code=" + response.Code);
            Console.WriteLine("SubCode=" + response.SubCode);
            Console.WriteLine("Message=" + response.Message);
            Console.WriteLine("TextCode=" + response.TextCode);
            Console.WriteLine("CreditsRemaining=" + response.CreditsRemaining);
            Console.WriteLine("-----");
            Console.WriteLine(response.Xml);

        }
    }
}

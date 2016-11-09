using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CVLizers
{
    public class JsonData
    {
        public String model { get; set; }
        public String language { get; set; }
        public String filename { get; set; }
        public String data { get; set; }
    }


    public class Program
    {
        private const string fileName = @"C:\Users\welly\Downloads\CV_WellyTambunan_V08.pdf";
        private const string productToken = "<<insert product token>>";

        public static void Main(string[] args)
        {
            Byte[] bytes = System.IO.File.ReadAllBytes(fileName);
            String file = Convert.ToBase64String(bytes);

            JsonData data = new JsonData();
            data.model = "cvlizer_3_0";
            data.language = "de";
            data.filename = fileName;
            data.data = file;

            string json = JsonConvert.SerializeObject(data);
            byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);

            System.Net.HttpWebRequest req = System.Net.WebRequest.Create("https://cvlizer.joinvision.com/cvlizer/rest/v1/extract/xml/")
                as System.Net.HttpWebRequest;

            req.Method = "POST";
            req.ContentType = "application/json";
            req.Headers[System.Net.HttpRequestHeader.ContentLength] = jsonBytes.Length.ToString();
            req.Headers[System.Net.HttpRequestHeader.Authorization] = "Bearer " + productToken;

            //req.Headers.Add("Authorization", "Bearer " + < Product - Token >);
            //req.ContentLength = jsonBytes.Length;

            using (System.IO.Stream post = req.GetRequestStreamAsync().Result) // todo please don't block the tread
            {
                post.Write(jsonBytes, 0, jsonBytes.Length);
            }

            string result = null;
            using (System.Net.HttpWebResponse resp = req.GetResponseAsync().Result
                as System.Net.HttpWebResponse)
            {
                System.IO.StreamReader reader =
                    new System.IO.StreamReader(resp.GetResponseStream());
                result = reader.ReadToEnd();
            }
            Console.Write(result);
            File.WriteAllText("cvlizer.xml", result);
            Console.ReadLine();
        }
    }
}

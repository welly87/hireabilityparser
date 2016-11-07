using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.StaticFiles;

namespace ConsoleApp1
{
    public class Program
    {

        private static string GetMimeMapping(string file)
        {
            string contentType;
            new FileExtensionContentTypeProvider().TryGetContentType(file, out contentType);
            return contentType ?? "application/octet-stream";
        }

        public static void Main(string[] args)
        {

            string xmlResponse = null;
            string productCode = "e9ad91af8d018c988537bb2822aff20f";
            
            var fileName = "CV Welly"; // Path.GetFileName(file.FileName);
            var path = @"E:\iPrepare\CV_WellyTambunan_V08.pdf";
            
            string mimeType = GetMimeMapping(fileName);

            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create("http://processing.resumeparser.com/requestprocessing.html");
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.Headers[HttpRequestHeader.Connection] = "Keep-Alive"; 
            //wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

            Stream rs = wr.GetRequestStreamAsync().Result;

            var formdata = new Dictionary<string, string>();
            formdata.Add("product_code", productCode);
            formdata.Add("document_title", fileName);

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in formdata.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, formdata[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, "document", fileName, mimeType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;

            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }

            fileStream.Dispose();

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Dispose();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponseAsync().Result;
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                var jsonResume = reader2.ReadToEnd();
                Console.WriteLine(jsonResume);
                File.WriteAllText("cv_json.json", jsonResume);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                if (wresp != null)
                {
                    wresp.Dispose();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }

            // redirect back to the index action to show the form once again
            Console.WriteLine("xml response " + xmlResponse);


        }
    }
}

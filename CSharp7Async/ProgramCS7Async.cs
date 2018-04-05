using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static System.Console;
using Microsoft.Threading;

namespace CSharp7Async
{
    enum AsyncModel
    {
        SimpleFor,
        AsyncFor,
        Parallel_For,
        Select
    }
    class Program
    {
        //static string sitemap = "https://www.aplitop.com/sitemap/pages";
        static string sitemap = "https://www.pagelr.com/sitemap.xml";
        //static string sitemap = "http://growthsupply.com/page-sitemap.xml";

        //static string[] urls = new string[] {"www.aplitop.com" };

        // TODO CS71 1.0 Async Main
        static async Task Main(string[] args)
        {
            bool enableJavaSCriptInPage = true;
            bool useAsyncPump = false;
            AsyncModel mode = AsyncModel.Select;
            if (useAsyncPump)
            {
                AsyncPump.Run(async delegate
                {
                    await DemoAsync(mode, enableJavaSCriptInPage);
                });
            }
            else
                await DemoAsync(mode, enableJavaSCriptInPage);
        }

        private static async Task DemoAsync(AsyncModel mode, bool useJavascript)
        {
            WriteLine($"Entered async Main on thread {Thread.CurrentThread.ManagedThreadId} in Mode: {mode.ToString()}");
            RemoveScreenShots();
            Stopwatch sw = Stopwatch.StartNew();
            using (HttpClient client = new HttpClient())
            {

                string[] sitemap_urls = await GetSiteMapUrls(client, sitemap);
                
                List<Task> allTasks = new List<Task>();
                switch (mode)
                {
                    case AsyncModel.SimpleFor:
                        {
                            for (int i = 0; i < sitemap_urls.Length; i++)
                            {
                                await GetScreenShotAsync(i, client, sitemap_urls, useJavascript);
                            }
                            break;
                        }
                    case AsyncModel.AsyncFor:
                        {
                            for (int i = 0; i < sitemap_urls.Length; i++)
                            {
                                Func<Task> at = async () =>
                                {
                                    await GetScreenShotAsync(i, client, sitemap_urls, useJavascript);
                                };

                                allTasks.Add(at());
                            }
                            await Task.WhenAll(allTasks);
                            break;
                        }
                    case AsyncModel.Parallel_For:
                        {

                            Parallel.For(0, sitemap_urls.Length, i =>
                            {
                                //allTasks.Add(Task.Factory.StartNew(async () =>
                                //{
                                //    await GetScreenShotAsync(i, client, sitemap_urls);
                                //}).Unwrap());
                                Func<Task> at = async () =>
                                {
                                    await GetScreenShotAsync(i, client, sitemap_urls, useJavascript);
                                };
                                allTasks.Add(at());
                            }
                            );

                            await Task.WhenAll(allTasks);
                            break;
                        }
                    case AsyncModel.Select:
                        {
                            await Task.WhenAll(sitemap_urls.Select(async (url, index) => await GetScreenShotAsync(index, client, sitemap_urls, useJavascript)));
                            break;
                        }
                }
            }
            sw.Stop();
            WriteLine($"Finishing async Main on thread {Thread.CurrentThread.ManagedThreadId} Duration {(sw.ElapsedMilliseconds / 1000f):F2} seconds ({sw.ElapsedMilliseconds} ms)");
        }

        private static void RemoveScreenShots()
        {

            var dir = new DirectoryInfo(".");

            foreach (var file in dir.EnumerateFiles("screenshot_??.png"))
            {
                file.Delete();
            }
        }

        private static async Task GetScreenShotAsync(int i, HttpClient client, string[] sitemap_urls, bool enableJavaSCriptInPage)
        {
            WriteLine($"\nStarting screenshot {i} on thread {Thread.CurrentThread.ManagedThreadId}");
            try
            {
                using (var stream = await GetScreenShot(client, sitemap_urls[i], enableJavaSCriptInPage))
                {
                    using (var fileStream = File.Create($"screenshot_{i}.png"))
                    {
                        await stream.CopyToAsync(fileStream);
                        WriteLine($"Completed screenshot {i} on thread {Thread.CurrentThread.ManagedThreadId}");
                    }
                }
            }
            catch(Exception ex)
            {
                WriteLine($"EXCEPTION {ex.GetType().Name} occured for '{sitemap_urls[i]}'\n{ex.Message}");
            }
        }

        private static async Task<string[]> GetSiteMapUrls(HttpClient client, string sitemap)
        {
            var streamTask = client.GetStringAsync(sitemap);
            var responseString = await streamTask;

            //XDocument xdoc = XDocument.Parse(responseString);

            //var locs = xdoc.Root.Descendants("loc");
            //var urls = locs.Select(u => u.Value).ToArray();


            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(responseString);

            var locElements = xmldoc.GetElementsByTagName("loc");
            var urls = locElements.Cast<XmlNode>().Select(n => n.InnerText).ToArray();
            return urls;

        }

        static async Task<Stream> GetScreenShot(HttpClient client, string url, bool useJavaScript)
        {
            var javaScriptPath = useJavaScript ? "/javascript" : "";
            var delayParam = useJavaScript ? "&delay=3000" : "";
            var streamTask = client.GetStreamAsync($"http://cloudtile.com/capture{javaScriptPath}?uri={Uri.EscapeUriString(url)}{delayParam}&b_width=1280&width=600&height=420&maxage=0");
            var responseStream = await streamTask;
            return responseStream;
        }
    }
}

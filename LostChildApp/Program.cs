using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace LostChild
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();

            //var account = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("LostChildStorageAccount", EnvironmentVariableTarget.Process));
            //var client = account.CreateCloudQueueClient();
            //var queue = client.GetQueueReference("lost-child-queue");
            //await queue.CreateIfNotExistsAsync();

            //var newMessage = "This is a test message";
            //var queueMessage = new CloudQueueMessage(newMessage);
            //await queue.AddMessageAsync(queueMessage);
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace AppApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    //.UseKestrel(options =>
                    //{
                    //    int port = 5443;
                    //    if (args.Length > 0)
                    //        int.TryParse(args[0], out port);

                    //    if (args.Length > 2)
                    //    {
                    //        options.ListenAnyIP(port, listenOptions =>
                    //        {
                    //            listenOptions.UseHttps(args[1], args[2]);
                    //        });
                    //    }
                    //    else
                    //    {
                    //        options.ListenAnyIP(port);
                    //    }
                    //});



                });
    }
}

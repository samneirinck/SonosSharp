using System.Linq;
using System.Xml.Linq;
using SonosSharp;
using System;
using System.Threading.Tasks;
using SonosSharp.Eventing;

namespace SonosConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string sonosIp = String.Empty;
            if (args.Length == 0)
            {
                Console.Write("Sonos IP > ");
                sonosIp = Console.ReadLine();
            }
            else
            {
                sonosIp = args[0];
            }

            DoLogic(sonosIp).Wait();
            
        }

        private static async Task DoLogic(string sonosIp)
        {
            var controller = new SonosController(sonosIp, new HttpServer());

            var sonosDeviceInformation = await controller.GetSonosDeviceAsync();

            await controller.EnableEventsAsync();

            Console.WriteLine("Connected to {0}", sonosDeviceInformation.RoomName);

            Console.WriteLine("Ready to take commands...{0}", Environment.NewLine);

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                if (input == null)
                    continue;

                var splitted = input.Split(' ');
                string command = splitted[0];
                string[] args = null;
                if (splitted.Length > 1)
                {
                    args = new string[splitted.Length - 1];
                    for (int i = 1; i < splitted.Length; i++)
                    {
                        args[i - 1] = splitted[i];
                    }
                }
                switch (command)
                {
                    case "stop":
                        await controller.StopAsync();
                        break;

                    case "play":
                        await controller.PlayAsync();
                        break;

                    case "volume":
                        if (args != null)
                        {
                            int desiredVolume;
                            if (!Int32.TryParse(args[0], out desiredVolume))
                            {
                                Console.WriteLine("Please specify an integer value");
                            }
                            else
                            {
                                await controller.SetVolumeAsync(desiredVolume);
                                Console.WriteLine("Volume set to {0}", desiredVolume);
                            }
                        }
                        else
                        {

                            int volume = await controller.GetVolumeAsync();
                            Console.WriteLine("Volume: " + volume);
                        }
                        break;

                    case "position":
                        var positionInfo = await controller.GetPositionInfoAsync();
                        Console.WriteLine(positionInfo.RelativeTime.ToString());

                        break;

                    case "bass":
                        Console.WriteLine("Bass: " + await controller.GetBassAsync());
                        break;
                        
                    case "mute":
                        bool muted = await controller.GetIsMutedAsync();

                        Console.WriteLine("Muted: " + (muted ? "YES" : "NO"));
                        break;

                    case "media":
                        var mediaInfo = await controller.GetMediaInfoAsync();

                        XNamespace ns = "http://purl.org/dc/elements/1.1/";
                        Console.WriteLine("Current: " + XElement.Parse(mediaInfo.CurrentUriMetaData).Descendants(ns + "title").First().Value);

                        break;

                    case "":
                        break;

                    default:
                        Console.WriteLine("Unknown command, please try again");
                        break;
                }


            }
        }
    }
}

// Надо в ссылки добавлять 
// C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Vitro.Client.Interop\v4.0_2022.1.0.2146__7d52b04b90a5a799\Vitro.Client.Interop.dll
//
// 10-07-2022 сборка заменена на 2129!
// 27-07-2022 сборка заменена на 2146!

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vitro.Client.Interop.Action;
using Vitro.Client.Interop.Service;
using Vitro.Client.Interop.Service.Impl;

namespace VitroCmd
{
    internal class Program
    {
        const string VITRO_VERSION = "2146";
        public static IClient client = null;
        public const string TEST_URN = @"vitro://vitro{d1f47bfb-a228-4711-b092-eb2ab172bbed}";

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 2)
                {
                    switch (args[0].ToLower())
                    {
                        case "-open":
                            Open(args[1]);
                            break;

                        case "-save":
                            Save(args[1]);
                            break;

                        case "-add":
                            Add(args[1]);
                            break;

                        default:
                            Help();
                            break;
                    }
                }
                else
                {
                    Help();
                }
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR {ex.Message}"); ;
                Environment.Exit(1);
            }
        }

        static Boolean Init()
        {
            IClientFactory clientFactory = new BaseClientFactoryImpl();
            client = clientFactory.Create(string.Empty);
            //ISecurityAction securityAction = client.CreateSecurityAction();
            client.CreateSecurityAction();
            return true;
        }

        static string Add(string filePath)
        {
            if (!(System.IO.File.Exists(filePath)))
            {
                throw new Exception($"file not exists {filePath}");
            }
            Init();
            ILibraryAction libraryAction = client.CreateLibraryAction();
            string localPath = libraryAction.Add(filePath);
            //string s1 = libraryAction.  e(localPath);

            ILibraryItemUrnAction urnLibraryAction = client.CreateLibraryItemUrnAction();
            string urn = urnLibraryAction.CreateUrnByPath(localPath);    
            Console.WriteLine(urn);
            return localPath;
        }

        static string Open(string urn)
        {
            string filePath = "";
            Init();
            // получаем инстанцию сервиса который отвечает за работу с Urn
            ILibraryItemUrnAction libraryItemUrnAction = client.CreateLibraryItemUrnAction();
            // получаем в локальный пользовательский кэш витро файл по его urn - параметр на вход идет
            filePath = libraryItemUrnAction.CheckOut(urn);
            
            Console.WriteLine(filePath);
            return filePath;
        }

        static bool Save(string urn)
        {
            bool result = false;
            Init();
            ILibraryItemUrnAction libraryItemUrnAction = client.CreateLibraryItemUrnAction();
            string res = libraryItemUrnAction.CheckIn(urn, $"VitroCmd \thost ={Environment.MachineName}");
            Console.WriteLine($"SAVE {urn}");
            return result;
        }

        static string GetExeFileName()
        {
            return System.IO.Path.GetFileName(Environment.GetCommandLineArgs()[0]);
        }

        static void Help()
        {
            ColorConsole.WriteWrappedHeader("Vitro command line tool by Spectrum (с) ", headerColor: ConsoleColor.Green);
            //ColorConsole.WriteEmbeddedColorLine("[yellow]Vitro command line tool by Spectrum (с)[/yellow]");
            //Console.WriteLine("Vitro command line tool by Spectrum (с) ");
            Console.WriteLine($"\tдля Витро клиента v{VITRO_VERSION}");
            Console.WriteLine();
            ColorConsole.WriteLine($"{GetExeFileName()} [-open |-save |-add] [\"urn\"|\"filename\"] ", ConsoleColor.Yellow);
            Console.WriteLine();
            ColorConsole.WritelnColor("\t[yellow]-add[/yellow] добавляет локальный файл в Vitro и возвращает его URN, filename=путь к добавляемому файлу");
            Console.WriteLine($"\t\t{GetExeFileName()} -add \"" + @"c:\tmp\file.txt" + "\"");
            ColorConsole.WritelnColor("\t[yellow]-open[/yellow] открывает файл из Vitro и возвращает локальный путь к нему");
            Console.WriteLine("\t\tВАЖНО - файл становится извлечён в Vitro. Не забудьте вернуть");
            Console.WriteLine($"\t\t{GetExeFileName()} -open \"{TEST_URN}\"");
            Console.WriteLine();
            ColorConsole.WritelnColor("\t[yellow]-save[/yellow] сохраняет файл в Vitro и возвращает локальный путь к нему");
            Console.WriteLine($"\t\t{GetExeFileName()} -save \"{TEST_URN}\"");
            Console.WriteLine();
            Console.WriteLine($"\t urn - URN из Vitro в кавычках, например \"{TEST_URN}\"");
        }
    }
}

//
// VitroCmd.exe -open "vitro://vitro{40c3836f-e717-489d-b53d-f9a4c56c880f}"
//
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
        public static IClient client = null;
        public const string TEST_URN = @"vitro://vitro{40c3836f-e717-489d-b53d-f9a4c56c880f}";

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
            Init();
            ILibraryAction libraryAction = client.CreateLibraryAction();
            string localPath = libraryAction.Add(filePath);
            //string s1 = libraryAction.  e(localPath);
            Console.WriteLine(localPath);
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
            Console.WriteLine("Vitro command line tool by Spectrum (с) ");
            Console.WriteLine("");
            Console.WriteLine($"{GetExeFileName()} [-open | -save] \"urn\" ");
            Console.WriteLine("\t-add добавляет флокальный файл в Vitro и возвращает его имя, urn=путь к добавляемому файлу");
            Console.WriteLine("\t-open открывает файл из Vitro и возвращает его имя");
            Console.WriteLine("\t-save сохраняет файл в Vitro и возвращает его имя");
            Console.WriteLine($"\t urn - URN из Vitro в кавычках, например {TEST_URN}");
        }
    }
}

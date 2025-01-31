// Alkalmazasfejlesztes C# alapokon a modern fejlesztesi iranyelvek bemutatasaval kurzus Kotelezo program
// Felegyi Mihaly Patrik
// 2022. aprilis-majus
using Microsoft.AspNetCore.Hosting; // Tipusokkal szolgal, amik segitenek konfiguralni es elinditani web alkalmazasokat (Peldaul: WebHostBuilder es annak UseStartup metodusa)
using Microsoft.Extensions.Hosting; // Osztalyokkal es interfeszekkel szolgal, amik segitenek konfiguralni es elinditani web alkalmazasokat (Peldaul: Host osztaly, IHostBuilder interfesz)

using System; // serializaciohoz namespace-ek
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;

using System.Linq; // Linq-hoz namespace

using System.Threading.Tasks; // async es Task-hoz namespace

namespace Todo
{
    // Generikushoz teszt class
    class Generikus<T> where T : struct
    {
        private T valtozo;

        //a konstruktor privat jelen esetben, mivel
        //a konstruktorok nem lehetnek generikusak!
        private Generikus() { }

        public static Generikus<T> Letrehoz(T parameter)
        {
            Generikus<T> vissza = new Generikus<T>();
            vissza.valtozo = parameter;
            return vissza;
        }

        public override string ToString()
        {
            return string.Format("valtozo tarolt tipusa: {0}, Erteke: {1}",
                                  valtozo.GetType().Name, valtozo);
        }
    }

    // Linq-hoz teszt class
    public class User
    {
        public string UserName { get; set; }
        public int TeendoCount { get; set; }
    }

    // Linq-hoz teszt class meg mindig
    public static class UserExtensions
    {
        public static bool IsNoob(this User user)
        {
            if (user.TeendoCount < 50)
                return true;
            return false;
        }
    }

    // Szerializaciohoz teszt class
    public class Teendo
    {
        [XmlAttribute("teendo")]
        public string teendo { get; set; }

        [XmlElement("Hiba")]
        public string Name { get; set; }
    }

    public class Program
    {
        // Az app belepesi pontja
        public static void Main(string[] args)
        {
            // Szerializacio KEZDETE
            #region szerializacio
            try
            {
                using (var file = File.CreateText("valami.txt"))
                {
                    file.WriteLine("Hello File kezeles!");
                }
            }
            catch (IOException ex)
            {
                //IO eset�n mindig kell!
                Console.WriteLine(ex.ToString());
            }
            List<Teendo> list = new List<Teendo>()
            {
                new Teendo
                {
                    teendo = "Ez",
                    Name = "Foo"
                },
                new Teendo
                {
                    teendo = "Az",
                    Name = "M�sik"
                },
                new Teendo
                {
                    teendo = "Amaz",
                    Name = "Asd"
                },
            };

            try
            {
                string jsonEncoded = JsonSerializer.Serialize(list, new JsonSerializerOptions
                {
                    WriteIndented = true,
                });
                File.WriteAllText("jsonExample.json", jsonEncoded);

                string vissza = File.ReadAllText("jsonExample.json");
                List<Teendo> pVissza = JsonSerializer.Deserialize<List<Teendo>>(vissza);
            }
            catch (Exception ex)
                when (ex is IOException || ex is JsonException)
            {
                Console.WriteLine(ex.ToString());
            }

            XmlSerializer xs = new XmlSerializer(typeof(List<Teendo>));
            using (var f = File.Create("test.xml"))
            {
                xs.Serialize(f, list);
            }

            using (var f = File.OpenRead("test.xml"))
            {
                List<Teendo> vissza = xs.Deserialize(f) as List<Teendo>;
            }
            #endregion
            // Szerializacio VEGE

            // Linq KEZDETE
            #region Linq
            List<User> Users = new List<User>
            {
            new User
            {
                UserName = "Alice",
                TeendoCount = 120,
            },
            new User
            {
                UserName = "Bob",
                TeendoCount = 30,
            },
            new User
            {
                UserName = "Istv�n",
                TeendoCount = 69,
            },
            };

            var bela = new User
            {
                UserName = "B�la",
                TeendoCount = 51,
            };

            bela.IsNoob();

            //LINQ - Language integrated Query
            //lambda syntax
            var max = Users.Max(user => user.TeendoCount);

            var Noobs = from user in Users
                        where user.IsNoob()
                        orderby user.TeendoCount descending
                        select user.UserName;

            foreach (var user in Noobs.Take(2))
            {
                Console.WriteLine(user);
            }
            #endregion
            // Linq VEGE

            // Generikus KEZDETE
            #region generikus
            var teszt1 = Generikus<int>.Letrehoz(22);
            var teszt2 = Generikus<double>.Letrehoz(33.2);
            var teszt3 = Generikus<char>.Letrehoz('A');
            //az alabbi hibat fog dobni, mert a string osztaly!
            //Generikus<string> teszt4 = Generikus.Letrehoz("Teszt");
            #endregion
            // Generikus VEGE

            // Aszinkron resz KEZDETE
            #region aszinkron
            // A task magja �gy egy async Action delegate.
            Task.Run(async () =>
             {
                 Console.WriteLine("Async elott");
                 await PrintCurrentTimeAsync();
                 Console.WriteLine("Async utan");
            });
            #endregion
            // Aszinkron resz VEGE

            CreateHostBuilder(args).Build().Run();
        }

        // Aszinkron Task method
        private static async Task PrintCurrentTimeAsync()
        {
            Console.WriteLine(DateTime.Now);
            await Task.Delay(2000);
            Console.WriteLine(DateTime.Now);
        }

        // record (immutable type) KEZDETE
        #region record
        private record NapiTeendo(string micsoda);
        private static NapiTeendo[] data = new NapiTeendo[]
        {
            new NapiTeendo("Ezt meg meg kell csinalni"),
            new NapiTeendo("Ezt is meg kell csinalni")
        };
        #endregion
        // record (immutable type) VEGE

        // Startup peldanyositasa
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

using System;
using Otus.Task1.Models;
namespace Otus.Task1
{
    class Program
    {


        static SaveFile Generate1()
        {
            var res = new SaveFile();
            res.Coords = (1241.44, 124145.4);
            res.CurrentLocation = "Dungeon";
            res.User = new User { Level = 10, Name = "Пушкин" };
            res.Items = new[] { new Item() { Name = "Топор", Quantity = 2 } };

            return res;
        }

        static void SerializeBinary(SaveFile sf){

        }

        
        static void SerializeJson(SaveFile sf){

        }


        static void SerializeXml(SaveFile sf){

        }

        static SaveFile Generate2()
        {
            var res = new SaveFile();
            res.Coords = (121.44, 124.4);
            res.CurrentLocation = "Subway";
            res.User = new User { Level = 10, Name = "Feodorov" };
            res.Items = new[] { new Item() { Name = "Stick", Quantity = -2 } };

            return res;
        }

        static void Main(string[] args)
        {
            var g1 = Generate1();
            var g2 = Generate2();

            SerializeBinary(g1);
            SerializeJson(g1);
            SerializeXml(g1);

            
            SerializeBinary(g2);
            SerializeJson(g2);
            SerializeXml(g2);
        }
    }
}

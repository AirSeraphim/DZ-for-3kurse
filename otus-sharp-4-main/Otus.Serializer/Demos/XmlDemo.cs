using System;
using System.IO;
using System.Xml.Serialization;

namespace Otus.Serializer
{

    public enum ShopType
    {
        Private = 1,
        Public = 2
    }
    [XmlRoot("ShopShop")]
    public class AutoShop
    {
        public ShopType ShopType { get; set; }

        [XmlArray("MyBeautifulCars")]
        public Car[] Cars { get; set; }
    }


    [XmlRoot("TestCar")]
    public class Car
    {
        [XmlAttribute("BarColor")]
        public string Color { get; set; }

        [XmlElement("New Name")]
        public string Name { get; set; }

        [XmlIgnore]
        public int Price { get; set; }
    }

    public class XmlDemo
    {
        public void Show()
        {
            var car = new Car { Name = "LADA", Color = "Red", Price = 222 };
            var car1 = new Car { Name = "Ниссан", Color = "Blue", Price = 222 };
            var shop = new AutoShop { Cars = new[] { car, car1 }, ShopType = ShopType.Public };


            var s = new XmlSerializer(typeof(AutoShop));


            using (var fs = new FileStream("demoDataFiles\\demo.xml", FileMode.Create))
            {
                s.Serialize(fs, shop);
            }
        }
    }
}
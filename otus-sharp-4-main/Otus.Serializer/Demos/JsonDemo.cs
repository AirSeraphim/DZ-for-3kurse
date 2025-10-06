using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Otus.Serializer
{


    public class MyConverter : JsonConverter<int>
    {
        public override int Read(
            ref Utf8JsonReader reader,
             System.Type typeToConvert,
              JsonSerializerOptions options)
        {
            var s = reader.GetString();

            return int.Parse(s.Replace("fancy_number_", ""));

        }

        public override void Write(
            Utf8JsonWriter writer,
             int value,
              JsonSerializerOptions options)
        {
            writer.WriteStringValue($"fancy_number_{value}");                                                                                                    //JsonStringEnumConverter 
        }
    }

    enum Status
    {
        Hellp = 1,

        Model2 = 2
    }
    class Library
    {
        public Status Status { get; set; }
        public string Name { get; set; }

        [JsonPropertyName("book_collection")]
        public IEnumerable<Book> Books { get; set; }

    }



    class Book
    {
        public Book(string name)
        {
            Name = name;
            ShouldInclude = "TRUE";
        }
        public string Name { get; set; }

        public string ShouldInclude;


        public IEnumerable<Page> Pages { get; set; }
    }

    class Page
    {
        public Page(int num)
        {
            Number = num;
        }
        

        [JsonConverter(typeof(MyConverter))]
        public int Number { get; set; }
    }

    public class JsonDemo
    {
        public void Show()
        {

            var pages = new[] { new Page(1), new Page(2), new Page(3) };
            var books = new[] { new Book("Война и мир") { Pages = pages } };

            var opt = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IncludeFields = true,

            };


            var library = new Library
            {
                Status = Status.Hellp,
                Name = "им. Ленина",
                Books = books
            };
            var s = JsonSerializer.Serialize(library, opt);
            File.WriteAllText("demoDataFiles\\file.json", s);
        }
    }
}
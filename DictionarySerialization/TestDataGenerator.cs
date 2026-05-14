using System.Text.Json;
using System.Text.Json.Serialization;
using Bogus;

namespace DictionarySerialization;

public class TestDataGenerator
{
    public static Dictionary<string, object> GenerateTestData()
    {
        var faker = new Faker();
        var guidId = Guid.NewGuid();

        // 生成 SimpleObject
        var simpleObject = new SimpleObject
        {
            Id = Guid.NewGuid(),
            Name = faker.Person.FullName,
            Email = faker.Internet.Email(),
            IsActive = faker.Random.Bool()
        };

        // 生成 List<string>
        var stringList = new List<string>
        {
            faker.Random.Word(),
            faker.Random.Word(),
            faker.Random.Word(),
            faker.Random.Word(),
            faker.Random.Word()
        };

        // 生成 List<ComplexObject>
        var complexObjectList = new List<ComplexObject>();
        for (int i = 0; i < 5; i++)
        {
            complexObjectList.Add(new ComplexObject
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Tags = new List<string>
                {
                    faker.Random.Word(),
                    faker.Random.Word()
                },
                Metadata = new Dictionary<string, object>
                {
                    { "created", DateTime.UtcNow },
                    { "version", faker.Random.Int(1, 10) }
                }
            });
        }

        return new Dictionary<string, object>
        {
            // 情境 1: Guid
            { "propertyA", guidId },
            // 情境 2: Object (SimpleObject)
            { "propertyB", simpleObject },
            // 情境 3: List<string>
            { "propertyC", stringList },
            // 情境 4: List<ComplexObject>
            { "propertyD", complexObjectList }
        };
    }

}
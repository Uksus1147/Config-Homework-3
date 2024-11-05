using NUnit.Framework;
using System.IO;
using System.Xml;

[TestFixture]
public class ConfigTranslatorTests
{
    private ConfigTranslator translator;

    [SetUp]
    public void Setup()
    {
        translator = new ConfigTranslator();
    }

    private string RunTranslation(string xmlContent)
    {
        using (StringReader stringReader = new StringReader(xmlContent))
        using (XmlReader xmlReader = XmlReader.Create(stringReader))
        {
            using (StringWriter stringWriter = new StringWriter())
            {
                Console.SetOut(stringWriter); // Перенаправляем вывод в строку
                translator.Translate(xmlReader);
                return stringWriter.ToString();
            }
        }
    }

    [Test]
    public void Translate_ShouldHandleConstant()
    {
        string xml = @"
        <config>
            <constant name='ServerPort'>8080</constant>
        </config>";

        string result = RunTranslation(xml).TrimEnd();
        Assert.AreEqual(NormalizeLineEndings("ServerPort = 8080\n\n").TrimEnd(), NormalizeLineEndings(result));
    }

    [Test]
    public void Translate_ShouldHandleDictionaryWithEntries()
    {
        string xml = @"
        <config>
            <dictionary name='DatabaseConfig'>
                <entry name='host'>""localhost""</entry>
                <entry name='port'>5432</entry>
            </dictionary>
        </config>";

        string result = RunTranslation(xml).TrimEnd();
        string expected = "DatabaseConfig = {\n    host = \"localhost\"\n    port = 5432\n}\n";
        Assert.AreEqual(NormalizeLineEndings(expected).TrimEnd(), NormalizeLineEndings(result));
    }

    [Test]
    public void Translate_ShouldHandleNestedElements()
    {
        string xml = @"
        <config>
            <constant name='AppName'>""MyApp""</constant>
            <dictionary name='UserSettings'>
                <entry name='theme'>""dark""</entry>
                <entry name='notifications'>true</entry>
            </dictionary>
        </config>";

        string result = RunTranslation(xml).TrimEnd();
        string expected = "AppName = \"MyApp\"\nUserSettings = {\n    theme = \"dark\"\n    notifications = true\n}\n";
        Assert.AreEqual(NormalizeLineEndings(expected).TrimEnd(), NormalizeLineEndings(result));
    }




    private string NormalizeLineEndings(string input)
    {
        return input.Replace("\r\n", "\n").Replace("\r", "\n");
    }

}

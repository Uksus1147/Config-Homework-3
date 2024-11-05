//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Xml;
//using System.Text.RegularExpressions;

//public class ConfigTranslator
//{
//    private Dictionary<string, string> constants = new Dictionary<string, string>();

//    static void Main()
//    {
//        string filePath = "input.xml"; // Убедитесь, что файл находится в этом пути

//        using (XmlReader reader = XmlReader.Create(filePath))
//        {
//            ConfigTranslator translator = new ConfigTranslator();
//            translator.Translate(reader);
//        }

//        Console.WriteLine("Нажмите любую клавишу для выхода...");
//        Console.ReadKey();
//    }

//    public void Translate(XmlReader reader)
//    {
//        var output = new StringBuilder();

//        while (reader.Read())
//        {
//            if (reader.NodeType == XmlNodeType.Element)
//            {
//                if (reader.Name == "constant")
//                {
//                    string name = reader.GetAttribute("name");
//                    reader.Read(); // Переход к значению
//                    string value = reader.Value.Trim();
//                    output.AppendLine($"{name} = {value}");
//                }
//                else if (reader.Name == "dictionary")
//                {
//                    string dictionaryName = reader.GetAttribute("name");
//                    output.AppendLine($"{dictionaryName} = {{");

//                    while (reader.Read() && !(reader.NodeType == XmlNodeType.EndElement && reader.Name == "dictionary"))
//                    {
//                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "entry")
//                        {
//                            string entryName = reader.GetAttribute("name");
//                            reader.Read(); // Переход к значению
//                            string entryValue = reader.Value.Trim();
//                            output.AppendLine($"    {entryName} = {entryValue}");
//                        }
//                    }

//                    output.AppendLine("}");
//                }
//            }
//        }

//        Console.WriteLine(output.ToString());
//    }



//    private string ParseValue(string value)
//    {
//        // Обработка вычислений констант, чисел, строк и словарей
//        if (Regex.IsMatch(value, @"^\d+$")) return value; // Число
//        if (value.StartsWith("\"") && value.EndsWith("\"")) return value; // Строка

//        // Вычисление константы
//        if (value.StartsWith("$(") && value.EndsWith(")"))
//        {
//            var constantName = value.Substring(2, value.Length - 3);
//            return constants.ContainsKey(constantName) ? constants[constantName] : throw new Exception($"Константа {constantName} не определена.");
//        }

//        return value;
//    }
//}
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

public class ConfigTranslator
{
    private Dictionary<string, string> constants = new Dictionary<string, string>();

    static void Main()
    {
        string filePath = "input.xml"; // Убедитесь, что файл находится в этом пути

        using (XmlReader reader = XmlReader.Create(filePath))
        {
            ConfigTranslator translator = new ConfigTranslator();
            translator.Translate(reader);
        }

        Console.WriteLine("Нажмите любую клавишу для выхода...");
        Console.ReadKey();
    }

    public void Translate(XmlReader reader)
    {
        var output = new StringBuilder();

        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                if (reader.Name == "constant")
                {
                    string name = reader.GetAttribute("name");
                    reader.Read(); // Переход к значению
                    string value = reader.Value.Trim();
                    constants[name] = value; // Сохраняем константу для подстановки
                    output.AppendLine($"{name} = {value}");
                }
                else if (reader.Name == "dictionary")
                {
                    string dictionaryName = reader.GetAttribute("name");
                    output.AppendLine($"{dictionaryName} = {{");

                    while (reader.Read() && !(reader.NodeType == XmlNodeType.EndElement && reader.Name == "dictionary"))
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "entry")
                        {
                            string entryName = reader.GetAttribute("name");
                            reader.Read(); // Переход к значению
                            string entryValue = reader.Value.Trim();

                            // Если entryValue имеет формат 'name = value', сохраняем его как константу
                            var match = Regex.Match(entryValue, @"^(\w+)\s*=\s*(.*)$");
                            if (match.Success)
                            {
                                string constantName = match.Groups[1].Value;
                                string constantValue = match.Groups[2].Value;
                                constants[constantName] = constantValue;
                                output.AppendLine($"    {entryName} = {constantValue}");
                            }
                            else
                            {
                                // Подставляем значение, если это подстановка, или выводим как есть
                                output.AppendLine($"    {entryName} = {ParseValue(entryValue)}");
                            }
                        }
                    }

                    output.AppendLine("}");
                }
            }
        }

        Console.WriteLine(output.ToString());
    }

    private string ParseValue(string value)
    {
        // Обработка вычислений констант, чисел, строк и словарей
        if (Regex.IsMatch(value, @"^\d+$")) return value; // Число
        if (value.StartsWith("\"") && value.EndsWith("\"")) return value; // Строка

        // Вычисление константы
        if (value.StartsWith("$(") && value.EndsWith(")"))
        {
            var constantName = value.Substring(2, value.Length - 3);
            return constants.ContainsKey(constantName) ? constants[constantName] : throw new Exception($"Константа {constantName} не определена.");
        }

        return value;
    }
}

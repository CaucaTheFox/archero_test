using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Utility
{
    public static class CsvParser
    {
        private static readonly CultureInfo Ci = CultureInfo.InvariantCulture;

        public static List<A> Parse<A>(string input, bool includesHeader, char separator) where A : new()
        {
            var fieldInfos = typeof(A).GetFields();

            var result = Process(input, includesHeader, separator)
                .Select(line => {
                    var a = new A();
                    for (var i = 0; i < fieldInfos.Length; i++) {
                        fieldInfos[i].SetValue(a, ParseIntoValue(fieldInfos[i].FieldType, line[i]));
                    }

                    return a;
                })
                .ToList();

            return result;
        }

        public static List<List<string>> Process(string input, bool includesHeader, char separator) =>
            input
                .Split(new[] {"\n", "\r\n"}, StringSplitOptions.RemoveEmptyEntries)
                .Skip(includesHeader ? 1 : 0)
                .Select(s => s.Split(separator).ToList())
                .ToList();

        public static void WriteToFile<A>(List<A> input, string path)
        {
            var result = SerializeCsv(input);
            File.WriteAllText(path, result);
        }

        public static string SerializeCsv<A>(List<A> input)
        {
            var builder = new StringBuilder();
            var fieldInfos = typeof(A).GetFields();

            foreach (var line in input) {
                for (var i = 0; i < fieldInfos.Length; i++) {
                    var field = fieldInfos[i].GetValue(line);
                    if (field.GetType().IsEnum) {
                        builder.Append((int) field);
                    }
                    else
                        switch (field) {
                            case float f:
                                builder.Append(f.ToString(Ci));
                                break;
                            case double d:
                                builder.Append(d.ToString(Ci));
                                break;
                            case decimal d:
                                builder.Append(d.ToString(Ci));
                                break;
                            default:
                                builder.Append(field);
                                break;
                        }

                    if (i < fieldInfos.Length - 1) {
                        builder.Append(',');
                    }
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }

        public static object ParseIntoValue(Type type, string operand)
        {
            if (type == typeof(string)) return operand;
            if (type == typeof(int)) return operand.AsInt();
            if (type == typeof(uint)) return operand.AsUInt();
            if (type == typeof(long)) return operand.AsLong();
            if (type == typeof(float)) return operand.AsFloat();
            if (type == typeof(double)) return operand.AsDouble();
            if (type == typeof(decimal)) return operand.AsDecimal();
            return type.IsEnum ? operand.AsEnum(type) : null;
        }

        private static int AsInt(this string input)
        {
            if (int.TryParse(input, out var result)) {
                return result;
            }
            
            throw new ArgumentException($"Can't parse {input} as int.");
        }
        
        private static uint AsUInt(this string input)
        {
            if (uint.TryParse(input, out var result)) {
                return result;
            }
            
            throw new ArgumentException($"Can't parse {input} as uint.");
        }

        private static long AsLong(this string input)
        {
            if (long.TryParse(input, out var result)) {
                return result;
            }
            
            throw new ArgumentException($"Can't parse {input} as long.");
        }

        private static float AsFloat(this string input)
        {
            if (float.TryParse(input, NumberStyles.Float, Ci, out var result)) {
                return result;
            }

            throw new ArgumentException($"Can't parse {input} as float");
        }

        private static double AsDouble(this string input)
        {
            if (double.TryParse(input, NumberStyles.Number, Ci, out var result)) {
                return result;
            }

            throw new ArgumentException($"Can't parse {input} as double");
        }

        private static decimal AsDecimal(this string input)
        {
            if (decimal.TryParse(input, NumberStyles.Number, Ci, out var result)) {
                return result;
            }

            throw new ArgumentException($"Can't parse {input} as decimal");
        }

        private static object AsEnum(this string input, Type type)
        {
            if (
                Enum.IsDefined(type, input) ||
                int.TryParse(input, out var intResult) && Enum.IsDefined(type, intResult)
            ) {
                return Enum.Parse(type, input);
            }

            throw new ArgumentException($"Can't parse {input} as {type.Name}"); 
        }
    }
}
using CsvHelper;
using SpanishPoint.Azure.Iswc.Api.Tests.Documentation.GenerateDocs.Extensions;
using SpanishPoint.Azure.Iswc.Api.Tests.Documentation.GenerateDocs.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace SpanishPoint.Azure.Iswc.Api.Tests.Documentation.GenerateDocs
{
    [ExcludeFromCodeCoverage]
    class Program
    {
        private static void Main()
        {
            var dir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.Parent;
            var resultFile = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "Docs\\CISAC_Unit_Tests.csv");
            XmlSerializer serializer = new XmlSerializer(typeof(Doc));

            IList<Doc> docs = new List<Doc>();

            foreach (var file in Directory.GetFiles(dir.FullName, "*Validator*.xml", SearchOption.AllDirectories))
            {
                using var stream = new StreamReader(file);
                docs.Add((Doc)serializer.Deserialize(stream));
            }

            IList<CsvRecord> records = new List<CsvRecord>();
            foreach (var doc in docs.DistinctBy(x => x.Assembly.Name))
            {
                foreach (var rule in doc.Members.Member.Where(x => x.Name.StartsWith('M')))
                {
                    records.Add(new CsvRecord
                    {
                        Component = doc.Assembly.Name.GetStringAtIdx(4),
                        Rule = rule.Name.GetStringAtIdx(7).TrimEnd("Tests"),
                        Test = rule.Name.GetStringAtIdx(8),
                        Description = rule.Summary.Trim()
                    });
                }
            }

            using var writer = new StreamWriter(resultFile);
            using var csv = new CsvWriter(writer);
            csv.WriteRecords(records);
        }
    }
}

using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;
using System.Linq;

namespace IS_Lab1_XML
{
    internal class XMLReadWithXLSTDOM
    {
        internal static void Read(string xmlpath)
        {
            XPathDocument document = new XPathDocument(xmlpath);
            XPathNavigator navigator = document.CreateNavigator();
            XmlNamespaceManager manager = new XmlNamespaceManager(navigator.NameTable);
            manager.AddNamespace("x", "http://rejestrymedyczne.ezdrowie.gov.pl/rpl/eksport-danych-v1.0");

            XPathExpression query = navigator.Compile("/x:produktyLecznicze/x:produktLeczniczy[@postac='Krem']");
            query.SetContext(manager);
            int count = navigator.Select(query).Count;
            Console.WriteLine("Liczba produktów leczniczych w postaci kremu: {0}", count);

            HashSet<string> uniqueEntities = new HashSet<string>();

            XPathExpression entityQuery = navigator.Compile("/x:produktyLecznicze/x:produktLeczniczy[@postac='Krem']/@podmiotOdpowiedzialny");
            entityQuery.SetContext(manager);
            XPathNodeIterator entityIterator = navigator.Select(entityQuery);

            while (entityIterator.MoveNext())
            {
                string entityName = entityIterator.Current.Value;
                uniqueEntities.Add(entityName);
            }

            Dictionary<string, int> entityCounts = new Dictionary<string, int>();

            foreach (string entity in uniqueEntities)
            {
                int entityCount = navigator.Select("/x:produktyLecznicze/x:produktLeczniczy[@postac='Krem' and @podmiotOdpowiedzialny='" + entity + "']", manager).Count;
                entityCounts.Add(entity, entityCount);
            }

            var topThreeEntities = entityCounts.OrderByDescending(e => e.Value).Take(3);

            Console.WriteLine("\nTop trzech podmiotów produkujących najwięcej kremów:");
            foreach (var entity in topThreeEntities)
            {
                Console.WriteLine("Nazwa podmiotu : {0}\tliczba produktów: {1}", entity.Key, entity.Value);
            }
        }
    }
}
using System;
using System.Xml;
using System.Collections.Generic;
namespace IS_Lab1_XML
{
    internal class XMLReadWithSAXApproach
    {
        private static int count;
        private static string targetPostac;
        private static string targetSubstancjaCzynna;

        internal static void Read(string xmlpath)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;

            // Odczyt zawartości dokumentu
            using (XmlReader reader = XmlReader.Create(xmlpath, settings))
            {
                // Resetowanie zmiennych
                count = 0;
                targetPostac = "Krem";
                targetSubstancjaCzynna = "Mometasoni furoas";

                // Wczytywanie danych z XML za pomocą SAX
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "produktLeczniczy")
                    {
                        string postac = reader.GetAttribute("postac");
                        string nazwaPowszechnieStosowana = reader.GetAttribute("nazwaPowszechnieStosowana");

                        // Zliczanie produktów leczniczych w postaci kremu, których jedyną substancją czynną jest Mometasoni furoas
                        if (postac == targetPostac && nazwaPowszechnieStosowana == targetSubstancjaCzynna)
                            count++;
                    }
                }
            }

            Console.WriteLine("Liczba produktów leczniczych w postaci {0}, których jedyną substancją czynną jest {1}: {2}", targetPostac, targetSubstancjaCzynna, count);

            XmlDocument doc = new XmlDocument();
            doc.Load(xmlpath);

            Dictionary<string, HashSet<string>> nazwaPostacMap = new Dictionary<string, HashSet<string>>();
            Dictionary<string, int> producentKremow = new Dictionary<string, int>();
            Dictionary<string, int> producentTabletek = new Dictionary<string, int>();

            var drugs = doc.GetElementsByTagName("produktLeczniczy");
            foreach (XmlNode d in drugs)
            {
                string nazwaPowszechnieStosowana = d.Attributes.GetNamedItem("nazwaPowszechnieStosowana").Value;
                string postac = d.Attributes.GetNamedItem("postac").Value;
                string producent = GetProducent(d);

                // Zliczanie preparatów leczniczych o takiej samej nazwie powszechnej, pod różnymi postaciami
                if (!nazwaPostacMap.ContainsKey(nazwaPowszechnieStosowana))
                {
                    nazwaPostacMap[nazwaPowszechnieStosowana] = new HashSet<string>();
                }
                nazwaPostacMap[nazwaPowszechnieStosowana].Add(postac);

                // Zliczanie kremów i tabletek dla każdego podmiotu producenta
                if (postac == "Krem")
                {
                    if (!producentKremow.ContainsKey(producent))
                    {
                        producentKremow[producent] = 0;
                    }
                    producentKremow[producent]++;
                }
                else if (postac == "Tabletka")
                {
                    if (!producentTabletek.ContainsKey(producent))
                    {
                        producentTabletek[producent] = 0;
                    }
                    producentTabletek[producent]++;
                }
            }

            Console.WriteLine("Liczba preparatów leczniczych o takiej samej nazwie powszechnej, pod różnymi postaciami:");
            foreach (var entry in nazwaPostacMap)
            {
                string nazwa = entry.Key;
                int liczbaPostaci = entry.Value.Count;
                Console.WriteLine("Nazwa: {0}, Liczba postaci: {1}", nazwa, liczbaPostaci);
            }

            Console.WriteLine("Podmioty producentujące najwięcej kremów:");
            PrintMaxProducent(producentKremow);

            Console.WriteLine("Podmioty producentujące najwięcej tabletek:");
            PrintMaxProducent(producentTabletek);
        }

        private static string GetProducent(XmlNode drug)
        {
            XmlNode podmiotNode = drug.SelectSingleNode("podmiotOdpowiedzialny");
            if (podmiotNode != null)
            {
                XmlNode nazwaNode = podmiotNode.SelectSingleNode("nazwa");
                if (nazwaNode != null)
                {
                    return nazwaNode.InnerText;
                }
            }
            return string.Empty;
        }

        private static void PrintMaxProducent(Dictionary<string, int> producentDict)
        {
            int maxCount = 0;
            List<string> maxProducenti = new List<string>();

            foreach (var entry in producentDict)
            {
                string producent = entry.Key;
                int count = entry.Value;

                if (count > maxCount)
                {
                    maxCount = count;
                    maxProducenti.Clear();
                    maxProducenti.Add(producent);
                }
                else if (count == maxCount)
                {
                    maxProducenti.Add(producent);
                }
            }

            foreach (string producent in maxProducenti)
            {
                Console.WriteLine("Producent: {0}, Liczba: {1}", producent, maxCount);
            }
        }
    }
}

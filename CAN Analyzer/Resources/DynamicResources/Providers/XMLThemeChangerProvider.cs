using DynamicResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CAN_Analyzer.Resources.DynamicResources
{
    internal class XMLThemeChangerProvider : BaseProvider<ThemeCultureInfo>
    {
        public XMLThemeChangerProvider(ThemeCultureInfo culture = null)
        {
            //если ничего не передано
            if (culture == null)
                culture = Cultures.ElementAt(0);

            //если вдруг передана ссылка на културу из списка
            if (!Cultures.Contains(culture))
            {
                Init(culture.Name);
            }
            else
                CurrentCulture = culture;
        }
        public XMLThemeChangerProvider(string language)
        {
            Init(language);
        }
        private void Init(string language)
        {
            language = language.ToLower();

            //ищем полное совпадение
            ThemeCultureInfo selectedCulture = Cultures.FirstOrDefault(x => x.Name.ToLower() == language);
            if (selectedCulture != null)
            {
                CurrentCulture = selectedCulture;
                return;
            }

            //если не находим полное совпадение культуры, то ищем совпадение с более общей культурой
            if (language.IndexOf('-') != -1)
                language = language.Remove(language.IndexOf('-'));

            selectedCulture = Cultures.FirstOrDefault(x => x.Name.ToLower() == language);
            if (selectedCulture != null)
                CurrentCulture = selectedCulture;
            else
                //если не нашли ни одного совпадение - назначаем дефолтную культуру(первую)
                CurrentCulture = Cultures.First();
        }

        private IEnumerable<ThemeCultureInfo> _cultures;
        private XElement _languages = XDocument.Load("1.xml").Element("Recordings");

        public override object GetResource(string key)
        {
            IEnumerable<XElement> record = _languages.Elements("Record").Where(x => x.Attributes("name").ElementAt(0).Value == key);
            return record.Count() > 0 ? record.ElementAt(0).Element(CurrentCulture.Name)?.Value ?? "not declared culture" : "null string";
        }

        public override IEnumerable<ThemeCultureInfo> Cultures => _cultures ?? (_cultures = new List<ThemeCultureInfo>
        {
            new ThemeCultureInfo("dark"),
            new ThemeCultureInfo("white")
        });
    }
}

using DynamicResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CANAnalyzer.Resources.DynamicResources
{

    internal class XMLLanguageChangerProvider : BaseProvider<LanguageCultureInfo>
    {

        private XMLLanguageChangerProvider(string file)
        {
            _languages = XDocument.Load(file).Element("Languages").Element("Recordings");
            var cultures = new List<LanguageCultureInfo>();

            foreach (var el in XDocument.Load(file).Element("Languages").Element("Cultures").Elements("Culture"))
            {
                cultures.Add(new LanguageCultureInfo(el.Value));
            }

            _cultures = cultures;

        }

        public XMLLanguageChangerProvider(string file, LanguageCultureInfo culture) : this(file)
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

        public XMLLanguageChangerProvider(string file, string theme) : this(file)
        {
            Init(theme);
        }

        private void Init(string language)
        {
            language = language.ToLower();

            //ищем полное совпадение
            LanguageCultureInfo selectedCulture = Cultures.FirstOrDefault(x => x.Name.ToLower() == language);
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

        private IEnumerable<LanguageCultureInfo> _cultures;
        private XElement _languages;

        public override object GetResource(string key)
        {
            IEnumerable<XElement> record = _languages.Elements("Record").Where(x => x.Attributes("name").ElementAt(0).Value == key);
            return record.Count() > 0 ? record.ElementAt(0).Element(CurrentCulture.Name)?.Value ?? "not declared culture" : "null";
        }

        public override IEnumerable<LanguageCultureInfo> Cultures => _cultures;
    }
}

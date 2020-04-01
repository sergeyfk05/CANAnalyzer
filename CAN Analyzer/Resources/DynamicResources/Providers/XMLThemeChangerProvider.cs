/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using DynamicResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CANAnalyzer.Resources.DynamicResources
{
    internal class XMLThemeChangerProvider : BaseProvider<ThemeCultureInfo>
    {

        private XMLThemeChangerProvider(string file)
        {
            _themes = XDocument.Load(file).Element("Themes").Element("Recordings");
            var cultures = new List<ThemeCultureInfo>();

            foreach(var el in XDocument.Load(file).Element("Themes").Element("Cultures").Elements("Culture"))
            {
                cultures.Add(new ThemeCultureInfo(el.Value));
            }

            _cultures = cultures;

        }
        
        public XMLThemeChangerProvider(string file, ThemeCultureInfo culture) : this(file)
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

        public XMLThemeChangerProvider(string file, string theme) : this(file)
        {
            Init(theme);
        }

        private void Init(string theme)
        {
            theme = theme.ToLower();

            //ищем полное совпадение
            ThemeCultureInfo selectedCulture = Cultures.FirstOrDefault(x => x.Name.ToLower() == theme);
            if (selectedCulture != null)
            {
                CurrentCulture = selectedCulture;
                return;
            }

            //если не находим полное совпадение культуры, то ищем совпадение с более общей культурой
            if (theme.IndexOf('-') != -1)
                theme = theme.Remove(theme.IndexOf('-'));

            selectedCulture = Cultures.FirstOrDefault(x => x.Name.ToLower() == theme);
            if (selectedCulture != null)
                CurrentCulture = selectedCulture;
            else
                //если не нашли ни одного совпадение - назначаем дефолтную культуру(первую)
                CurrentCulture = Cultures.First();
        }

        private IEnumerable<ThemeCultureInfo> _cultures;
        private XElement _themes;

        public override object GetResource(string key)
        {
            IEnumerable<XElement> record = _themes.Elements("Record").Where(x => x.Attributes("name").ElementAt(0).Value == key);
            return record.Count() > 0 ? record.ElementAt(0).Element(CurrentCulture.Name)?.Value ?? "not declared culture" : "null";
        }

        public override IEnumerable<ThemeCultureInfo> Cultures => _cultures;
    }
}

using CANAnalyzer.Resources.DynamicResources;
using DynamicResource;
using System;
using System.Reflection;

namespace CANAnalyzer.Models.ViewData
{
    public static class ContentPageDataExtensions
    {
        public static void UpdateLocalization(this ContentPageData data)
        {
            if (data.NavData != null)
                data.NavData.Text = (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource(data.LocalizedKey);
        }
        public static void UpdateTheme(this ContentPageData data)
        {
            if (data.NavData != null)
                data.NavData.ImageSource = new Uri(
                new Uri(Assembly.GetExecutingAssembly().Location),
                (string)Manager<ThemeCultureInfo>.StaticInstance.GetResource(data.ImageKey));
        }
    }
}

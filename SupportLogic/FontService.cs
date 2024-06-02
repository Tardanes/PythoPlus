using System.Collections.Generic;

namespace PythoPlus
{
    public class FontService : IFontService
    {
        public List<string> GetRegisteredFonts()
        {
            // Здесь добавляем названия шрифтов, которые зарегистрированы в приложении
            return new List<string>
            {
                "OpenSansRegular",
                "OpenSansSemibold"
            };
        }
    }
}

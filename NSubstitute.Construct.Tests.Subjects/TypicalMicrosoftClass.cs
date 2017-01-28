using System;

namespace Tests.Subjects
{
    public sealed class TypicalMicrosoftClass
    {
        private readonly string _constructionTime;

        public TypicalMicrosoftClass()
        {
            _constructionTime = DateTime.Now.ToLongTimeString();
        }

        public string GetConstructionTime()
        {
            return _constructionTime;
        }
    }
}
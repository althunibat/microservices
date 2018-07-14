using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Api.Options
{
    public class ElasticSearchOptions
    {
        /// <summary>
        /// ; separated Urls
        /// </summary>
        public string Urls { get; set; }

        public IEnumerable<Uri> Uris => string.IsNullOrWhiteSpace(Urls)
            ? new List<Uri>()
            : new List<Uri>(Urls.Split(';').Select(url => new Uri(url)));
    }
}
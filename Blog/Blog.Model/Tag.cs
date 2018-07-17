using System.Collections.Generic;
using Framework.Model;

namespace Blog.Model {
    public class Tag:Entity {
        public Tag() {
            Posts = new List<PostTag>();
        }
        public string Name { get; set; }
        public string Url { get; set; }
        public IList<PostTag> Posts { get; set; }
    }
}
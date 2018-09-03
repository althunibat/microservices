using System.Collections.Generic;
using Framework.Model;

namespace Blog.Model {
    public class Excerpt:Entity {
        public string Content { get; set; }
        public IList<Post> Posts { get; set; }
    }
}
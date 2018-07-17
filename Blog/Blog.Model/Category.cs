using System.Collections.Generic;
using Framework.Model;

namespace Blog.Model {
    public class Category:Entity {
        public Category() {
            Posts = new List<Post>();
        }
        public string Name { get; set; }
        public string Url { get; set; }
        public IList<Post> Posts { get; set; }
    }
}
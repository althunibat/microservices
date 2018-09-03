// Godwit  - Blog.Model
// 2018.07.17
// A

using System.Collections.Generic;
using Framework.Model;

namespace Blog.Model {
    public class Article : Entity {
        public string Content { get; set; }
        public IList<Post> Posts { get; set; }
    }
}
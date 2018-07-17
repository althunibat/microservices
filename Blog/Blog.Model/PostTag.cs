using Framework.Model;

namespace Blog.Model {
    public class PostTag:Entity {
        public int TagId { get; set; }

        public int PostId { get; set; }

        public Tag Tag { get; set; }

        public Post Post { get; set; }
    }
}
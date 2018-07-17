using System;
using System.Collections.Generic;
using Framework.Model;

namespace Blog.Model {
    public class Post:Entity {
        public Post() {
            PostTags = new List<PostTag>();
        }
        public string Title { get; set; }
        public string Meta { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Link { get; set; }
        public bool Published { get; set; }
        public string Image { get; set; }
        public string SmallImage { get; set; }
        public string IconImage { get; set; }
        public DateTimeOffset? PostedOn { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }
        public DateTimeOffset? ModifiedOn { get; set; }
        public string Notes { get; set; }
        public int? ExcerptId { get; set; }
        public int? ArticleId { get; set; }
        public int? CategoryId { get; set; }
        public Excerpt Excerpt { get; set; }
        public Article Article { get; set; }
        public Category Category { get; set; }
        public IList<PostTag> PostTags { get; set; }
    }
}
using System.Collections.Generic;
using Framework.Model;

namespace Blog.Model {
    public class User : Entity<string> {
        protected User() {
        }

        public User(string id) {
            Id = id;
        }

        public string Email { get; set; }
    }
}
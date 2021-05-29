using System.Linq;

namespace JsonLoggerDemo
{
    internal class User
    {
        private static int _topUserId = 0;
        public int  Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public User(string firstName, string lastName)
        {
            Id = ++_topUserId;
            FirstName = firstName;
            LastName = lastName;
        }

        public static implicit operator User(string name)
        {
            var parts = name.Split(" ");
            return new User(parts.First(), parts.Last());
        }
    }
}
using Xunit;
using Eventify_High_Performance_Event_Management_API.Models;

namespace Eventify.Tests.Models
{
    public class UserTest
    {
        [Fact]
        public void User_Email_ShouldBeSetCorrectly()
        {
            var user = new User();
            user.Email = "test@example.test";

            var result = user.Email;

            Assert.Equal("test@example.test", result);
        }
        [Fact]
        public void User_IsAdmin_ShouldDefaultFalse()
        {
            var user = new User();

            var result = user.IsAdmin;

            Assert.False(result);
        
        }
    }
}
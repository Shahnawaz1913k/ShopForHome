namespace ShopForHome.Api.DTOs 
{ 
    public class UserCreateDto {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } } 
    public class UserUpdateDto {
        public string Username { get; set; }
         public string? Password { get; set; }
        public string Role { get; set; } } 
    public class UserViewDto {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; } } 
}
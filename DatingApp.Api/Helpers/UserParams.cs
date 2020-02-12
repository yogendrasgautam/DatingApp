namespace DatingApp.Api.Helpers
{
    public class UserParams : PagingParams
    {
        public int UserId { get; set; }
        public string Gender { get; set; }
        public int MinAge { get; set; } =18;
        public int MaxAge { get; set; } =99;
        public string OrderBy { get; set; }
        public bool Likees { get; set; }
        public bool Likers { get; set; }
    }
}
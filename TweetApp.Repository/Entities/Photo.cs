﻿#nullable disable
namespace TweetApp.Repository.Entities
{
    public class Photo
    {
        public int Id { get; set; }
        public string PublicId { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
    }
}

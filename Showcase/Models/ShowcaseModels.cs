namespace Showcase.Models
{
    public abstract class EntityWithUrl
    {
        public string Url { get; set; }
    }

    public abstract class EntityWithUrlAndThumbnail : EntityWithUrl
    {
        public string ThumbnailUrl { get; set; }
    }

    public class Alternative : EntityWithUrl
    {
        public string Quality { get; set; }
    }

    public class CardImage : EntityWithUrl
    {
        public int H { get; set; }
        public int W { get; set; }
    }

    public class Cast
    {
        public string Name { get; set; }
    }

    public class Director
    {
        public string Name { get; set; }
    }

    public class Gallery : EntityWithUrlAndThumbnail
    {
        public string Title { get; set; }
        public string Id { get; set; }
    }

    public class KeyArtImage : EntityWithUrl
    {
        public int H { get; set; }
        public int W { get; set; }
    }

    public class ShowcaseRoot
    {
        public string Body { get; set; }
        public List<CardImage> CardImages { get; set; }
        public List<Cast> Cast { get; set; }
        public string Cert { get; set; }
        public string Class { get; set; }
        public List<Director> Directors { get; set; }
        public int Duration { get; set; }
        public List<string> Genres { get; set; }
        public string Headline { get; set; }
        public string Id { get; set; }
        public List<KeyArtImage> KeyArtImages { get; set; }
        public string LastUpdated { get; set; }
        public string Quote { get; set; }
        public int Rating { get; set; }
        public string ReviewAuthor { get; set; }
        public string SkyGoId { get; set; }
        public string SkyGoUrl { get; set; }
        public string Sum { get; set; }
        public string Synopsis { get; set; }
        public string Url { get; set; }
        public List<Video> Videos { get; set; }
        public ViewingWindow ViewingWindow { get; set; }
        public string Year { get; set; }
        public List<Gallery> Galleries { get; set; }
        public string Sgid { get; set; }
        public string SgUrl { get; set; }
    }

    public class Video : EntityWithUrl
    {
        public string Title { get; set; }
        public List<Alternative> Alternatives { get; set; }
        public string Type { get; set; }
        public string ThumbnailUrl { get; set; }
    }

    public class ViewingWindow
    {
        public string StartDate { get; set; }
        public string WayToWatch { get; set; }
        public string EndDate { get; set; }
        public string Title { get; set; }
    }


}

namespace MZBlog.Core
{
    public interface IViewProjection<tIn, tOut>
    {
        tOut Project(tIn input);
    }
}
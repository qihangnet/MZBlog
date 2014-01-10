namespace MZBlog.Core
{
    public interface IViewProjectionFactory
    {
        TOut Get<TIn, TOut>(TIn input);
    }
}
namespace MZBlog.Core
{
    public interface ICommandInvoker<in TIn, out TOut>
    {
        TOut Execute(TIn command);
    }

    public interface ICommandInvokerFactory
    {
        TOut Handle<TIn, TOut>(TIn input);
    }
}
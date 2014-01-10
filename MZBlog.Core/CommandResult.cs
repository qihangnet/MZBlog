using System.Collections.Generic;
using System.Linq;

namespace MZBlog.Core
{
    public class CommandResult
    {
        private readonly List<CommandError> _errors = new List<CommandError>();

        public CommandResult()
        { }

        public static CommandResult SuccessResult
        {
            get
            {
                return new CommandResult();
            }
        }

        public CommandResult(string trrorMessage)
        {
            AddError(trrorMessage);
        }

        public bool Success
        {
            get { return !_errors.Any(); }
        }

        public void AddError(string trror)
        {
            _errors.Add(new CommandError(trror));
        }

        public string[] GetErrors()
        {
            return _errors.Select(t => t.Message).ToArray();
        }
    }

    public class CommandError
    {
        public virtual string Message { get; private set; }

        public CommandError(string message)
        {
            Message = message;
        }
    }
}
namespace exys228.HopeCommands
{
	public class Command
	{
		public delegate void DCallback(string[] args);

		public string CommandText { get; private set; }
		public string Description { get; private set; }

		private DCallback Callback;

		public Command(string command, string description, DCallback callback)
		{
			CommandText = command;
			Description = description;
			Callback = callback;
		}

		/// <summary>
		/// Execute command with arguments.
		/// </summary>
		/// <param name="args">Arguments to pass to callback function</param>
		public void Execute(string[] args) => Callback(args); // TODO: add version w/o args
	}
}

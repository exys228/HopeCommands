namespace exys228.HopeCommands
{
	public class Command
	{
		public delegate void DCallback(string[] args);

		private DCallback Callback;

		public string CommandText { get; private set; }

		public string Description { get; private set; }

		public Command(string command, string description, DCallback callback)
		{
			CommandText = command;
			Description = description;
			Callback = callback;
		}

		public void Execute(string[] args) => Callback(args); // todo: version without args
	}
}
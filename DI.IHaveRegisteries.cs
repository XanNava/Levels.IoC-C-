namespace Levels.Universal {
	// try https://stackoverflow.com/questions/15578668/constraining-class-generic-type-to-a-tuple/15578908

	public partial class DI {
		public interface IHaveRegisteries {
			protected String.ID<DI.Scope> Scope {
				get; set;
			}
			protected object Source {
				get; set;
			}

			/// <summary>
			/// Used to register items to scope. Note you should save scope to this.scope and this.source for Unregister().
			/// </summary>
			/// <param name="scope"></param>
			public void Register(in String.ID<DI.Scope> scope, object source);

			public void Unregister();
		}
	}
}

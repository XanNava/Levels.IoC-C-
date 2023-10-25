namespace Levels.Universal {
	using System;
    using UnityEngine;

	[Settings.DI(InjectInterface = true)]
	public class Extends<T> : IInject {
		protected internal T _source;

		public Func<string> DiTagFunc;

		public virtual string DiTag { get; set; } = "";

		public void Receive(T value) {
			_source = value;
		}

		public void Receive(object[] values) {
			_source = (T)values[0];
		}

		public (Type, string)[] Request() {
			Debug.Log("INJECT!!!!!!");
			Debug.Log(DiTag);
			return new[]
			{
				(typeof(T), DiTagFunc.Invoke())
			};
		} 
		
	}
}
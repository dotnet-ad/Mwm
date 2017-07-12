namespace Mwm.UI
{
	using System;
	using System.Collections.Generic;

	public class FactoryBuilder : IUIBuilder
	{
		private Dictionary<string, Func<IElement>> factories = new Dictionary<string, Func<IElement>>();

		public FactoryBuilder Register(string name, Func<IElement> factory)
		{
			this.factories[name] = factory;
			return this;
		}

		public FactoryBuilder Register<T>(string name = null, params object[] args) where T : IElement
		{
			name = name ?? typeof(T).Name;
			return this.Register(name, () => (T)Activator.CreateInstance(typeof(T),args));
		}

		public IElement Create(string name) => this.factories[name].Invoke();

		public bool CanCreate(string name) => this.factories.ContainsKey(name);
	}
}

using System.Collections.ObjectModel;
using System.Text;
using Mwm.UI.Html;
namespace Mwm.AspNetCore
{
	using System.Net.WebSockets;
	using System.Threading;
	using System.Threading.Tasks;
	using System;
	using System.Reflection;
	using System.Collections.Generic;
	using System.Linq;
	using Mwm.UI;

	public class ElementObserver : IDisposable
	{
		#region Classes

		public class ElementPropertyDefinition
		{
			public ElementPropertyDefinition(int id, PropertyInfo info)
			{
				this.Identifier = (byte)id;
				this.Info = info;
			}

			public byte Identifier { get; }

			public PropertyInfo Info { get; }
		}

		public class ElementEventDefinition
		{
			public ElementEventDefinition(int id, EventInfo info)
			{
				this.Identifier = (byte)id;
				this.Info = info;
			}

			public byte Identifier { get; }

			public EventInfo Info { get; }
		}

		#endregion

		#region Constructors

		public ElementObserver(IElement element, WebSocket socket, ISerializer serializer, IRenderer renderer)
		{
			this.element = element;
			this.serializer = serializer;
			this.renderer = renderer;
			this.socket = socket;
			this.element.PropertyChanged += OnElementPropertyChanged;

			var runtimeProperties = this.element.GetType().GetRuntimeProperties().OrderBy(x => x.Name).ToArray();
			for (int i = 0; i < runtimeProperties.Length; i++)
			{
				var info = runtimeProperties[i];
				this.properties[info.Name] = new ElementPropertyDefinition(i, info);
			}

			var runtimeEvents = this.element.GetType().GetRuntimeEvents().OrderBy(x => x.Name).ToArray();
			for (int i = 0; i < runtimeEvents.Length; i++)
			{
				var info = runtimeEvents[i];
				this.events[info.Name] = new ElementEventDefinition(i, info);
			}

			if(element is IPanel panel)
			{
				this.subobservers = panel.Children.Select(x => new ElementObserver(x, socket, serializer, renderer)).ToArray();
			}
			else if (element is Page page)
			{
				this.subobservers = new[] { new ElementObserver(page.Content, socket, serializer, renderer) };
			}
		}

		#endregion

		#region Fields

		private IEnumerable<ElementObserver> subobservers = Enumerable.Empty<ElementObserver>();

		private ISerializer serializer;

		private IRenderer renderer;

		private IElement element;

		private WebSocket socket;

		private Dictionary<string, ElementPropertyDefinition> properties = new Dictionary<string, ElementPropertyDefinition>();

		private Dictionary<string, ElementEventDefinition> events = new Dictionary<string, ElementEventDefinition>();

		#endregion

		public IReadOnlyDictionary<string, ElementPropertyDefinition> Properties => new ReadOnlyDictionary<string, ElementPropertyDefinition>(this.properties);

		public IReadOnlyDictionary<string, ElementEventDefinition> Events => new ReadOnlyDictionary<string, ElementEventDefinition>(this.events);

		#region Private methods

		private async void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			await this.SendPropertyAsync(e.PropertyName);
		}

		private Task SendPropertyAsync(string name)
		{
			var info = this.properties[name];
			var newValue = info.Info.GetValue(this.element);

			// Reading bytes
			var identifierByte = (byte)this.element.Identifier;
			var instructionByte = Instructions.PropertyChanged;
			var valueBytes = this.serializer.Serialize(newValue);
			var bytes = new[] { instructionByte, identifierByte, (byte)name.Length, }
								.Concat(Encoding.UTF8.GetBytes(name))
								.Concat(valueBytes).ToArray();

			var segment = new ArraySegment<byte>(bytes, 0, bytes.Length);
			return this.socket.SendAsync(segment, WebSocketMessageType.Binary, true, CancellationToken.None);
		}

		public Task SendViewAsync()
		{
			var js = this.renderer.Render(element);
			var instructionByte = Instructions.Navigate;
			var bytes = new[] { instructionByte, }
								.Concat(Encoding.UTF8.GetBytes(js))
								.ToArray();
			var segment = new ArraySegment<byte>(bytes, 0, bytes.Length);
			return this.socket.SendAsync(segment, WebSocketMessageType.Binary, true, CancellationToken.None);
		}

		#endregion

		#region Public methods

		public ElementObserver FindElement(int id)
		{
			if (id == this.element.Identifier)
				return this;

			foreach (var child in this.subobservers)
			{
				var found = child.FindElement(id);
				if (found != null) return found;
			}

			return null;
		}

		public void Update(byte[] data)
		{
			var identifierByte = data[0];

			if(identifierByte == this.element.Identifier)
			{
				var instructionByte = data[1];
				var propertyByte = data[2];
				var valueBytes = data.Skip(3).ToArray();

				switch (instructionByte)
				{
					case Instructions.PropertyChanged:
						var info = this.properties.FirstOrDefault(x => x.Value.Identifier == propertyByte);
						var newValue = this.serializer.Deserialize(valueBytes, info.Value.Info.PropertyType);
						info.Value.Info.SetValue(this.element, newValue);
						break;
					default:
						break;
				}
			}
		}

		public void RaiseEvent(string name, byte[] arg, ISerializer serializer)
		{
			var raiseMethod = this.element.GetType().GetMethod($"Raise{name}", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

			var args = new object[0];

			var parameters = raiseMethod.GetParameters();
			if(parameters.Any())
			{
				var t = parameters.First().ParameterType;
				var v = serializer.Deserialize(arg, t);
				args = new[] { v };

			}

			raiseMethod.Invoke(this.element, args);
		}

		public void Dispose() 
		{
			this.element.Dispose();
			this.element.PropertyChanged -= OnElementPropertyChanged;

			foreach (var child in this.subobservers)
			{
				child.Dispose();
			}
		}

		#endregion
	}
}

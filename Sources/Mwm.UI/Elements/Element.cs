namespace Mwm.UI
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Runtime.CompilerServices;

	public abstract class Element : IElement
	{
		public Element()
		{
			this.margin = new Thickness(0, 0, 0, 0);
			this.width = -1;
			this.height = -1;
		}

		#region Static

		public static int GlobalIdentifier { get; private set; } = 0;

		public static void ResetIdentifiers() => GlobalIdentifier = 0;

		#endregion

		#region Fields

		private string name;

		private object dataContext;

		private Visibility visibility;

		private int width, height;

		private Alignment horizontalAlignment, verticalAlignment;

		private Thickness margin;

		private WeakReference<IElement> parent;

		#endregion

		#region Properties

		public int Identifier { get; } = GlobalIdentifier++;

		public string Name 
		{ 
			get => this.name;
			set => this.Set(ref name, value); 
		}

		public Visibility Visibility
		{
			get => this.visibility;
			set => this.Set(ref visibility, value);
		}

		public Thickness Margin
		{
			get => this.margin;
			set => this.Set(ref margin, value);
		}

		public Alignment VerticalAlignment
		{
			get => this.verticalAlignment;
			set => this.Set(ref verticalAlignment, value);
		}

		public Alignment HorizontalAlignment
		{
			get => this.horizontalAlignment;
			set => this.Set(ref horizontalAlignment, value);
		}

		public int Width
		{
			get => this.width;
			set => this.Set(ref width, value);
		}

		public int Height
		{
			get => this.height;
			set => this.Set(ref height, value);
		}
		public virtual object DataContext 
		{ 
			get => this.dataContext ?? this.Parent?.DataContext; 
			set 
			{
				if(this.dataContext != value)
				{
					this.dataContext = value;
					UpdateBindings();
				}
			}
		}

		public virtual void UpdateBindings()
		{
			foreach (var binding in this.bindings)
			{
				binding.Source = this.DataContext;
			}

			foreach (var binding in this.bindingsToSource)
			{
				binding.Target = this.DataContext;
			}
		}

		public IElement Parent 
		{
			get => this.parent != null && this.parent.TryGetTarget(out IElement result) ? result : null;
			set => this.parent = value != null ? new WeakReference<IElement>(value) : null; 
		}

		#endregion

		#region Observable

		public event PropertyChangedEventHandler PropertyChanged;

		protected bool Set<T>(ref T field, T value, [CallerMemberName]string name = null)
		{
			var changed = !EqualityComparer<T>.Default.Equals(field, value);

			if (changed)
			{
				field = value;
				RaiseProperty(name);
			}

			return changed;
		}

		public void RaiseProperty(string property) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

		public void RaiseProperties(params string[] properties)
		{
			foreach (var property in properties)
			{
				this.RaiseProperty(property);
			}
		}

		private List<Binding> bindings = new List<Binding>();
		private List<Binding> bindingsToSource = new List<Binding>();

		public void Bind(string toProperty, string fromContextProperty, IValueConverter converter = null)
		{
			var binding = new Binding(this.DataContext, fromContextProperty, this, toProperty, converter);
			this.bindings.Add(binding);
		}

		public void BindTwoWay(string toProperty, string fromContextProperty, IValueConverter converter = null)
		{
			this.Bind(toProperty, fromContextProperty, converter);

			var binding = new Binding(this, toProperty, this.DataContext, fromContextProperty , converter.Invert());
			this.bindingsToSource.Add(binding);
		}

		public void Dispose()
		{
			foreach (var binding in bindings)
			{
				binding.Dispose();
			}
		}

		#endregion
	}
}

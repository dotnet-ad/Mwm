using System.Globalization;
namespace Mwm.UI
{
	using System;
	using System.ComponentModel;
	using System.Reflection;

	public class Binding : IDisposable
	{
		public Binding(object source, string sourceProperty, object target, string targetProperty, IValueConverter converter)
		{
			this.SourcePropertyName = sourceProperty;
			this.TargetPropertyName = targetProperty;
			this.Source = source;
			this.Target = target;
			this.Converter = converter;
		}

		private object source, target;

		public IValueConverter Converter { get; }

		public string SourcePropertyName { get; }

		public string TargetPropertyName { get; }

		public PropertyInfo SourceProperty { get; set; }

		public PropertyInfo TargetProperty { get; set; }

		public object Source 
		{
			get => this.source;
			set
			{
				if(this.source != value)
				{
					if (this.source is INotifyPropertyChanged observable)
					{
						this.SourceProperty = null;
						observable.PropertyChanged -= OnPropertyChanged;
					}

					this.source = value;

					if(this.source != null)
					{
						this.SourceProperty = this.source?.GetType()?.GetRuntimeProperty(this.SourcePropertyName);
					}

					if (this.source is INotifyPropertyChanged newobservable)
					{
						newobservable.PropertyChanged += OnPropertyChanged;
					}

					this.Update();
				}
			}
		}

		public object Target
		{
			get => this.target;
			set
			{
				if (this.target != value)
				{
					this.target = value;

					if (this.target != null)
					{
						this.TargetProperty = this.target?.GetType()?.GetRuntimeProperty(this.TargetPropertyName);
					}

					this.Update();
				}
			}
		}

		private void Update()
		{
			if(this.Source != null && this.Target != null)
			{
				var newValue = SourceProperty.GetValue(this.Source);

				if (this.Converter != null)
					newValue = this.Converter.Convert(newValue, TargetProperty.PropertyType, null, CultureInfo.CurrentCulture);
				
				TargetProperty.SetValue(this.Target, newValue);
			}
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if(e.PropertyName == this.SourceProperty.Name)
			{
				this.Update();
			}
		}

		public void Dispose()
		{
			if (this.Source is INotifyPropertyChanged observable)
			{
				observable.PropertyChanged -= OnPropertyChanged;
			}
		}
	}
}

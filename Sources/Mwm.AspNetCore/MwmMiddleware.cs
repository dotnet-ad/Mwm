namespace Mwm.AspNetCore
{
	using System;
	using System.Linq;
	using System.Net.WebSockets;
	using System.Threading.Tasks;
	using System.Threading;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Routing;
	using Mwm.UI.Html;
	using Mwm.UI;
	using System.Text;
	using System.Reflection;

	public class MwmMidleware : INavigation
	{
		#region Fields

		private readonly RequestDelegate next;

		private readonly MwmOptions options;

		#endregion

		#region Constructors

		public MwmMidleware(RequestDelegate next, MwmOptions options)
		{
			this.next = next;
			this.options = options;
		}

		#endregion

		#region Sockets

		private WebSocket socket;

		private ElementObserver view;

		private async Task InvokeSocket(HttpContext context)
		{
			this.socket = await context.WebSockets.AcceptWebSocketAsync();

			var buffer = new byte[1024 * 4];

			WebSocketReceiveResult result = null;

			while (result == null || !result.CloseStatus.HasValue)
			{
				result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

				if (buffer.Length >= 1)
				{
					var instruction = buffer.First();

					switch (instruction)
					{
						case Instructions.Navigate:
							var viewnameBytes = buffer.Take(result.Count).Skip(1).ToArray();
							var viewname = Encoding.UTF8.GetString(viewnameBytes).Trim();
							await NavigateAsync(viewname);
							break;

						case Instructions.Event:
							if (view != null)
							{
								var identifier = buffer[1];
								var source = view.FindElement(identifier);
								if(source != null)
								{
									var eventNameLength = buffer[2];
									var eventname = Encoding.UTF8.GetString(buffer.Skip(3).Take(eventNameLength).ToArray());
									var i = 3 + eventNameLength;
									var length = result.Count - i;
									var arg = buffer.Skip(i).Take(length).ToArray();
									source.RaiseEvent(eventname, arg, this.options.Serializer);
								}
							}
							break;
					}

				}
			}

			view?.Dispose();

			await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
		}

		#endregion

		public async Task Invoke(HttpContext context)
		{
			var dict = new RouteValueDictionary();
			var path = context.Request.Path.Value;

			if(Routing.TryMatch("mwm/socket", path, dict) && context.WebSockets.IsWebSocketRequest)
			{
				await this.InvokeSocket(context);
				return;
			}

			try
			{
				path = path.TrimStart('/');
				if (string.IsNullOrEmpty(path))
					path = "mwwm.html";

				var content = Content.Load(path);

				content = content.Replace("{{initial}}","Home");

				await context.Response.WriteAsync(content);
			}
			catch (Exception ex)
			{
				await next.Invoke(context);
			}
		}

		public async Task NavigateAsync(string page)
		{
			if (this.options.UIBuilder.CanCreate(page))
			{
				if (view != null)
				{
					view.Dispose();
				}

				Element.ResetIdentifiers();
				var element = this.options.UIBuilder.Create(page);
				if(element is Page p)
				{
					p.Navigation = this;
				}

				view = new ElementObserver(element, socket, this.options.Serializer, this.options.Renderer);
				await view.SendViewAsync();
			}
		}
	}

	public static class MwmMidlewareExtensions
	{
		public static IApplicationBuilder UseMwm(this IApplicationBuilder builder, MwmOptions options)
		{
			return builder.UseMiddleware<MwmMidleware>(options);
		}
	}
}

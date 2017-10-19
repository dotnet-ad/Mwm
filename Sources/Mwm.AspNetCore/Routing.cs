using System;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;

namespace Mwm.AspNetCore
{
	public static class Routing
	{
		#region Matching

		public static bool TryMatch(string routeTemplate, string requestPath, RouteValueDictionary result)
		{
			var template = TemplateParser.Parse(routeTemplate);
			var matcher = new TemplateMatcher(template, GetDefaults(template));
			return matcher.TryMatch(requestPath, result);
		}

		private static RouteValueDictionary GetDefaults(RouteTemplate parsedTemplate)
		{
			var result = new RouteValueDictionary();

			foreach (var parameter in parsedTemplate.Parameters)
			{
				if (parameter.DefaultValue != null)
				{
					result.Add(parameter.Name, parameter.DefaultValue);
				}
			}

			return result;
		}

		#endregion
	}
}

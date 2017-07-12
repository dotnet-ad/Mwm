using System;
namespace Mwm.AspNetCore
{
	public interface ISerializer
	{
		byte[] Serialize(object value);


		object Deserialize(byte[] data, Type type);
	}
}

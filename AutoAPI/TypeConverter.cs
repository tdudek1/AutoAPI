using System;
using System.ComponentModel;

namespace AutoAPI
{
	public class TypeConverter
	{
		public static object ChangeType(string value, Type type)
		{
			return TypeDescriptor.GetConverter(type).ConvertFromInvariantString(value);
		}
	}
}

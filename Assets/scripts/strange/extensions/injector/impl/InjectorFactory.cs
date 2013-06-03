/**
 * @class strange.extensions.injector.impl.InjectorFactory
 * 
 * The Factory that instantiates all instances.
 */

using System;
using System.Collections.Generic;
using strange.extensions.injector.api;

namespace strange.extensions.injector.impl
{
	public class InjectorFactory : IInjectorFactory
	{
		protected Dictionary<IInjectionBinding, Dictionary<object, object>> objectMap;

		public InjectorFactory ()
		{
			objectMap = new Dictionary<IInjectionBinding, Dictionary<object, object>> ();
		}

		public object Get(IInjectionBinding binding, object[] args)
		{
			if (binding == null)
			{
				throw new InjectionException ("InjectorFactory cannot act on null binding", InjectionExceptionType.NULL_BINDING);
			}
			InjectionBindingType type = binding.type;
			if (objectMap.ContainsKey(binding) == false)
			{
				objectMap [binding] = new Dictionary<object, object> ();
			}

			switch (type)
			{
				case InjectionBindingType.SINGLETON:
					return singletonOf (binding, args);
				case InjectionBindingType.VALUE:
					return valueOf (binding);
				default:
					break;
			}
			return instanceOf (binding, args);
		}

		public object Get(IInjectionBinding binding)
		{
			return Get (binding, null);
		}

		/// Generate a Singleton instance
		protected object singletonOf(IInjectionBinding binding, object[] args)
		{
			object name = binding.name;
			Dictionary<object, object> dict = objectMap [binding];
			if (dict.ContainsKey(name) == false)
			{
				dict [name] = createFromValue(binding.value, args);
			}
			return dict[name];
		}

		/// The binding already has a value. Simply return it.
		protected object valueOf(IInjectionBinding binding)
		{
			return binding.value;
		}

		/// Generate a new instance
		protected object instanceOf(IInjectionBinding binding, object[] args)
		{
			return createFromValue(binding.value, args);
		}

		/// Call the Activator to attempt instantiation the given object
		protected object createFromValue(object o, object[] args)
		{
			Type value = (o is Type) ? o as Type : o.GetType ();
			object retv = null;
			try
			{
				if (args == null || args.Length == 0)
				{
					retv = Activator.CreateInstance (value);
				}
				else
				{
					retv = Activator.CreateInstance(value, args);
				}
			}
			catch
			{
				//No-op
			}
			return retv;
		}
	}
}

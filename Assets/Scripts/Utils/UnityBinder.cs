﻿using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX
using UnityEditor;
#endif

/// <summary>
/// UnityBinder entry class. Use this class to setup any Unity Object that has any
/// Binder Attributes
/// </summary>
public static class UnityBinder 
{
	/// <summary>
	/// Inject an Object's field that have attributes.
	/// </summary>
	/// <param name="obj">The object to inject</param>
	public static void Inject(Object obj)
	{
		var bindingFlags = BindingFlags.Instance |
		                   BindingFlags.NonPublic |
		                   BindingFlags.Public;
		
		var fields = obj.GetType().GetFields(bindingFlags);

		foreach (var field in fields)
		{
			var injections = (Binder[])field.GetCustomAttributes(typeof(Binder), true);

			if (injections.Length > 0)
			{
				foreach (var inject in injections)
				{
					inject.InjectInto(obj, field);
				}
			}
		}

		var methods = obj.GetType().GetMethods(bindingFlags);
		foreach (var method in methods)
		{
			var injections = (BindOnClick[]) method.GetCustomAttributes(typeof(BindOnClick), true);

			if (injections.Length > 0)
			{
				foreach (var inject in injections)
				{
					inject.InjectInto(obj, method);
				}
			}
		}
	}

	private static GameObject DeepFind(string name)
	{
		if (name.StartsWith("/"))
		{
			string[] temp = name.Split('/');

			GameObject current = null;
			for (int i = 1; i < temp.Length; i++)
			{
				string n = temp[i];
				if (current == null)
				{
					current = GameObject.Find(n);
					if (current == null)
					{
						current = FindInActiveObjectByName(n);
					}
				}
				else
				{
					current = current.transform.Find(n).gameObject;
				}
			}

			return current;
		}
		
		return GameObject.Find(name);
	}

	internal static GameObject FindInActiveObjectByName(string name)
	{
		if (name.StartsWith("/"))
			return DeepFind(name);
		
		Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>();
		for (int i = 0; i < objs.Length; i++)
		{
			if (objs[i].hideFlags == HideFlags.None)
			{
				if (objs[i].name == name)
				{
					return objs[i].gameObject;
				}
			}
		}
		return null;
	}
}

/// <summary>
/// Abstract resource to represent any kind of Bind
/// </summary>
public abstract class Binder : Attribute
{
	public abstract void InjectInto(Object obj, FieldInfo field);
}

/// <summary>
/// Bind a method to an OnClick event that is triggered by a Button.
/// 
/// By default, the Button is searched on the gameObject attached to the script being bound
/// You may specify a GameObject to search in by supplying the Editor path in the constructor
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class BindOnClick : Attribute
{
	private string buttonPath;

	public BindOnClick(string buttonPath = "")
	{
		this.buttonPath = buttonPath;
	}
	
	public void InjectInto(Object obj, MethodInfo method)
	{
		GameObject fromObj;
		if (string.IsNullOrEmpty(buttonPath))
		{
			var component = obj as Component;
			if (component != null)
			{
				fromObj = component.gameObject;
			}
			else
			{
				Debug.LogError("fromObject empty for field " + method.Name + ", and no default gameObject could be found!");
				return;
			}
		}
		else
		{
			fromObj = GameObject.Find(buttonPath);

			if (fromObj == null)
			{
				fromObj = UnityBinder.FindInActiveObjectByName(buttonPath);

				if (fromObj == null)
				{
					Debug.LogError("Could not find GameObject with name " + buttonPath + " for field " + method.Name);

					return;
				}
			}
		}

		var button = fromObj.GetComponent<Button>();
		if (button == null)
		{
			Debug.LogError("No Button Component found on GameObject @ " + buttonPath);
			return;
		}
		
		button.onClick.AddListener(delegate { method.Invoke(obj, new object[0]); });
	}
}

/// <summary>
/// Attribute to bind a resource at runtime
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class BindResource : Binder
{
	private static MethodInfo _cacheMethod;
	private static MethodInfo _cacheNotGeneric;

	public static MethodInfo GenericResourceLoad
	{
		get
		{
			if (_cacheMethod != null) return _cacheMethod;
			
			var methods = typeof(Resources).GetMethods();

			foreach (var method in methods)
			{
				if (method.Name != "Load" || !method.IsGenericMethod) continue;

				_cacheMethod = method;
				break;
			}

			return _cacheMethod;
		}
	}

	public static MethodInfo ResourceLoad
	{
		get
		{
			if (_cacheNotGeneric != null) return _cacheNotGeneric;

			_cacheNotGeneric = typeof(Resources).GetMethod("Load", new[] {typeof(string)});

			return _cacheNotGeneric;
		}
	}
	
	public string path;

	public BindResource(string path)
	{
		this.path = path;
	}


	public override void InjectInto(Object obj, FieldInfo field)
	{
		var injectType = field.FieldType;

		bool bindPrefab = false;
		object rawResult;
		if (injectType == typeof(GameObject))
		{
			bindPrefab = true;

			injectType = typeof(Object);

			rawResult = ResourceLoad.Invoke(null, new object[] {path});
		}
		else
		{
			var genericMethod = GenericResourceLoad.MakeGenericMethod(injectType);
			rawResult = genericMethod.Invoke(null, new object[] { path });
		}
		

		if (rawResult == null)
		{
			Debug.LogError("Could not find resource of type " + injectType + " for field " + field.Name);
		}
		else if (!injectType.IsInstanceOfType(rawResult))
		{
			Debug.LogError("Could not cast resource of type " + rawResult.GetType() + " to type of " + injectType + " for field " + field.Name);
		}
		else
		{
			if (bindPrefab)
			{
				var objResult = rawResult as Object;
				var instance = Object.Instantiate(objResult) as GameObject;
				
				field.SetValue(obj, instance);
			}
			else
			{
				field.SetValue(obj, rawResult);
			}
		}
	}
}

/// <summary>
/// Attribute to Bind a field to a component at runtime
/// </summary>
[AttributeUsage(AttributeTargets.Field)] 
public class BindComponent : Binder
{

	public int index = 0;
	public string fromObject = "";
	public string fromChild = "";

	public BindComponent(int index = 0, string fromObject = "", string fromChild = "")
	{
		this.index = index;
		this.fromObject = fromObject;
		this.fromChild = fromChild;
	}

	public bool suppressError { get; set; }

	public override void InjectInto(Object obj, FieldInfo field)
	{
		var injectType = field.FieldType;
					
		var unityCall = typeof(GameObject).GetMethod("GetComponents", new Type[0]);
		if (unityCall == null)
		{
			if (!suppressError)
				Debug.LogError("Could not find method GetComponents !!");
			return;
		}

		GameObject fromObj;
		if (string.IsNullOrEmpty(fromObject) && string.IsNullOrEmpty(fromChild))
		{
			var component = obj as Component;
			if (component != null)
			{
				fromObj = component.gameObject;
			}
			else
			{
				if (!suppressError)
					Debug.LogError("fromObject empty for field " + field.Name + ", and no default gameObject could be found!");
				return;
			}
		}
		else if (string.IsNullOrEmpty(fromChild))
		{
			fromObj = GameObject.Find(fromObject);

			if (fromObj == null)
			{
				fromObj = UnityBinder.FindInActiveObjectByName(fromObject);

				if (fromObj == null)
				{
					if (!suppressError)
						Debug.LogError("Could not find GameObject with name " + fromObject + " for field " + field.Name);

					return;
				}
			}
		}
		else
		{
			var component = obj as Component;
			if (component != null)
			{
				var temp = component.transform.Find(fromChild);

				if (temp == null)
				{
					if (!suppressError)
						Debug.LogError("Could not find child with name " + fromChild + " for field " + field.Name);
					return;
				}

				fromObj = temp.gameObject;
			}
			else
			{
				if (!suppressError)
					Debug.LogError("Could not find child with name " + fromChild + " for field " + field.Name);
				return;
			}

		}

		if (injectType == typeof(GameObject))
		{
			field.SetValue(obj, fromObj);
			return;
		}
					

		var genericMethod = unityCall.MakeGenericMethod(injectType);
		var rawResult = genericMethod.Invoke(fromObj, null);

		if (rawResult == null)
		{
			if (!suppressError)
				Debug.LogError("Could not find component of type " + injectType + " for field " + field.Name);
		} 
		else if (rawResult is object[])
		{
			var result = rawResult as object[];

			if (result.Length > 0)
			{
				if (index >= result.Length)
				{
					if (!suppressError)
						Debug.LogError("Could not find component of type " + injectType + " for field " + field.Name + " at index " + index);
				}
				else
				{
					var found = result[index];
								
					field.SetValue(obj, found);
				}
			}
			else
			{
				if (!suppressError)
					Debug.LogError("Could not find component of type " + injectType + " for field " + field.Name);
			}
		}
	}
}

/// <summary>
/// A MonoBehavior that injects fields in the Awake() function
/// </summary>
public class BindableMonoBehavior : MonoBehaviour {

	/// <summary>
	/// The standard Unity Awake() function. This function will invoke UnityBinder.Inject.
	/// 
	/// If you override this Awake() function, be sure to call base.Awake()
	/// </summary>
	public virtual void Awake()
	{
		UnityBinder.Inject(this);
	}
	
	#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX
	[MenuItem("Assets/Create/Bindable C# Script")]
	public static void CreateScript()
	{
		string copyPath = UnityUtils.GetSelectedPath() + "/BindableScript.cs";
		
		if( File.Exists(copyPath) == false ){ // do not overwrite
			using (StreamWriter outfile = 
				new StreamWriter(copyPath))
			{
				outfile.WriteLine("using UnityEngine;");
				outfile.WriteLine("");
				outfile.WriteLine("public class BindableScript : BindableMonoBehavior {");
				outfile.WriteLine("    ");
				outfile.WriteLine("    ");
				outfile.WriteLine("    // Use this for initialization");
				outfile.WriteLine("    void Start () {");
				outfile.WriteLine("        ");
				outfile.WriteLine("    }");
				outfile.WriteLine("    ");         
				outfile.WriteLine("    ");
				outfile.WriteLine("    // Update is called once per frame");
				outfile.WriteLine("    void Update () {");
				outfile.WriteLine("        ");
				outfile.WriteLine("    }");
				outfile.WriteLine("}");
			}
		}
		AssetDatabase.Refresh();
	}
	#endif
}
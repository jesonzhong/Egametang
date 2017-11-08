using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using UnityEngine;

namespace Model
{
	public static class DllHelper
	{
#if ILRuntime
		public static void LoadHotfixAssembly()
		{
			GameObject code = (GameObject)Resources.Load("Code");
			byte[] assBytes = code.GetComponent<ReferenceCollector>().Get<TextAsset>("Hotfix.dll").bytes;
			byte[] mdbBytes = code.GetComponent<ReferenceCollector>().Get<TextAsset>("Hotfix.pdb").bytes;

			using (MemoryStream fs = new MemoryStream(assBytes))
			using (MemoryStream p = new MemoryStream(mdbBytes))
			{
				Init.Instance.AppDomain.LoadAssembly(fs, p, new Mono.Cecil.Pdb.PdbReaderProvider());
			}
		}
#else
		public static Assembly LoadHotfixAssembly()
		{
#if UNITY_EDITOR
            GameObject code = (GameObject)Resources.Load("Code");
            byte[] assBytes = code.Get<TextAsset>("Hotfix.dll").bytes;
            byte[] mdbBytes = code.Get<TextAsset>("Hotfix.mdb").bytes;
            Assembly assembly = Assembly.Load(assBytes, mdbBytes);
            return assembly;
#else
            byte[] assBytes = null;
            byte[] mdbBytes = null;

            string url = Application.streamingAssetsPath + "/AssetBundles/" + SysUtil.GetPlatformName() + "/assets/bundles/code/hotfix.dll";
            if (EnvCheckInit.NeedSyncWithServer)
            {
                string updateDir = Application.persistentDataPath + "/AssetBundles/" + SysUtil.GetPlatformName() + "/assets/bundles/code/hotfix.dll";
                if (File.Exists(updateDir))
                {
                    url = updateDir;
                }
            }
            AssetBundle ab = AssetBundle.LoadFromFile(url);
            if (ab != null)
            {
                UnityEngine.Object[] objs = ab.LoadAllAssets();
                if (objs.Length == 0)
                {
                    SampleDebuger.LogError("Load hotfix.dll error");
                    return null;
                }

                TextAsset txt = objs[0] as TextAsset;

                //获取二进制数据的字节数组
                assBytes = txt.bytes;
            }
            Assembly assembly = Assembly.Load(assBytes, mdbBytes);
            return assembly;
#endif


        }
#endif

        public static Type[] GetHotfixTypes()
		{
#if ILRuntime
			ILRuntime.Runtime.Enviorment.AppDomain appDomain = Init.Instance.AppDomain;
			if (appDomain == null)
			{
				return new Type[0];
			}

			return appDomain.LoadedTypes.Values.Select(x => x.ReflectionType).ToArray();
#else
			if (ObjectEvents.Instance.HotfixAssembly == null)
			{
				return new Type[0];
			}
			return ObjectEvents.Instance.HotfixAssembly.GetTypes();
#endif
		}

		public static Type[] GetMonoTypes()
		{
			List<Type> types = new List<Type>();
			foreach (Assembly assembly in ObjectEvents.Instance.GetAll())
			{
				types.AddRange(assembly.GetTypes());
			}
			return types.ToArray();
		}

#if ILRuntime
		public static IMethod[] GetMethodInfo(string typeName)
		{
			ILRuntime.Runtime.Enviorment.AppDomain appDomain = Init.Instance.AppDomain;
			if (appDomain == null)
			{
				return new IMethod[0];
			}
			
			return appDomain.GetType(typeName).GetMethods().ToArray();
		}

		public static IType GetType(string typeName)
		{
			return Init.Instance.AppDomain.GetType(typeName);
		}
#endif
	}
}
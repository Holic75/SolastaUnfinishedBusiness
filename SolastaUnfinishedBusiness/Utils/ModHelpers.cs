using System;
using System.Linq;
using System.Reflection;

namespace SolastaUnfinishedBusiness.Utils
{
    internal static class ModHelpers
    {
        internal static Assembly GetModAssembly(string modName)
        {
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.Contains(modName));
        }

        internal static Type GetModType(string modName, string typeName)
        {
            return GetModAssembly(modName)?.GetExportedTypes().FirstOrDefault(x => x.FullName.Contains(typeName));
        }

        internal static bool SetModField(string modName, string typeName, string fieldName, object value)
        {
            var type = GetModType(modName, typeName);

            if (type != null)
            {
                var fieldInfo = type.GetField(fieldName);

                if (fieldInfo != null)
                {
                    try
                    {
                        fieldInfo.SetValue(type, value);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }

                }
            }

            return false;
        }
    }
}
using UnityEditor;
using UnityEditor.Compilation;

namespace Vengadores.Utility.RecompileEverything.Editor
{
    public class RecompileEverything
    {
        [MenuItem("Smashlab/Utility/Recompile Everything", false, 5000)]
        public static void Run()
        {
#if UNITY_2021_1_OR_NEWER
            CompilationPipeline.RequestScriptCompilation(RequestScriptCompilationOptions.CleanBuildCache);         
#elif UNITY_2019_3_OR_NEWER
            CompilationPipeline.RequestScriptCompilation();
#elif UNITY_2017_1_OR_NEWER
             var editorAssembly = System.Reflection.Assembly.GetAssembly(typeof(Editor));
             var editorCompilationInterfaceType = editorAssembly?.GetType("UnityEditor.Scripting.ScriptCompilation.EditorCompilationInterface");
             var dirtyAllScriptsMethod = editorCompilationInterfaceType?.GetMethod("DirtyAllScripts", BindingFlags.Static | BindingFlags.Public);
             dirtyAllScriptsMethod?.Invoke(editorCompilationInterfaceType, null);
#endif
        }
    }
}
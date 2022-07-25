namespace Maroontress.SolutionFileEditor
{
    using System;
    using System.Reflection;
    using System.Collections.Generic;
    using Microsoft.Unity.VisualStudio.Editor;
    using Unity.CodeEditor;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    public sealed class CodeEditorGimmick
    {
        public static readonly Type vsEditorType = typeof(VisualStudioEditor);

        static CodeEditorGimmick()
        {
            var editor = CodeEditor.Editor;
            var externalEditor = editor.CurrentCodeEditor;
            if (externalEditor is not VisualStudioEditor)
            {
                Debug.Log("external editor is unexpected type");
                return;
            }
            var typeInfo = typeof(CodeEditor).GetTypeInfo();
            var filedInfo = typeInfo.GetDeclaredField(
                "m_ExternalCodeEditors");
            if (filedInfo is null)
            {
                Debug.Log("filedInfo is null");
                return;
            }
            var list = new List<IExternalCodeEditor>
            {
                new CustomEditor(externalEditor)
            };
            filedInfo.SetValue(editor, list);
        }
    }
}

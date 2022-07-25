namespace Maroontress.SolutionFileEditor
{
    using System.IO;
    using System.Linq;
    using Unity.CodeEditor;
    using UnityEngine;

    public sealed class CustomEditor : IExternalCodeEditor
    {
        public CustomEditor(IExternalCodeEditor parent)
        {
            Parent = parent;
        }

        private IExternalCodeEditor Parent { get; }

        public CodeEditor.Installation[] Installations
            => Parent.Installations;

        public void Initialize(string editorInstallationPath)
            => Parent.Initialize(editorInstallationPath);

        public void OnGUI()
            => Parent.OnGUI();

        public bool OpenProject(
            string filePath = "", int line = -1, int column = -1)
        {
            var b = Parent.OpenProject(filePath, line, column);
            var baseDir = Directory.GetParent(Application.dataPath).FullName;
            var dotEditorConfigFile = Path.Combine(baseDir, ".editorconfig");
            if (!File.Exists(dotEditorConfigFile))
            {
                return b;
            }
            var name = Directory.GetParent(Application.dataPath).Name;
            var slnFile = Path.Combine(baseDir, name + ".sln");
            var text = File.ReadAllText(slnFile);
            var label = "\"Solution Items\"";
            var uuid1 = "\"{2150E333-8FDC-42A3-9474-1A3956D46DE8}\"";
            var uuid2 = "\"{08208B3A-3128-4BD6-B50E-2547307B9680}\"";
            var array = new[]
            {
                "EndProject",
                $"Project({uuid1}) = {label}, {label}, {uuid2}",
                "\tProjectSection(SolutionItems) = preProject",
                "\t\t.editorconfig = .editorconfig",
                "\tEndProjectSection",
                "EndProject",
                "Global",
            };
            var newText = string.Concat(array.Select(i => i + "\r\n"));
            if (!text.Contains(newText))
            {
                File.WriteAllText(
                    slnFile,
                    text.Replace("EndProject\r\nGlobal\r\n", newText));
            }
            return b;
        }

        public void SyncAll()
            => Parent.SyncAll();

        public void SyncIfNeeded(
            string[] addedFiles,
            string[] deletedFiles,
            string[] movedFiles,
            string[] movedFromFiles, 
            string[] importedFiles)
        {
            Parent.SyncIfNeeded(
                addedFiles,
                deletedFiles,
                movedFiles,
                movedFromFiles,
                importedFiles);
        }

        public bool TryGetInstallationForPath(
            string editorPath, out CodeEditor.Installation installation)
                => Parent.TryGetInstallationForPath(
                    editorPath, out installation);
    }
}

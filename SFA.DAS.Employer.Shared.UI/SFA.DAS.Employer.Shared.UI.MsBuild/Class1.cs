#if NET462
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SFA.DAS.Employer.Shared.UI.Demo.Models
{
    public class DecorateWithDirective : Task
    {
        private readonly List<ITaskItem> _processedFiles = new List<ITaskItem>();

        [Required]
        public ITaskItem[] FilesToProcess { get; set; }

        public override bool Execute()
        {
            try
            {                
                return ExecuteCore();                
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
            }
            return false;
        }

        [Output]
        public ITaskItem[] ProcessedFiles
        {
            get
            {
                return _processedFiles.ToArray();
            }
        }

        private bool ExecuteCore()
        {
            Log.LogMessage($"In ExecuteCore. Check files to process.");
            if (FilesToProcess == null || !FilesToProcess.Any())
            {
                Log.LogMessage($"In ExecuteCore. No files exist to process.");
                return true;
            }

            Log.LogMessage($"In ExecuteCore. Files exist. Number of files is {FilesToProcess.Length}");
            foreach (var file in FilesToProcess)
            {
                string filePath = file.GetMetadata("FullPath");
                var contents = File.ReadAllText(filePath);

                if (!contents.StartsWith("#if NET462"))
                {
                    using (var fileStream = File.Create(filePath))
                    using (var writer = new StreamWriter(fileStream))
                    {
                        writer.WriteLine("#if NET462");
                        writer.WriteLine(contents);
                        writer.WriteLine("#endif");
                    }
                }

                //File.Move(filePath, ""); // TODO: move to configured location

                var taskItem = new TaskItem(filePath);

                _processedFiles.Add(taskItem);
            }

            return true;
        }
    }
}
#endif

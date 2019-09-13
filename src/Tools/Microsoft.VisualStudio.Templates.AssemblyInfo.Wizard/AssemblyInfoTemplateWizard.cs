﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using EnvDTE;
using Microsoft;
using Microsoft.VisualStudio.Templates.AssemblyInfo.Wizard;
using Microsoft.VisualStudio.TemplateWizard;

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

public class AssemblyInfoTemplateWizard : IWizard
{
    private const int WrapCommentLineAfter = 80;
    private const string CSharpCommentPrefix = "// ";
    private const string VBCommentPrefix = "' ";

    private static readonly string CSharp_Minimal_AssemblyInfoTemplate = $@"using System.Runtime.InteropServices;

{WrapComment(WizardResources.GenerationInfo, CSharpCommentPrefix)}

{WrapComment(WizardResources.ComVisibleInfo, CSharpCommentPrefix)}
[assembly: ComVisible(false)]

{WrapComment(WizardResources.GuidInfo, CSharpCommentPrefix)}
[assembly: Guid(""$guid1$"")]
";
    
    private static readonly string VB_Minimal_AssemblyInfoTemplate = $@"Import System.Runtime.InteropServices

{WrapComment(WizardResources.GenerationInfo, VBCommentPrefix)}

{WrapComment(WizardResources.ComVisibleInfo, VBCommentPrefix)}
<Assembly: ComVisible(False)> 

{WrapComment(WizardResources.GuidInfo, VBCommentPrefix)}
<Assembly: Guid(""$guid1$"")> 
";

    public void BeforeOpeningFile(ProjectItem projectItem)
    {
    }

    public void ProjectFinishedGenerating(Project project)
    {
    }

    public void RunFinished()
    {
    }

    public void ProjectItemFinishedGenerating(ProjectItem projectItem)
    {
        try
        {
            var prop = projectItem.ContainingProject.Properties.Item("GenerateAssemblyInfo")?.Value;
            if (prop == null) return;

            if (bool.TryParse(prop.ToString(), out bool autoGenerated) && autoGenerated)
            {
                string filePath = projectItem.Properties.Item("FullPath").Value.ToString();

                // if SDK style project, overwrite file with other contents
                if (filePath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                {
                    File.WriteAllText(filePath, CSharp_Minimal_AssemblyInfoTemplate);
                }
                else // we only get called for C# and VB, so no need to check this
                {
                    File.WriteAllText(filePath, VB_Minimal_AssemblyInfoTemplate);
                }
            }
        }
        catch
        {
        }
    }

    public bool ShouldAddProjectItem(string filePath) => true;

    public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
    {
    }

    private static string WrapComment(string text, string commentPrefix)
    {
        commentPrefix = commentPrefix ?? string.Empty;
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        var sb = new StringBuilder();

        sb.Append(commentPrefix);
        int column = commentPrefix.Length;

        foreach (char c in text)
        {
            if (char.IsWhiteSpace(c) && column > WrapCommentLineAfter)
            {
                sb.AppendLine();
                sb.Append(commentPrefix);
                column = commentPrefix.Length;
            }
            else
            {
                sb.Append(c);
                column++;
            }
        }
        sb.AppendLine();

        return sb.ToString();
    }
}

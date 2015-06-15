// <copyright file="TemplateCompletionHandler.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

namespace T4Toolbox.VisualStudio.Editor
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.OLE.Interop;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text.Editor;
    using IServiceProvider = System.IServiceProvider;

    /// <summary>
    /// Initiates completion sessions for Text Templates in Visual Studio editors when user is typing directives.
    /// </summary>
    internal sealed class TemplateCompletionHandler : IOleCommandTarget
    {
        internal ICompletionBroker CompletionBroker;
        internal IOleCommandTarget NextHandler;
        internal IServiceProvider ServiceProvider;
        internal ITextView TextView;

        private ICompletionSession completionSession;

        public int Exec(ref Guid commandGroup, uint command, uint options, IntPtr input, IntPtr output)
        {
            Debug.Assert(this.CompletionBroker != null, "completionBroker");
            Debug.Assert(this.NextHandler != null, "nextHandler");
            Debug.Assert(this.ServiceProvider != null, "serviceProvider");
            Debug.Assert(this.TextView != null, "textView");

            if (VsShellUtilities.IsInAutomationFunction(this.ServiceProvider))
            {
                return this.NextHandler.Exec(ref commandGroup, command, options, input, output);
            }

            // Commit or dismiss the current completion session
            if (this.completionSession != null && !this.completionSession.IsDismissed)
            {
                if (commandGroup == VSConstants.VSStd2K && 
                    (command == (uint)VSConstants.VSStd2KCmdID.RETURN || command == (uint)VSConstants.VSStd2KCmdID.TAB))
                {
                    if (this.completionSession.SelectedCompletionSet.SelectionStatus.IsSelected)
                    {
                        this.completionSession.Commit();
                        return VSConstants.S_OK;
                    }

                    this.completionSession.Dismiss();
                }
            }

            // Execute next handler to pass the command to the text buffer
            int result = this.NextHandler.Exec(ref commandGroup, command, options, input, output);

            // Trigger new or filter the current completion session
            if (commandGroup == VSConstants.VSStd2K)
            {
                if (command == (uint)VSConstants.VSStd2KCmdID.TYPECHAR && char.IsLetter((char)(ushort)Marshal.GetObjectForNativeVariant(input)))
                {
                    if (this.completionSession == null)
                    {
                        this.completionSession = this.CompletionBroker.TriggerCompletion(this.TextView);

                        // completion session may not have been created, perhaps because there are no completion sets at current caret position?
                        if (this.completionSession != null) 
                        {
                            this.completionSession.Dismissed += this.CompletionSessionDismissed;
                        }
                    }

                    if (this.completionSession != null && !this.completionSession.IsDismissed)
                    {
                        this.completionSession.Filter();
                    }
                }

                if (command == (uint)VSConstants.VSStd2KCmdID.BACKSPACE || command == (uint)VSConstants.VSStd2KCmdID.DELETE)
                {
                    if (this.completionSession != null && !this.completionSession.IsDismissed)
                    {
                        this.completionSession.Filter();
                    }
                }
            }

            return result;
        }

        public int QueryStatus(ref Guid commandGroup, uint numberOfCommands, OLECMD[] commands, IntPtr commandText)
        {
            Debug.Assert(this.NextHandler != null, "nextHandler");
            return this.NextHandler.QueryStatus(ref commandGroup, numberOfCommands, commands, commandText);
        }

        private void CompletionSessionDismissed(object sender, EventArgs e)
        {
            this.completionSession.Dismissed -= this.CompletionSessionDismissed;
            this.completionSession = null;
        }
    }
}
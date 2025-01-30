using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MVVM.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Threading;
using WPF_MoveWindow.Properties;

namespace WPF_MoveWindow.Models
{
    enum EFeedback
    {
        fbNoTarget,
        fbWinRdyIntact,
        fbFoundTarget,
        fbObjNotResponding,
        fbExceptionOcc,
        fbWndNotMbl,
        fbClWndnla,
        fbClWndcnbM,
        fbObjNotSupPattern,
        fbObjSupPattern,
        fdWinMovedto,
        fdKoorEqualtoReq,
        fdKWouldWinOffscreen
    }
    public partial class MoveWindowModel:NotificationObjectCT, IMoveWindowModel
    {
        private AutomationElement? _targetWindow;
        private TransformPattern? _transformPattern;
        private WindowPattern? _windowPattern;

        private delegate void FeedbackDelegate(string message);

        private delegate void ClientControlsDelegate(object src);


        [ObservableProperty]
        private Point _targetLocation;

        [ObservableProperty]
        private bool _EnableKoorInput;

        [ObservableProperty]
        ObservableCollection<string> _feedBackList=new();

        public MoveWindowModel()
        {
            try
            {
                // Obtain an AutomationElement from the target window handle.
                _targetWindow = StartTargetApp(Resources.TargetApplication);

                // Does the automation element exist?
                if (_targetWindow == null)
                {
                    Feedback(EFeedback.fbNoTarget);
                    return;
                }
                Feedback(EFeedback.fbFoundTarget);

                // find current location of our window
                TargetLocation = _targetWindow.Current.BoundingRectangle.Location;

                // Obtain required control patterns from our automation element
                _windowPattern = GetControlPattern(_targetWindow,
                    WindowPattern.Pattern) as WindowPattern;

                if (_windowPattern == null) return;

                // Make sure our window is usable.
                // WaitForInputIdle will return before the specified time 
                // if the window is ready.
                if (false == _windowPattern.WaitForInputIdle(10000))
                {
                    Feedback(EFeedback.fbObjNotResponding);
                    return;
                }
                Feedback(EFeedback.fbWinRdyIntact);

                // Register for required events
                RegisterForEvents(
                    _targetWindow, WindowPattern.Pattern, TreeScope.Element);

                // Obtain required control patterns from our automation element
                _transformPattern =
                    GetControlPattern(_targetWindow, TransformPattern.Pattern)
                        as TransformPattern;

                if (_transformPattern == null) return;

                // Is the TransformPattern object moveable?
                if (_transformPattern.Current.CanMove)
                {
                    // Enable our WindowMove fields
                    EnableKoorInput = true;

                    // Move element
                    _transformPattern.Move(0, 0);
                }
                else
                {
                    Feedback(EFeedback.fbWndNotMbl);
                }
            }
            catch (ElementNotAvailableException)
            {
                Feedback(EFeedback.fbClWndnla);
            }
            catch (InvalidOperationException)
            {
                Feedback(EFeedback.fbClWndcnbM);
            }
            catch (Exception exc)
            {
                Feedback(EFeedback.fbExceptionOcc, exc.ToString());
            }
        }

        private void Feedback(EFeedback fb,params string[] sMsg)
        {
            string? sText = Resources.ResourceManager.GetString(fb.ToString(), Resources.Culture);
            if (sText is null) return;
            if (sMsg.Length == 0)
                FeedBackList.Add(sText);
            else
                FeedBackList.Add(string.Format(CultureInfo.CurrentCulture,sText, sMsg));
        }

        /// <summary>
        ///     Starts the target application.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        /// <returns>The target automation element.</returns>
       // Obsolete: [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private AutomationElement? StartTargetApp(string app)
        {
            try
            {
                // Start application.
                var p = Process.Start(app);
                if (p.WaitForInputIdle(20000))
                    Feedback(EFeedback.fbWinRdyIntact);
                else return null;

                // Give application a second to startup.
                Thread.Sleep(2000);

                // Return the automation element
                return AutomationElement.FromHandle(p.MainWindowHandle);
            }
            catch (ArgumentException e)
            {
                Feedback(EFeedback.fbExceptionOcc, e.ToString());
                return null;
            }
            catch (Win32Exception e)
            {
                Feedback(EFeedback.fbExceptionOcc, e.ToString());
                return null;
            }
        }

        /// <summary>
        ///     Gets a specified control pattern.
        /// </summary>
        /// <param name="ae">
        ///     The automation element we want to obtain the control pattern from.
        /// </param>
        /// <param name="ap">The control pattern of interest.</param>
        /// <returns>A ControlPattern object.</returns>
        private object? GetControlPattern(
            AutomationElement ae, AutomationPattern ap)
        {
            object? oPattern = null;

            if (false == ae.TryGetCurrentPattern(ap, out oPattern))
            {
                Feedback(EFeedback.fbObjNotSupPattern, ap.ProgrammaticName );
                return null;
            }

            Feedback(EFeedback.fbObjSupPattern, ap.ProgrammaticName);

            return oPattern;
        }

        /// <summary>
        ///     Register for events of interest.
        /// </summary>
        /// <param name="ae">The automation element of interest.</param>
        /// <param name="ap">The control pattern of interest.</param>
        /// <param name="ts">The tree scope of interest.</param>
        private void RegisterForEvents(AutomationElement ae,
            AutomationPattern ap, TreeScope ts)
        {
            if (ap.Id == WindowPattern.Pattern.Id)
            {
                // The WindowPattern Exposes an element's ability 
                // to change its on-screen position or size.

                // The following code shows an example of listening for the 
                // BoundingRectangle property changed event on the window.
          //      Feedback("Start listening for WindowMove events for the control.");

                // Define an AutomationPropertyChangedEventHandler delegate to 
                // listen for window moved events.
                var moveHandler =
                    new AutomationPropertyChangedEventHandler(OnWindowMove);

                Automation.AddAutomationPropertyChangedEventHandler(
                    ae, ts, moveHandler,
                    AutomationElement.BoundingRectangleProperty);
            }
        }

        /// <summary>
        ///     Update client controls based on target location changes.
        /// </summary>
        /// <param name="src">The object that raised the event.</param>
        private void UpdateClientControls(object src)
        {
            // If window is minimized, no need to report new screen coordinates
            if (_windowPattern?.Current.WindowVisualState == WindowVisualState.Minimized)
                return;

            var ptCurrent =
                ((AutomationElement)src).Current.BoundingRectangle.Location;
            if (TargetLocation != ptCurrent )
            {
                Feedback(EFeedback.fdWinMovedto, TargetLocation.ToString() , ptCurrent.ToString());
                TargetLocation = ptCurrent;
            }
        }


        /// <summary>
        ///     Window move event handler.
        /// </summary>
        /// <param name="src">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void OnWindowMove(object src, AutomationPropertyChangedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(DispatcherPriority.Send,
                               new ClientControlsDelegate(UpdateClientControls), src);
            // Pass the same function to BeginInvoke.
        }

        /// <summary>
        ///     Handles the 'Move' button invoked event.
        ///     By default, the Move method does not allow an object
        ///     to be moved completely off-screen.
        /// </summary>
        /// <param name="src">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        [RelayCommand]
        private void MoveBtn()
        {
            try
            {
                var x=TargetLocation.X;
                var y=TargetLocation.Y;
                // Should validate the requested screen location
                if ((x < 0) ||
                    (x >= (SystemParameters.WorkArea.Width -
                           _targetWindow?.Current.BoundingRectangle.Width)))
                {
                    Feedback(EFeedback.fdKWouldWinOffscreen,"X");
                }

                if ((y < 0) ||
                    (y >= (SystemParameters.WorkArea.Height -
                           _targetWindow?.Current.BoundingRectangle.Height)))
                {
                    Feedback(EFeedback.fdKWouldWinOffscreen, "Y");
                }
                 

                // transformPattern was obtained from the target window.
                _transformPattern?.Move(TargetLocation.X, TargetLocation.Y);
            }
            catch (ElementNotAvailableException)
            {
                Feedback(EFeedback.fbClWndnla);
            }
            catch (InvalidOperationException)
            {
                Feedback(EFeedback.fbClWndcnbM);
            }
        }

    }
}

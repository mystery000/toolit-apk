using System.Diagnostics;
using System.Linq;
using System.Timers;
using Foundation;
using Toolit.Resourses;
using UIKit;
using Xamarin.Forms;

namespace Toolit.iOS
{
    [Register(nameof(TouchEventUIApplication))]
    public class TouchEventUIApplication : UIApplication
    {
        private static Timer _inactivityTimer;
        private static bool _isTimerConnected;

        public override void SendEvent(UIEvent uiEvent)
        {
            base.SendEvent(uiEvent);

            if (uiEvent.AllTouches != null &&
                uiEvent.AllTouches.Any(evnt => (evnt as UITouch)?.Phase == UITouchPhase.Began))
            {
                ResetTimer();
            }
        }

        internal void InitTimer()
        {
            _inactivityTimer = new Timer(AppConstants.SessionTimeoutDuration);
            if (!_isTimerConnected) // disallow multiple subscriptions
            {
                _inactivityTimer.Elapsed += InactivityTimer_Elapsed;
                _isTimerConnected = true;
            }

            _inactivityTimer.Start();

            Debug.WriteLine("Inactivity timer created");
        }

        private void ResetTimer()
        {
            if (_inactivityTimer == null)
            {
                InitTimer();
            }
            else
            {
                // this effectively resets the timer and starts it again
                _inactivityTimer.Stop();
                _inactivityTimer.Start();

                Debug.WriteLine("Inactivity timer reset");
            }
        }


        private void InactivityTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // generic parameter passed to the method doesn't have to be the sender class
            // TODO: uncomment if bankid's auto sign out rule will be followed
            //MessagingCenter.Send<object>(this, AppConstants.SessionTimeoutMessage);
            Debug.WriteLine("Inactivity timer elapsed");
        }
    }
}
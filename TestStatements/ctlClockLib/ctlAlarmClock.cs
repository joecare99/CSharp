using System;
using System.Drawing;
using System.ComponentModel;

namespace ctlClockLib
{
    /// <summary>
    /// Class ctlAlarmClock.
    /// Implements the <see cref="ctlClock" />
    /// </summary>
    /// <seealso cref="ctlClock" />
    public partial class ctlAlarmClock : ctlClock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ctlAlarmClock"/> class.
        /// </summary>
        public ctlAlarmClock()
        {
            InitializeComponent();
        }

        private DateTime dteAlarmTime;
        private bool blnAlarmSet;
        private bool blnColorTicker;

        // These properties will be declared as public to allow future
        // developers to access them.
        /// <summary>
        /// Gets or sets the alarm time.
        /// </summary>
        /// <value>The alarm time.</value>

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]

        // These properties will be declared as public to allow future
        // developers to access them.
        /// <summary>
        /// Gets or sets the alarm time.
        /// </summary>
        /// <value>The alarm time.</value>

        public DateTime AlarmTime
        {
            get
            {
                return dteAlarmTime;
            }
            set
            {
                dteAlarmTime = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [alarm set].
        /// </summary>
        /// <value><c>true</c> if [alarm set]; otherwise, <c>false</c>.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public bool AlarmSet
        {
            get
            {
                return blnAlarmSet;
            }
            set
            {
                blnAlarmSet = value;
            }
        }

        protected override void timer1_Tick(object sender, EventArgs e)
        {
            // Calls the Timer1_Tick method of ctlClock.
            base.timer1_Tick(sender, e);
            // Checks to see if the alarm is set.
            if (AlarmSet == false)
                return;
            else
            // If the date, hour, and minute of the alarm time are the same as
            // the current time, flash an alarm.
            {
                if (AlarmTime.Date == DateTime.Now.Date && AlarmTime.Hour ==
                    DateTime.Now.Hour && AlarmTime.Minute == DateTime.Now.Minute)
                {
                    // Sets lblAlarmVisible to true, and changes the background color based on
                    // the value of blnColorTicker. The background color of the label
                    // will flash once per tick of the clock.
                    lblAlarm.Visible = true;
                    if (blnColorTicker == false)
                    {
                        lblAlarm.BackColor = Color.Red;
                        blnColorTicker = true;
                    }
                    else
                    {
                        lblAlarm.BackColor = Color.Blue;
                        blnColorTicker = false;
                    }
                }
                else
                {
                    // Once the alarm has sounded for a minute, the label is made
                    // invisible again.
                    lblAlarm.Visible = false;
                }
            }
        }

        private void btnAlarmOff_Click(object sender, EventArgs e)
        {
            // Turns off the alarm.
            AlarmSet = false;
            // Hides the flashing label.
            lblAlarm.Visible = false;
        }
    }
}

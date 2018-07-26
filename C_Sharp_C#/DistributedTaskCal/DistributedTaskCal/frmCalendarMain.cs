using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DistributedTaskCal.Model;

namespace DistributedTaskCal
{
    public partial class frmCalendarMain : Form
    {
        public static List<CalEvent> CalEventList = new List<CalEvent>();
        public Random objRand = new Random(1);
        public static string notifyStatus = "";

        public frmCalendarMain()
        {
            InitializeComponent();
            ClearALL();
        }

        // FUnction for clearing data from all the fields of the UI
        private void ClearALL()
        {
            CalendarMonEv.RemoveAllBoldedDates();
            CalendarMonEv.RemoveAllMonthlyBoldedDates();
            txtTitle.Text = "";
            txtLoc.Text = "";
            cmbEventType.DataSource = Enum.GetValues(typeof(CalEventType));
            dtStart.Value = DateTime.Now;
            dtEnd.Value = DateTime.Now;
            lblNotify.Text = notifyStatus;
            listEvents.DataSource = null;
        }

        // Function to add event
        private CalEvent AddCalEvent()
        {
            CalEvent objCalEvent = new CalEvent();

            try
            {
                objCalEvent.ID = objRand.Next(1, 50000);
                objCalEvent.EventType = (CalEventType)Enum.Parse(typeof(CalEventType), cmbEventType.SelectedValue.ToString());
                objCalEvent.Title = txtTitle.Text;
                //objCalEvent.Description = txtDesc.Text;            
                objCalEvent.Location = txtLoc.Text;
                objCalEvent.DateofEvent = CalendarMonEv.SelectionRange.Start;                
                objCalEvent.IsAllDay = chkAllDay.Checked;

                if(!chkAllDay.Checked)
                {
                    objCalEvent.StartTime = dtStart.Value.TimeOfDay;
                    objCalEvent.EndTime = dtEnd.Value.TimeOfDay;
                }

                CalEventList.Add(objCalEvent);
            }
            catch (Exception ex)
            {
                objCalEvent = new CalEvent();
                lblError.Text = ex.Message;
            }

            return objCalEvent;
        }

        // Function for sending protobuf object
        private void SendDistEvent(CalEvent objCalEvent)
        {
            DistCalendarEvent objDistEvent = new DistCalendarEvent();

            try
            { 
                objDistEvent.id = objCalEvent.ID;
                objDistEvent.type = (DistCalendarEvent.EventType)Enum.Parse(typeof(DistCalendarEvent.EventType), objCalEvent.EventType.ToString());
                objDistEvent.title = objCalEvent.Title;
                objDistEvent.location = objCalEvent.Location;
                objDistEvent.date = (long)(objCalEvent.DateofEvent - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;

                if (objCalEvent.IsAllDay == false)
                {
                    objDistEvent.start_time = objCalEvent.StartTime.ToString();
                    objDistEvent.end_time = objCalEvent.EndTime.ToString();
                }
                else
                {
                    objDistEvent.allday = objCalEvent.IsAllDay;
                }

                Program.calendarPub.SendObject(objDistEvent);
            }
            catch (Exception ex)
            {
                objDistEvent = new DistCalendarEvent();
                lblError.Text = ex.Message;
            }
        }

        // Function for receiving protobuf object
        public static void ReceiveDistEvent(DistCalendarEvent distEventObj)
        {
            CalEvent newCalEventObj = new CalEvent();
            try
            {
                // Getting values from the rcv object
                newCalEventObj.ID = distEventObj.id;
                newCalEventObj.EventType = (CalEventType)Enum.Parse(typeof(CalEventType), distEventObj.type.ToString());
                newCalEventObj.Title = distEventObj.title;
                newCalEventObj.Location = distEventObj.location;
                newCalEventObj.IsAllDay = distEventObj.allday;

                if (distEventObj.date != 0)
                {
                    long milliSec = distEventObj.date;
                    DateTime time = new DateTime(1970, 1, 1, 0, 0, 0);
                    DateTime currentTime = time.AddMilliseconds(milliSec);
                    newCalEventObj.DateofEvent = currentTime;
                }

                if (distEventObj.allday == false)
                {
                    newCalEventObj.StartTime = DateTime.Parse(distEventObj.start_time).TimeOfDay;
                    newCalEventObj.EndTime = DateTime.Parse(distEventObj.end_time).TimeOfDay;
                }

                CalEventList.Add(newCalEventObj);

                notifyStatus = "Event Notification Received! ";
            }
            catch (Exception ex)
            {
                notifyStatus = ex.Message;
            }
        }

        // Loading all created events in the Calendar
        private void LoadEvents()
        {
            try
            {
                if(CalEventList.Count > 0)
                {
                    foreach (CalEvent objEventItem in CalEventList)
                    {
                        CalendarMonEv.SelectionStart = objEventItem.DateofEvent;
                        CalendarMonEv.SelectionEnd = objEventItem.DateofEvent;
                        CalendarMonEv.AddBoldedDate(objEventItem.DateofEvent);
                        CalendarMonEv.UpdateBoldedDates();
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }

        // Button Event handler for Adding event
        private void btnAdd_Click(object sender, EventArgs e)
        {
           CalEvent objCalEV = AddCalEvent();
           SendDistEvent(objCalEV);
           ClearALL();
           LoadEvents();
           lblNotify.Text = "Event Created and Notification sent!";
        }

        // Button Event handler for Refreshing fields of events
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ClearALL();
            LoadEvents();
            lblNotify.Text = notifyStatus;
        }

        // check for All Day event
        private void chkAllDay_CheckedChanged(object sender, EventArgs e)
        {
            if(chkAllDay.Checked == true)
            {
                dtStart.Enabled = false;
                dtEnd.Enabled = false;
            }
            else
            {
                dtStart.Enabled = true;
                dtEnd.Enabled = true;
            }
        }

        // Event handler for selecting individual event from Calendar
        private void CalendarMonEv_DateSelected(object sender, DateRangeEventArgs e)
        {
            try
            {
                if (CalEventList.Count > 0)
                {
                    List<CalEvent> eventObj = CalEventList.Where(d => d.DateofEvent.Day == e.Start.Day).ToList();

                    listEvents.DataSource = eventObj;
                    listEvents.DisplayMember = "Title";
                    listEvents.ValueMember = "ID";
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }

        // Event Handler for selection of individual events from the Listbox
        private void listEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Get the currently selected item from the List of Events
                CalEvent curObjEvent = (CalEvent)listEvents.SelectedItem;

                // Get the event details and show in the fields
                if (curObjEvent != null)
                {
                    cmbEventType.DataSource = Enum.GetValues(typeof(CalEventType));
                    cmbEventType.SelectedItem = (CalEventType)curObjEvent.EventType;
                    txtTitle.Text = curObjEvent.Title;
                    txtLoc.Text = curObjEvent.Location;
                    dtStart.Value = new DateTime(curObjEvent.DateofEvent.Year, curObjEvent.DateofEvent.Month, curObjEvent.DateofEvent.Day, curObjEvent.StartTime.Hours, curObjEvent.StartTime.Minutes, curObjEvent.StartTime.Seconds);
                    dtEnd.Value = new DateTime(curObjEvent.DateofEvent.Year, curObjEvent.DateofEvent.Month, curObjEvent.DateofEvent.Day, curObjEvent.EndTime.Hours, curObjEvent.EndTime.Minutes, curObjEvent.EndTime.Seconds);
                    chkAllDay.Checked = curObjEvent.IsAllDay;
                }
                else
                {
                    ClearALL();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }
    }
}

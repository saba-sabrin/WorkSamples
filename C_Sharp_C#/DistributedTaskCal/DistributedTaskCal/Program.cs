using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;
using org.umundo;
using org.umundo.s11n;
using org.umundo.core;
using DistributedTaskCal.Model;

namespace DistributedTaskCal
{
    public class CalendarReceiver : ITypedReceiver
    {
        public void ReceiveObject(object obj, org.umundo.core.Message msg)
        {
            DistCalendarEvent distEventObj = (DistCalendarEvent)obj;

            if(distEventObj != null)
            {
                frmCalendarMain.ReceiveDistEvent(distEventObj);
                frmCalendarMain.notifyStatus = "Event Object Received!";
            }
        }
    }

    public static class Program
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern void SetDllDirectory(string lpPathName);

        public static Discovery disc;
        public static Node calendarNode;
        public static TypedSubscriber calendarSub;
        public static TypedPublisher calendarPub;
        public static CalendarReceiver calendarRcv;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

            string dllPath = projectPath + "\\dlls";

            if (System.Environment.Is64BitProcess)
            {
                SetDllDirectory(dllPath + "\\csharp64");
            }
            else
            {
                SetDllDirectory(dllPath + "\\csharp");
            }

            calendarNode = new Node();
            calendarRcv = new CalendarReceiver();
            calendarSub = new TypedSubscriber("s11nCalendar");
            calendarSub.setReceiver(calendarRcv);
            calendarPub = new TypedPublisher("s11nCalendar");
            
            DistCalendarEvent eventObj = new DistCalendarEvent();
            calendarSub.RegisterType(eventObj.GetType().Name, eventObj.GetType());

            calendarNode.addPublisher(calendarPub);
            calendarNode.addSubscriber(calendarSub);

            disc = new Discovery(Discovery.DiscoveryType.MDNS);
            disc.add(calendarNode);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmCalendarMain());

            calendarNode.removePublisher(calendarPub);
            calendarNode.removeSubscriber(calendarSub);
            disc.remove(calendarNode);
            System.GC.Collect();
        }
    }
}

using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using GT = Gadgeteer;
using SmartSecSystem;

namespace SmartSecSystem.Views
{
    public class IdleTimeView : SuperView
    {
        private Text txtTime;
        private Text txtDate;

        public IdleTimeView()
        {
            txtTime = new Text()
            {
                Font = Resources.GetFont(Resources.FontResources.small),
                Width = 320,
                TextAlignment = TextAlignment.Center,
                ForeColor = GT.Color.White,
                TextContent = DateTime.Now.ToString("HH:mm:ss")
            };
            this.AddElement(txtTime, 40, 0);

            txtDate = new Text()
            {
                Font = Resources.GetFont(Resources.FontResources.small),
                Width = 320,
                TextAlignment = TextAlignment.Center,
                ForeColor = GT.Color.White,
                TextContent = DateTime.Now.ToString("dd/MM/yyyy")
            };
            this.AddElement(txtDate, 80, 0);
        }

        public void UpdateTime()
        {
            txtTime.TextContent = Program.currentTime;
        }
    }
}

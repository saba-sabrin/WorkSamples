using System;
using System.Runtime.CompilerServices;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation;

using GT = Gadgeteer;
using Microsoft.SPOT.Input;

namespace SmartSecSystem.Views
{
    public class StartupView : SuperView
    {
        private Text txtMessage;
        private Image img_icon;
        private Canvas canvasMenu;

        public StartupView()
        {
            SetTitle("Smart Security System", Resources.GetFont(Resources.FontResources.NinaB), GT.Color.Black, GT.Color.LightGray, GT.Color.Green);

            // Set image for wait
            Bitmap bm = new Bitmap(Resources.GetBytes(Resources.BinaryResources.wait), Bitmap.BitmapImageType.Jpeg);
            img_icon = new Image(bm);
            this.AddElement(img_icon, 90, 135);

            txtMessage = new Text()
            {
                Font = Resources.GetFont(Resources.FontResources.small),
                ForeColor = Color.Black,
                Width = 320,
                TextAlignment = TextAlignment.Center,
                TextContent = "Initializing system..."
            };

            this.AddElement(txtMessage, 140, 0);
        }

        private void CreateMenuItem(Bitmap bitmap, TouchEventHandler handler,
            int iconMarginLeft, string labelContent, int labelMarginLeft)
        {
            var image = new Image(bitmap);
            image.TouchDown += handler;
            canvasMenu.Children.Add(image, 0, iconMarginLeft);

            var label = new Text()
            {
                Font = Resources.GetFont(Resources.FontResources.NinaB),
                Width = 120,
                TextAlignment = TextAlignment.Center,
                TextWrap = true,
                TextContent = labelContent
            };
            label.TouchDown += handler;
            canvasMenu.Children.Add(label, 54, labelMarginLeft);
        }

        public void SetMessage(string msg)
        {
            txtMessage.TextContent = msg;
        }
    }
}

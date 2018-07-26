/* Copyright 2016 Saba Sabrin, saba.sabrin@gmail.com

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation;

using GT = Gadgeteer;

namespace SmartSecSystem.Views
{
    public abstract class SuperView : Canvas
    {
        private const int width = 320;
        private const int height = 40;

        // Setting different brush types depending on the color for backgroud 
        public Border SetTitle(string title, Font font, GT.Color foreColor, GT.Color startColor, GT.Color endColor)
        {
            Brush backgroundBrush = null;

            if (startColor == endColor)
                backgroundBrush = new SolidColorBrush(startColor);
            else
                backgroundBrush = new LinearGradientBrush(startColor, endColor);

            return SetTitle(title, font, foreColor, backgroundBrush);
        }

        // Setting the main title
        public Border SetTitle(string title, Font font, GT.Color foreColor, Brush backgroundBrush)
        {
            Border titleBar = new Border();
            titleBar.Width = width;
            titleBar.Height = height;
            titleBar.Background = backgroundBrush;

            Text text = new Text(font, title);
            text.Width = width;
            text.ForeColor = foreColor;
            text.SetMargin(5);
            titleBar.Child = text;

            AddElement(titleBar, 0, 0);

            return titleBar;
        }

        public Border AddStatusBar(UIElement element, GT.Color backgroundColor)
        {
            return AddStatusBar(element, height, backgroundColor, backgroundColor);
        }

        public Border AddStatusBar(UIElement element, int height, GT.Color backgroundColor)
        {
            return AddStatusBar(element, height, backgroundColor, backgroundColor);
        }

        public Border AddStatusBar(UIElement element, GT.Color startColor, GT.Color endColor)
        {
            return AddStatusBar(element, height, startColor, endColor);
        }

        public Border AddStatusBar(UIElement element, int height, GT.Color startColor, GT.Color endColor)
        {
            Brush backgroundBrush = null;
            if (startColor == endColor)
                backgroundBrush = new SolidColorBrush(startColor);
            else
                backgroundBrush = new LinearGradientBrush(startColor, endColor);

            return AddStatusBar(element, height, backgroundBrush);
        }

        public Border AddStatusBar(UIElement element, Brush backgroundBrush)
        {
            return AddStatusBar(element, height, backgroundBrush);
        }

        public Border AddStatusBar(UIElement element, int height, Brush backgroundBrush)
        {
            Border statusBar = new Border();
            statusBar.Width = width;
            statusBar.Height = height;
            statusBar.Background = backgroundBrush;

            int left, top, right, bottom;
            element.GetMargin(out left, out top, out right, out bottom);
            left = System.Math.Max(left, 5);
            top = System.Math.Max(top, 5);
            bottom = System.Math.Max(bottom, 5);
            element.SetMargin(left, top, right, bottom);
            statusBar.Child = element;

            AddElement(statusBar, 215, 0);

            return statusBar;
        }

        // Adding UI element as child to the parent UI
        public void AddElement(UIElement element, int top, int left)
        {
            Children.Add(element);
            Canvas.SetTop(element, top);
            Canvas.SetLeft(element, left);
        }
    }
}

using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;

using GT = Gadgeteer;

namespace SmartSecSystem.Views
{
    public static class UIElementExtensions
    {
        public static void Add(this UIElementCollection collection, UIElement element, int top, int left)
        {
            collection.Add(element);
            Canvas.SetTop(element, top);
            Canvas.SetLeft(element, left);
        }
    }
}

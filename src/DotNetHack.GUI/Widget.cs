﻿using DotNetHack.GUI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetHack.GUI
{
    /// <summary>
    /// Widget
    /// </summary>
    public abstract class Widget : IDisposable
    {
        /// <summary>
        /// Create a new widget
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Widget(string text, int x, int y, int width, int height)
        {
            Size = new Size(width, height);
            Location = new Point(x, y);
            Location.X = x;
            Location.Y = y;
            Text = text;
            Widgets = new Stack<Widget>();
            Widgets.Push(this);
            Buffer = new DisplayBuffer(width, height);
        }

        /// <summary>
        /// Create a new Widget
        /// </summary>
        /// <param name="text">the text to initialize the widget with</param>
        /// <param name="region">the screen region for the widget</param>
        public Widget(string text, IScreenRegion region)
            : this(text, region.Location.X, region.Location.Y, region.Width, region.Height)
        {

        }

        /// <summary>
        /// Create a new Widget
        /// </summary>
        /// <param name="text">the text to initialize the widget with</param>
        public Widget(string text)
            : this(text, 0, 0, 0, 0)
        {

        }

        /// <summary>
        /// Buffer
        /// </summary>
        public DisplayBuffer Buffer { get; private set; }

        /// <summary>
        /// InitializeWidget
        /// </summary>
        public virtual void InitializeWidget()
        {

        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="widget">Add a widget</param>
        public void Add(Widget widget)
        {
            Widgets.Push(widget);
        }

        /// <summary>
        /// Width
        /// </summary>
        public int Width
        {
            get { return Size.Width; }
            //set { Size.Width = value; }
        }

        /// <summary>
        /// Height
        /// </summary>
        public int Height 
        {
            get { return Size.Height; }
            //set { Size.Height = value; }
        }

        /// <summary>
        /// Location
        /// </summary>
        public IPoint Location { get; set; }

        /// <summary>
        /// Size
        /// </summary>
        public IDimensional Size { get; set; }

        /// <summary>
        /// Text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// ActiveWidget
        /// </summary>
        public Widget ActiveWidget { get { return Widgets.Peek(); } }

        /// <summary>
        /// Widgets
        /// </summary>
        public Stack<Widget> Widgets { get; set; }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
        }
    }
}

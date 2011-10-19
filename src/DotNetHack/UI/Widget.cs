﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetHack.Game.Interfaces;
using DotNetHack.Game;

namespace DotNetHack.UI
{
    /// <summary>
    /// Left, Right, Center
    /// </summary>
    // public enum HorizontalAlignment { Left, Right, Center }
    /// <summary>
    /// Top, Bottom, Center
    /// </summary>
    // public enum VerticalAlignment { Top, Bottom, Center }

    /// <summary>
    /// 
    /// </summary>
    public enum Align { IOO, OIO, OOI };

    /// <summary>
    /// 
    /// </summary>
    public class Widget
    {
        public Widget() { }

        public event EventHandler OnShow;

        public event EventHandler OnHide;

        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public bool IsVisible { get; set; }

        public virtual void Show()
        {
            IsVisible = true;
            CursorState = CursorState.CurrentCursorState;
            Console.SetCursorPosition(X, Y);
            if (OnShow != null)
                OnShow(this, null);
        }

        public virtual void Hide()
        {
            IsVisible = false;
            if (OnHide != null)
                OnHide(this, null);
        }

        public virtual void Refresh()
        {

        }

        /// <summary>
        /// CursorState
        /// </summary>
        public CursorState CursorState { get; set; }

        /// <summary>
        /// DisplayRegion
        /// </summary>
        public DisplayRegion DisplayRegion
        {
            get { return new DisplayRegion(X, Y, X + Width, Y + Height); }
        }
    }
}
﻿namespace MaterialSkin.Controls
{
    using MaterialSkin.Animations;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    /// <summary>
    /// Defines the <see cref="MaterialFloatingActionButton" />
    /// </summary>
    public class MaterialFloatingActionButton : Button, IMaterialControl
    {
        /// <summary>
        /// Gets or sets the Depth
        /// </summary>
        [Browsable(false)]
        public int Depth { get; set; }

        /// <summary>
        /// Gets the SkinManager
        /// </summary>
        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        /// <summary>
        /// Gets or sets the MouseState
        /// </summary>
        [Browsable(false)]
        public MouseState MouseState { get; set; }

        /// <summary>
        /// Defines the FAB_SIZE
        /// </summary>
        private const int FAB_SIZE = 56;

        /// <summary>
        /// Defines the FAB_MINI_SIZE
        /// </summary>
        private const int FAB_MINI_SIZE = 40;

        /// <summary>
        /// Defines the FAB_ICON_MARGIN
        /// </summary>
        private const int FAB_ICON_MARGIN = 16;

        /// <summary>
        /// Defines the FAB_MINI_ICON_MARGIN
        /// </summary>
        private const int FAB_MINI_ICON_MARGIN = 8;

        /// <summary>
        /// Defines the FAB_ICON_SIZE
        /// </summary>
        private const int FAB_ICON_SIZE = 24;

        /// <summary>
        /// Gets or sets a value indicating whether Mini
        /// </summary>
        public bool Mini
        {
            get { return _mini; }
            set { setSize(value); }
        }

        /// <summary>
        /// Defines the _mini
        /// </summary>
        private bool _mini = false;

        /// <summary>
        /// Gets or sets a value indicating whether AnimateShowHideButton
        /// </summary>
        public bool AnimateShowHideButton
        {
            get { return _animateShowButton; }
            set { _animateShowButton = value; }
        }

        /// <summary>
        /// Defines the _animateShowButton
        /// </summary>
        private bool _animateShowButton;

        /// <summary>
        /// Gets or sets the Icon
        /// </summary>
        public Image Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }

        /// <summary>
        /// Defines the _icon
        /// </summary>
        private Image _icon;

        /// <summary>
        /// Defines the _isHiding
        /// </summary>
        private bool _isHiding = false;

        /// <summary>
        /// Defines the _animationManager
        /// </summary>
        private readonly AnimationManager _animationManager;

        /// <summary>
        /// Defines the _showAnimationManager
        /// </summary>
        private readonly AnimationManager _showAnimationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialFloatingActionButton"/> class.
        /// </summary>
        public MaterialFloatingActionButton()
        {

            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            Size = new Size(FAB_SIZE, FAB_SIZE);
            _animationManager = new AnimationManager(false)
            {
                Increment = 0.03,
                AnimationType = AnimationType.EaseOut
            };
            _animationManager.OnAnimationProgress += sender => Invalidate();

            _showAnimationManager = new AnimationManager(true)
            {
                Increment = 0.1,
                AnimationType = AnimationType.EaseOut
            };
            _showAnimationManager.OnAnimationProgress += sender => Invalidate();
            _showAnimationManager.OnAnimationFinished += _showAnimationManager_OnAnimationFinished;
        }

        /// <summary>
        /// The setSize
        /// </summary>
        /// <param name="mini">The mini<see cref="bool"/></param>
        private void setSize(bool mini)
        {
            Size = mini ? new Size(FAB_MINI_SIZE + 10, FAB_MINI_SIZE + 14) : new Size(FAB_SIZE + 10, FAB_SIZE + 14);
            _mini = mini;
        }

        /// <summary>
        /// The _showAnimationManager_OnAnimationFinished
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        private void _showAnimationManager_OnAnimationFinished(object sender)
        {
            if (_isHiding)
            {
                Visible = false;
                _isHiding = false;
            }
        }

        private const int CS_DROPSHADOW = 0x00020000;
        protected override CreateParams CreateParams
        {
            get
            {
                // add the drop shadow flag for automatically drawing
                // a drop shadow around the form
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        /// <summary>
        /// The OnPaint
        /// </summary>
        /// <param name="pevent">The pevent<see cref="PaintEventArgs"/></param>
        protected override void OnPaint(PaintEventArgs pevent)
        {
            setSize(_mini);

            var g = pevent.Graphics;

            Rectangle fabBounds = _mini ? new Rectangle(4, 7, FAB_MINI_SIZE, FAB_MINI_SIZE) : new Rectangle(4, 7, FAB_SIZE, FAB_SIZE);

            g.Clear(SkinManager.GetApplicationBackgroundColor());
            g.SmoothingMode = SmoothingMode.AntiAlias;

            SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0));
            g.FillEllipse(shadowBrush, new Rectangle(fabBounds.X - 1, fabBounds.Y - 1, fabBounds.Width + 2, fabBounds.Height + 2));
            g.FillEllipse(shadowBrush, new Rectangle(fabBounds.X - 0, fabBounds.Y + 2, fabBounds.Width + 0, fabBounds.Height + 0));
            g.FillEllipse(shadowBrush, new Rectangle(fabBounds.X - 0, fabBounds.Y + 1, fabBounds.Width + 0, fabBounds.Height + 0));

            g.FillEllipse(SkinManager.ColorScheme.AccentBrush, fabBounds);


            if (_animationManager.IsAnimating())
            {
                GraphicsPath regionPath = new GraphicsPath();
                regionPath.AddEllipse(new Rectangle(fabBounds.X - 1, fabBounds.Y - 1, fabBounds.Width + 2, fabBounds.Height + 2));
                Region fabRegion = new Region(regionPath);

                GraphicsContainer gcont = g.BeginContainer();
                g.SetClip(fabRegion, CombineMode.Replace);

                for (int i = 0; i < _animationManager.GetAnimationCount(); i++)
                {
                    var animationValue = _animationManager.GetProgress(i);
                    var animationSource = _animationManager.GetSource(i);
                    var rippleBrush = new SolidBrush(Color.FromArgb((int)(51 - (animationValue * 50)), Color.White));
                    var rippleSize = (int)(animationValue * Width * 2);
                    g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize, rippleSize));
                }

                g.EndContainer(gcont);
            }


            if (Icon != null)
            {
                g.DrawImage(Icon, new Rectangle(fabBounds.Width / 2 - 11 + 4, fabBounds.Height / 2 - 11 + 7, 24, 24));
            }


            if (_showAnimationManager.IsAnimating())
            {
                int target = Convert.ToInt32((_mini ? FAB_MINI_SIZE : FAB_SIZE) * _showAnimationManager.GetProgress());
                fabBounds.Width = target == 0 ? 1 : target;
                fabBounds.Height = target == 0 ? 1 : target;
                fabBounds.X = Convert.ToInt32(((_mini ? FAB_MINI_SIZE : FAB_SIZE) / 2) - (((_mini ? FAB_MINI_SIZE : FAB_SIZE) / 2) * _showAnimationManager.GetProgress()));
                fabBounds.Y = Convert.ToInt32(((_mini ? FAB_MINI_SIZE : FAB_SIZE) / 2) - (((_mini ? FAB_MINI_SIZE : FAB_SIZE) / 2) * _showAnimationManager.GetProgress()));
            }

            // Clip to a round shape with a 1px padding
            GraphicsPath clipPath = new GraphicsPath();
            clipPath.AddEllipse(new Rectangle(fabBounds.X - 2, fabBounds.Y - 2, fabBounds.Width + 4, fabBounds.Height + 5));
            Region = new Region(clipPath);
        }

        /// <summary>
        /// The OnMouseUp
        /// </summary>
        /// <param name="mevent">The mevent<see cref="MouseEventArgs"/></param>
        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            _animationManager.StartNewAnimation(AnimationDirection.In, mevent.Location);
        }

        /// <summary>
        /// Defines the origin
        /// </summary>
        private Point origin;

        /// <summary>
        /// The Hide
        /// </summary>
        public new void Hide()
        {
            if (Visible)
            {
                _isHiding = true;
                _showAnimationManager.StartNewAnimation(AnimationDirection.Out);
            }
        }

        /// <summary>
        /// The Show
        /// </summary>
        public new void Show()
        {
            if (!Visible)
            {
                origin = Location;
                _showAnimationManager.StartNewAnimation(AnimationDirection.In);
                Visible = true;
            }
        }
    }
}
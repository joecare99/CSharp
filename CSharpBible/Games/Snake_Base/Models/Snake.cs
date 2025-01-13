﻿// ***********************************************************************
// Assembly         : Snake_Base
// Author           : Mir
// Created          : 08-25-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Snake.cs" company="JC-Soft">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Model;
using BaseLib.Helper;
using BaseLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using Snake_Base.Models.Interfaces;
using Game_Base.Model;

/// <summary>
/// The Models namespace.
/// </summary>
/// <autogeneratedoc />
namespace Snake_Base.Models
{
    /// <summary>
    /// Class SnakeBodyPart.
    /// Implements the <see cref="Snake_Base.Models.SnakeGameObject" />
    /// Implements the <see cref="BaseLib.Interfaces.IParentedObject{Snake_Base.Models.Snake}" />
    /// </summary>
    /// <seealso cref="Snake_Base.Models.SnakeGameObject" />
    /// <seealso cref="BaseLib.Interfaces.IParentedObject{Snake_Base.Models.Snake}" />
    public class SnakeBodyPart : SnakeGameObject, IParentedObject<Snake>
    {
        #region Properties
        #region private Properties
        /// <summary>
        /// The sn parent
        /// </summary>
        /// <autogeneratedoc />
        private Snake? _snParent;
        #endregion
        /// <summary>
        /// Gets or sets the snake.
        /// </summary>
        /// <value>The snake.</value>
        public Snake? Snake { get => _snParent; set => SetParent(value); }
        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="CallerMember">The caller member.</param>
        public void SetParent(Snake? value, [CallerMemberName] string CallerMember = "") => value.SetProperty(ref _snParent, null, CallerMember);

        /// <summary>
        /// Gets or sets the next part.
        /// </summary>
        /// <value>The next part.</value>
        public SnakeBodyPart? NextPart { get; set; } = null;

        /// <summary>
        /// Gets or sets the previous part.
        /// </summary>
        /// <value>The next part.</value>
        public SnakeBodyPart? PrevPart { get; set; }= null;
        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="SnakeBodyPart" /> class.
        /// </summary>
        public SnakeBodyPart() : this(null) { }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <returns>System.Nullable&lt;Snake&gt;.</returns>
        /// <autogeneratedoc />
        Snake? IParentedObject<Snake>.GetParent() => _snParent;
        /// <summary>
        /// Initializes a new instance of the <see cref="SnakeBodyPart" /> class.
        /// </summary>
        /// <param name="snake">The snake.</param>
        public SnakeBodyPart(Snake? snake):base()
        {
            _snParent = snake;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnakeBodyPart" /> class.
        /// </summary>
        /// <param name="place">The place.</param>
        /// <param name="snake">The snake.</param>
        /// <param name="playfield">The playfield.</param>
        public SnakeBodyPart(Point place,Snake? snake, IPlayfield2D<ISnakeGameObject>? playfield = null) :base(place,playfield)
        {
            _snParent = snake;
        }
        #endregion
    }

    /// <summary>
    /// Class SnakeHead.
    /// Implements the <see cref="Snake_Base.Models.SnakeBodyPart" />
    /// </summary>
    /// <seealso cref="Snake_Base.Models.SnakeBodyPart" />
    public class SnakeHead : SnakeBodyPart
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SnakeHead" /> class.
        /// </summary>
        /// <param name="snake">The snake.</param>
        public SnakeHead(Snake? snake = null) : base(snake) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SnakeHead" /> class.
        /// </summary>
        /// <param name="place">The place.</param>
        /// <param name="snake">The snake.</param>
        /// <param name="playfield">The playfield.</param>
        public SnakeHead(Point place, Snake? snake, IPlayfield2D<ISnakeGameObject>? playfield = null):base(place,snake,playfield) { }
    }

    /// <summary>
    /// Class SnakeTail.
    /// Implements the <see cref="Snake_Base.Models.SnakeBodyPart" />
    /// </summary>
    /// <seealso cref="Snake_Base.Models.SnakeBodyPart" />
    public class SnakeTail : SnakeBodyPart
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SnakeTail" /> class.
        /// </summary>
        /// <param name="snake">The snake.</param>
        public SnakeTail(Snake? snake = null) : base(snake) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SnakeTail" /> class.
        /// </summary>
        /// <param name="place">The place.</param>
        /// <param name="snake">The snake.</param>
        /// <param name="playfield">The playfield.</param>
        public SnakeTail(Point place, Snake? snake, IPlayfield2D<ISnakeGameObject>? playfield = null) : base(place, snake, playfield) { }
    }

    /// <summary>
    /// Class Snake.
    /// </summary>
    public class Snake : NotificationObjectAdv
    {
        #region Properties
        #region private Properties
        /// <summary>
        /// The playfield
        /// </summary>
        /// <autogeneratedoc />
        private IPlayfield2D<ISnakeGameObject>? _playfield;
        /// <summary>
        /// The sn head
        /// </summary>
        /// <autogeneratedoc />
        private SnakeHead _snHead;
        /// <summary>
        /// The sn body
        /// </summary>
        /// <autogeneratedoc />
        private List<SnakeBodyPart> _snBody = new List<SnakeBodyPart>();
        /// <summary>
        /// The sn tail
        /// </summary>
        /// <autogeneratedoc />
        private SnakeTail _snTail;
        /// <summary>
        /// The sn length
        /// </summary>
        /// <autogeneratedoc />
        private int _snLength;
        #endregion
        /// <summary>
        /// The alive
        /// </summary>
        public bool alive;

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        public int Length { get => _snLength; private set => SetProperty(ref _snLength,value); }
        /// <summary>
        /// Gets the head position.
        /// </summary>
        /// <value>The head position.</value>
        public Point HeadPos { get => _snHead.Place; }

        public event EventHandler OnSnakeEatsApple;
        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="Snake" /> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="playfield">The playfield.</param>
        public Snake(Point start, IPlayfield2D<ISnakeGameObject>? playfield=null) {
            playfield ??= SnakeGameObject.DefaultParent; 
            _playfield = playfield;
            _snTail = new SnakeTail(start,this);
            _snHead = new SnakeHead(start,this);
            _snHead.NextPart = _snTail;
            _snTail.PrevPart = _snHead;
            Length = 2; // Head & Tail;
            alive = true;
        }

        /// <summary>
        /// Snakes the move.
        /// </summary>
        /// <param name="dir">The dir.</param>
        public void SnakeMove(Direction dir)
        {
            var nextPlace = Offsets.DirOffset(dir, _snHead.Place);
            if (!_playfield?.Rect.Contains(nextPlace) ?? false) { alive = false;return; }
            if (_playfield?[nextPlace] is SnakeBodyPart) { alive = false; return; }
            if (_playfield?[nextPlace] is Apple) {
                OnSnakeEatsApple?.Invoke(this, new EventArgs());
                _playfield[nextPlace] = null;
                Length++; 
            }
            SnakeBodyPart? _mRun = _snHead;
            var _nxtPlace = nextPlace;
            var bCount = 1;
            while ((_mRun != null) && (_mRun.NextPart != _snTail))
            {
                (_mRun.Place, _nxtPlace) = (_nxtPlace, _mRun.Place);
                if (bCount==1)
                  RaisePropertyChangedAdv(_mRun.OldPlace, _mRun.Place, nameof(HeadPos));
                _mRun = _mRun.NextPart;
                bCount++;
            } 
            if (bCount++ < _snLength && _mRun != null) {
                (_mRun.Place, _nxtPlace) = (_nxtPlace, _mRun.Place);
                if (bCount == 2)
                    RaisePropertyChangedAdv(_mRun.OldPlace, _mRun.Place, nameof(HeadPos));
                var _snBody = new SnakeBodyPart(_nxtPlace,this,_playfield);
                _snBody.ResetOldPlace();
                _snBody.NextPart = _mRun.NextPart;
                _snBody.PrevPart = _mRun;    
                _mRun.NextPart!.PrevPart = _snBody;
                _mRun.NextPart= _snBody;
            }
            else if (_mRun != null)
            {
                (_mRun.Place, _nxtPlace) = (_nxtPlace, _mRun.Place);
                if (bCount == 2)
                    RaisePropertyChangedAdv(_mRun.OldPlace, _mRun.Place, nameof(HeadPos));
                _snTail.Place = _nxtPlace;
            }
        }
        #endregion
    }
}
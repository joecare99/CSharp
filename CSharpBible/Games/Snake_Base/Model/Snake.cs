﻿using BaseLib.Helper;
using BaseLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Snake_Base.Model
{
    /// <summary>
    /// Class SnakeBodyPart.
    /// Implements the <see cref="Snake_Base.Model.SnakeGameObject" />
    /// Implements the <see cref="BaseLib.Interfaces.IParentedObject{Snake_Base.Model.Snake}" />
    /// </summary>
    /// <seealso cref="Snake_Base.Model.SnakeGameObject" />
    /// <seealso cref="BaseLib.Interfaces.IParentedObject{Snake_Base.Model.Snake}" />
    public class SnakeBodyPart : SnakeGameObject, IParentedObject<Snake>
    {
        #region Properties
        #region private Properties
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
        public void SetParent(Snake? value, [CallerMemberName] string CallerMember = "") => Property.SetProperty(ref _snParent, value, null, CallerMember);

        /// <summary>
        /// Gets or sets the next part.
        /// </summary>
        /// <value>The next part.</value>
        public SnakeBodyPart? NextPart { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="SnakeBodyPart"/> class.
        /// </summary>
        public SnakeBodyPart() : this(null) { }

        Snake? IParentedObject<Snake>.GetParent() => _snParent;
        /// <summary>
        /// Initializes a new instance of the <see cref="SnakeBodyPart"/> class.
        /// </summary>
        /// <param name="snake">The snake.</param>
        public SnakeBodyPart(Snake? snake):base()
        {
            _snParent = snake;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnakeBodyPart"/> class.
        /// </summary>
        /// <param name="place">The place.</param>
        /// <param name="snake">The snake.</param>
        /// <param name="playfield">The playfield.</param>
        public SnakeBodyPart(Point place,Snake? snake, Playfield2D<SnakeGameObject>? playfield = null) :base(place,playfield)
        {
            _snParent = snake;
        }
        #endregion
    }

    /// <summary>
    /// Class SnakeHead.
    /// Implements the <see cref="Snake_Base.Model.SnakeBodyPart" />
    /// </summary>
    /// <seealso cref="Snake_Base.Model.SnakeBodyPart" />
    public class SnakeHead : SnakeBodyPart
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SnakeHead"/> class.
        /// </summary>
        /// <param name="snake">The snake.</param>
        public SnakeHead(Snake? snake = null) : base(snake) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SnakeHead"/> class.
        /// </summary>
        /// <param name="place">The place.</param>
        /// <param name="snake">The snake.</param>
        /// <param name="playfield">The playfield.</param>
        public SnakeHead(Point place, Snake? snake, Playfield2D<SnakeGameObject>? playfield = null):base(place,snake,playfield) { }
    }

    /// <summary>
    /// Class SnakeTail.
    /// Implements the <see cref="Snake_Base.Model.SnakeBodyPart" />
    /// </summary>
    /// <seealso cref="Snake_Base.Model.SnakeBodyPart" />
    public class SnakeTail : SnakeBodyPart
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SnakeTail"/> class.
        /// </summary>
        /// <param name="snake">The snake.</param>
        public SnakeTail(Snake? snake = null) : base(snake) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SnakeTail"/> class.
        /// </summary>
        /// <param name="place">The place.</param>
        /// <param name="snake">The snake.</param>
        /// <param name="playfield">The playfield.</param>
        public SnakeTail(Point place, Snake? snake, Playfield2D<SnakeGameObject>? playfield = null) : base(place, snake, playfield) { }
    }

    /// <summary>
    /// Class Snake.
    /// </summary>
    public class Snake
    {
        #region Properties
        #region private Properties
        private Playfield2D<SnakeGameObject>? _playfield;
        private SnakeHead _snHead;
        private List<SnakeBodyPart> _snBody = new List<SnakeBodyPart>();
        private SnakeTail _snTail;
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
        public int Length { get => _snLength; }
        /// <summary>
        /// Gets the head position.
        /// </summary>
        /// <value>The head position.</value>
        public Point HeadPos { get=>_snHead.Place; }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="Snake"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="playfield">The playfield.</param>
        public Snake(Point start, Playfield2D<SnakeGameObject>? playfield=null) {
            playfield ??= SnakeGameObject.DefaultParent; 
            _playfield = playfield;
            _snTail = new SnakeTail(start,this);
            _snHead = new SnakeHead(start,this);
            _snHead.NextPart = _snTail;
            _snLength = 2; // Head & Tail;
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
            if (_playfield?[nextPlace] is Apple) { _snLength++; }
            SnakeBodyPart? _mRun = _snHead;
            var _nxtPlace = nextPlace;
            var bCount = 1;
            while (_mRun?.NextPart != _snTail && _mRun != null)
            {
                (_mRun.Place, _nxtPlace) = (_nxtPlace, _mRun.Place);
                _mRun = _mRun.NextPart;
                bCount++;
            } 
            if (bCount++ < _snLength && _mRun != null) {
                (_mRun.Place, _nxtPlace) = (_nxtPlace, _mRun.Place);
                var _snBody = new SnakeBodyPart(_nxtPlace,this,_playfield);
                _snBody.NextPart = _mRun.NextPart;
                _mRun.NextPart= _snBody;
            }
            else if (_mRun != null)
            {
                (_mRun.Place, _nxtPlace) = (_nxtPlace, _mRun.Place);
                _snTail.Place = _nxtPlace;
            }
        }
        #endregion
    }
}

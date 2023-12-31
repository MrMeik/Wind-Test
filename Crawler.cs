﻿using Microsoft.Xna.Framework;
using MonoGame.Extended.VectorDraw;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindTest.Util;

namespace WindTest
{
    public class Crawler
    {
        private readonly LinkedList<Vector2> _movementHistory = new LinkedList<Vector2>();
        private readonly int _targetLength;
        private int _currentLength;

        private bool FullLength => _currentLength >= _targetLength;

        private Vector2 _headPosition;
        private Vector2 _velocity;
        private float _speed = 2f;

        private float _turnTimer = 0f;
        private EventTimer _leaveTrailTimer = new EventTimer { Loop = true, TargetTime = 1f };

        public Crawler(Vector2 position, int targetLength)
        {
            _targetLength = targetLength;
            _movementHistory.AddFirst(position);
            _headPosition = position;
            _currentLength = 1;

            var (xVelocity, yVelocity) = Math.SinCos(Game1.RNG.NextDouble() * 2d * Math.PI);
            _velocity = new Vector2((float)xVelocity, (float)yVelocity) * _speed;

            var refPos = _headPosition;
            for (; _currentLength < _targetLength; _currentLength++)
            {
                refPos -= _velocity;
                _movementHistory.AddLast(refPos);
            }
        }

        public void Move(GameTime gt)
        {
            float deltaTime = (float)gt.ElapsedGameTime.TotalSeconds;

            _turnTimer += deltaTime;
            if (_turnTimer > Game1.RNG.NextDouble() * .5f + .25f)
            {
                _velocity = Vector2.Transform(_velocity, Matrix.CreateRotationZ((float)((Game1.RNG.NextDouble() - .5f) * Math.PI)));
                _turnTimer = 0f;
            }

            _headPosition += _velocity * deltaTime;

            if (_leaveTrailTimer.IncreaseTime(deltaTime))
            {
                if (FullLength)
                {
                    // Once at full length start deleting last edges
                    _movementHistory.RemoveLast();
                }
                else
                {
                    // Keep increasing length until full
                    _currentLength++;
                }

                // Add new head position
                _movementHistory.AddFirst(_headPosition);
            }
        }

        public void Draw(PrimitiveDrawing drawing)
        {
            if (_currentLength < 4)
            {
                drawing.DrawPolygon(Vector2.Zero, _movementHistory.ToArray(), Color.Pink, closed: false);
                return;
            }

            var pathPoints = GetSplinePathPoints().ToArray();

            drawing.DrawPolygon(Vector2.Zero, pathPoints, Color.Pink, closed: false);
        }

        private IEnumerable<Vector2> GetSplinePathPoints()
        {
            foreach (var p in GetHeadPoints()) yield return p;

            var p0 = _movementHistory.First.Next;
            var p1 = p0.Next;
            var p2 = p1.Next;
            var p3 = p2.Next;

            while (p3?.Next != null)
            {
                foreach (var p in GetSplinePoints(p0.Value, p1.Value, p2.Value, p3.Value)) yield return p;

                p0 = p1;
                p1 = p2;
                p2 = p3;
                p3 = p3.Next;
            }

            foreach (var p in GetProjectedTailPoints()) yield return p;
        }

        private IEnumerable<Vector2> GetHeadPoints()
        {
            var p0 = _movementHistory.First;
            var p1 = p0.Next;
            var p2 = p1.Next;
            var p3 = p2.Next;

            var fractionComplete = 1 - _leaveTrailTimer.GetPercentComplete();


            var dots = GetSplinePoints(p0.Value, p1.Value, p2.Value, p3.Value, fractionComplete);

            return dots;
        }

        private IEnumerable<Vector2> GetProjectedTailPoints()
        {
            var p0 = _movementHistory.Last;
            var p1 = p0.Previous;
            var p2 = p1.Previous;
            var p3 = p2.Previous;

            var fractionComplete = FullLength ? _leaveTrailTimer.GetPercentComplete() : 0f;
            // FIXME: Bug when drawing crawler less than full length

            return GetSplinePoints(p0.Value, p1.Value, p2.Value, p3.Value, fractionComplete).Reverse();
        }

        private IEnumerable<Vector2> GetSplinePoints(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float startingT = 0f)
        {
            float tStep = startingT;
            for (; tStep <= 1f; tStep += .25f)
            {
                yield return Vector2.CatmullRom(p0, p1, p2, p3, tStep);
            }
            yield break;

        }
    }

}

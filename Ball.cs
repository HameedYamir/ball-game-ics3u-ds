using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using GameUtility;

namespace GameStateDemo
{
    public class Ball
    {
        private float gravity = (float) 40 / 60;
        private float bounceDamping = 0.5f;
        public int ballType { get; }

        public Texture2D texture;
        public Rectangle ballRec;
        public Vector2 position;
        public Vector2 velocity;
        public Action<Ball, Ball> CollisionCallback;

        public Ball(Texture2D texture)
        {
            this.texture = texture;
            ballRec = new Rectangle(1200 / 2 - texture.Width / 2, 60, texture.Width, texture.Height);
            position.X = ballRec.X;
            position.Y = ballRec.Y;
        }

        public void DrawBall(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, ballRec, Color.White);
        }

        public Vector2 GetPos()
        {
            return position;
        }
        public void SetPosX(float x)
        {
            this.position.X = x;
            this.ballRec.X = (int)x;
        }

        public void SetPosY(float y)
        {
            this.position.Y = y;
            this.ballRec.Y = (int)y;
        }

        public void SetVelX(float x)
        {
            this.velocity.X = x;

        }

        public void SetVelY(float y)
        {
            this.velocity.Y = y;

        }

        public void SetVel(Vector2 vel)
        {
            velocity = vel;
        }
        public Vector2 GetVel()
        {
            return velocity;
        }

        public void DropBall()
        {
            velocity.Y += gravity;
            position.Y += velocity.Y;
            ballRec.Y = (int)position.Y;

        }

        public void BounceVertical()
        {
            velocity.Y = -velocity.Y * bounceDamping;
        }

        public void BounceHorizontal()
        {
            velocity.X = -velocity.X * bounceDamping;
        }

        public void Update(GameTime gameTime)
        {

        }

        public bool IsColliding(Ball otherBall)
        {
            float distance = Vector2.Distance(this.position, otherBall.position);
            float combinedRadius = this.texture.Width / 2 + otherBall.texture.Width / 2;
            return distance < combinedRadius;
        }

        public void HandleCollision(Ball otherBall, Texture2D[] textures)
        {
            if (this.texture == otherBall.texture)
            {
                CollisionCallback?.Invoke(this, otherBall);
            }
            else
            {
                Vector2 normal = position - otherBall.position;
                normal.Normalize();

                float penetrationDepth = 0.5f * (texture.Width / 2 + otherBall.texture.Width / 2 - Vector2.Distance(this.position, otherBall.position));
                Vector2 separation = normal * penetrationDepth;
                this.position += separation;
                otherBall.position -= separation;

                this.velocity.X *= -bounceDamping;
                this.velocity.Y *= -bounceDamping;
                otherBall.velocity.X *= -bounceDamping;
                otherBall.velocity.Y *= -bounceDamping;
            }
        }

        public Texture2D GetNextTexture(Texture2D[] textures)
        {
            int currentIndex = Array.IndexOf(textures, this.texture);
            int nextIndex = (currentIndex + 1) % textures.Length;
            return textures[nextIndex];
        }

        private void AdjustSizeToTexture()
        {
            ballRec = new Rectangle(ballRec.X, ballRec.Y, texture.Width, texture.Height);
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PatternRecognition
{
    public class Character
    {
        public Texture2D texture;
        public Rectangle characterRec;
        public int speed { get; set; }
        public int lengthX { get; set; }
        public int lengthY { get; set; }
        public bool isMoving{ get; set; }

        public int moveNumber { get; set; }
        public int solutionNumber { get; set; }
        public bool isHighlighted { get; set; }
        public bool isUserHighlighted { get; set; }

        public int totalFrames { get; set; }
        public int currentFrame { get; set; }
        public Texture2D[] texturesArray { get; set; }

        public Character()
        {
            //texture = Content.Load<Texture2D>("buttonOutline");
        }

        public Character(Texture2D tex, Rectangle rec)
        {
            texture = tex;
            characterRec = rec;
            isMoving = false;
            lengthX = characterRec.Right - characterRec.Left;
            lengthY = characterRec.Bottom - characterRec.Top;
        }

        public Character(Texture2D tex, Rectangle rec, Texture2D[] newTextures)
        {
            texture = tex;
            characterRec = rec;
            isMoving = false;
            lengthX = characterRec.Right - characterRec.Left;
            lengthY = characterRec.Bottom - characterRec.Top;

            texturesArray = newTextures;
            currentFrame = 0;
            totalFrames = newTextures.Length;
        }

        public Character(Rectangle rec, ref Texture2D[] newTextures)
        {
            texture = newTextures[0];
            characterRec = rec;
            isMoving = false;
            lengthX = characterRec.Right - characterRec.Left;
            lengthY = characterRec.Bottom - characterRec.Top;

            texturesArray = newTextures;
            currentFrame = 0;
            totalFrames = newTextures.Length;
        }

        public void setRec(Rectangle newValue)
        {
            characterRec = newValue;
        }

        public void setPos(Rectangle referencRectangle)
        {
            characterRec.X = referencRectangle.X;
            characterRec.Y = referencRectangle.Y;
            
        }

        public void addToRecWidth(int numToAdd)
        {
            characterRec.Width += numToAdd;
        }
        public void setRecWidth(int newValue)
        {
            characterRec.Width = newValue;
        }
        public void setRecHeight(int newValue)
        {
            characterRec.Height = newValue;
        }
        public virtual void setRecX(int newValue)
        {
            characterRec.X = newValue;
        }
        public void addToRecX(int newValue)
        {
            characterRec.X += newValue;
        }
        public void addToRecX(int newValue, Rectangle boundsRec)
        {
            if (newValue > 0 && characterRec.Right + 1 < boundsRec.Right)
            {
                for (int i = 0; i < newValue; i++)
                {
                    characterRec.X += 1;

                }
            }
            else if (newValue < 0 && characterRec.Left - 1 > boundsRec.Left)
            {
                for (int i = 0; i < newValue; i++)
                {
                    characterRec.X -= 1;

                }
            }
        }

        public void adjustToBeInBounds(Rectangle boundsRec)
        {
            bool shouldKeepAdjusting = true;

            while (shouldKeepAdjusting)
            {
                shouldKeepAdjusting = false;
                if (characterRec.Right + 1 > boundsRec.Right)
                {
                    characterRec.X -= 1;
                    shouldKeepAdjusting = true;
                }
                if (characterRec.Left - 1 < boundsRec.Left)
                {
                    characterRec.X += 1;
                    shouldKeepAdjusting = true;
                }
                if (characterRec.Bottom + 1 > boundsRec.Bottom)
                {
                    characterRec.Y -= 1;
                    shouldKeepAdjusting = true;
                }
                if (characterRec.Top - 1 < boundsRec.Top)
                {
                    characterRec.Y += 1;
                    shouldKeepAdjusting = true;
                }
            }
        }
        public void addToRecX()
        {
            characterRec.X += 1;
        }

        public void shrinkUniformly(int amountToShrinkEachSide)
        {


            characterRec.X += amountToShrinkEachSide;
            characterRec.Width -= amountToShrinkEachSide * 2;
            if (characterRec.Width > (double)(1.4 * (double)characterRec.Height))
            {
                characterRec.Y += amountToShrinkEachSide * 2;
                characterRec.Height -= amountToShrinkEachSide * 4;
            }
            else
            {
                characterRec.Y += amountToShrinkEachSide;
                characterRec.Height -= amountToShrinkEachSide * 2;
            }

        }

        public virtual void setRecY(int newValue)
        {
            characterRec.Y = newValue;
        }
        public void addToRecY(int newValue)
        {
            characterRec.Y += newValue;
        }
        public void addToRecY()
        {
            characterRec.Y += 1;
        }
        public int getRecY()
        {
            return characterRec.Y;
        }
        public Rectangle getRec()
        {
            return characterRec;
        }
        public int getRecX()
        {
            return characterRec.X;
        }

        public void changeImage(Texture2D newTex)
        {
            texture = newTex;
        }

        public virtual void changeImage()
        {
            //texture = newTex;
        }

        public void animate()
        {
            if (currentFrame < totalFrames - 1)
            {
                currentFrame++;
            }
            else
            {
                currentFrame = 0;
            }
            this.texture = null;
            this.texture = texturesArray[currentFrame];

        }

        public virtual void drawCharacter(SpriteBatch sb)
        {
            sb.Draw(texture, characterRec, Color.White);
        }

        public void drawCharacter(SpriteBatch sb, Color color)
        {
            sb.Draw(texture, characterRec, color);
        }

    }
}

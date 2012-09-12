using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WindowsGame1
{
    class Snowflake
    {
        int TTL;
        public Vector2 Position;
        public Vector2 Movement;
        int Lived;

        public Snowflake(Vector2 Pos, Vector2 Vector, int TimeToLive)
        {
            Position = Pos;
            Movement = Vector;
            TTL = TimeToLive;
            Lived = 0;
        }

        public Boolean Update(GameTime time)
        {
            Lived += time.ElapsedGameTime.Milliseconds;
            if (Lived >= TTL)
                return true;

            Position += Movement;

            return false;
        }
    }
}

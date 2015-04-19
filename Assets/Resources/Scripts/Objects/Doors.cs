using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


	public class Doors : FSprite
	{
		private FSprite door;
		public Doors(Boolean vert)
		{
			door = new FSprite("door");
			isVertical = vert;
			if (!isVertical)
			{
				door.rotation = 90f;
			}
		}

		private enum States
		{
			CLOSED,
			OPENING,
			OPEN
		}
		private Boolean _isVertical;
		private Boolean isVertical
		{
			get { return _isVertical; }
			set { _isVertical = value; }
		}
        private States _currentState;
        private States currentState
        {
            get { return _currentState; }
            set { _currentState = value; }
        }
        private const float DOOR_WIDTH = 32;
        private const float DOOR_WIDTH_HALF = DOOR_WIDTH / 2;

        public void Update()
        {
        }

		public Boolean IsPlayerColliding(Player player)
		{
            float xDist = player.x - door.x;
            float yDist = player.y - door.y;
            switch (currentState)
            {
                case States.CLOSED:
                    if (xDist < (DOOR_WIDTH_HALF + player.tilemap.tileWidth))
                        return true;
                    else if (yDist < (DOOR_WIDTH_HALF + player.tilemap.tileHeight))
                        return true;
                    break;
            }
            return false;
		}
	}


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
			else
			{

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

		public Boolean IsPlayerColliding(Player player)
		{

			return false;
		}
	}


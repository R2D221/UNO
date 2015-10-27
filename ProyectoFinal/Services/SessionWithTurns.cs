using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProyectoFinal.Models;

namespace ProyectoFinal.Services
{
	public class SessionWithTurns
	{
		private readonly IReadOnlyList<HandModel> hands;
		private int current;

		public Direction Direction { get; private set; }
		public Stack<Card> Deck { get; set; } = new Stack<Card>();
		public Stack<Card> DiscardPile { get; set; } = new Stack<Card>();

		public SessionWithTurns(Direction direction, params HandModel[] hands)
		{
			var _hands = new HandModel[hands.Length];
			this.hands = _hands;
			hands.CopyTo(_hands, 0);
			current = this.hands.Select((h, i) => new { h.IsTheirTurn, i }).Single(x => x.IsTheirTurn).i;
			this.Direction = direction;
		}

		public IEnumerable<UserModel> Users => hands.Select(h => h.User);

		public HandModel Current => hands[current];

		public string MoveNext()
		{
			hands[current].IsTheirTurn = false;
			switch (Direction)
			{
				case Direction.Counterclockwise:
				{
					current++;
					current = current % hands.Count;
				}
				break;
				case Direction.Clockwise:
				{
					current--;
					current = (current + hands.Count) % hands.Count;
				}
				break;
				default: throw new InvalidOperationException("Unknown direction");
			}

			hands[current].IsTheirTurn = true;
			return hands[current].User.Id;
		}

		public Direction Reverse()
		{
			Direction = Direction ^ Direction.Clockwise; //<-- fast way to alternate
			return Direction;
		}

		public bool IsTheirTurn(string userId)
			=> hands.Any(h => h.IsTheirTurn && h.User.Id == userId);

		public HandModel GetHandForUserId(string userId)
			=> hands.Single(h => h.User.Id == userId);

		public IEnumerable<HandModel> HandsVisibleToUser(string userId)
		{
			var index = hands.Select((h, i) => new { Id = h.User.Id, Index = i }).Single(x => x.Id == userId).Index;

			yield return hands[index];

			for (int i = 1; i < hands.Count; i++)
			{
				var hand = hands[(i + index) % hands.Count];
				yield return new HandModel
				{
					Id = hand.Id,
					IsTheirTurn = hand.IsTheirTurn,
					User = hand.User,
					Cards = Enumerable.Repeat(new Card { Color = (Color)(-1), Rank = (Rank)(-1) }, hand.Cards.Count).ToList(),
				};
			}
		}
	}
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using ProyectoFinal.Models;
using ProyectoFinal.Utils;

namespace ProyectoFinal.Services
{
	public class GamesService : ServiceBase<ApplicationDbContext>
	{
		public GamesService(ApplicationDbContext db = null) : base(db) { }

		private static ConcurrentDictionary<Guid, SessionWithTurns> activeSessions;
		private static ConcurrentDictionary<string, Lazy<ConcurrentDictionary<Guid, SessionWithTurns>>> activeSessionsByUser;
		static GamesService()
		{
			activeSessions = new ConcurrentDictionary<Guid, SessionWithTurns>();
			activeSessionsByUser = new ConcurrentDictionary<string, Lazy<ConcurrentDictionary<Guid, SessionWithTurns>>>();

			using (var db = new ApplicationDbContext())
			{
				foreach (var session in db.Sessions)
				{
					var sessionWithTurns = new SessionWithTurns(session.Direction, ToModel(session.Hand1), ToModel(session.Hand2), ToModel(session.Hand3), ToModel(session.Hand4))
					{
						Deck = new Stack<Card>(session.Deck.Where(c => !c.IsDiscarded).OrderByDescending(c => c.Order).Select(c => c.Card).ToList()),
						DiscardPile = new Stack<Card>(session.Deck.Where(c => c.IsDiscarded).OrderBy(c => c.Order).Select(c => c.Card).ToList().Concat(new[] { session.DiscardPileTop })),
					};
					activeSessions.TryAdd(session.Id, sessionWithTurns);
					activeSessionsByUser.GetOrAdd(session.Hand1.UserId, _ => new ConcurrentDictionary<Guid, SessionWithTurns>())
						.TryAdd(session.Id, sessionWithTurns);
					activeSessionsByUser.GetOrAdd(session.Hand2.UserId, _ => new ConcurrentDictionary<Guid, SessionWithTurns>())
						.TryAdd(session.Id, sessionWithTurns);
					activeSessionsByUser.GetOrAdd(session.Hand3.UserId, _ => new ConcurrentDictionary<Guid, SessionWithTurns>())
						.TryAdd(session.Id, sessionWithTurns);
					activeSessionsByUser.GetOrAdd(session.Hand4.UserId, _ => new ConcurrentDictionary<Guid, SessionWithTurns>())
						.TryAdd(session.Id, sessionWithTurns);
				}
			}
		}

		private static UserModel ToModel(ApplicationUser user)
		{
			return new UserModel
			{
				Id = user.Id,
				Name = user.Name,
				Photo = user.Photo,
			};
		}

		private static HandModel ToModel(Hand hand)
		{
			return new HandModel
			{
				Id = hand.Id,
				User = ToModel(hand.User),
				IsTheirTurn = hand.IsTheirTurn,
				Cards = hand.Cards.ToList(),
			};
		}

		public Guid Create(string userId1, string userId2, string userId3, string userId4)
		{
			var cardsList = db.Cards.ToList();
			cardsList.Shuffle();
			var cards = new Stack<Card>(cardsList);

			var hand1 = new Hand
			{
				IsTheirTurn = true,
				UserId = userId1,
				Cards = Enumerable.Range(0, 7).Select(_ => cards.Pop()).ToList(),
			};
			var hand2 = new Hand
			{
				IsTheirTurn = false,
				UserId = userId2,
				Cards = Enumerable.Range(0, 7).Select(_ => cards.Pop()).ToList(),
			};
			var hand3 = new Hand
			{
				IsTheirTurn = false,
				UserId = userId3,
				Cards = Enumerable.Range(0, 7).Select(_ => cards.Pop()).ToList(),
			};
			var hand4 = new Hand
			{
				IsTheirTurn = false,
				UserId = userId4,
				Cards = Enumerable.Range(0, 7).Select(_ => cards.Pop()).ToList(),
			};

			var firstCard = cards.Pop();

			var deck = cards.Select((c, i) => new CardInPile
			{
				Card = c,
				IsDiscarded = false,
				Order = i,
			}).ToList();

			var session = new Session
			{
				Direction = Direction.Counterclockwise,
				DiscardPileTop = firstCard,
				Deck = deck,
			};

			db.Sessions.Add(session);

			db.SaveChanges();

			hand1.Session = session; session.Hand1 = hand1;
			hand2.Session = session; session.Hand2 = hand2;
			hand3.Session = session; session.Hand3 = hand3;
			hand4.Session = session; session.Hand4 = hand4;

			db.SaveChanges();

			db.Entry(hand1).Reference(a => a.User).Load();
			db.Entry(hand2).Reference(a => a.User).Load();
			db.Entry(hand3).Reference(a => a.User).Load();
			db.Entry(hand4).Reference(a => a.User).Load();

			var sessionWithTurns = new SessionWithTurns(session.Direction, ToModel(hand1), ToModel(hand2), ToModel(hand3), ToModel(hand4))
			{
				Deck = cards,
				DiscardPile = new Stack<Card>(new[] { firstCard }),
			};

			activeSessions.TryAdd(session.Id, sessionWithTurns);
			activeSessionsByUser.GetOrAdd(userId1, _ => new ConcurrentDictionary<Guid, SessionWithTurns>())
				.TryAdd(session.Id, sessionWithTurns);
			activeSessionsByUser.GetOrAdd(userId2, _ => new ConcurrentDictionary<Guid, SessionWithTurns>())
				.TryAdd(session.Id, sessionWithTurns);
			activeSessionsByUser.GetOrAdd(userId3, _ => new ConcurrentDictionary<Guid, SessionWithTurns>())
				.TryAdd(session.Id, sessionWithTurns);
			activeSessionsByUser.GetOrAdd(userId4, _ => new ConcurrentDictionary<Guid, SessionWithTurns>())
				.TryAdd(session.Id, sessionWithTurns);

			return session.Id;
		}

		public IEnumerable<SessionInfoModel> GetActiveSessionsForUser(string userId)
		{
			foreach (var entry in activeSessionsByUser[userId].Value)
			{
				yield return new SessionInfoModel
				{
					Id = entry.Key,
					Players = entry.Value.Users,
					DiscardPileTop = entry.Value.DiscardPile.Peek(),
				};
			}
		}

		public SessionModel GetSession(Guid sessionId, string userId)
		{
			var session = activeSessions[sessionId];

			return new SessionModel
			{
				Id = sessionId,
				Direction = session.Direction,
				DiscardPileTop = session.DiscardPile.Peek(),
				DeckCount = session.Deck.Count,
				DiscardPileCount = session.DiscardPile.Count - 1,
				Hands = session.HandsVisibleToUser(userId).ToList(),
			};
		}

		public string TryUseCard(Guid sessionId, string userId, int cardId)
		{
			var session = activeSessions[sessionId];
			var hand = session.GetHandForUserId(userId);

			if (!hand.IsTheirTurn)
				return null;

			var card = hand.Cards.SingleOrDefault(c => c.Id == cardId);
			if (card == null)
				return null;

			var discardPileTop = session.DiscardPile.Peek();
			if (card.Color != discardPileTop.Color && card.Rank != discardPileTop.Rank)
				return null;

			hand.Cards.Remove(card);
			session.DiscardPile.Push(card);
			var newUserTurn = session.MoveNext();

			var cardToRemove = db.Hands.Find(hand.Id).Cards.Single(c => c.Id == card.Id);
			db.Hands.Find(hand.Id).Cards.Remove(cardToRemove);
			var dbSession = db.Sessions.Find(sessionId);
			var dbDiscard = dbSession.DiscardPileTop;
			var lastOrder = dbSession.Deck.Where(c => c.IsDiscarded).OrderByDescending(c => c.Order).Select(c => c.Order).FirstOrDefault();
			dbSession.Deck.Add(new CardInPile
			{
				Card = dbDiscard,
				IsDiscarded = true,
				Order = lastOrder + 1,
			});
			dbSession.DiscardPileTopId = card.Id;

			dbSession.Hand1.IsTheirTurn = dbSession.Hand1.UserId == newUserTurn ? true : false;
			dbSession.Hand2.IsTheirTurn = dbSession.Hand2.UserId == newUserTurn ? true : false;
			dbSession.Hand3.IsTheirTurn = dbSession.Hand3.UserId == newUserTurn ? true : false;
			dbSession.Hand4.IsTheirTurn = dbSession.Hand4.UserId == newUserTurn ? true : false;

			db.SaveChanges();

			return newUserTurn;
		}
	}
}